using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using WebApi2Template.Models;

namespace WebApi2Template.Stores
{
    /// <summary>
    /// T 泛型參數為此 Store 預設要回傳的 DataModel 類別名稱
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseStore<T> : IDisposable, IStore<T>
        where T : IDataModel
    {
        private const string _defaultConnectionStringName = "Db"; // 預設要使用的資料庫連線設定名稱，對應 connectionStrings 的 Name

        private SqlConnection _connection;
        private ErrorLogStore _errorLogStore;

        private bool _disposed = false;

        /// <summary>
        /// 如果要簡化繼承此類別所預設使用的資料庫連線設定名稱，可修改 _defaultConnectionStringName
        /// </summary>
        public BaseStore() : this(_defaultConnectionStringName)
        { }

        /// <summary>
        /// 自定義預設使用的資料庫連線設定名稱所用的建構式
        /// </summary>
        /// <param name="connectionStringName">預設使用的資料庫連線設定名稱</param>
        public BaseStore(string connectionStringName)
        {
            this.ConnectionStringName = connectionStringName;

            _errorLogStore = new ErrorLogStore(connectionStringName);

            _connection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString);
        }

        /// <summary>
        /// 取得 Store 所使用的資料庫連線設定名稱
        /// </summary>
        public string ConnectionStringName { get; private set; }

        /// <summary>
        /// 取得 Store 所使用的 Sql 連線物件 (SqlConnection)
        /// </summary>
        public SqlConnection Connection => _connection;

        public virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                }

                if (disposing)
                {
                    _connection.Dispose();
                }
            }

            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            _errorLogStore.Dispose();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 統一處理紀錄 Sql 錯誤，並將轉換後的 StoreException 拋回
        /// </summary>
        /// <param name="ex">SQL 錯誤物件</param>
        /// <param name="sql">當下執行的 TSQL</param>
        /// <returns></returns>
        private StoreException ProcessStoreException(SqlException ex, string sql)
        {
            // 將原本的錯誤資訊物件 ex 轉型為我們自訂的 StoreException
            var exception = new StoreException(ex);

            // Log into ErrorLog in DB
            var id = _errorLogStore.InsertStoreExceptionLog(exception, sql);

            // 並把 ErrorLog 所記錄的代碼寫到 exception 後拋出，提供前端利用
            exception.ErrorLogId = id;

            return exception;
        }

        /// <summary>
        /// 使用 Dapper 執行 SQL 後取得資料集合物件，且集合項目型別為類別繼承時所指定的預設資料模型 T
        /// </summary>
        /// <param name="strSql">欲操作使用的 TSQL 陳述式</param>
        /// <param name="param">相關要帶給 TSQL 的參數項目，非必要的參數</param>
        /// <param name="transaction">如果有進行資料庫交易機制，請將交易物件傳入提供，非必要的參數</param>
        /// <returns></returns>
        public IList<T> DapperQuery(string strSql, object param = null, IDbTransaction transaction = null)
        {
            return this.DapperQuery<T>(strSql, param, transaction);
        }

        /// <summary>
        /// 使用 Dapper 執行 SQL 後取得資料集合物件，但集合項目型別不為類別繼承時所指定的 T
        /// </summary>
        /// <typeparam name="U">自行指定此方法要回傳的項目資料模型</typeparam>
        /// <param name="strSql">欲操作使用的 TSQL 陳述式</param>
        /// <param name="param">相關要帶給 TSQL 的參數項目，非必要的參數</param>
        /// <param name="transaction">如果有進行資料庫交易機制，請將交易物件傳入提供，非必要的參數</param>
        /// <returns></returns>
        public IList<U> DapperQuery<U>(string strSql, object param = null, IDbTransaction transaction = null)
        {
            try
            {
                return _connection.Query<U>(
                    sql: strSql,
                    param: param,
                    transaction: transaction).ToList();
            }
            catch (SqlException ex)
            {
                throw ProcessStoreException(ex, strSql);
            }
        }

        /// <summary>
        /// 使用 Dapper 執行 SQL 後，取回結果中的第一筆資料
        /// </summary>
        /// <param name="strSql">欲操作使用的 TSQL 陳述式</param>
        /// <param name="param">相關要帶給 TSQL 的參數項目，非必要的參數</param>
        /// <param name="transaction">如果有進行資料庫交易機制，請將交易物件傳入提供，非必要的參數</param>
        /// <returns></returns>
        public T DapperQueryFirst(string strSql, object param = null, IDbTransaction transaction = null)
        {
            return this.DapperQueryFirst<T>(strSql, param, transaction);
        }

        /// <summary>
        /// 使用 Dapper 執行 SQL 後，取回結果為集合物件中的第一筆資料
        /// </summary>
        /// <typeparam name="U">自行指定此方法要回傳的項目資料模型</typeparam>
        /// <param name="strSql">欲操作使用的 TSQL 陳述式</param>
        /// <param name="param">相關要帶給 TSQL 的參數項目，非必要的參數</param>
        /// <param name="transaction">如果有進行資料庫交易機制，請將交易物件傳入提供，非必要的參數</param>
        /// <returns></returns>
        public U DapperQueryFirst<U>(string strSql, object param = null, IDbTransaction transaction = null)
        {
            try
            {
                return _connection.QueryFirst<U>(
                    sql: strSql,
                    param: param,
                    transaction: transaction);
            }
            catch (SqlException ex)
            {
                throw ProcessStoreException(ex, strSql);
            }
        }

        /// <summary>
        /// 使用 Dapper 執行沒有要回傳資料的 SQL，並取得 SQL 所操作影響的紀錄筆數
        /// </summary>
        /// <param name="strSql">欲操作使用的 TSQL 陳述式</param>
        /// <param name="param">相關要帶給 TSQL 的參數項目，非必要的參數</param>
        /// <param name="transaction">如果有進行資料庫交易機制，請將交易物件傳入提供，非必要的參數</param>
        /// <returns>回傳此操作所影響的資料筆數</returns>
        public int DapperExecute(string strSql, object param = null, IDbTransaction transaction = null)
        {
            try
            {
                return _connection.Execute(
                    sql: strSql,
                    param: param,
                    transaction: transaction);
            }
            catch (SqlException ex)
            {
                throw ProcessStoreException(ex, strSql);
            }
        }

        /// <summary>
        /// 使用 Dapper 執行 SQL 後，取得執行結果中的第一筆、第一欄資料內容
        /// </summary>
        /// <typeparam name="U">資料類型，例如：int, string...</typeparam>
        /// <param name="strSql">欲操作使用的 TSQL 陳述式</param>
        /// <param name="param">相關要帶給 TSQL 的參數項目，非必要的參數</param>
        /// <param name="transaction">如果有進行資料庫交易機制，請將交易物件傳入提供，非必要的參數</param>
        /// <returns></returns>
        public U DapperExecuteScalar<U>(string strSql, object param = null, IDbTransaction transaction = null)
        {
            try
            {
                return _connection.ExecuteScalar<U>(
                    sql: strSql,
                    param: param,
                    transaction: transaction);
            }
            catch (SqlException ex)
            {
                throw ProcessStoreException(ex, strSql);
            }
        }
    }
}
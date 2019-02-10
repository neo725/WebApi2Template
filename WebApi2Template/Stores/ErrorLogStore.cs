using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace WebApi2Template.Stores
{
    public class ErrorLogStore : IDisposable
    {
        private SqlConnection _connection;

        /// <summary>
        /// 預設建構式，並且使用 ErrorLogConnectionName 所指定的連線名稱
        /// </summary>
        public ErrorLogStore()
        {
            var connectionStringName = ConfigurationManager.AppSettings["ErrorLogConnectionName"];

            _connection = CreateConnection(connectionStringName);
        }

        /// <summary>
        /// 建構式，使用參數 connectionStringName 所指定的連線名稱
        /// </summary>
        /// <param name="connectionStringName">指定的連線名稱 (對應至 connectionString.Name)</param>
        public ErrorLogStore(string connectionStringName)
        {
            _connection = CreateConnection(ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString);
        }

        /// <summary>
        /// 建立 Sql 連線物件
        /// </summary>
        /// <param name="connectionString">完整的連線字串</param>
        /// <returns></returns>
        private SqlConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }

                _connection.Dispose();
            }
        }

        public int InsertStoreExceptionLog(StoreException ex, string sql)
        {
            var message = $"發生於 TSQL 行 : {ex.LineNumber}。\r\n{ex.Message}";

            return this.InsertLog(message, sql, ex.Source, ex.StackTrace);
        }

        /// <summary>
        /// 在資料庫紀錄一筆錯誤訊息，如果紀錄成功，則回傳紀錄代碼 (ErrorLog.Id)，若記錄失敗，則回傳數值 0
        /// </summary>
        /// <param name="message">要記錄的錯誤訊息，通常為 Exception 的 Message</param>
        /// <param name="sql">要記錄的 Sql 內容，非必要參數</param>
        /// <param name="source">要記錄的 Source 資訊，通常在 Exception 的 Source 屬性，非必要參數</param>
        /// <param name="stacktrace">要記錄的追蹤堆疊資訊，通常在 Exception 的 StackTrace 屬性，非必要參數</param>
        /// <param name="url">要記錄的網址路徑資訊，非必要資訊</param>
        /// <returns>紀錄代碼 (ErrorLog.Id)</returns>
        public int InsertLog(string message, string sql = null, string source = null, string stacktrace = null, string url = null)
        {
            var strSql = @"
insert into [ErrorLog]
(Message, Sql, Source, StackTrace, Url)
values
(@message, @sql, @source, @stacktrace, @url)

select SCOPE_IDENTITY()
";
            try
            {
                return _connection.QueryFirst<int>(strSql, new
                {
                    Message = message,
                    Sql = sql,
                    Source = source,
                    StackTrace = stacktrace,
                    Url = url
                });
            }
            catch (SqlException)
            {
                return 0;
            }
        }
    }
}
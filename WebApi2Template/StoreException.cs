using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace WebApi2Template
{
    public class StoreException : Exception
    {
        public StoreException(SqlException ex)
            : base(ex.Message, ex)
        {
            this.LineNumber = ex.LineNumber;
        }

        public int LineNumber { get; set; }

        /// <summary>
        /// 取得或設定已經完成紀錄在資料庫 ErrorLog 資料表中的紀錄代碼
        /// </summary>
        public virtual int ErrorLogId { get; set; }
    }
}
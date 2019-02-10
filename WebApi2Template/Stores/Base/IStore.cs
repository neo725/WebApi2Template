using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi2Template.Models;

namespace WebApi2Template.Stores
{
    public interface IStore
    {
        #region " 介面屬性定義 "

        SqlConnection Connection { get; }

        #endregion
    }

    public interface IStore<T> : IStore
    {
        #region " 介面方法定義 "

        IList<T> DapperQuery(string strSql, object param = null, IDbTransaction transaction = null);

        #endregion

    }
}

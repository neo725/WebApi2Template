using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi2Template.Models
{
    public class BaseDataModel : IDataModel
    {
        /// <summary>
        /// 取得或設定資料代碼
        /// </summary>
        public virtual int Id { get ; set ; }
    }
}
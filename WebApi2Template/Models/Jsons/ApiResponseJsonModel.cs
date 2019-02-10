using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi2Template.Models.Jsons
{
    /// <summary>
    /// 所有 Api Controller 要回應的 Json 資料物件類別
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponseJsonModel<T> : BaseResponseJsonModel<T>
    {}
}
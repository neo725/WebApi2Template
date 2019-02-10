using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Reflection;
using System.Diagnostics;
using WebApi2Template.Models;
using WebApi2Template.Models.Base;
using WebApi2Template.Models.Jsons;
using WebApi2Template.Stores;
using System.Diagnostics.Contracts;

namespace WebApi2Template.Controllers
{
    /// <summary>
    /// ApiController 基底類別
    /// </summary>
    /// <typeparam name="S">提供此 ApiController 預設要使用的 Store，資料存取層類別 Store Class</typeparam>
    /// <typeparam name="U">提供此 ApiController 預設要使用的 Model，資料類別 Model Class</typeparam>
    /// <typeparam name="J">提供此 ApiController 預設要使用的 JsonModel，資料類別 Model.Json Class</typeparam>
    public class BaseApiController<S, U, J> : ApiController
        where S : IStore, IDisposable, new()
    {
        private ErrorLogStore _errorLogStore;

        public BaseApiController()
        {
            this._errorLogStore = new ErrorLogStore();

        }

        /// <summary>
        /// 建立預設的資料存取層 (Store)，此 Store 為 Controller 繼承 BaseApiController 基底類別時，所定義的泛型類別 S
        /// </summary>
        /// <returns></returns>
        protected S CreateDefaultStore()
        {
            return new S();
        }

        /// <summary>
        /// 建立指定類別的資料存取層 (Store)
        /// </summary>
        /// <typeparam name="CS">要建立的資料存取層類別名稱</typeparam>
        /// <returns></returns>
        protected CS CreateStore<CS>()
            where CS : new()
        {
            return new CS();
        }

        /// <summary>
        /// 自訂 API 回應，建議如非必要，不需使用此方法做 API 回傳
        /// </summary>
        /// <typeparam name="U">API 所回傳的資料類別名稱，例如：CourseJsonModel</typeparam>
        /// <param name="statusCode">HTTP 狀態碼 (Http Status Code)</param>
        /// <param name="data">要回傳的資料物件</param>
        /// <param name="resultNumber">自定義的回傳狀態碼，如果為成功回傳，大多為 0，或大於 0 的正整數，如果為失敗回傳，則必須設定小於 0 的負整數</param>
        /// <param name="result">自定義的回傳成功與否的狀態註記</param>
        /// <returns></returns>
        protected IHttpActionResult ApiContent<CU>(HttpStatusCode statusCode, CU data, int resultNumber, bool result = false)
        {
            var baseJson = new ApiResponseJsonModel<CU>
            {
                Result = result,
                ResultNumber = resultNumber
            };

            if (data != null)
            {
                var isJsonDataModel = 
                    data.GetType().GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IJsonDataModelConvert<CU>));

                if (isJsonDataModel)
                {
                    var jsonOfU = (IJsonDataModelConvert<CU>)data;

                    baseJson.Data = jsonOfU.ToJsonModel();
                }
                else
                {
                    baseJson.Data = data;
                }
            }

            return base.Content(statusCode, baseJson);
        }

        protected IHttpActionResult ApiContent<CU>(HttpStatusCode statusCode, IList<CU> data, int resultNumber, bool result = false)
        {
            var baseJson = new ApiResponseJsonModel<IList<CU>>
            {
                Result = result,
                ResultNumber = resultNumber
            };

            if (data != null)
            {
                var isDataModelList =
                    data.GetType().GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<IJsonDataModelConvert<CU>>));

                if (isDataModelList)
                {
                    var listOfU = (IList<IJsonDataModelConvert<CU>>)data;

                    var list = new List<CU>();
                    foreach (var item in listOfU)
                    {
                        list.Add(item.ToJsonModel());
                    }
                    baseJson.Data = list;
                }
                else
                {
                    baseJson.Data = data;
                }
            }

            return base.Content(statusCode, baseJson);
        }

        /// <summary>
        /// 回傳 API 成功完成，且狀態碼為 200 (OK)
        /// </summary>
        /// <typeparam name="CU">自定義 API 所回傳的資料模型類別名稱，例如：CourseModel</typeparam>
        /// <typeparam name="CJ">自定義 API 所回傳的 Json 模型類別名稱，例如：CourseJsonModel</typeparam>
        /// <param name="data">要回傳的資料物件</param>
        /// <param name="resultNumber">自定義的回傳狀態碼，必須為 >= 0 的整數，非必要參數，預設為 0</param>
        /// <returns></returns>
        protected IHttpActionResult ApiOk<CU, CJ>(CU data, int resultNumber = 0)
            where CU : IJsonDataModelConvert<CJ>, IDataModel
            where CJ : IJsonModel
        {
            Contract.Requires(resultNumber >= 0, "傳入 resultNumber 必須 >= 0");

            return this.ApiContent<CU>(HttpStatusCode.OK, data, resultNumber, true);
        }

        protected IHttpActionResult ApiOk(U data, int resultNumber = 0)
        {
            Contract.Requires(resultNumber >= 0, "傳入 resultNumber 必須 >= 0");

            return this.ApiContent<U>(HttpStatusCode.OK, data, resultNumber, true);
        }

        /// <summary>
        /// 回傳 Api 成功完成，且狀態碼為 200 (OK)
        /// </summary>
        /// <typeparam name="CU">API 所回傳的資料類別名稱，例如：CourseModel</typeparam>
        /// <param name="data">要回傳的資料物件</param>
        /// <param name="resultNumber">自定義的回傳狀態碼，必須為 >= 0 的整數，非必要參數，預設為 0</param>
        /// <returns></returns>
        protected IHttpActionResult ApiOk<CU, CJ>(IList<CU> data, int resultNumber = 0)
            where CU : IJsonDataModelConvert<CJ>, IDataModel
            where CJ : IJsonModel
        {
            var stackTrace = new StackTrace();
            stackTrace.GetFrame(1).GetMethod().GetCustomAttributes().Any(a => a.GetType() == typeof(HttpGetAttribute));

            Contract.Requires(resultNumber >= 0, "傳入 resultNumber 必須 >= 0");
            

            return this.ApiContent<IList<CU>>(HttpStatusCode.OK, data, resultNumber, true);
        }

        protected IHttpActionResult ApiOk(IList<U> data, int resultNumber = 0)
        {
            Contract.Requires(resultNumber >= 0, "傳入 resultNumber 必須 >= 0");

            return this.ApiContent<IList<U>>(HttpStatusCode.OK, data, resultNumber, true);
        }
        /// <summary>
        /// 回傳 API 伺服器處理錯誤，狀態碼為 500 (Internal Server Error)，ResultNumber 預設為 -99999
        /// 錯誤類型為一般錯誤 (Exception)
        /// </summary>
        /// <param name="exception">觸發錯誤的 Exception 物件</param>
        /// <param name="resultNumber">自定義的回傳狀態代碼，必須為小於 0 的整數，非必要參數，預設為 -99999</param>
        /// <returns></returns>
        protected IHttpActionResult ApiServerError(Exception exception, int resultNumber = -99999)
        {
            Contract.Requires(resultNumber < 0, "傳入 resultNumber 必須 < 0");

            var errorLogId = 
                this._errorLogStore.InsertLog(exception.Message, source: exception.Source, stacktrace: exception.StackTrace);

            return this.ApiContent<string>(HttpStatusCode.InternalServerError,
                String.Format("系統發生錯誤，紀錄代碼為 {0}", errorLogId),
                resultNumber);
        }

        /// <summary>
        /// 回傳 API 伺服器處理錯誤，狀態碼為 500 (Internal Server Error)，ResultNumber 為 -99999
        /// 錯誤類型為資料存取層錯誤 (StoreException)
        /// </summary>
        /// <param name="exception">觸發的資料存取層 Exception 物件</param>
        /// <returns></returns>
        protected IHttpActionResult ApiServerError(StoreException exception)
        {
            return this.ApiContent<string>(HttpStatusCode.InternalServerError,
                String.Format("系統發生錯誤，紀錄代碼為 {0}", exception.ErrorLogId),
                -99999);
        }

        /// <summary>
        /// 回應 HttpStatusCode = 400 (Bad Request) 錯誤的要求
        /// 
        /// Request 要求資源的條件不正確，與要求的資源不存在 (Not Found) 不一樣
        /// </summary>
        /// <param name="message">自定義回傳的錯誤訊息</param>
        /// <param name="resultNumber">自定義回傳的錯誤代碼，必須為小於 0 的整數</param>
        /// <returns></returns>
        protected IHttpActionResult ApiBadRequest(string message, int resultNumber)
        {
            Contract.Requires(resultNumber < 0, "傳入 resultNumber 必須 < 0");

            return this.ApiContent<string>(HttpStatusCode.BadRequest, message, resultNumber);
        }

        /// <summary>
        /// 回應 HttpStatusCode = 401 (Unauthorized) 未授權的要求
        /// 
        /// Request 未提供或未通過授權
        /// </summary>
        /// <param name="message">自定義回傳的錯誤訊息</param>
        /// <param name="resultNumber">自定義回傳的錯誤代碼，必須為小於 0 的整數，非必要參數，預設為 -9999</param>
        /// <returns></returns>
        protected IHttpActionResult ApiUnauthorized(string message, int resultNumber = -9999)
        {
            Contract.Requires(resultNumber < 0, "傳入 resultNumber 必須 < 0");

            return this.ApiContent<string>(HttpStatusCode.Unauthorized, message, resultNumber);
        }

        /// <summary>
        /// 回應 HttpStatusCode = 403 (Forbidden) 不具有所要求的內容的權限
        /// 
        /// Request 已提供且通過授權，但無權限可以存取要求的內容
        /// </summary>
        /// <param name="message">自定義回傳的錯誤訊息</param>
        /// <param name="resultNumber">自定義回傳的錯誤代碼，必須為小於 0 的整數</param>
        /// <returns></returns>
        protected IHttpActionResult ApiForbidden(string message, int resultNumber)
        {
            Contract.Requires(resultNumber < 0, "傳入 resultNumber 必須 < 0");

            return this.ApiContent<string>(HttpStatusCode.Forbidden, message, resultNumber);
        }

        /// <summary>
        /// 回應 HttpStatusCode = 404 (NotFound) 所要求的資源內容不存在
        /// 
        /// Request 所要求的內容不存在
        /// </summary>
        /// <param name="message">自定義回傳的錯誤訊息</param>
        /// <param name="resultNumber">自定義回傳的錯誤代碼，必須為小於 0 的整數</param>
        /// <returns></returns>
        protected IHttpActionResult ApiNotFound(string message, int resultNumber)
        {
            Contract.Requires(resultNumber < 0, "傳入 resultNumber 必須 < 0");

            return this.ApiContent<string>(HttpStatusCode.NotFound, message, resultNumber);
        }

        /// <summary>
        /// 回應 Post 或 Put 所要求的操作已經成功的回傳 (HttpStatusCode = 201)
        /// </summary>
        /// <typeparam name="CU">回傳資料的型別</typeparam>
        /// <param name="data">要回傳的資料</param>
        /// <param name="resultNumber">自訂的回傳代碼，必須為 >= 0 的整數，非必要參數，如不輸入則預設為 0</param>
        /// <returns></returns>
        protected IHttpActionResult ApiCreated<CU, CJ>(CU data, int resultNumber = 0)
            where CU : IJsonDataModelConvert<CJ>, IDataModel
        {
            Contract.Requires(resultNumber >= 0, "傳入 resultNumber 必須 >= 0");

            return this.ApiContent<CU>(HttpStatusCode.Created, data, resultNumber);
        }

        /// <summary>
        /// 回應伺服器已接受 Post, Put 或 Delete 的操作，可是沒有資料需要回傳 (HttpStatusCode = 204)
        /// </summary>
        /// <param name="resultNumber">自定義的回傳狀態代碼，必須為 >= 0 的整數，非必要參數，如不輸入則預設為 0</param>
        /// <returns></returns>
        protected IHttpActionResult ApiNoContent(int resultNumber = 0)
        {
            Contract.Requires(resultNumber >= 0, "傳入 resultNumber 必須 >= 0");

            return this.ApiContent<object>(HttpStatusCode.Created, null, resultNumber, true);
        }

        /// <summary>
        /// 回應伺服器已接受 Delete 的操作 (HttpStatusCode = 202)
        /// </summary>
        /// <param name="resultNumber">自定義的回傳狀態代碼，必須為 >= 0 的整數，非必要參數，如不輸入則預設為 0</param>
        /// <returns></returns>
        protected IHttpActionResult ApiAccepted(int resultNumber = 0)
        {
            Contract.Requires(resultNumber >= 0, "傳入 resultNumber 必須 >= 0");

            return this.ApiContent<object>(HttpStatusCode.Accepted, null, resultNumber);
        }
    }
}
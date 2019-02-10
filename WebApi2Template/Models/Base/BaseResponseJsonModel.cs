using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi2Template.Models.Jsons
{
    /// <summary>
    /// T 為回傳 Json 的屬性 Data 類別名稱，例如為 CourseJsonModel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseResponseJsonModel<T>
    {
        /// <summary>
        /// 取得或設定回傳 Json 的作業狀態成功與否表示，也就是 SCE 所定義的 API 回傳規範中的 Result 屬性
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// 取得或設定 Json 的回傳自定義狀態代碼，也就是 SCE 所定義的 API 回傳規範中的 ResultNumber 屬性
        /// </summary>
        public int ResultNumber { get; set; }

        // 說明：關於此屬性定義 JsonProperty 的原因
        // 如果此屬性值為 Null 時，則輸入 Json 時，會略過此屬性的輸出
        /// <summary>
        /// 取得或設定 Json 實際回傳的資料內容，也就是 SCE 所定義的 API 回傳規範中的 Data 屬性
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T Data { get; set; }
    }
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApi2Template.Models.Base;

namespace WebApi2Template.Models.Jsons
{
    public class SampleCourseJsonModel : IJsonModel
    {
        /// <summary>
        /// 取得或設定課程代碼
        /// </summary>
        public string CourseNo { get; set; }

        /// <summary>
        /// 取得或設定課程名稱
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// 取得或設定資料建立時間
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 取得或設定資料最後更新時間
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? UpdateDate { get; set; }
    }
}
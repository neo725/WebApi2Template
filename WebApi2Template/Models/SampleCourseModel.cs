using System;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi2Template.Models.Base;
using WebApi2Template.Models.Jsons;

namespace WebApi2Template.Models
{
    public class SampleCourseModel : BaseDataModel, IJsonDataModelConvert<SampleCourseJsonModel>
    {
        // 不用特別寫 Id 這個屬性，已經放在 BaseDataModel 中，除非有額外處理需求可以覆寫

        [Column("course_name")]
        /// <summary>
        /// 取得或設定課程名稱
        /// </summary>
        public string Name { get; set; }

        [Column("course_mark")]
        /// <summary>
        /// 取得或設定課程資料狀態碼
        /// </summary>
        public int Mark { get; set; }

        [Column("create_date")]
        /// <summary>
        /// 取得或設定資料建立時間
        /// </summary>
        public DateTime CreateDate { get; set; }

        [Column("update_date")]
        /// <summary>
        /// 取得或設定資料更新時間，可為 Null 值
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        public SampleCourseJsonModel ToJsonModel()
        {
            var jsonModel = new SampleCourseJsonModel();

            return jsonModel;
        }
    }
}
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi2Template.Models
{
    public class SampleManModel : BaseDataModel
    {
        [Column("man_name")]
        /// <summary>
        /// 取得或設定課程名稱
        /// </summary>
        public string Name { get; set; }
    }
}
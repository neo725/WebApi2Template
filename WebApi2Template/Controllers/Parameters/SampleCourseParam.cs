using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi2Template.Controllers.Parameters
{
    public class SampleCourseParam
    {
        /// <summary>
        /// 對應課程代碼 course_no 欄位
        /// </summary>
        public string CourseNo { get; set; }

        /// <summary>
        /// 對應課程名稱 course_name 欄位
        /// </summary>
        public string CourseName { get; set; }
    }
}
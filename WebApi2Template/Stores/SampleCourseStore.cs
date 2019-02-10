using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApi2Template.Models;

namespace WebApi2Template.Stores
{
    /// <summary>
    /// 這個 Store 為示範類別，繼承 BaseStore，並指定大部分方法預設回傳的資料模型類別為 SampleCourseModel
    /// </summary>
    public class SampleCourseStore : BaseStore<SampleCourseModel>
    {
        /// <summary>
        /// 實作此物件類別時，預設資料庫連線設定使用 ican_db 的項目
        /// </summary>
        public SampleCourseStore()
            : base("ican_db")
        { }

        /// <summary>
        /// 取得所有課程資料集合
        /// </summary>
        /// <returns></returns>
        public IList<SampleCourseModel> GetCourses()
        {
            // TSQL 撰寫風格說明：
            // 1. TSQL 內容請從弟一個字元對齊
            // 2. 如為 select 操作，請一行一個欄位，並將欄位之間的逗號放在欄位名稱前面
            // 3. 第二個 (含) 之後的欄位要做縮排
            // 4. 如為 update 操作，set 欄位值的規則與 2, 3 相同
            // 4. 資料表名稱或特殊關鍵字以中括號包覆
            // 5. 如有交集或聯集其他資料表，請加上別名定義，例如 inner join [Course] c
            // 6. 如有交集或聯集其他資料表，在 select 的欄位名稱盡量可以加上別名，明確了解欄位是從哪一個資料表所取用
            // 7. where 條件也請一個條件擺一行
            // 8. 第二個 (含) 之後的條件要做縮排
            // 9. 如果包含多段 TSQL 操作，在各段 TSQL 結尾加上分號 (;)

            var strSql = @"
select Id
    , cour_name_1000
    , course_no
    , create_date
    , update_date
from [Course]
where cour_mark = 10
";
            return base.DapperQuery(strSql);
        }

        /// <summary>
        /// 取得指定課程代碼 (course_no) 之課程資料
        /// </summary>
        /// <param name="course_no"></param>
        /// <returns></returns>
        public SampleCourseModel GetInUseCourseByNo(string course_no)
        {
            var strSql = @"
select Id
    , course_no
    , course_name
    , create_date
    , update_date
from [Course]
where course_mark = 10
    and course_no = @course_no
";
            return base.DapperQueryFirst(strSql,
                new
                {
                    course_no
                });
        }

        /// <summary>
        /// 取得相同課程名稱的資料是否已經存在
        /// </summary>
        /// <param name="course_name">課程名稱</param>
        /// <returns></returns>
        public bool CheckIsExistsByCourseName(string course_name)
        {
            var strSql = @"
select COUNT(*)
from [Course]
where course_name = @course_name
";
            var rowCount = base.DapperExecuteScalar<int>(strSql,
                new
                {
                    course_name = course_name.Trim()
                });

            return rowCount > 0;
        }

        /// <summary>
        /// 新增一門課程資料
        /// </summary>
        /// <param name="course_no">課程代碼</param>
        /// <param name="course_name">課程名稱</param>
        /// <returns></returns>
        public SampleCourseModel CreateCourse(string course_no, string course_name)
        {
            var strSql = @"
insert into [Course]
(course_no, course_name, create_date, course_mark)
values
(@course_no, @course_name, GETUTCDATE(), 10);

select Id
    , course_no
    , course_name
    , create_date
    , update_date
from [Course]
where Id = SCOPE_IDENTITY();
";
            var newCourseModel = base.DapperQueryFirst(strSql,
                new
                {
                    course_no,
                    course_name
                });

            return newCourseModel;
        }

        /// <summary>
        /// 更新指定課程代碼的資料
        /// </summary>
        /// <param name="course_no">指定的課程代碼</param>
        /// <param name="model_to_update">要更新的資料模型物件</param>
        /// <returns></returns>
        public SampleCourseModel UpdateCourse(string course_no, SampleCourseModel model_to_update)
        {
            var strSql = @"
update [Course]
set course_name = @course_name
    , course_mark = @course_mark
    , update_date = GETUTCDATE()
where course_no = @course_no;

select Id
    , course_no
    , course_name
    , create_date
    , update_date
from [Course]
where course_no = @course_no
";
            var updatedCourseModel = base.DapperQueryFirst(strSql,
                new
                {
                    course_no,
                    course_name = model_to_update.Name,
                    course_mark = model_to_update.Mark
                });

            return updatedCourseModel;
        }

        /// <summary>
        /// 刪除指定課程代碼之紀錄
        /// </summary>
        /// <param name="course_no">指定的課程代碼</param>
        /// <returns></returns>
        public bool DeleteCourseByNo(string course_no)
        {
            var strSql = @"
update [Course]
set course_mark = 40
where course_no = @course_no
";
            var rowEffected = base.DapperExecute(strSql,
                new
                {
                    course_no
                });

            return rowEffected > 0;
        }
    }
}
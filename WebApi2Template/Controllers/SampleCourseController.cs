using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApi2Template.Controllers.Parameters;
using WebApi2Template.Models;
using WebApi2Template.Models.Jsons;
using WebApi2Template.Stores;

namespace WebApi2Template.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "GET, POST, PUT, DELETE, OPTIONS")]
    [RoutePrefix("api/course")]
    public class SampleCourseController : BaseApiController<SampleCourseStore, SampleCourseModel, SampleCourseJsonModel>
    {
        /// <summary>
        /// 取得所有課程資料
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("")]
        public IHttpActionResult GetCourses()
        {
            try
            {
                using (var courseStore = CreateDefaultStore())
                {
                    var courses = courseStore.GetCourses();

                    if (courses == null)
                    {
                        return ApiNotFound($"目前沒有課程資料", 1);
                    }

                    return ApiOk(courses);
                }
            }
            catch (StoreException ex)
            {
                return ApiServerError(ex);
            }
            catch (Exception ex)
            {
                return ApiServerError(ex);
            }
        }

        public IHttpActionResult CreateCourse(SampleCourseParam param)
        {
            try
            {
                using (var courseStore = CreateDefaultStore())
                {
                    if (courseStore.CheckIsExistsByCourseName(param.CourseName))
                    {
                        return ApiBadRequest("課程名稱已經存在", -1);
                    }

                    var courseCreated = courseStore.CreateCourse(param.CourseNo, param.CourseName);

                    return ApiCreated<SampleCourseModel, SampleCourseJsonModel>(courseCreated);
                }
            }
            catch (StoreException ex)
            {
                return ApiServerError(ex);
            }
            catch (Exception ex)
            {
                return ApiServerError(ex);
            }
        }
    }
}
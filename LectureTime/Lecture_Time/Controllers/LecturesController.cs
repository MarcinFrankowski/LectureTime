using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Lecture_Time.Models;

namespace Lecture_Time.Controllers
{
    [Authorize]
    public class LecturesController : Controller
    {
        //private ApplicationDbContext db = new ApplicationDbContext();
        private LectureTimeDbApi LTDbApi = new LectureTimeDbApi();


        // GET: Lectures/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Courses");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Lecture lecture = LTDbApi.GetLecture(id.Value);
            if (lecture == null)
            {
                return HttpNotFound();
            }
            return View(lecture);
        }

        // GET: Lectures/Create
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">course id</param>
        /// <returns></returns>
        public ActionResult Create(int? id)
        {
            //get current user - check if is lecturer of course id or admin
            var currentUser = LTDbApi.GetUser(User.Identity.Name);
            var course = LTDbApi.GetCourse(id);
            if (course == null)
            {
                return RedirectToAction("Details", "Courses", new { id = id });
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!currentUser.IsAdmin && currentUser.Id != course.Lecturer.LTUserId)
            {
                return RedirectToAction("Details", "Courses", new { id = id });
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.CourseId = id;
            return View(new Lecture { CourseId = id.Value });
        }

        // POST: Lectures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Lecture lecture)
        {
            if (!string.IsNullOrEmpty(lecture.VideoLink) && !lecture.VideoLink.Contains("watch?v=") && !lecture.VideoLink.Contains("embed/"))
            {
                ModelState.AddModelError(string.Empty, "Link to youtube video must contain \"watch?v=\" or \"embed/\" part, just like this: https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            }
            //get current user - check if is lecturer of course id or admin
            var currentUser = LTDbApi.GetUser(User.Identity.Name);
            var course = LTDbApi.GetCourse(lecture.CourseId);
            if (course == null)
            {
                return RedirectToAction("Details", "Courses", new { id = lecture.CourseId });
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!currentUser.IsAdmin && currentUser.Id != course.Lecturer.LTUserId)
            {
                return RedirectToAction("Details", "Courses", new { id = lecture.CourseId });
            }

            if (ModelState.IsValid)
            {
                lecture.CreationDate = DateTime.Now;
                if (!string.IsNullOrEmpty(lecture.VideoLink) && lecture.VideoLink.Contains("watch?v="))
                {
                    lecture.VideoLink = lecture.VideoLink.Replace("watch?v=", "embed/");
                }
                LTDbApi.AddLecture(lecture);
                return RedirectToAction("Details", "Courses", new {id = lecture.CourseId });
            }

            ViewBag.CourseId = course.Id;
            return View(lecture);
        }

        // GET: Lectures/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Courses");
            }

            Lecture lecture = LTDbApi.GetLecture(id.Value);
            if (lecture == null)
            {
                return HttpNotFound();
            }

            var currentUser = LTDbApi.GetUser(User.Identity.Name);
            if (!currentUser.IsAdmin && currentUser.Id != lecture.Course.Lecturer.LTUserId)
            {
                return RedirectToAction("Details", new { id = id });
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LectureViewModel model = new LectureViewModel
            {
                AdditionalContent = lecture.AdditionalContent,
                Content = lecture.Content,
                Id = lecture.Id,
                LectureName = lecture.LectureName,
                VideoLink = lecture.VideoLink
            };
            ViewBag.CourseId = lecture.CourseId;
            return View(model);
        }

        // POST: Lectures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(LectureViewModel lectureModel)
        {
            if (!string.IsNullOrEmpty(lectureModel.VideoLink) && !lectureModel.VideoLink.Contains("watch?v=") && !lectureModel.VideoLink.Contains("embed/"))
            {
                ModelState.AddModelError(string.Empty, "Link to youtube video must contain \"watch?v=\" or \"embed/\" part, just like this: https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            }
            if (ModelState.IsValid)
            {
                var currentUser = LTDbApi.GetUser(User.Identity.Name);
                var lecture = LTDbApi.GetLecture(lectureModel.Id);
                if (!currentUser.IsAdmin && currentUser.Id != lecture.Course.Lecturer.LTUserId)
                {
                    return RedirectToAction("Details", new { id = lecture.Id });
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }


                lecture.AdditionalContent = lectureModel.AdditionalContent;
                lecture.Content = lectureModel.Content;
                lecture.LectureName = lectureModel.LectureName;
                if (!string.IsNullOrEmpty(lectureModel.VideoLink) && lectureModel.VideoLink.Contains("watch?v="))
                {
                    lecture.VideoLink = lectureModel.VideoLink.Replace("watch?v=", "embed/");
                }                
                LTDbApi.UpdateLecture(lecture);
                return RedirectToAction("Details", "Courses", new { id = lecture.CourseId });
            }
            return View(lectureModel);
        }


        // GET: Lectures/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            Lecture lecture = LTDbApi.GetLecture(id.Value);
            if (lecture == null)
            {
                return HttpNotFound();
            }

            var currentUser = LTDbApi.GetUser(User.Identity.Name);
            if (!currentUser.IsAdmin && currentUser.Id != lecture.Course.Lecturer.LTUserId)
            {
                return RedirectToAction("Details", new { id = id });
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.CourseId = lecture.CourseId;
            return View(lecture);
        }

        // POST: Lectures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var lecture = LTDbApi.GetLecture(id);
            var currentUser = LTDbApi.GetUser(User.Identity.Name);

            // TODO weź to napisz jak człowiek jak już bedziesz umiał myśleć
            if (!currentUser.IsAdmin && currentUser.Id != lecture.Course.Lecturer.LTUserId)
            {
                return RedirectToAction("Details", new { id = id });
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            LTDbApi.RemoveLecture(lecture);
            return RedirectToAction("Details", "Courses", new { id = lecture.CourseId });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> AddComment(Comment comment)
        {


            var currentUser = LTDbApi.GetUser(User.Identity.Name);
            var lecture = LTDbApi.GetLecture(comment.LectureId);
            ViewBag.CourseId = lecture.CourseId;

            if (!currentUser.IsAdmin && !lecture.Course.LTUsers.Contains(currentUser))
            {
                return RedirectToAction("Details", new { id = comment.LectureId });
            }

            comment.LTUser = currentUser;
            comment.SubmitDate = DateTime.Now;

            lecture.Comments.Add(comment);
            LTDbApi.UpdateLecture(lecture);
            return RedirectToAction("Details", "Lectures", new { id = comment.LectureId });

        }
        //protected override void Dispose(bool disposing)
        //{

        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}

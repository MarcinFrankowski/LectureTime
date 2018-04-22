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
    public class CoursesController : Controller
    {
        //private ApplicationDbContext db = new ApplicationDbContext();
        private LectureTimeDbApi LTDbApi = new LectureTimeDbApi();

        // GET: Courses
        public async Task<ActionResult> Index()
        {
            var currentUser = LTDbApi.GetUser(User.Identity.Name);
            if (currentUser.IsAdmin)
            {
                currentUser.Courses.Clear();
                currentUser.Courses = LTDbApi.GetAllCourses();
            }
            else
            {
                currentUser.Courses = currentUser.Courses ?? new List<Course>();
            }
            return View(currentUser);
        }

        // GET: Courses/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            var courseViewModel =  this.GetCourseViewModel(id.Value ,User.Identity.Name);

            

            if (courseViewModel == null)
            {
                return HttpNotFound();
            }
            return View(courseViewModel);
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            List<LTUser> allStudents = LTDbApi.GetAllStudents();

            var model = new CourseViewModel
            {
                StudentsCollection = new List<SelectListItem>(allStudents
                    .Select(student => new SelectListItem
                    {
                        Text = student.UserName,
                        Value = student.Id.ToString(),
                        Selected = false
                    }
                    ))
            };
            return View(model);
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CourseViewModel courseViewModel)
        {
            if (ModelState.IsValid)
            {
                var currentUser = LTDbApi.GetUser(User.Identity.Name);
                var course = new Course();
                course.AdditionalContent = courseViewModel.Course.AdditionalContent;
                course.Content = courseViewModel.Course.Content;
                course.CourseName = courseViewModel.Course.CourseName;
                course.Results = courseViewModel.Course.Results;

                course.CreationDate = DateTime.Now;
                course.Lecturer = LTDbApi.GetLecturerByUserId(currentUser.Id);
                course.LecturerId = course.Lecturer.LecturerId;
                course.CreationDate = DateTime.Now;
                course.LTUsers = new List<LTUser>();
                course.LTUsers.Add(currentUser);
                var availableStudents = courseViewModel.StudentsCollection?.Where(s => s.Selected).ToList() ?? new List<SelectListItem>();
                foreach (var student in availableStudents)
                {
                    var studentObj = LTDbApi.GetUser(Int32.Parse(student.Value));
                    if (!course.LTUsers.Contains(studentObj))
                    {
                        course.LTUsers.Add(studentObj);
                    }

                }
                LTDbApi.AddCourse(course);

                // db.Course.Add(course);
                //db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(courseViewModel);
        }

        // GET: Courses/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CourseViewModel course = this.GetCourseViewModel(id.Value, User.Identity.Name);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CourseViewModel courseViewModel)
        {

            //Course course = courseViewModel.Course;
            Course course = LTDbApi.GetCourse(courseViewModel.Course.Id);
            var currentUser = LTDbApi.GetUser(User.Identity.Name);

            // TODO weź to napisz jak człowiek jak już bedziesz umiał myśleć
            if (currentUser.IsAdmin){}
            else if (course.Lecturer.LTUser.Id != currentUser.Id)
            {
                ModelState.AddModelError("Błąd","Nie możesz edytować tego kursu");
            }


            if (ModelState.IsValid)
            {
                course.Content = courseViewModel.Course.Content;
                course.CourseName = courseViewModel.Course.CourseName;
                course.AdditionalContent = courseViewModel.Course.AdditionalContent;
                course.Results = courseViewModel.Course.Results;
                course.LTUsers.Clear();
                course.LTUsers.Add(currentUser);
                var availableStudents = courseViewModel.StudentsCollection?.Where(s => s.Selected).ToList() ?? new List<SelectListItem>();
                foreach (var student in availableStudents)
                {
                    var studentObj = LTDbApi.GetUser(Int32.Parse(student.Value));
                    if (!course.LTUsers.Contains(studentObj))
                    {
                        course.LTUsers.Add(studentObj);
                    }
                    
                }

                LTDbApi.UpdateCourse(course);
                return RedirectToAction("Index");
            }
            return View(courseViewModel);
        }

        // GET: Courses/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Course course = LTDbApi.GetCourse(id);
            var currentUser = LTDbApi.GetUser(User.Identity.Name);
            if (course == null)
            {
                return HttpNotFound();
            }
            if (!currentUser.IsAdmin && course.Lecturer.LTUser.Id != currentUser.Id)
            {
                return RedirectToAction("Index");
            }

            var courseModel = this.GetCourseViewModel(id.Value, User.Identity.Name);
            return View(courseModel);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Course course = LTDbApi.GetCourse(id);
            var currentUser = LTDbApi.GetUser(User.Identity.Name);

            if (!currentUser.IsAdmin && course.Lecturer.LTUser.Id != currentUser.Id)
            {
                return RedirectToAction("Index");
            }

            LTDbApi.RemoveCourse(course);
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        private CourseViewModel GetCourseViewModel(int courseId, string userName)
        {
            Course course = LTDbApi.GetCourse(courseId);
            var currentUser = LTDbApi.GetUser(userName);
            List<LTUser> allStudents = LTDbApi.GetAllStudents();
            List<string> addedStudents = course.LTUsers.Select(u => u.UserName).ToList();

            // Get courseViewModel
            CourseViewModel courseViewModel = new CourseViewModel
            {
                Course = course,
                CurrentUser = currentUser,
                StudentsCollection = new List<SelectListItem>(allStudents
                    .Select(student => new SelectListItem
                    {
                        Text = student.UserName,
                        Value = student.Id.ToString(),
                        Selected = addedStudents.Contains(student.UserName) ? true:false
                    }
                    )),
                CanEdit = course.Lecturer.LTUserId == currentUser.Id ? true : false
            };
            return courseViewModel;
        }
    }
}


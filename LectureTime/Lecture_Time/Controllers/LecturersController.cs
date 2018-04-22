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
    public class LecturersController : Controller
    {
        private LectureTimeDbApi LTDbApi = new LectureTimeDbApi();

        // GET: Lecturers/Details
        public async Task<ActionResult> Details()
        {
            var currentUser = LTDbApi.GetUser(User.Identity.Name);

            var lecturer = LTDbApi.GetLecturerByUserId(currentUser.Id);
            if (lecturer == null)
            {
                return HttpNotFound();
            }
            var lecturerModel = new LecturerViewModel
            {
                Description = lecturer.Description,
                Title = lecturer.Title,
                LTUser = currentUser
                
            };
            return View(lecturerModel);
        }

        // GET: Lecturers/Edit
        public async Task<ActionResult> Edit()
        {
            var currentUser = LTDbApi.GetUser(User.Identity.Name);

            var lecturer = LTDbApi.GetLecturerByUserId(currentUser.Id);
            if (lecturer == null)
            {
                return HttpNotFound();
            }
            var lecturerModel = new LecturerViewModel
            {
                Description = lecturer.Description,
                Title = lecturer.Title,
                LTUser = currentUser
            };
            return View(lecturerModel);
        }

        // POST: Lecturers/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(LecturerViewModel lecturerModel)
        {
            var currentUser = LTDbApi.GetUser(User.Identity.Name);

            var lecturer = LTDbApi.GetLecturerByUserId(currentUser.Id);
            if (lecturer == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                lecturer.Description = lecturerModel.Description;
                lecturer.Title = lecturerModel.Title;
                LTDbApi.UpdateLecturer(lecturer);
                return RedirectToAction("Details");
            }
            lecturerModel.LTUser = currentUser;
            return View(lecturerModel);
        }
    }
}

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
    public class AppointmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private LectureTimeDbApi LTDbApi = new LectureTimeDbApi();

        // GET: Shows all cyclic appointments
        public async Task<ActionResult> Index()
        {
            var currentUser = LTDbApi.GetUser(User.Identity.Name);
            List<CyclicAppointment> cAppointments;
            Lecturer lecturer;
            List<Appointment> viewAppointments = new List<Appointment>();
            if (currentUser.IsLecturer)
            {
                lecturer = LTDbApi.GetLecturerByUserId(currentUser.Id);
                cAppointments = LTDbApi.GetCyclicAppointmentForLectrer(lecturer.LecturerId);
                foreach (var capp in cAppointments)
                {
                    viewAppointments.AddRange(capp.Appointments);
                }
            }
            else
            {
                viewAppointments = LTDbApi.GetAppointments(currentUser.Id);
            }

            // Get only 30 last appointments
            viewAppointments = viewAppointments.OrderByDescending(ap => ap.Id).Take(30).ToList();
            LecturersWithAppointmentsViewModel appointmentsViewModel = new LecturersWithAppointmentsViewModel
            {
                Lecturers = LTDbApi.GetAllLecturers(),// GetAllCyclicAppointments().Select(ca => ca.Lecturer).Distinct().ToList(),
                AssignedAppointments = viewAppointments,
                LTUser = currentUser
            };
            return View(appointmentsViewModel);
        }

        // GET: Appointments/Details/5 - get appoitments by lecturer
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cyclicAppointments = LTDbApi.GetCyclicAppointmentForLectrer(id.Value);
            var currentUser = LTDbApi.GetUser(User.Identity.Name);
            var lecturer = LTDbApi.GetLecturerById(id.Value);
            var cavm = cyclicAppointments.Select(ca => new CyclicAppointmentViewModel
            {
                DayOfWeek = ca.DayOfWeek,
                Id = ca.Id,
                Location = ca.Location,
                StartTime = ca.StartTime,
                EndTime = ca.EndTime
            }).ToList();

            if (cyclicAppointments == null)
            {
                return HttpNotFound();
            }
            CyclicAppointmentDetailsViewModel model = new CyclicAppointmentDetailsViewModel
            {
                CurrentUser = currentUser,
                CyclicAppointments = cavm,
                CanEdit = LTDbApi.GetLecturerById(id.Value).LTUserId == currentUser.Id ? true : false,
                Lecturer = lecturer,
                CanAssign = currentUser.IsStudent
                
            };
            return View(model);
        }

        /// <summary>
        /// Accept appointment
        /// </summary>
        /// <param name="id">appointment id</param>
        /// <returns></returns>
        public async Task<ActionResult> AcceptAppointment(int? id)
        {
            var currentUser = LTDbApi.GetUser(User.Identity.Name);
            var appointment = LTDbApi.GetAppointment(id);
            if (currentUser.Id != appointment.CyclicAppointment.Lecturer.LTUserId)
            {
                return View("Index");
            }

            appointment.IsAccepted = true;
            LTDbApi.UpdateAppointment(appointment);

            return RedirectToAction("Index");
        }

            // GET: Appointments/Create
            public ActionResult Create()
        {
            var currentUser = LTDbApi.GetUser(User.Identity.Name);
            if (currentUser.IsStudent && !currentUser.IsAdmin)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CyclicAppointment cyclicAppointment)
        {
            if (ModelState.IsValid)
            {
                var currentUser = LTDbApi.GetUser(User.Identity.Name);
                if (currentUser.IsStudent && !currentUser.IsAdmin)
                {
                    return RedirectToAction("Index");
                }
                var lecturer = LTDbApi.GetLecturerByUserId(currentUser.Id);

                cyclicAppointment.LecturerId = lecturer.LecturerId;

                LTDbApi.AddCyclicAppointment(cyclicAppointment);
                return RedirectToAction("Index");
            }

            return View(cyclicAppointment);
        }

        // GET: Appointments/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            CyclicAppointment cyclicAppointment = LTDbApi.GetCyclicAppointment(id.Value);
            var currentUser = LTDbApi.GetUser(User.Identity.Name);

            if (cyclicAppointment == null)
            {
                //return RedirectToAction("Index");
                return HttpNotFound();
            }

            if (!currentUser.IsAdmin || cyclicAppointment.Lecturer.LTUserId != currentUser.Id)
            {
                return RedirectToAction("Index");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CyclicAppointmentViewModel model = new CyclicAppointmentViewModel
            {
                DayOfWeek = cyclicAppointment.DayOfWeek,
                Location = cyclicAppointment.Location,
                StartTime = cyclicAppointment.StartTime,
                EndTime = cyclicAppointment.EndTime,
                Id = cyclicAppointment.Id
            };
            return View(model);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CyclicAppointmentViewModel cyclicAppointmentViewModel)
        {
            if (ModelState.IsValid)
            {
                var currentUser = LTDbApi.GetUser(User.Identity.Name);
                var cyclicAppointment = LTDbApi.GetCyclicAppointment(cyclicAppointmentViewModel.Id);

                if (!currentUser.IsAdmin && currentUser.Id != cyclicAppointment.Lecturer.LTUserId)
                {
                    return RedirectToAction("Index");
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                cyclicAppointment.DayOfWeek = cyclicAppointmentViewModel.DayOfWeek;
                cyclicAppointment.StartTime = cyclicAppointmentViewModel.StartTime;
                cyclicAppointment.EndTime = cyclicAppointmentViewModel.EndTime;
                cyclicAppointment.Location = cyclicAppointmentViewModel.Location;
                LTDbApi.UpdateCyclicAppointment(cyclicAppointment);
                return RedirectToAction("Details",new { id = cyclicAppointment.LecturerId });
            }
            return View(cyclicAppointmentViewModel);
        }

        // GET: Appointments/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var currentUser = LTDbApi.GetUser(User.Identity.Name);
            var cyclicAppointment = LTDbApi.GetCyclicAppointment(id.Value);

            if (!currentUser.IsAdmin && currentUser.Id != cyclicAppointment.Lecturer.LTUserId)
            {
                return RedirectToAction("Index");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (cyclicAppointment == null)
            {
                return HttpNotFound();
            }
            CyclicAppointmentViewModel model = new CyclicAppointmentViewModel
            {
                DayOfWeek = cyclicAppointment.DayOfWeek,
                Location = cyclicAppointment.Location,
                StartTime = cyclicAppointment.StartTime,
                EndTime = cyclicAppointment.EndTime,
                Id = cyclicAppointment.Id,
                
            };
            return View(model);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var currentUser = LTDbApi.GetUser(User.Identity.Name);
            var cyclicAppointment = LTDbApi.GetCyclicAppointment(id);

            if (!currentUser.IsAdmin && currentUser.Id != cyclicAppointment.Lecturer.LTUserId)
            {
                return RedirectToAction("Index");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (cyclicAppointment == null)
            {
                return HttpNotFound();
            }

            LTDbApi.RemoveCyclicAppointment(cyclicAppointment);
            return RedirectToAction("Index");
        }

        // GET: available cyclic appointment dates dates
        public async Task<ActionResult> Assign(int? id)
        {
            var cyclicAppointment = LTDbApi.GetCyclicAppointment(id.Value);
            var currentUser = LTDbApi.GetUser(User.Identity.Name);

            if(!currentUser.IsStudent)
            {
                return RedirectToAction("Index");
            }

            var appointments = cyclicAppointment.Appointments;

            var allDates = Helpers.GetCyclicDates(cyclicAppointment.StartTime,cyclicAppointment.EndTime, cyclicAppointment.DayOfWeek);
            var assignedDates = appointments.Select(a => a.Date).ToList();
            var availableDates = allDates.Where(date => !assignedDates.Contains(date)).ToList();

            var availableDatesSelectList = availableDates.Select(d => new SelectListItem
            {
                Text = d.ToString("dd.MM.yyyy HH:mm"),
                Value = d.ToString()
            }).ToList();

            AssignAppointmentViewModel model = new AssignAppointmentViewModel
            {
                CyclicAppointmentId = cyclicAppointment.Id,
                AvailableDates = availableDatesSelectList
            };

            return View(model);
        }

        // POST: create an appointment based on cyclic appointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Assign(AssignAppointmentViewModel appointmentViewModel)
        {

            var currentUser = LTDbApi.GetUser(User.Identity.Name);
            if (!currentUser.IsStudent)
            {
                return RedirectToAction("Index");
            }

            var cyclicAppointment = LTDbApi.GetCyclicAppointment(appointmentViewModel.CyclicAppointmentId);
            DateTime appointmentDate = DateTime.Parse(appointmentViewModel.SelectedDate);
            var appoinmtent = new Appointment {
                LTUser = currentUser,
                Date = appointmentDate
            };
            cyclicAppointment.Appointments.Add(appoinmtent);
            LTDbApi.UpdateCyclicAppointment(cyclicAppointment);

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
    }
}

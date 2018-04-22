using Lecture_Time.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lecture_Time
{
    public class LectureTimeDbApi : ILectureTimeDbApi
    {
        private ApplicationDbContext _context = new ApplicationDbContext();

        public void GetUserCourses(int userId)
        {
            _context.Course.Where(c => c.LTUsers.Any(u => u.Id == userId));
        }

        public List<Course> GetAllCourses()
        {
            return _context.Course.ToList();
        }

        public LTUser GetUser(string userName)
        {
            return _context.LTUser.Where(u => u.UserName == userName).SingleOrDefault();
        }

        internal LecturersWithAppointmentsViewModel GetLecturersWithAppointments()
        {
            throw new NotImplementedException();
        }

        public List<CyclicAppointment> GetAllCyclicAppointments()
        {
            return _context.CyclicAppointments.ToList();
        }

        public List<Appointment> GetAppointments(int userId)
        {
            return _context.Appointment.Where(ap => ap.LTUserId == userId || ap.CyclicAppointment.Lecturer.LTUserId == userId).Distinct().ToList();
        }

        public CyclicAppointment GetCyclicAppointment(int id)
        {
            return _context.CyclicAppointments.Where(ca=>ca.Id == id).SingleOrDefault();
        }

        public LTUser GetUser(int userId)
        {
            return _context.LTUser.Where(u => u.Id == userId).SingleOrDefault();
        }

        public List<CyclicAppointment> GetCyclicAppointmentForLectrer(int lecturerId)
        {
           return _context.CyclicAppointments.Where(ca => ca.LecturerId == lecturerId).ToList();
        }

        public List<Lecturer> GetAllLecturers()
        {
            return _context.Lecturer.ToList();
        }

        public Lecture GetLecture(int lectureId)
        {
            return _context.Lecture.Where(l => l.Id == lectureId).SingleOrDefault();
        }

        public List<Lecture> GetLecturesByCourse(int courseId)
        {
            return _context.Lecture.Where(l => l.CourseId == courseId).ToList();
        }


        public void AddUser(LTUser user)
        {

            if (_context.LTUser.Where(u => u.UserName == user.UserName).Count() > 0)
            {
                return;
            }            
            _context.LTUser.Add(user);
            _context.SaveChanges();
        }

        public void AddLecturer(Lecturer lecturer)
        {

            if (_context.LTUser.Where(u => u.UserName == lecturer.LTUser.UserName).Count() > 0)
            {
                return;
                throw new ArgumentException("Lecturer with that user id already exists");
            }
            _context.Lecturer.Add(lecturer);
            _context.SaveChanges();
        }

        public Appointment GetAppointment(int? id)
        {
            return _context.Appointment.Where(ap => ap.Id == id).SingleOrDefault();
        }

        internal void UpdateAppointment(Appointment appointment)
        {
            _context.SaveChanges();
        }

        internal void AddCyclicAppointment(CyclicAppointment cyclicAppointment)
        {
            _context.CyclicAppointments.Add(cyclicAppointment);
            _context.SaveChanges();
        }

        public Course GetCourse(int? id)
        {
            return _context.Course.Where(c => c.Id == id).SingleOrDefault();
        }

        public List<LTUser> GetAllStudents()
        {
            return _context.LTUser.Where(u => u.IsStudent).ToList();
        }

        public void AddCourse(Course course)
        {
            _context.Course.Add(course);
            _context.SaveChanges();
        }

        public Lecturer GetLecturerByUserId(int userId)
        {
            return _context.Lecturer.Where(l => l.LTUserId == userId).SingleOrDefault();
        }

        internal void AddLecture(Lecture lecture)
        {
            _context.Lecture.Add(lecture);
            _context.SaveChanges();
        }

        public Lecturer GetCourseOwner(int courseId)
        {
            return _context.Course.Where(c => c.Id == courseId).Single().Lecturer;
        }

        public void UpdateCourse(Course course)
        {
            _context.SaveChanges();
        }
        public void UpdateLecturer(Lecturer lecturer)
        {
            _context.SaveChanges();
        }

        public void RemoveCourse(Course course)
        {
            _context.Course.Remove(course);
            _context.SaveChanges();
        }

        public void UpdateLecture(Lecture lecture)
        {
            _context.SaveChanges();
        }

        public void UpdateCyclicAppointment(CyclicAppointment cyclicAppointment)
        {
            _context.SaveChanges();
        }

        public Lecturer GetLecturerById(int lecturerId)
        {
            return _context.Lecturer.Where(l => l.LecturerId == lecturerId).SingleOrDefault();
        }

        public void RemoveLecture(Lecture lecture)
        {
            _context.Lecture.Remove(lecture);
            _context.SaveChanges();
        }

        public void SetAdmin(LTUser lTUser)
        {
            lTUser.IsAdmin = true;
            lTUser.IsLecturer = true;
            _context.SaveChanges();
        }

        public void AddComment(Comment comment)
        {
            _context.Comment.Add(comment);
            _context.SaveChanges();
        }

        public void RemoveCyclicAppointment(CyclicAppointment cyclicAppointment)
        {
            var childAppointments = _context.Appointment.Where(ap => ap.CyclicAppointmentId == cyclicAppointment.Id);
            _context.Appointment.RemoveRange(childAppointments);
            _context.CyclicAppointments.Remove(cyclicAppointment);
            _context.SaveChanges();
        }
    }
}
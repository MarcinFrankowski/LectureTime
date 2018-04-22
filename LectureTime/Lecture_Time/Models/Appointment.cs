using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lecture_Time.Models
{
    public class Appointment
    {

        /// <summary>
        /// Appointment id
        /// </summary>
        [Required]
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Appointment date
        /// </summary>
        [DisplayName("Asssigned appointment date")]
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime? Date { get; set; }

        public bool IsAccepted { get; set; }


        // DB RELATIONS

        /// <summary>
        /// Assigned student id
        /// </summary>
        public int? LTUserId { get; set; }

        /// <summary>
        /// Assigned user navigation property
        /// </summary>
        public virtual LTUser LTUser { get; set; }

        public int? CyclicAppointmentId { get; set; }

        public virtual CyclicAppointment CyclicAppointment { get; set; }


    }
    public class CyclicAppointment
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [DisplayName("Day of the week")]
        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [DisplayName("Start time")]
        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        [Column(TypeName = "datetime2")]
        public DateTime StartTime { get; set; }

        [DisplayName("End time")]
        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        [Column(TypeName = "datetime2")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Appointment location
        /// </summary>
        [DisplayName("Location")]
        [Required]
        public string Location { get; set; }
        /// <summary>
        /// Owner lecturer id
        /// </summary>
        [Required]
        public int LecturerId { get; set; }

        /// <summary>
        /// LEcturer navigation property
        /// </summary>
        public virtual Lecturer Lecturer { get; set; }

        public virtual IList<Appointment> Appointments { get; set; }

    }

    [NotMapped]
    public class CyclicAppointmentViewModel
    {
        public int Id { get; set; }

        [DisplayName("Day of the week")]
        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [DisplayName("Start time")]
        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public DateTime StartTime { get; set; }

        [DisplayName("End time")]
        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Appointment location
        /// </summary>
        [DisplayName("Location")]
        [Required]
        public string Location { get; set; }

        public List<Appointment> Appointments { get; set; }
    }

    [NotMapped]
    public class CyclicAppointmentDetailsViewModel
    {
        public List<CyclicAppointmentViewModel> CyclicAppointments { get; set; }
        public LTUser CurrentUser { get; set; }
        public bool CanEdit { get; set; }
        public bool CanAssign { get; set; }
        public Lecturer Lecturer { get; set; }
    }

    [NotMapped]
    public class LecturersWithAppointmentsViewModel
    {
        public List<Lecturer> Lecturers { get; set; }
        public List<Appointment> AssignedAppointments { get; set; }
        public LTUser LTUser { get; set; }
    }

    [NotMapped]
    public class AssignAppointmentViewModel
    {
        public int CyclicAppointmentId { get; set; }

        public string SelectedDate { get; set; }
        public List<SelectListItem> AvailableDates { get; set; }
        public LTUser LTUser { get; set; }


    }
}

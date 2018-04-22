using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Lecture_Time.Models
{
    public class LTUser
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        [Key]
        [Column(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }

        public string Email { get; set; }

        [Required]
        [StringLength(450)]
        [Index(IsUnique = true)]
        public string UserName { get; set; }

        /// <summary>
        /// Lecturer flag
        /// </summary>
        public bool IsLecturer { get; set; }
        /// <summary>
        /// Student flag
        /// </summary>
        public bool IsStudent { get; set; }
        /// <summary>
        /// Admin flag
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Appointments assigned to the user
        /// </summary>
        public virtual IList<Appointment> Appointments { get; set; } //= new List<Appointment>();

        /// <summary>
        /// Courses assigned to the user
        /// </summary>
        public virtual IList<Course> Courses { get; set; }// = new List<Course>();

        public virtual IList<Comment> Comments { get; set; }// = new List<Comment>();
    }
}

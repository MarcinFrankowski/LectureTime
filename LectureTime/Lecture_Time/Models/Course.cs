using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.ComponentModel;

namespace Lecture_Time.Models
{
    public class Course
    {
        /// <summary>
        /// Course id
        /// </summary>
        [Required]
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Course display name
        /// </summary>
        [DisplayName("Name")]
        public string CourseName { get; set; }


        /// <summary>
        /// Course creation date
        /// </summary>
        [DisplayName("Creation date")]
        public DateTime? CreationDate { get; set; }


        /// <summary>
        /// Course description
        /// </summary>
        [DisplayName("Description")]
        public string Content { get; set; }

        /// <summary>
        /// Course additional content
        /// </summary>
        [DisplayName("Literature")]
        public string AdditionalContent { get; set; }

        /// <summary>
        /// Results - a link to results
        /// </summary>
        [DisplayName("Results")]
        public string Results { get; set; }


        /// <summary>
        /// Course lectures list
        /// </summary>
        public virtual IList<Lecture> Lectures { get; set; }

        public virtual IList<LTUser> LTUsers { get; set; }

        public int? LecturerId { get; set; }
        public virtual Lecturer Lecturer { get; set; }

    }
}

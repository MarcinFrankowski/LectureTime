using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Lecture_Time.Models
{

    /// <summary>
    /// DB Entity class
    /// </summary>
    public class Lecturer
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        [Key]
        public int LecturerId { get; set; }

        /// <summary>
        /// Lecturer description
        /// </summary>
        [DisplayName("Lecturer description")]
        public string Description { get; set; }

        
        public int LTUserId { get; set; }
        public virtual LTUser LTUser { get; set; }
        /// <summary>
        /// Lecturers scientific title
        /// </summary>
        [DisplayName("Lecturer title")]
        public string Title { get; set; }
    }

    [NotMapped]
    public class LecturerViewModel
    {
        [DisplayName("Lecturer description")]
        public string Description { get; set; }

        [DisplayName("Lecturer title")]
        public string Title { get; set; }

        public LTUser LTUser { get; set; }
    }
}

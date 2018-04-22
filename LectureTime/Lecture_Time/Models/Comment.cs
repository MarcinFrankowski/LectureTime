using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace Lecture_Time.Models
{
    public class Comment
    {
        /// <summary>
        /// Comment id
        /// </summary>
        [Required]
        [Key]
        public int Id { get; set; }



        /// <summary>
        /// Submision date
        /// </summary>
        [Required]
        public DateTime? SubmitDate { get; set; }

        /// <summary>
        /// Comment content
        /// </summary>
        [DisplayName("Comment content")]
        public string CommentContent { get; set; }


        // DB Relations

        /// <summary>
        /// Assigned student id
        /// </summary>
        public int LTUserId { get; set; }

        /// <summary>
        /// Assigned user navigation property
        /// </summary>
        public virtual LTUser LTUser { get; set; }

        /// <summary>
        /// Corresponding lecture id
        /// </summary>
        public int LectureId { get; set; }

        /// <summary>
        /// Corresponding lecture navigation property
        /// </summary>
        public virtual Lecture Lecture { get; set; }


    }
}
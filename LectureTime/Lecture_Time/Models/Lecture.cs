using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Lecture_Time.Models
{
    public class Lecture
    {
        /// <summary>
        /// Lecture id
        /// </summary>
        [Required]
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Lecture display name
        /// </summary>
        [Required]
        [DisplayName("Title")]
        public string LectureName { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        [DisplayName("Creation date")]
        public DateTime? CreationDate { get; set; }

        /// <summary>
        /// Content / description
        /// </summary>
        [DisplayName("Description")]
        public string Content { get; set; }

        /// <summary>
        /// Additional content
        /// </summary>
        [DisplayName("Learning materials")]
        public string AdditionalContent { get; set; }

        /// <summary>
        /// Link to youtube video. Must contain "watch?v=" part, just like this: https://www.youtube.com/watch?v=dQw4w9WgXcQ
        /// </summary>
        [DisplayName("YouTube video link")]
        public string VideoLink { get; set; }

        /// <summary>
        /// List of comments associated to this lecture
        /// </summary>
        public virtual IList<Comment> Comments { get; set; }

        /// <summary>
        /// Id of a parent course
        /// </summary>
        [Required]
        public int CourseId { get; set; }

        public virtual Course Course { get; set; }
    }
    
    [NotMapped]
    public class LectureViewModel
    {
        /// <summary>
        /// Lecture id
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Lecture display name
        /// </summary>
        [Required]
        [DisplayName("Title")]
        public string LectureName { get; set; }

        /// <summary>
        /// Content / description
        /// </summary>
        [DisplayName("Description")]
        public string Content { get; set; }

        /// <summary>
        /// Additional content
        /// </summary>
        [DisplayName("Learning materials")]
        public string AdditionalContent { get; set; }

        /// <summary>
        /// Link to youtube video. Must contain "watch?v=" part, just like this: https://www.youtube.com/watch?v=dQw4w9WgXcQ
        /// </summary>
        [DisplayName("YouTube video link")]
        public string VideoLink { get; set; }

    }
}

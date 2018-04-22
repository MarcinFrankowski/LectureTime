using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lecture_Time.Models
{
    [NotMapped]
    public class CoursesViewModel 
    {
        public LTUser CurrentUser { get; set; }

        public bool CanEdit { get; set; }
    }

    [NotMapped]
    public class CourseViewModel
    {
        public Course Course { get; set; }

        public LTUser CurrentUser { get; set; }

        public bool CanEdit { get; set; }

        public List<SelectListItem> StudentsCollection { get; set; }
    }
}

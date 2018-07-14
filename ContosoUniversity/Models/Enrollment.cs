using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoUniversity.Models
{

    public enum Grade
    {
        A, B, C, D, E, F
    }

    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        public int CourseID { get; set; }        // foreign key and the corresponding navigation property is Coure below
        public int StudentID { get; set; }      // foreign key and the corresponding navigation property is Student below
        public Grade? Grade { get; set; }     // the question mark indicates that the grade property is nullable

        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }



    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using ContosoUniversity.Models;


// There are 4 db initialization stratagies:
// CreateDatabaseIfNotExists
// DropCreateDatabaseIfModelChanges
// DropCreateDatabaseAlways
// Custom DB Initializer: create your own
// Creating own custom db initializer class "SchoolInitializer" by inheriting from the initializer "DropCreateDatabaseIfModelChanges" 
// This separates the database initialization from a context class. 
// Added element <contexts> <context type  <databaseInitializer type .... to Web.config so the initializer class is used.
// The creating and setting of the initializer can also be done in the config files <appSettings><add key="..... "</appSettings>
// NOTE: When app is deployed, change the db initialization to the correct value so the db is not deleted.

namespace ContosoUniversity.DAL
{ 

    public class SchoolInitializer : System.Data.Entity.DropCreateDatabaseAlways<SchoolContext>
    {
        // Seed method takes db context object (context class with 3 DbSets) as input paramter and adds new entities to database (test data) 
         protected override void Seed(SchoolContext context)
        {
            var students = new List<Student>
            {
            new Student{FirstMidName="Kate",LastName="Wannell",EnrollmentDate=DateTime.Parse("1966-10-07"),Email="kate.com"},
            new Student{FirstMidName="Natalie",LastName="Ford",EnrollmentDate=DateTime.Parse("2002-12-30"),Email="natalie.com"},
            new Student{FirstMidName="Jerry",LastName="Black",EnrollmentDate=DateTime.Parse("2003-06-25"),Email="jerry.com"},
             };

            // s is the variable name for each item in the collection that will be added to the Students DbSet
            students.ForEach(s => context.Students.Add(s));

            // Not necessary to save the changes after each group of entities but doing so will help you find the 
            // source of the problem if an exception occurs whilst the code is writing to the database. 
            context.SaveChanges();

            var courses = new List<Course>
            {
            new Course{CourseID=1050,Title="Chemistry",Credits=3,},
            new Course{CourseID=4022,Title="Microeconomics",Credits=3,},
            new Course{CourseID=4041,Title="Macroeconomics",Credits=3,},
            new Course{CourseID=1045,Title="Calculus",Credits=4,},
            new Course{CourseID=3141,Title="Trigonometry",Credits=4,},
            new Course{CourseID=2021,Title="Composition",Credits=3,},
            new Course{CourseID=2042,Title="Literature",Credits=4,}
            };
            courses.ForEach(s => context.Courses.Add(s));
            context.SaveChanges();

            var enrollments = new List<Enrollment>
            {
            new Enrollment{StudentID=1,CourseID=1050,Grade=Grade.A},
            new Enrollment{StudentID=1,CourseID=4022,Grade=Grade.C},
            new Enrollment{StudentID=1,CourseID=4041,Grade=Grade.B},
            new Enrollment{StudentID=2,CourseID=1045,Grade=Grade.B},
            new Enrollment{StudentID=2,CourseID=3141,Grade=Grade.F},
            new Enrollment{StudentID=2,CourseID=2021,Grade=Grade.F},
            new Enrollment{StudentID=3,CourseID=1050},

            };
            enrollments.ForEach(s => context.Enrollments.Add(s));
            context.SaveChanges();

        }
    }
}
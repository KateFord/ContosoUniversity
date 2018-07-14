using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContosoUniversity.Models;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity;

namespace ContosoUniversity.DAL
{
    // SchoolContext is inheriting from the System.Data.Entity.DbContext class
    public class SchoolContext : DbContext
    {
        // Constructor with a connection string explicitely named and passed in. If one is not named and passed in, 
        // the Entity Framework assumes the connection string is the same name as the class, so in this instance 
       // SchoolContext. Must be good practise to be explicit. 
        public SchoolContext() : base("SchoolContext")
        {
        }

        //DbSet or entity set usually corresponds to a database table
        //An Entity corresponds to a row in the table
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Course> Courses { get; set; }

        //Note: 
        // Could have omitted the DbSet<Enrollment> and DbSet<Course> statements
        // Entity Framework would include them implicitly because the student entity
        // references the enrollment entity and the enrollment entity references the course entity. 

    }
}
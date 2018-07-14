using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using PagedList;

// ViewBag Explained:

// ViewBag is a property – considered a dynamic object without pre-set properties – that enables you to share values dynamically between the controller 
// and view within ASP.NET MVC applications. public object ViewBag { get; }
//You can define the properties that you want by adding them, and you would need to use the same property name if you want to
//retrieve these values in view. In the controller, you can set up properties such as:

// public ActionResult Index() 
//{
//ViewBag.Title = "Put your page title here";
//ViewBag.Description = "Put your page description here";
//ViewBag.UserNow = new User()
//{
//  Name = "Your Name",
//   ID = 4,
//};
// return View();
//}

//To display these properties in view, you would need to use the same property names. 

//<h3>@ViewBag.Title</h3>
//<p>@ViewBag.Description</p>
//Your name:
//<div>
//<dl>
//<dt>Name:</dt>
//<dd>@ViewBag.UserNow.Name</dd>
//<dt>ID:</dt>
//<dd>@ViewBag.CurrentUser.ID</dd>
//</dl>
//</div>

//The LINQ query syntax starts with from keyword and ends with select keyword.
// string collection
//            IList<string> stringList = new List<string>() {
//    "C# Tutorials",
//    "VB.NET Tutorials",
//    "Learn C++",
//    "MVC Tutorials" ,
//    "Java"
//};

// LINQ Query Syntax
//  var result = from s in stringList
//                       where s.Contains("Tutorials")
//                       select s;

// result: Result variable
// s: Range variable
// strList: Sequence (IEnumerable or Iqueryable collection)
// where: Standard Query Operator
// select: Standard Query Operator
// Contains: Conditional expression

//Query syntax starts with a From clause followed by a Range variable. The From clause is structured
//like "From rangeVariableName in IEnumerablecollection".In English, this means, from each object in
//the collection.It is similar to a foreach loop: foreach (Student s in studentList).

//After the From clause, you can use different Standard Query Operators to filter, group, join elements of
//the collection.There are around 50 Standard Query Operators available in LINQ.In the above figure, we
//have used "where" operator (aka clause) followed by a condition. This condition is generally expressed
//using lambda expression.

// LINQ query syntax always ends with a Select or Group clause.The Select clause is used to shape the data.
//You can select the whole object as it is or only some properties of it.In the above example, we selected the
//each resulted string elements.

// NOTE:
//In many cases you can call the same method either on an Entity Framework entity set or as an extension method
//on an in-memory collection.The results are normally the same but in some cases may be different.

// For example, the .NET Framework implementation of the Contains method returns all rows when you pass an empty string
// to it, but the Entity Framework provider for SQL Server Compact 4.0 returns zero rows for empty strings. Therefore
// the code in the example(putting the Where statement inside an if statement) makes sure that you get the same 
// results for all versions of SQL Server.Also, the.NET Framework implementation of the Contains method performs a
// case-sensitive comparison by default, but Entity Framework SQL Server providers perform case-insensitive comparisons
// by default.Therefore, calling the ToUpper method to make the test explicitly case-insensitive ensures that results do not 
// change when you change the code later to use a repository, which will return an IEnumerable collection instead of an
// IQueryable object. (When you call the Contains method on an IEnumerable collection, you get the.NET Framework
// implementation; when you call it on an IQueryable object, you get the database provider implementation.)

//Null handling may also be different for different database providers or when you use an IQueryable object compared to when
//you use an IEnumerable collection.For example, in some scenarios a Where condition such as table.Column != 0 may not return
//columns that have null as the value.For more information, see Incorrect handling of null variables in 'where' clause.

namespace ContosoUniversity.Controllers
{
    public class StudentController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: Students
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            // ViewBag properties and ternary statements to determine the order by clause. 
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var students = from s in db.Students              
                           select s;

            // LINQ statement to add a where clause ... where last name or firstmid name contains the search string
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString)
                                                              || s.FirstMidName.Contains(searchString));
            }
 
            // Order by the appropriate property depending upon the value of sortOrder
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 3;
            // The ?? operator is called the null-coalescing operator. It returns the left-hand operand if the operand is not null; otherwise
            // it returns the right hand operand.
            int pageNumber = (page ?? 1);
            return View(students.ToPagedList(pageNumber, pageSize));
        }

        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        //This code adds the Student entity created by the ASP.NET MVC model binder to the Students entity 
        //set and then saves the changes to the database. (Model binder refers to the ASP.NET MVC functionality
        //that makes it easier for you to work with data submitted by a form; a model binder converts posted form
        //values to CLR types and passes them to the action method in parameters.In this case, the model binder
        //instantiates a Student entity for you using property values from the Form collection.)

        //Security warning - The ValidateAntiForgeryToken attribute helps prevent cross-site request forgery attacks.It requires
        //a corresponding Html.AntiForgeryToken() statement in the view, which you'll see later.

        // BIND: a way to protect against over-posting in create scenarios, for example adding &Secret=OverPost 

        // An alternative way to prevent overposting that is preferred by many developers is to use view models rather
        // than entity classes with model binding.Include only the properties you want to update in the view model.
        // Once the MVC model binder has finished, copy the view model properties to the entity instance, optionally
        // using a tool such as AutoMapper.Use db.Entry on the entity instance to set its state to Unchanged, and then
        // set Property("PropertyName").IsModified to true on each entity property that is included in the view model.
        // This method works in both edit and create scenarios.

[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName,FirstMidName,EnrollmentDate,Email")] Student student)  // id will be auto populated
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            catch (DataException /* dex */)
            {
                // Log the error (uncomment dex varaible name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Find method is used to retrieve the selected student entity (row in the dbset
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        // Changed scaffolded code to prevent overposting and ....
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ID,LastName,FirstMidName,EnrollmentDate,Email")] Student student)

        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(student).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //     }
        //     return View(student);
        // }

        //These changes implement a security best practice to prevent overposting, The scaffolder generated a Bind
        //attribute and added the entity created by the model binder to the entity set with a Modified flag. That code
        //is no longer recommended because the Bind attribute clears out any pre-existing data in fields not listed in
        //the Include parameter. In the future, the MVC controller scaffolder will be updated so that it doesn't generate
        //Bind attributes for Edit methods.

        //The new code reads the existing entity and calls TryUpdateModel to update fields from user input in the posted
        //form data.The Entity Framework's automatic change tracking sets the Modified flag on the entity. When the SaveChanges
        //method is called, the Modified flag causes the Entity Framework to create SQL statements to update the database row.
        //Concurrency conflicts are ignored, and all columns of the database row are updated, including those that the user didn't
        //change. (A later tutorial shows how to handle concurrency conflicts, and if you only want individual fields to be updated
        //in the database, you can set the entity to Unchanged and set individual fields to Modified.)

        //As a best practice to prevent overposting, the fields that you want to be updateable by the Edit page are whitelisted in the
        //TryUpdateModel parameters.Currently there are no extra fields that you're protecting, but listing the fields that you want
        //the model binder to bind ensures that if you add fields to the data model in the future, they're automatically protected until 
        //you explicitly add them here.

        //As a result of these changes, the method signature of the HttpPost Edit method is the same as the HttpGet edit method; 
        //therefore you've renamed the method EditPost.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var studentToUpdate = db.Students.Find(id);

            if (TryUpdateModel(studentToUpdate, "",
               new string[] { "LastName", "FirstMidName", "EnrollmentDate" }))
            {
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(studentToUpdate);
        }


        // GET: Students/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Student student = db.Students.Find(id);
        //    if (student == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(student);
        //}

        // Replaced the scaffolded GET: 
        // Added a try-catch block to the HttpPost Delete method to handle any errors that might occur when the database is updated.
        //If an error occurs, the HttpPost Delete method calls the HttpGet Delete method, passing it a parameter that indicates that an
        //error has occurred. The HttpGet Delete method then redisplays the confirmation page along with the error message, giving the
        //user an opportunity to cancel or try again.
        //This code accepts an optional parameter that indicates whether the method was called after a failure to save changes.
        //This parameter is false when the HttpGet Delete method is called without a previous failure.When it is called by the HttpPost
        //Delete method in response to a database update error, the parameter is true and an error message is passed to the view.
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }


        // POST: Students/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Student student = db.Students.Find(id);
        //    db.Students.Remove(student);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        // Replaced scaffolded code above, this method performs the actual delete operation and catches any database update errors.
        // This code retrieves the selected entity, then calls the Remove method to set the entity's status to Deleted. When SaveChanges
        //is called, a SQL DELETE command is generated. You have also changed the action method name from DeleteConfirmed to Delete.
        // The scaffolded code named the HttpPost Delete method DeleteConfirmed to give the HttpPost method a unique signature. 
        //( The CLR requires overloaded methods to have different method parameters.) Now that the signatures are unique, you can stick
        // with the MVC convention and use the same name for the HttpPost and HttpGet delete methods.

        //As noted, the HttpGet Delete method doesn't delete the data. Performing a delete operation in response to a GET request
        //(or for that matter, performing any edit operation, create operation, or any other operation that changes data) creates 
        //a security risk. For more information, see ASP.NET MVC Tip #46 — Don't use Delete Links because they create Security Holes
        //on Stephen Walther's blog.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Student student = db.Students.Find(id);
                db.Students.Remove(student);

                //If improving performance in a high-volume application is a priority, you could avoid an unnecessary SQL query to retrieve
                //the row by replacing the lines of code that call the Find and Remove methods with the following code:
                //Student studentToDelete = new Student() { ID = id };
                //db.Entry(studentToDelete).State = EntityState.Deleted;
                // This code instantiates a Student entity using only the primary key value and then sets the entity state to Deleted.
                //That's all that the Entity Framework needs in order to delete the entity.

                db.SaveChanges();
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }


        //To close database connections and free up the resources they hold as soon as possible, dispose of the context instance when you
        //are done with it. Scaffolding code creates this method. The base Controller class already implements the IDisposable interface, so
        //this code simply adds an override to the Dispose(bool) method to explicitly dispose the context instance.
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

//Entity States and the Attach and SaveChanges Methods

//The database context keeps track of whether entities in memory are in sync with their corresponding rows in the database, 
//    and this information determines what happens when you call the SaveChanges method. For example, when you pass a new
//    entity to the Add method, that entity's state is set to Added. Then when you call the SaveChanges method, the database context
//issues a SQL INSERT command.

//An entity may be in one of thefollowing states:

//Added.The entity does not yet exist in the database.The SaveChanges method must issue an INSERT statement.
//Unchanged.Nothing needs to be done with this entity by the SaveChanges method.When you read an entity from the database,
//    the entity starts out with this status.
//Modified.Some or all of the entity's property values have been modified. The SaveChanges method must issue an UPDATE statement.
//Deleted.The entity has been marked for deletion.The SaveChanges method must issue a DELETE statement.
//Detached.The entity isn't being tracked by the database context.
//In a desktop application, state changes are typically set automatically. In a desktop type of application, you read an entity and make
//    changes to some of its property values.This causes its entity state to automatically be changed to Modified.Then when you call
//    SaveChanges, the Entity Framework generates a SQL UPDATE statement that updates only the actual properties that you changed.

//The disconnected nature of web apps doesn't allow for this continuous sequence. The DbContext that reads an entity is disposed
// after a page is rendered. When the HttpPost Edit action method is called, a new request is made and you have a new instance of 
// the DbContext, so you have to manually set the entity state to Modified. Then when you call SaveChanges, the Entity Framework
// updates all columns of the database row, because the context has no way to know which properties you changed.


//If you want the SQL Update statement to update only the fields that the user actually changed, you can save the original values in
//        some way (such as hidden fields) so that they are available when the HttpPost Edit method is called.Then you can create a Student
//    entity using the original values, call the Attach method with that original version of the entity, update the entity's values to the new
//values, and then call SaveChanges. For more information, see Entity states and SaveChanges and Local Data in the MSDN Data Developer
//    Center.

//Handling Transactions
//By default the Entity Framework implicitly implements transactions.In scenarios where you make changes to multiple rows or
//tables and then call SaveChanges, the Entity Framework automatically makes sure that either all of your changes succeed or
//all fail. If some changes are done first and then an error happens, those changes are automatically rolled back. For scenarios 
//where you need more control -- for example, if you want to include operations done outside of Entity Framework in a transaction -- see
//Working with Transactions on MSDN.


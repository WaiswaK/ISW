using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ISW.Models;

namespace ISW.Controllers
{
    public class LeavesController : Controller
    {
        private static ISWEntities db = new ISWEntities();

        // GET: Leaves
        public ActionResult Index()
        {
            var leaves = db.Leaves.Include(l => l.Employee);
            return View(leaves.ToList());
        }

        // GET: Leaves/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Leave leave = db.Leaves.Find(id);
            if (leave == null)
            {
                return HttpNotFound();
            }
            return View(leave);
        }

        // GET: Leaves/Create
        public ActionResult Create()
        {
            ViewBag.Employee_ID = new SelectList(db.Employees, "Employee_ID", "First_Name");
            return View();
        }

        // POST: Leaves/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Leave_ID,Employee_ID,Start_Date,End_Date")] Leave leave)
        {
            
            //Get Department ID of staff
            var current_employee = db.Employees.Where(c => c.Employee_ID == leave.Employee_ID).Single();
            //Get all staff from same Department
            var employeesquery = db.Employees.Where(c => c.Department_ID == current_employee.Department_ID).ToList();
            //Get List of all employees in Leave List
            var leave_list = db.Leaves.ToList().ToList();
            

            if (Found(leave_list, current_employee.Department_ID, leave.Start_Date, leave.End_Date))
            {
                ModelState.AddModelError("", "This leave request can't be approved due to overlapong dates");
                return View(leave);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    db.Leaves.Add(leave);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.Employee_ID = new SelectList(db.Employees, "Employee_ID", "First_Name", leave.Employee_ID);
                return View(leave);
            }           
        }

        // GET: Leaves/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Leave leave = db.Leaves.Find(id);
            if (leave == null)
            {
                return HttpNotFound();
            }
            ViewBag.Employee_ID = new SelectList(db.Employees, "Employee_ID", "First_Name", leave.Employee_ID);
            return View(leave);
        }

        // POST: Leaves/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Leave_ID,Employee_ID,Start_Date,End_Date")] Leave leave)
        {
            if (ModelState.IsValid)
            {
                db.Entry(leave).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Employee_ID = new SelectList(db.Employees, "Employee_ID", "First_Name", leave.Employee_ID);
            return View(leave);
        }

        // GET: Leaves/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Leave leave = db.Leaves.Find(id);
            if (leave == null)
            {
                return HttpNotFound();
            }
            return View(leave);
        }

        // POST: Leaves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Leave leave = db.Leaves.Find(id);
            db.Leaves.Remove(leave);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        //Compare a single date to see if it over laps
        private static bool DateOverlap(DateTime givendate, DateTime startdate, DateTime enddate)
        {
            if (DateTime.Compare(startdate, givendate) < 0 && DateTime.Compare(enddate, givendate) > 0)
                return true;
            if (DateTime.Compare(startdate, givendate) == 0 || DateTime.Compare(enddate, givendate) == 0)
                return true;
            else return false;
        }
        //Checking if found Employee from list is in same department
        private static bool Exists(string department, Employee employee)
        {
            if (employee.Department_ID == department)
                return true;
            else return false;
        }
        //Check if Found Employee in leave list is in same department as current and dates are overlaping
        private static bool Found(List<Leave> leave, string department, DateTime startdate, DateTime enddate)
        {
            bool final = false;
            foreach (var leave_item in leave)
            {

                if (Exists(department, GetEmployee(leave_item.Employee_ID)))
                {
                    if (DateOverlap(startdate, leave_item.Start_Date, leave_item.End_Date))
                        final = true;
                    else
                    {
                        if(DateOverlap(enddate, leave_item.Start_Date, leave_item.End_Date))
                        final = true;
                    }
                }
            }
            return final;
        }
        //Get Employee from id
        private static Employee GetEmployee(string employeeid)
        {
            return db.Employees.Where(c => c.Employee_ID == employeeid).Single();
        }
    }
}

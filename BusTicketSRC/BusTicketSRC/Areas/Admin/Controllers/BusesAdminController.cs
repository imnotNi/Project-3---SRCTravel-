using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BusTicketSRC.Models;

namespace BusTicketSRC.Areas.Admin.Controllers
{
    public class BusesAdminController : Controller
    {
        private BusTicketDB db = new BusTicketDB();

        // GET: Admin/BusesAdmin
        public ActionResult Index()
        {
            return View(db.Buses.ToList());
        }

        // GET: Admin/BusesAdmin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bus bus = db.Buses.Find(id);
            if (bus == null)
            {
                return HttpNotFound();
            }
            return View(bus);
        }

        // GET: Admin/BusesAdmin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/BusesAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "bus_id,bus_type,capacity,plate_number,other_bus_details")] Bus bus)
        {
            if (ModelState.IsValid)
            {
                db.Buses.Add(bus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bus);
        }

        // GET: Admin/BusesAdmin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bus bus = db.Buses.Find(id);
            if (bus == null)
            {
                return HttpNotFound();
            }
            return View(bus);
        }

        // POST: Admin/BusesAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "bus_id,bus_type,capacity,plate_number,other_bus_details")] Bus bus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bus);
        }

        // GET: Admin/BusesAdmin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bus bus = db.Buses.Find(id);
            if (bus == null)
            {
                return HttpNotFound();
            }
            return View(bus);
        }

        // POST: Admin/BusesAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bus bus = db.Buses.Find(id);
            db.Buses.Remove(bus);
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
    }
}

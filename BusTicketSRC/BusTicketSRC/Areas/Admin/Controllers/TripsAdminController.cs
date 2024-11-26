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
    public class TripsAdminController : Controller
    {
        private BusTicketDB db = new BusTicketDB();

        // GET: Admin/TripsAdmin
        public ActionResult Index(string start_locationSearch = "", string end_locationSearch = "", string departure_daySearch = "")
        {
            using (BusTicketDB db = new BusTicketDB())
            {
                // Lấy danh sách unique sources và destinations từ database
                var start_locationOptions = db.Trips.Select(t => t.start_location).Distinct().ToList();
                var end_locationOptions = db.Trips.Select(t => t.end_location).Distinct().ToList();
                var departure_dayOptions = db.Trips.Select(t => t.departure_day).Distinct().ToList();

                // Gửi danh sách options lên View thông qua ViewBag
                ViewBag.Start_locationOptions = start_locationOptions;
                ViewBag.End_locationOptions = end_locationOptions;
                ViewBag.Departure_dayOptions = departure_dayOptions;

                // Lấy danh sách Trip từ database dựa trên điều kiện tìm kiếm
                List<Trip> trips = db.Trips
                    .Include(t => t.Bus) // Include Bus information
                    .Where(row =>
                        (string.IsNullOrEmpty(start_locationSearch) || row.start_location.ToLower().Contains(start_locationSearch.ToLower())) &&
                        (string.IsNullOrEmpty(end_locationSearch) || row.end_location.ToLower().Contains(end_locationSearch.ToLower())) &&
                        (string.IsNullOrEmpty(departure_daySearch) || row.departure_day.ToString().Contains(departure_daySearch)))
                    .ToList();


                // Gửi dữ liệu tìm kiếm và danh sách Trip lên View
                ViewBag.Start_locationSearch = start_locationSearch;
                ViewBag.End_locationSearch = end_locationSearch;
                ViewBag.Departure_daySearch = departure_daySearch;
                return View(trips);
            }
        }

        // GET: Admin/TripsAdmin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = db.Trips.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            return View(trip);
        }

        // GET: Admin/TripsAdmin/Create
        public ActionResult Create()
        {
            ViewBag.bus_id = new SelectList(db.Buses, "bus_id", "bus_type");
            return View();
        }

        // POST: Admin/TripsAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "trip_id,bus_id,start_location,end_location,distance,departure_time,departure_day,Price,other_trip_details")] Trip trip)
        {
            // Kiểm tra nếu trip_id đã tồn tại
            if (db.Trips.Any(t => t.trip_id == trip.trip_id))
            {
                ModelState.AddModelError("trip_id", "Trip ID already exists. Please choose a different one.");
            }

            if (ModelState.IsValid)
            {
                db.Trips.Add(trip);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.bus_id = new SelectList(db.Buses, "bus_id", "bus_type", trip.bus_id);
            return View(trip);
        }


        // GET: Admin/TripsAdmin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = db.Trips.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            ViewBag.bus_id = new SelectList(db.Buses, "bus_id", "bus_type", trip.bus_id);
            return View(trip);
        }

        // POST: Admin/TripsAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "trip_id,bus_id,start_location,end_location,distance,departure_time,departure_day,Price,other_trip_details")] Trip trip)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trip).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.bus_id = new SelectList(db.Buses, "bus_id", "bus_type", trip.bus_id);
            return View(trip);
        }

        // GET: Admin/TripsAdmin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = db.Trips.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            return View(trip);
        }

        // POST: Admin/TripsAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trip trip = db.Trips.Find(id);
            db.Trips.Remove(trip);
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

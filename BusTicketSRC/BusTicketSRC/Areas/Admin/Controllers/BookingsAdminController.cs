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
    public class BookingsAdminController : Controller
    {
        private BusTicketDB db = new BusTicketDB();

        // GET: Admin/BookingsAdmin
        public ActionResult Index(string search = "")
        {
            BusTicketDB dB = new BusTicketDB();
            List<Booking> bookings = dB.Bookings.Where(row =>
            row.phone.Contains(search)).ToList();
            return View(bookings);
        }

        // GET: Admin/BookingsAdmin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        public ActionResult Create()
        {
            ViewBag.ticket_id = new SelectList(db.Tickets, "ticket_id", "ticket_status");

            // Materialize the results by calling ToList() before string formatting
            var tripsList = db.Trips.ToList().Select(t => new
            {
                trip_id = t.trip_id,
                displayText = $"{t.start_location} to {t.end_location} With type bus {t.Bus.bus_type}"
            }).ToList();

            ViewBag.trip_id = new SelectList(tripsList, "trip_id", "displayText");

            return View();
        }


        // POST: Admin/BookingsAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "booking_id,customer_name,phone,email,trip_id,booking_date,departure_day,departure_time,seat_number,age,booking_price,ticket_id")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                booking.booking_date = DateTime.Now;

                db.Bookings.Add(booking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ticket_id = new SelectList(db.Tickets, "ticket_id", "ticket_status", booking.ticket_id);
            ViewBag.trip_id = new SelectList(db.Trips, "trip_id", "trip_id", booking.trip_id);
            return View(booking);
        }

        // GET: Admin/BookingsAdmin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            ViewBag.ticket_id = new SelectList(db.Tickets, "ticket_id", "ticket_status", booking.ticket_id);
            ViewBag.trip_id = new SelectList(db.Trips, "trip_id", "trip_id", booking.trip_id);
            return View(booking);
        }

        // POST: Admin/BookingsAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "booking_id,customer_name,phone,email,trip_id,booking_date,departure_day,departure_time,seat_number,age,booking_price,ticket_id")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ticket_id = new SelectList(db.Tickets, "ticket_id", "ticket_status", booking.ticket_id);
            ViewBag.trip_id = new SelectList(db.Trips, "trip_id", "trip_id", booking.trip_id);
            return View(booking);
        }

        // GET: Admin/BookingsAdmin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Admin/BookingsAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // Add this action to your controller

        public JsonResult IsSeatNumberAvailable(int tripId, string seatNumber)
        {
            bool isAvailable = false;

            if (!string.IsNullOrEmpty(seatNumber) && int.TryParse(seatNumber, out int seatNum))
            {
                if (seatNum >= 1 && seatNum <= 32)
                {
                    // Seat number is valid, check availability
                    isAvailable = !db.Bookings.Any(b => b.trip_id == tripId && b.seat_number == seatNumber);
                }
            }

            return Json(isAvailable, JsonRequestBehavior.AllowGet);
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

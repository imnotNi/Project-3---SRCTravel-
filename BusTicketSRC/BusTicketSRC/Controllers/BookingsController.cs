using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BusTicketSRC.Models;

namespace BusTicketSRC.Controllers
{
    public class BookingsController : GlobalController
    {
        private BusTicketDB db = new BusTicketDB();

        // GET: Bookings
        public ActionResult Index()
        {
            return View();
        }
        // GET: Bookings/Details/5
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
        // GET: Bookings/Create
        public ActionResult Create(int tripId, int busId, string departureTime, string departureDay)
        {
            // Populate the SelectList for buses
            ViewBag.DDLBusList = new SelectList(db.Buses, "bus_id", "bus_type");
            ViewBag.SelectedTripId = tripId;
            ViewBag.SelectedBusId = busId;
            ViewBag.SelectedDepartureTime = departureTime;
            ViewBag.SelectedDepartureDay = departureDay;

            // Populate the SelectList for trips
            ViewBag.DDLTripList = db.Trips
                .ToList()
                .Select(t => new SelectListItem
                {
                    Value = t.trip_id.ToString(),
                   
                    Text = $"{t.start_location} to {t.end_location} - With bus type:{t.Bus.bus_id} "
                })
                .ToList();

            // Create a Booking model
            var booking = new Booking();

            // Set the selected trip ID based on the parameter
            if (tripId!=0)
            {
                booking.trip_id = tripId;
                ViewBag.SelectedTripId = tripId;

                // Set the selected trip directly in the ViewBag.DDLTripList
                ViewBag.DDLTripList = new SelectList(
                    new List<SelectListItem>
                    {
                new SelectListItem
                {
                    Value = tripId.ToString(),
                 
                    Text = $"{db.Trips.Find(tripId).start_location} to {db.Trips.Find(tripId).end_location} - With bus type:{db.Buses.Find(busId).bus_type}"
                }
                    },
                    "Value",
                    "Text",
                    tripId
                ) ;

                // Update DDLBusList based on the selected trip
                ViewBag.DDLBusList = new SelectList(db.Buses.Where(b => b.Trips.Any(t => t.trip_id == tripId)), "bus_id", "bus_type", busId);

                // Set the departure_time and departure_day based on the selected trip
                var selectedTrip = db.Trips.Find(tripId);
                booking.departure_time = selectedTrip.departure_time;
                booking.departure_day = selectedTrip.departure_day;

                // Set the booking_price based on the selected trip
                booking.booking_price = selectedTrip.Price;
            }

            // Load the selected seats for the trip and add them to ViewBag
            ViewBag.SelectedSeats = db.Bookings.Where(b => b.trip_id == tripId).Select(b => b.seat_number).ToList();
            PopulateDropdowns(booking);
            if (ViewBag.DDLTripList == null)
            {
                // If it's not set, initialize it (you can modify this part based on your actual logic)
                ViewBag.DDLTripList = new List<SelectListItem>();
            }

            return View(booking);
        }
        public bool IsSeatBookedForTrip(int tripId, int seatNumber)
        {
            // Your implementation here
            // For example, query your database to check if the seat is booked for the given tripId
            var isBooked = db.Bookings.Any(b => b.trip_id == tripId && b.seat_number == seatNumber.ToString());
            return isBooked;
        }
        private void PopulateDropdowns(Booking booking)
        {
            ViewBag.DDLBusList = new SelectList(db.Buses, "bus_id", "bus_type");
            

            // Set the selected trip ID in ViewBag for use in the view
            ViewBag.SelectedTripId = booking.trip_id;

            // Load the selected seats for the trip and add them to ViewBag
            ViewBag.SelectedSeats = db.Bookings
                .Where(b => b.trip_id == booking.trip_id)
                .Select(b => b.seat_number)
                .ToList();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "customer_name,email,phone,trip_id,age,departure_time,departure_day,seat_number,booking_price")] Booking booking)
        {
            ViewBag.DDLTripList = db.Trips
                .Select(t => new SelectListItem
                {
                    Value = t.trip_id.ToString(),
                    Text = t.start_location + " to " + t.end_location 
                })
                .ToList();

            if (ModelState.IsValid)
            {
                // Lưu dữ liệu vào Session
                List<Booking> bookingList = Session["BookingList"] as List<Booking> ?? new List<Booking>();

                // Phân tách chuỗi seat_number thành các giá trị riêng biệt
                var selectedSeats = booking.seat_number.Split(',');

                // Tạo Booking cho mỗi ghế và thêm vào danh sách
                foreach (var seat in selectedSeats)
                {
                    var newBooking = new Booking
                    {
                        customer_name = booking.customer_name,
                        email = booking.email,
                        phone = booking.phone,
                        trip_id = booking.trip_id,
                        age = booking.age,
                        departure_time = booking.departure_time,
                        departure_day = booking.departure_day,
                        seat_number = seat.Trim(), // Loại bỏ khoảng trắng nếu có
                        booking_price = booking.booking_price
                    };

                    bookingList.Add(newBooking);
                }

                Session["BookingList"] = bookingList;
                return RedirectToAction("BookingList");
            }
            else
            {
                // ModelState không hợp lệ, trả về view với thông báo lỗi
                return View();
            }
        }
        public ActionResult BookingList()
        {
            List<Booking> bookingList = Session["BookingList"] as List<Booking> ?? new List<Booking>();

            // Pass db to the view using ViewBag
            ViewBag.db = new BusTicketDB(); // Replace this with your actual way of getting the database context

            return View(bookingList);
        }


        public ActionResult Pay()
        {
            var bookings = Session["BookingList"] as List<Booking>;

            if (bookings != null && bookings.Any())
            {
                // Group bookings by trip_id
                var groupedBookings = bookings.GroupBy(b => b.trip_id);

                foreach (var tripBookings in groupedBookings)
                {
                    // Calculate the total price for the ticket
                    var totalTicketPrice = tripBookings.Sum(b => b.booking_price) ?? 0; // Use the null coalescing operator

                    var ticket = new Ticket
                    {
                        total_price = totalTicketPrice,
                        ticket_status = "Paid"
                    };

                    // Add the Ticket to the database
                    db.Tickets.Add(ticket);
                    db.SaveChanges();

                    // Iterate through each booking for the specific trip and create Booking with associated Ticket
                    foreach (var tempBookingInfo in tripBookings)
                    {
                        var booking = new Booking
                        {
                            customer_name = tempBookingInfo.customer_name,
                            phone = tempBookingInfo.phone,
                            email = tempBookingInfo.email,
                            trip_id = tempBookingInfo.trip_id, // Use the provided trip_id from each booking
                            departure_time = tempBookingInfo.departure_time,
                            departure_day = tempBookingInfo.departure_day,
                            seat_number = tempBookingInfo.seat_number,
                            age = tempBookingInfo.age,
                            booking_price = tempBookingInfo.booking_price,
                            booking_date = DateTime.Now,
                            ticket_id = ticket.ticket_id
                        };

                        // Add the Booking to the database
                        db.Bookings.Add(booking);
                        db.SaveChanges();
                    }
                }

                // Remove the session data after processing
                Session.Remove("BookingList");
            }

            return RedirectToAction("Index", "Home");
        }




        // GET: Bookings/Edit/5
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
            ViewBag.bus_id = new SelectList(db.Buses, "bus_id", "bus_type");
            ViewBag.trip_id = new SelectList(db.Trips, "trip_id", "start_location", booking.trip_id);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "booking_id,customer_name,email,phone,bus_id,trip_id,booking_date,departure_time,seat_number,age,booking_price")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
          ;
            ViewBag.trip_id = new SelectList(db.Trips, "trip_id", "start_location", booking.trip_id);
            return View(booking);
        }

        // GET: Bookings/Delete/5
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

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public JsonResult IsSeatBooked(int tripId, string seatNumber)
        {
            var isBooked = db.Bookings.Any(b => b.trip_id == tripId && b.seat_number == seatNumber);
            return Json(isBooked, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetTripPrice(int tripId)
        {
            var trip = db.Trips.Find(tripId);
            if (trip != null)
            {
                return Json(trip.Price, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;

            // Adjust age if the birthday hasn't occurred yet this year
            if (dateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
        [HttpPost]
        public ActionResult CancelBooking(int tripId, string seatNumber)
        {
            // Retrieve the list of bookings from the session
            List<Booking> bookingList = Session["BookingList"] as List<Booking>;

            if (bookingList != null)
            {
                // Find the booking to cancel based on tripId and seatNumber
                var bookingToCancel = bookingList.FirstOrDefault(b => b.trip_id == tripId && b.seat_number == seatNumber);

                if (bookingToCancel != null)
                {
                    // Remove the booking from the list
                    bookingList.Remove(bookingToCancel);

                    // Update the session with the modified booking list
                    Session["BookingList"] = bookingList;

                    // You can perform additional logic if needed, such as updating the total price

                    // Return a success status (you might want to return JSON if using AJAX)
                    return Content("Booking cancelled successfully");
                }
            }

            // Return an error status if the booking was not found or if session is not properly initialized
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Content("Error cancelling booking");
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
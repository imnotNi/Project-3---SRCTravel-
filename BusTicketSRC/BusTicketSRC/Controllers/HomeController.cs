using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BusTicketSRC.Helper;
using BusTicketSRC.Models;
using BusTicketSRC.Shared;
using BusTicketSRC.ViewModel;

namespace BusTicketSRC.Controllers
{
    public class HomeController : GlobalController
    {
        private BusTicketDB db = new BusTicketDB();

        // GET: Trips
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

                // Gán giá trị số lượng ghế trống cho mỗi chuyến đi
                foreach (var trip in trips)
                {
                    // Giả sử bạn có một hàm GetAvailableSeats trả về số lượng ghế trống cho chuyến đi
                    ViewData[$"AvailableSeatsForTrip_{trip.trip_id}"] = GetAvailableSeats(trip.trip_id);
                }

                // Gửi dữ liệu tìm kiếm và danh sách Trip lên View
                ViewBag.Start_locationSearch = start_locationSearch;
                ViewBag.End_locationSearch = end_locationSearch;
                ViewBag.Departure_daySearch = departure_daySearch;
                return View(trips);
            }
        }

        public int GetAvailableSeats(int tripId)
        {
            // Retrieve the booked seats from the database
            var bookedSeats = db.Bookings
                .Where(b => b.trip_id == tripId && b.seat_number != null)
                .AsEnumerable() // Switch to in-memory processing
                .SelectMany(b => b.seat_number.Split(',')) // Flatten the arrays
                .Where(seat => !string.IsNullOrWhiteSpace(seat)) // Exclude empty seats
                .Distinct() // Collect distinct seat numbers
                .ToList(); // Materialize the list

            // Retrieve the trip from the database to get the total capacity
            var trip = db.Trips
.Where(t => t.trip_id == tripId && t.Bus != null)
                .FirstOrDefault();

            // If the trip is found, calculate the available seats
            if (trip != null)
            {
                // Calculate available seats by subtracting booked seats from total capacity
                var totalSeats = trip.Bus?.capacity ?? 0;
                var bookedSeatsCount = bookedSeats.Count();
                return Math.Max(0, totalSeats - bookedSeatsCount);
            }

            return 0; // Default value
        }


        [HttpPost]
        public ActionResult BookNow(int tripId)
        {
            var selectedTrip = db.Trips.Find(tripId);

            if (selectedTrip != null)
            {
                var bookingInfo = new
                {
                    TripId = selectedTrip.trip_id,
                    StartLocation = selectedTrip.start_location,
                    EndLocation = selectedTrip.end_location,
                    DepartureDay = selectedTrip.departure_day,
                    DepartureTime = selectedTrip.departure_time,
                    Price = selectedTrip.Price
                };

                // Store the booking information in session
                Session["TempBookingInfo"] = bookingInfo;

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
        // GET: Trips/Details/5
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

        // GET: Trips/Create
        public ActionResult Create()
        {
            ViewBag.bus_id = new SelectList(db.Buses, "bus_id", "bus_type");
            return View();
        }

        // POST: Trips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "trip_id,bus_id,start_location,end_location,distance,departure_time,departure_day,Price,other_trip_details,BusType,SeatsAvailable")] Trip trip)
        {
            if (ModelState.IsValid)
            {
                db.Trips.Add(trip);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.bus_id = new SelectList(db.Buses, "bus_id", "bus_type", trip.bus_id);
            return View(trip);
        }

        // GET: Trips/Edit/5
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

        // POST: Trips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "trip_id,bus_id,start_location,end_location,distance,departure_time,departure_day,Price,other_trip_details,BusType,SeatsAvailable")] Trip trip)
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

        // GET: Trips/Delete/5
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

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trip trip = db.Trips.Find(id);
            db.Trips.Remove(trip);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            if (!string.IsNullOrEmpty(Session[Const.USERIDSESSION]?.ToString()))
            {
                return Redirect("~/");
            }
            if (!string.IsNullOrEmpty(Session[Const.ADMINIDSESSION]?.ToString()))
            {
                return Redirect("~/admin");
            }
            ViewBag.Error = "";
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "This account is Invalid";
                return View();
            }

            //admin
            if (login.Username == Const.ADMINUSERNAME && login.Password == Const.ADMINPASSWORD)
            {
                Session[Const.ADMINIDSESSION] = Const.ADMINUSERNAME;
                TempData["success"] = "Login SuccessFully";
                return Redirect("~/admin");
            }
            //user
            string username = login.Username;
            string pass = login.Password.ToString().ToMD5();
            User acc = db.Users.FirstOrDefault(x => x.username == username && x.password != null);
            if (acc == null)
            {
                TempData["error"] = "Account does not exist";
            }
            else
            {
                if (acc.password.Equals(pass))
                {
                    Session[Const.USERIDSESSION] = acc.user_id.ToString();
                    Session[Const.USERNAMESESSION] = "Hello " + acc.username + "!".ToString();

                    TempData["success"] = "Login SuccessFully!";
               
                    return Redirect("~/");
                }
                else
                {
                    TempData["error"] = "Password is Invalid";
                }
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session[Const.ADMINIDSESSION] = "";
            Session[Const.USERIDSESSION] = "";
            Session[Const.USERNAMESESSION] = "";
            TempData["success"] = "Logout SuccessFully";
            return Redirect("~/");
        }
        public ActionResult Register()
        {
            if (!string.IsNullOrEmpty(Session[Const.USERIDSESSION]?.ToString()))
            {
                return Redirect("~/");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterModel register)
        {
            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    username = register.Username,
                    password = register.Password,
                    email = register.Email,
                    phone = register.Phone,
                };
                user.password = register.Password.ToMD5();

                var check_username = db.Users.FirstOrDefault(x => x.phone == register.Username);
                if (check_username != null)
                {
                    TempData["error"] = "Phone number has been registered!";
                    return View(register);
                }
                else
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    TempData["success"] = "Register SuccessFully";
                    return RedirectToAction("Login", "Home");
                }
            }
            else
            {
                if (register.Password != register.ConfirmPassword)
                {
                    TempData["error"] = "Password and Confirm passwords do not match";
                    return View(register);
                }
                TempData["error"] = "Incorrect information entered. Please try again!";
                return View(register);
            }


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

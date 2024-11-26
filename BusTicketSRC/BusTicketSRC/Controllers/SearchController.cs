using BusTicketSRC.Models;
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

namespace BusTicketSRC.Controllers
{
    public class SearchController : GlobalController
    {
        public ActionResult Index(string search = "")
        {
            if (string.IsNullOrEmpty(search))
            {
                return View();
            }
            else
            {
                BusTicketDB dB = new BusTicketDB();

                // Sử dụng ToLower để so sánh không phân biệt chữ hoa chữ thường
                string normalizedSearch = search.ToLower();

                // Tìm kiếm với điều kiện là chuỗi tìm kiếm trùng khớp chính xác 100%
                List<Booking> bookings = dB.Bookings
                    .Where(row => row.phone.ToLower() == normalizedSearch)
                    .ToList();

                // Nếu có ít nhất một sự khớp chính xác, hiển thị dữ liệu
                if (bookings.Count > 0)
                {
                    return View(bookings);
                }
                else
                {
                    // Nếu không có sự khớp chính xác, có thể chuyển hướng hoặc xử lý khác theo ý muốn của bạn
                    return View(); // Hiển thị trang mặc định hoặc thông báo không có dữ liệu
                }
            }
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusTicketDB dB = new BusTicketDB();
            Booking booking = dB.Bookings.Find(id);

            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }
        // POST: Search/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BusTicketDB dB = new BusTicketDB();
            Booking booking = dB.Bookings.Find(id);

            if (booking == null)
            {
                return HttpNotFound();
            }

            dB.Bookings.Remove(booking);
            dB.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
    

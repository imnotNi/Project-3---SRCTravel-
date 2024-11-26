using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace BusTicketSRC.Models
{
    public partial class BusTicketDB : DbContext
    {
        public BusTicketDB()
            : base("name=BusTicketDB")
        {
        }

        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Bus> Buses { get; set; }
        public virtual DbSet<PhanQuyen> PhanQuyens { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<Trip> Trips { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>()
                .Property(e => e.departure_day)
                .IsUnicode(false);

            modelBuilder.Entity<Booking>()
                .Property(e => e.departure_time)
                .IsUnicode(false);

            modelBuilder.Entity<Booking>()
                .Property(e => e.seat_number)
                .IsUnicode(false);

            modelBuilder.Entity<Booking>()
                .Property(e => e.booking_price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Ticket>()
                .Property(e => e.total_price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Ticket>()
                .Property(e => e.ticket_status)
                .IsUnicode(false);

            modelBuilder.Entity<Trip>()
                .Property(e => e.distance)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Trip>()
                .Property(e => e.departure_time)
                .IsUnicode(false);

            modelBuilder.Entity<Trip>()
                .Property(e => e.departure_day)
                .IsUnicode(false);

            modelBuilder.Entity<Trip>()
                .Property(e => e.Price)
                .HasPrecision(10, 2);
        }
    }
}

namespace BusTicketSRC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Booking")]
    public partial class Booking
    {
        [Key]
        public int booking_id { get; set; }

        [StringLength(255)]
        public string customer_name { get; set; }

        [StringLength(15)]
        public string phone { get; set; }

        [StringLength(255)]
        public string email { get; set; }

        public int? trip_id { get; set; }

        public DateTime? booking_date { get; set; }

        [StringLength(50)]
        public string departure_day { get; set; }

        [StringLength(50)]
        public string departure_time { get; set; }

        [StringLength(50)]
        public string seat_number { get; set; }

        [StringLength(15)]
        public string age { get; set; }

        public decimal? booking_price { get; set; }

        public int? ticket_id { get; set; }

        public virtual Ticket Ticket { get; set; }

        public virtual Trip Trip { get; set; }
    }
}

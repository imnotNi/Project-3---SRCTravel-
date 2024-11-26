namespace BusTicketSRC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Trip")]
    public partial class Trip
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Trip()
        {
            Bookings = new HashSet<Booking>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int trip_id { get; set; }

        public int? bus_id { get; set; }

        [Required(ErrorMessage = "Start location is required.")]
        [StringLength(255)]
        public string start_location { get; set; }

        [Required(ErrorMessage = "End location is required.")]
        [StringLength(255)]
        public string end_location { get; set; }
        [Required(ErrorMessage = "Distance is required.")]

        public decimal? distance { get; set; }

        [Required(ErrorMessage = "Departure time is required.")]

        [StringLength(50)]
        public string departure_time { get; set; }
        [Required(ErrorMessage = "Departure Day is required.")]

        [StringLength(50)]
        public string departure_day { get; set; }
        [Required(ErrorMessage = "Price is required.")]

        public decimal? Price { get; set; }

        public string other_trip_details { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Booking> Bookings { get; set; }

        public virtual Bus Bus { get; set; }
    }
}

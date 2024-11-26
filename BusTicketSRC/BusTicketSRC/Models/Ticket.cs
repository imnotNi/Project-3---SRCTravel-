namespace BusTicketSRC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Ticket")]
    public partial class Ticket
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ticket()
        {
            Bookings = new HashSet<Booking>();
        }

        [Key]
        public int ticket_id { get; set; }
        [Required(ErrorMessage = "Total Price is required.")]
        public decimal? total_price { get; set; }
        [Required(ErrorMessage = "Ticket Status is required.")]
        [StringLength(1000)]
        public string ticket_status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}

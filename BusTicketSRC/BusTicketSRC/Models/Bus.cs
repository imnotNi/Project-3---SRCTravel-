namespace BusTicketSRC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Bus")]
    public partial class Bus
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Bus()
        {
            Trips = new HashSet<Trip>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int bus_id { get; set; }

        [Required(ErrorMessage = "Bus type is required")]
        [StringLength(255)]
        public string bus_type { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        public int? capacity { get; set; }

        [Required(ErrorMessage = "Plate number is required")]
        [StringLength(255)]
        public string plate_number { get; set; }

        public string other_bus_details { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Trip> Trips { get; set; }
    }
}

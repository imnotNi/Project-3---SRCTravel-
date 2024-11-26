namespace BusTicketSRC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        [Key]
        public int user_id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(255)]
        public string username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(255)]
        public string password { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(255)]
        public string email { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [StringLength(15)]
        public string phone { get; set; }
    }
}
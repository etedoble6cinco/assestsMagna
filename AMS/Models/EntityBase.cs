using System;
using System.ComponentModel.DataAnnotations;

namespace AMS.Models
{
    public class EntityBase
    {
        [Display(Name = "Fecha de Creacion")]
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public bool Cancelled { get; set; }
    }
}

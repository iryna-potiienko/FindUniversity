using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FindUniversity
{
    public partial class Countries
    {
        public Countries()
        {
            Universities = new HashSet<Universities>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage ="Обов'язкове поле!")]
        [MinLength(3)]
        [Remote(action: "Validation", controller: "Countries", AdditionalFields = nameof(Id))]
        [Display(Name="Країна")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Обов'язкове поле!")]
        [Display(Name = "Університет")]
        public virtual ICollection<Universities> Universities { get; set; }
    }
}

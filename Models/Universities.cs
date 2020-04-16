using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FindUniversity
{
    public partial class Universities
    {
        public Universities()
        {
            Faculties = new HashSet<Faculties>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage ="Обов'язкове поле!")]
        [MinLength(3)]
        [Remote(action: "Validation", controller: "Universities", AdditionalFields = nameof(Id))]
        [Display(Name="Університет")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Обов'язкове поле!")]
        [Display(Name = "Країна")]
        public int CountryId { get; set; }

        [Display(Name = "Країна")]
        public virtual Countries Country { get; set; }

        [Display(Name = "Факультети")]
        public virtual ICollection<Faculties> Faculties { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FindUniversity
{
    public partial class Faculties
    {
        public Faculties()
        {
            FacultyEducationalProg = new HashSet<FacultyEducationalProg>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage ="Обов'язкове поле!")]
        [MinLength(5)]
        [Remote(action: "Validation", controller: "Faculties", AdditionalFields = nameof(Id))]
        [Display(Name = "Назва факультету")]
        public string Name { get; set; }
        [Display(Name="Інформація про факультет")]
        public string Info { get; set; }

        [Required(ErrorMessage = "Обов'язкове поле!")]
        [Display(Name = "Університет")]
        public int UniversityId { get; set; }

        [Display(Name = "Університет")]
        public virtual Universities University { get; set; }

        public virtual ICollection<FacultyEducationalProg> FacultyEducationalProg { get; set; }
    }
}

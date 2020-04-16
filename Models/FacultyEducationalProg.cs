using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FindUniversity
{
    public partial class FacultyEducationalProg
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Обов'язкове поле!")]
        [Remote(action: "Validation", controller: "FacultyEducationalProgs", AdditionalFields = nameof(FacultyId))]
        [Display(Name = "Освітня програма")]
        public int EducationalProgId { get; set; }
        [Required(ErrorMessage = "Обов'язкове поле!")]
        [Display(Name = "Факультет")]
        public int FacultyId { get; set; }

        [Display(Name = "Освітня програма")]
        public virtual EducationalProg EducationalProg { get; set; }
        [Display(Name = "Факультет")]
        public virtual Faculties Faculty { get; set; }
    }
}

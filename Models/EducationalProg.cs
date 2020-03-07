using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FindUniversity
{
    public partial class EducationalProg
    {
        public EducationalProg()
        {
            FacultyEducationalProg = new HashSet<FacultyEducationalProg>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "Обов'язкове поле!")]
        [Display(Name = "Назва")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Обов'язкове поле!")]
        [Display(Name = "Спеціальність")]
        public int SpecialtiesId { get; set; }
        [Display(Name = "Ціна")]
        public decimal? Price { get; set; }


        [Display(Name = "Спеціальність")]
        public virtual Specialties Specialties { get; set; }

        
        public virtual ICollection<FacultyEducationalProg> FacultyEducationalProg { get; set; }
    }
}

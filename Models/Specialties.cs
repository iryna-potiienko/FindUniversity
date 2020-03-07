using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FindUniversity
{
    public partial class Specialties
    {
        public Specialties()
        {
            EducationalProg = new HashSet<EducationalProg>();
        }

        public int Id { get; set; }
        [Required (ErrorMessage="Обов'язкове поле!")]
        [Display(Name = "Назва спеціальності")]
        public string Name { get; set; }
        [Display(Name = "Інформація про спеціальність")]
        public string Info { get; set; }

        [Display(Name = "Освітня програма")]
        public virtual ICollection<EducationalProg> EducationalProg { get; set; }
    }
}

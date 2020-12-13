using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TechJobsPersistent.Data;
using TechJobsPersistent.Models;

namespace TechJobsPersistent.ViewModels
{
    public class AddJobViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Employer ID Missing!")]
        public int EmployerId { get; set; }

        public List<SelectListItem> Employers { get; set; }

        public List<Skill> Skills { get; set; }

        public AddJobViewModel(List<Employer> employers, List<Skill> skills)
        {
            Employers = new List<SelectListItem>();
            
            Skills = new List<Skill>();
            Skills = skills.ToList();

            foreach (var employee in employers)
            {
                Employers.Add(new SelectListItem
                {
                    Value = employee.Id.ToString(),
                    Text = employee.Name
                }); ;
            }
        }

        public AddJobViewModel(){}
    }
}

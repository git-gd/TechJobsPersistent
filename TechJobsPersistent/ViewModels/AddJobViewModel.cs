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

        // I used a private setter because these lists need to be rebuilt if the ModelState is invalid
        public List<SelectListItem> Employers { get; private set; }

        public List<Skill> Skills { get; set; }

        public AddJobViewModel(List<Employer> employers, List<Skill> skills)
        {
            SetEmployers(employers);
            Skills = skills;
        }

        public AddJobViewModel(){}

        public void SetEmployers(List<Employer> employers)
        {
            Employers = new List<SelectListItem>();

            foreach (Employer employee in employers)
            {
                Employers.Add(new SelectListItem
                {
                    Value = employee.Id.ToString(),
                    Text = employee.Name
                }); ;
            }
        }
    }
}

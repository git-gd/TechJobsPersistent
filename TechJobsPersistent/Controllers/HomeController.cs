using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TechJobsPersistent.Models;
using TechJobsPersistent.ViewModels;
using TechJobsPersistent.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TechJobsPersistent.Controllers
{
    public class HomeController : Controller
    {
        private JobDbContext context;

        public HomeController(JobDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            List<Job> jobs = context.Jobs.Include(j => j.Employer).ToList();

            return View(jobs);
        }

        [HttpGet("/Add")]
        public IActionResult AddJob()
        {
            AddJobViewModel addJobViewModel = new AddJobViewModel(context.Employers.ToList(), context.Skills.ToList());
            return View(addJobViewModel);
        }

        [HttpPost]
        public IActionResult ProcessAddJobForm(AddJobViewModel addJobViewModel)
        {
            if (ModelState.IsValid)
            {
                // save and redirect
                // In the ProcessAddJobForm() method, pass in a new parameter: an array of strings called selectedSkills. When we allow the user to select multiple checkboxes, the user’s selections are stored in a string array. The way we connect the checkboxes together is by giving the name attribute on the<input> tag the name of the array.In this case, each<input> tag on the form for the skills checkboxes should have "selectedSkills" as the name.
                // After you add a new parameter, you want to set up a loop to go through each item in selectedSkills.This loop should go right after you create a new Job object and before you add that Job object to the database.
                // Inside the loop, you will create a new JobSkill object with the newly - created Job object.You will also need to parse each item in selectedSkills as an integer to use for SkillId.
                // Add each new JobSkill object to the DbContext object, but do not add an additional call to SaveChanges() inside the loop! One call at the end of the method is enough to get the updated info to the database.
            }

            // We need to repopulate the dropdown list if our ModelState is invalid
            // ... this can't possibly be the best way to do this
            if (addJobViewModel.Employers == null)
            {
                List<Employer> employers = context.Employers.ToList();
                addJobViewModel.Employers = new List<SelectListItem>();
                employers.ForEach(e => addJobViewModel.Employers.Add(new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                }));
            }
            if (addJobViewModel.Skills == null)
            {
                addJobViewModel.Skills = context.Skills.ToList();
            }

            return View("AddJob", addJobViewModel);
        }

        public IActionResult Detail(int id)
        {
            Job theJob = context.Jobs
                .Include(j => j.Employer)
                .Single(j => j.Id == id);

            List<JobSkill> jobSkills = context.JobSkills
                .Where(js => js.JobId == id)
                .Include(js => js.Skill)
                .ToList();

            JobDetailViewModel viewModel = new JobDetailViewModel(theJob, jobSkills);
            return View(viewModel);
        }
    }
}

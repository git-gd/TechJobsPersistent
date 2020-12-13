using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TechJobsPersistent.Data;
using TechJobsPersistent.Models;
using TechJobsPersistent.ViewModels;

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
        public IActionResult ProcessAddJobForm(AddJobViewModel addJobViewModel, String[] selectedSkills)
        {
            if (ModelState.IsValid)
            {
                Job job = new Job
                {
                    EmployerId = addJobViewModel.EmployerId,
                    Name = addJobViewModel.Name
                };

                context.Jobs.Add(job);
                context.SaveChanges();

                // Our selected Skill Id is returned in the selectedSkills string Array
                // Our job Id was just created by MySQL in the previous SaveChanges()
                foreach (string selected in selectedSkills)
                {
                    JobSkill jobSkill = new JobSkill
                    {
                        SkillId = int.Parse(selected),
                        JobId = job.Id
                    };

                    context.JobSkills.Add(jobSkill);
                }

                context.SaveChanges();

                return Redirect("/home/");
            }

            // We need to repopulate the dropdown list if our ModelState is invalid
            addJobViewModel.SetEmployers(context.Employers.ToList());
            addJobViewModel.Skills = context.Skills.ToList();

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

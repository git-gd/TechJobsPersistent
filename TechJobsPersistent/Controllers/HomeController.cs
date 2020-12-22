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

        public IActionResult Index(string sort)
        {
            List<Job> jobs;
            if (sort != null)
            {
                ViewBag.Sort = sort;
                if (sort == "Employer")
                {
                    jobs = context.Jobs.Include(j => j.Employer).OrderBy(o => o.Employer.Name).ToList();
                } else
                {
                    jobs = context.Jobs.Include(j => j.Employer).OrderBy(o => o.Employer.Location).ToList();
                }
            }
            else {
                jobs = context.Jobs.Include(j => j.Employer).OrderBy(o => o.Name).ToList(); // TODO: If the app can't connnect to MySQL this throws an error.. catch and deal with this
            }            

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
                Job job = new Job
                {
                    EmployerId = addJobViewModel.EmployerId,
                    Name = addJobViewModel.Name
                };

                context.Jobs.Add(job);

                foreach (string selected in addJobViewModel.SelectedSkills)
                {
                    JobSkill jobSkill = new JobSkill
                    {
                        SkillId = int.Parse(selected),
                        Job = job
                    };

                    context.JobSkills.Add(jobSkill);
                }

                context.SaveChanges();

                return Redirect("/home/");
            }

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

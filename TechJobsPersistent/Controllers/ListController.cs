using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TechJobsPersistent.Data;
using TechJobsPersistent.Models;
using TechJobsPersistent.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TechJobsPersistent.Controllers
{
    public class ListController : Controller
    {
        internal static Dictionary<string, string> ColumnChoices = new Dictionary<string, string>()
        {
            {"all", "All"},
            {"employer", "Employer"},
            {"skill", "Skill"}
        };

        internal static List<string> TableChoices = new List<string>()
        {
            "employer",
            "skill"
        };

        private JobDbContext context;

        public ListController(JobDbContext dbContext)
        {
            context = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewBag.columns = ColumnChoices;
            ViewBag.tablechoices = TableChoices;
            ViewBag.employers = context.Employers.ToList();
            ViewBag.skills = context.Skills.ToList();
            // The following two lists noemployers and noskills are used to highlight employers and skills with no jobs
            ViewBag.noemployers = context.Jobs.Select(j => j.EmployerId).ToList();
            ViewBag.noskills = context.JobSkills.Select(ns => ns.SkillId).ToList();
            return View();
        }

        // list jobs by column and value
        public IActionResult Jobs(string column, string value)
        {
            List<Job> jobs = new List<Job>();
            List<JobDetailViewModel> displayJobs = new List<JobDetailViewModel>();

            if (column.ToLower().Equals("all"))
            {
                jobs = context.Jobs
                    .Include(j => j.Employer)
                    .ToList();

                foreach (var job in jobs)
                {
                    List<JobSkill> jobSkills = context.JobSkills
                        .Where(js => js.JobId == job.Id)
                        .Include(js => js.Skill)
                        .ToList();

                    JobDetailViewModel newDisplayJob = new JobDetailViewModel(job, jobSkills);
                    displayJobs.Add(newDisplayJob);
                }

                ViewBag.title = "All Jobs";
            }
            else
            {
                if (column == "employer")
                {
                    jobs = context.Jobs
                        .Include(j => j.Employer)
                        .Where(j => j.Employer.Name == value)
                        .ToList();

                    foreach (Job job in jobs)
                    {
                        List<JobSkill> jobSkills = context.JobSkills
                        .Where(js => js.JobId == job.Id)
                        .Include(js => js.Skill)
                        .ToList();

                        JobDetailViewModel newDisplayJob = new JobDetailViewModel(job, jobSkills);
                        displayJobs.Add(newDisplayJob);
                    }
                }
                else if (column == "skill")
                {
                    List<JobSkill> jobSkills = context.JobSkills
                        .Where(j => j.Skill.Name == value)
                        .Include(j => j.Job)
                        .ToList();

                    foreach (var job in jobSkills)
                    {
                        Job foundJob = context.Jobs
                            .Include(j => j.Employer)
                            .Single(j => j.Id == job.JobId);

                        List<JobSkill> displaySkills = context.JobSkills
                            .Where(js => js.JobId == foundJob.Id)
                            .Include(js => js.Skill)
                            .ToList();

                        JobDetailViewModel newDisplayJob = new JobDetailViewModel(foundJob, displaySkills);
                        displayJobs.Add(newDisplayJob);
                    }
                }
                ViewBag.title = "Jobs with " + ColumnChoices[column] + ": " + value;
            }
            ViewBag.jobs = displayJobs;

            return View();
        }
    }
}

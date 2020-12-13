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
    public class SearchController : Controller
    {
        private JobDbContext context;

        public SearchController(JobDbContext dbContext)
        {
            context = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewBag.columns = ListController.ColumnChoices;
            return View();
        }

        public IActionResult Results(string searchType, string searchTerm)
        {
            List<Job> jobs;
            List<JobDetailViewModel> displayJobs = new List<JobDetailViewModel>();

            if (string.IsNullOrEmpty(searchTerm))
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
            }
            else
            {
                if (searchType == "employer")
                {
                    jobs = context.Jobs
                        .Include(j => j.Employer)
                        .Where(j => j.Employer.Name.Contains(searchTerm))
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
                else if (searchType == "skill")
                {
                    List<JobSkill> jobSkills = context.JobSkills
                        .Where(j => j.Skill.Name.Contains(searchTerm))
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
                else if (searchType == "all")
                {
                    /*
                     * Search All includes - Employer Name, Job Name and Skill Name
                     * 
                     * I chose to use a Union to combine the JobSkill Skill Name query with the Employer and Job Name query
                     * 
                     * I used the Job table for the Union output since a list of Jobs is needed for the View
                     */

                    List<Job> jobsFound = context.Jobs
                        .Include(j => j.Employer)
                        .Where(j => j.Employer.Name.Contains(searchTerm) || j.Name.Contains(searchTerm))
                    .Union(
                        context.JobSkills
                            .Include(js => js.Job.Employer)
                            .Where(js => js.Skill.Name.Contains(searchTerm))
                            .Select(js => js.Job)
                    ).ToList();

                    foreach (Job job in jobsFound)
                    {
                        List<JobSkill> jobSkills = context.JobSkills
                            .Include(js => js.Skill)
                            .Where(js => js.JobId == job.Id)
                            .ToList();

                        JobDetailViewModel newDisplayJob = new JobDetailViewModel(job, jobSkills);
                        displayJobs.Add(newDisplayJob);
                    }
                }
            }

            ViewBag.columns = ListController.ColumnChoices;
            ViewBag.title = "Jobs with " + ListController.ColumnChoices[searchType] + ": " + searchTerm;
            ViewBag.jobs = displayJobs;

            return View("Index");
        }
    }
}

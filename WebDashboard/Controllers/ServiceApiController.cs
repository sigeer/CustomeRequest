using Microsoft.AspNetCore.Mvc;
using RequestProcessor;
using RequestProcessor.Models;
using WebDashboard.Attributes;

namespace ViteTest.Controllers
{
    [IgnoreMiddleware]
    [ApiController]
    [Route("/api/[action]")]
    public class ServiceApiController : ControllerBase
    {
        private readonly ILogger<ServiceApiController> _logger;
        readonly JobDataLogic _logic;

        public ServiceApiController(ILogger<ServiceApiController> logger, JobDataLogic logic)
        {
            _logger = logger;
            _logic = logic;
        }

        #region
        public List<JobTableViewModel> GetJobList()
        {
            return _logic.GetTableView();
        }
        public JobTableViewModel? LoadJob(int id)
        {
            return _logic.GetTableView().FirstOrDefault(x => x.JobStoreId == id);
        }
        [HttpPost]
        public bool AddOrUpdateJob([FromBody]JobTableViewModel job)
        {
            _logic.AddOrUpdateJob(job);
            return true;
        }
        [HttpDelete]
        public bool DeleteJob(int id)
        {
            _logic.DeleteJob(id);
            return true;
        }
        public void JobOn(int id)
        {
            _logic.JobOn(id);
        }
        public void JobOff(int id)
        {
            _logic.JobOff(id);
        }
        #endregion


        public List<HttpRequestUnit> GetJobDetailList()
        {
            return _logic.GetJobDetailList();
        }
        public HttpRequestUnit? LoadJobDetail(int id)
        {
            return _logic.GetJobDetailList().FirstOrDefault(x => x.Id == id);
        }
        [HttpPost]
        public bool AddOrUpdateJobDetail([FromBody] HttpRequestUnit jobDetail)
        {
            _logic.AddOrUpdateJobDetail(jobDetail);
            return true;
        }
        [HttpDelete]
        public bool DeleteJobDetail(int id)
        {
            _logic.DeleteJobDetail(id);
            return true;
        }

        public async Task<object?> RunJobDetail(int id)
        {
            return await _logic.DoJobDetail(id);
        }


        public List<ActionOptionViewModel> GetActionOptionList()
        {
            return _logic.GetActionOptionList();
        }


        public List<JobResult> GetJobResultList(int jobDetailId)
        {
            return _logic.GetJobResultList(jobDetailId);
        }
    }
}
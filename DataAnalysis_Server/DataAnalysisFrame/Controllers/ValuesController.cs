using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataAnalysis.Application.IService;
using DataAnalysis.Application.IService.ICallService;
using DataAnalysis.Application.IService.IJobService;
using DataAnalysis.Application.IService.IUserService;
using DataAnalysis.Component.Tools.Log;
using DataAnalysis.Core.Data.Entity;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalysisFrame.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IAmplitudeService _iAmplitudeService;
        private readonly IExecuteSocketService _executeQueueService;
        private readonly IObtainService _iObtainService;
        private readonly IUserAssetsService _iUserAssetsService;
        public ValuesController(IAmplitudeService iAmplitudeService,
            IExecuteSocketService executeQueueService, IObtainService iObtainService,
            IUserAssetsService iUserAssetsService)
        {
            _iAmplitudeService = iAmplitudeService;
            _executeQueueService = executeQueueService;
            _iObtainService = iObtainService;
            _iUserAssetsService = iUserAssetsService;
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            //_iObtainService.GetNewTransactionRecord();
            //_iObtainService.GetMmarketDetail("ada");
            return new string[] { "value1", "value2" };
        }
        // GET api/values
        [HttpGet]
        [Route("jobTest")]
        public IEnumerable<string> GetJobTest()
        {
            //_executeQueueService.ExecuteDetpthQueueJob();
            return new string[] { "value1", "value2" };
        }
        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

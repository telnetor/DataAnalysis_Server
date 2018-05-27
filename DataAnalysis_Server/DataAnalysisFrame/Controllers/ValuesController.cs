using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataAnalysis.Application.IService;
using DataAnalysis.Application.IService.IJobService;
using DataAnalysis.Component.Tools.Log;
using DataAnalysis.Core.Data.Entity;
using DataAnalysis.Core.Data.Entity.UnitTestEntity;
using DataAnalysis.Core.Data.IRepositories.IUnitRepositories;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalysisFrame.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IAmplitudeService _iAmplitudeService;
        private readonly IExecuteQueueService _executeQueueService;
        public ValuesController(IAmplitudeService iAmplitudeService, IExecuteQueueService executeQueueService)
        {
            _iAmplitudeService = iAmplitudeService;
            _executeQueueService = executeQueueService;
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            //Thread.Sleep(5000);
            //_iAmplitudeService.StartWebSocket();
            //_executeQueueService.ExecuteDetpthQueueJob();
            #region
            //string sql = "SELECT * FROM Test WHERE TsId=@TsId AND TsName=@TsName";
            //ResponseMsg<IEnumerable<TestEntity>> res=_iTestRepository.ExecuteQuery<TestEntity>(sql, new { TsId = 1, TsName = "SmallHan" });
            //ResponseMsg<int> resAdd=_iTestRepository.Add<TestEntity>(new TestEntity()
            //{
            //    TsName = "SmallHan",
            //    TsAge = 20
            //});
            //ResponseMsg<List<TestEntity>> resList=_iTestRepository.GetList(p => p.TsId == 1 && p.TsName == "SmallHan");
            ////_iTestRepository.Add(new TestEntity() { TsName = "小明", TsAge = 20 });
            //ResponseMsg<int> resModify = _iTestRepository.Modify(p => new TestEntity()
            //{
            //    TsAge = 99
            //}, o => o.TsId == 1);
            //ResponseMsg<int> resDel = _iTestRepository.Remove(p => p.TsId == 4);
            #endregion
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

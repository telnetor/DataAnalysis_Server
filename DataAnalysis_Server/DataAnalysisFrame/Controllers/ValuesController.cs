using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAnalysis.Core.Data.Entity.UnitTestEntity;
using DataAnalysis.Core.Data.IRepositories.IUnitRepositories;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalysisFrame.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ITestRepository _iTestRepository;
        public ValuesController(ITestRepository iTestRepository)
        {
            _iTestRepository = iTestRepository;
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _iTestRepository.Add<TestEntity>(new TestEntity()
            {
                TsName = "SmallHan",
                TsAge = 20
            });
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

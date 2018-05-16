
using DataAnalysis.Core.Data.Entity;
using DataAnalysis.Core.Data.Entity.UnitTestEntity;
using DataAnalysis.Core.Data.IRepositories.IUnitRepositories;
using DataAnalysis.Manipulation.DapperExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysis.Core.Data.Repository.Repositories.UnitRepository
{
    public class TestRepository :BaseRepository<TestEntity>,ITestRepository
    {
        private readonly IUnitOfWork _iUnitOfWork;
        public TestRepository(IUnitOfWork iUnitOfWork):base(iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;
        }
    }
}

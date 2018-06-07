
using DataAnalysis.Core.Data.Entity;
using DataAnalysis.Core.Data.Entity.UserAssetEntity;
using DataAnalysis.Core.Data.IRepositories;
using DataAnalysis.Core.Data.IRepositories.IUserRepositories;
using DataAnalysis.Manipulation.DapperExtension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysis.Core.Data.Repository.Repositories.UserRepositories
{
    public class BalanceRepository :BaseRepository<BalanceEntity>,IBalanceRepository
    {
        private readonly IUnitOfWork _iUnitOfWork;
        public BalanceRepository(IUnitOfWork iUnitOfWork):base(iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;
        }

       
    }
}

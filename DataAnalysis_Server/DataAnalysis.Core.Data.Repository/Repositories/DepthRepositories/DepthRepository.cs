﻿
using DataAnalysis.Core.Data.Entity;
using DataAnalysis.Core.Data.Entity.DepthEntity;
using DataAnalysis.Core.Data.IRepositories.IDepthRepositories;
using DataAnalysis.Manipulation.DapperExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysis.Core.Data.Repository.Repositories.DepthRepositories
{
    public class DepthRepository :BaseRepository<DepthAnalysisEntity>, IDepthRepository
    {
        private readonly IUnitOfWork _iUnitOfWork;
        public DepthRepository(IUnitOfWork iUnitOfWork):base(iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;
        }
    }
}
﻿using System.Threading.Tasks;
using Core.DataAccess;
using Core.Entities.Concrete;

namespace DataAccess.Abstract
{
    public interface IGroupClaimRepository : IDocumentDbRepository<GroupClaim>
    {
    }
}
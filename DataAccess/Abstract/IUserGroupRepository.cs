﻿using Core.DataAccess;
using Core.Entities.Concrete;

namespace DataAccess.Abstract
{
    public interface IUserGroupRepository : IDocumentDbRepository<UserGroup>
    {
    }
}
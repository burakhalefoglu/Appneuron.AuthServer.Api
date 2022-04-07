using Core.DataAccess.MongoDb;
using Core.Entities.Concrete;
using DataAccess.Abstract;

namespace DataAccess.Concrete.MongoDb;

public class MDbLogRepository : MongoDbRepositoryBase<Log>, ILogRepository
{
    public MDbLogRepository() : base(Collections.Collections.Log)
    {
    }
}
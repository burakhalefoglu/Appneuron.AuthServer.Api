using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.TableMappers;

namespace DataAccess.Concrete.Cassandra;

public class CassLogRepository : CassandraRepositoryBase<Log>, ILogRepository
{
    public CassLogRepository() : base(MappingConfiguration.Global.Define<LogMapper>())
    {
    }
}
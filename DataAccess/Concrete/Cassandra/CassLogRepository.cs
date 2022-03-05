using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Tables;
using DataAccess.Concrete.Cassandra.Tables.TableMappers;

namespace DataAccess.Concrete.Cassandra
{
    public class CassLogRepository : CassandraRepositoryBase<Log>, ILogRepository
    {
        public CassLogRepository() : base(CassandraTableQueries.Log,
            MappingConfiguration.Global.Define<LogMapper>())
        {
        }
    }
}
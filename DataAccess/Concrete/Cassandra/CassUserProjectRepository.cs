using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Tables;
using DataAccess.Concrete.Cassandra.Tables.TableMappers;
using Entities.Concrete;

namespace DataAccess.Concrete.Cassandra
{
    public class CassUserProjectRepository : CassandraRepositoryBase<UserProject>, IUserProjectRepository
    {
        public CassUserProjectRepository() : base(CassandraTableQueries.UserProject,
            MappingConfiguration.Global.Define<UserProjectMapper>())
        {
        }
    }
}
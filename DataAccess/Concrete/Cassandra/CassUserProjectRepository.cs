using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.TableMappers;
using Entities.Concrete;

namespace DataAccess.Concrete.Cassandra
{
    public class CassUserProjectRepository : CassandraRepositoryBase<UserProject>, IUserProjectRepository
    {
        public CassUserProjectRepository() : base(MappingConfiguration.Global.Define<UserProjectMapper>())
        {
        }
    }
}
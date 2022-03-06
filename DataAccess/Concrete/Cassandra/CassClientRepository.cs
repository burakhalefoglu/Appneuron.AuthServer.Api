using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.TableMappers;
using Entities.Concrete;

namespace DataAccess.Concrete.Cassandra
{
    public class CassClientRepository : CassandraRepositoryBase<Client>, IClientRepository
    {
        public CassClientRepository() : base(MappingConfiguration.Global.Define<ClientMapper>())
        {
        }
    }
}
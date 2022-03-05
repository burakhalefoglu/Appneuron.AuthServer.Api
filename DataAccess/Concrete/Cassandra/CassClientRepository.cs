using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Tables;
using DataAccess.Concrete.Cassandra.Tables.TableMappers;

namespace DataAccess.Concrete.Cassandra
{
    public class CassClientRepository : CassandraRepositoryBase<Client>, IClientRepository
    {
        public CassClientRepository() : base(CassandraTableQueries.Client,
            MappingConfiguration.Global.Define<ClientMapper>())
        {
        }
    }
}
using Core.DataAccess.Cassandra;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Contexts;
using DataAccess.Concrete.Cassandra.Tables;

namespace DataAccess.Concrete.Cassandra
{
    public class CassClientRepository : CassandraRepositoryBase<Client>, IClientRepository
    {
        public CassClientRepository() : base( CassandraTableQueries.Client)
        {
        }
    }
}
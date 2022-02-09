using Core.DataAccess.Cassandra;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Contexts;

namespace DataAccess.Concrete.Cassandra
{
    public class CassTranslateRepository : CassandraRepositoryBase<Translate>, ITranslateRepository
    {
        public CassTranslateRepository(CassandraContexts cassandraContexts, string tableQuery) : base(
            cassandraContexts.CassandraConnectionSettings, tableQuery)
        {
        }
    }
}
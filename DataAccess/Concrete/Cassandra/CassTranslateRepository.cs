using Core.DataAccess.Cassandra;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Contexts;
using DataAccess.Concrete.Cassandra.Tables;

namespace DataAccess.Concrete.Cassandra
{
    public class CassTranslateRepository : CassandraRepositoryBase<Translate>, ITranslateRepository
    {
        public CassTranslateRepository(CassandraContextBase cassandraContexts) : base(
            cassandraContexts.CassandraConnectionSettings, CassandraTableQueries.Translate)
        {
        }
    }
}
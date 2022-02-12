using Core.DataAccess.Cassandra;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Contexts;

namespace DataAccess.Concrete.Cassandra
{
    public class CassUserClaimRepository : CassandraRepositoryBase<UserClaim>, IUserClaimRepository
    {
        public CassUserClaimRepository(CassandraContextBase cassandraContexts, string tableQuery) : base(
            cassandraContexts.CassandraConnectionSettings, tableQuery)
        {
        }
    }
}
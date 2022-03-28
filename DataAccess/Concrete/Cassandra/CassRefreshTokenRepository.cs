using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.TableMappers;
using Entities.Concrete;

namespace DataAccess.Concrete.Cassandra;

public class CassRefreshTokenRepository : CassandraRepositoryBase<RefreshToken>, IRefreshTokenrepository
{
    public CassRefreshTokenRepository() : base(MappingConfiguration.Global.Define<RefreshTokenMapper>())
    {
    }
}


using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.TableMappers;
using Entities.Concrete;

namespace DataAccess.Concrete.Cassandra;

public class CassTranslateRepository : CassandraRepositoryBase<Translate>, ITranslateRepository
{
    public CassTranslateRepository() : base(MappingConfiguration.Global.Define<TranslateMapper>())
    {
    }
}
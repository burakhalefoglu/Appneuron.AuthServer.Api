using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.TableMappers;
using Entities.Concrete;

namespace DataAccess.Concrete.Cassandra;

public class CassLanguageRepository : CassandraRepositoryBase<Language>, ILanguageRepository
{
    public CassLanguageRepository() : base(MappingConfiguration.Global.Define<LanguageMapper>())
    {
    }
}
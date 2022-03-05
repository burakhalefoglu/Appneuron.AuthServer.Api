using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Tables;
using DataAccess.Concrete.Cassandra.Tables.TableMappers;

namespace DataAccess.Concrete.Cassandra
{
    public class CassLanguageRepository : CassandraRepositoryBase<Language>, ILanguageRepository
    {
        public CassLanguageRepository() : base(CassandraTableQueries.Language,
            MappingConfiguration.Global.Define<LanguageMapper>())
        {
        }
    }
}
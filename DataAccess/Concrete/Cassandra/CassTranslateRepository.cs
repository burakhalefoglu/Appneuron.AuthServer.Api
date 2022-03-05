using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.TableMappers;

namespace DataAccess.Concrete.Cassandra
{
    public class CassTranslateRepository : CassandraRepositoryBase<Translate>, ITranslateRepository
    {
        public CassTranslateRepository() : base(MappingConfiguration.Global.Define<TranslateMapper>())
        {
        }
    }
}
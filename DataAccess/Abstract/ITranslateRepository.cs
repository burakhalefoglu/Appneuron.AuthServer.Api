using Core.DataAccess;
using Core.Entities.Concrete;

namespace DataAccess.Abstract
{
    public interface ITranslateRepository : IDocumentDbRepository<Translate>
    {
    }
}
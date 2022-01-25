using Core.DataAccess;
using Core.Entities.Concrete;

namespace DataAccess.Abstract
{
    public interface IClientRepository : IDocumentDbRepository<Client>
    {
    }
}
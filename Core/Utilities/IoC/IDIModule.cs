using Microsoft.Extensions.DependencyInjection;

namespace Core.Utilities.IoC;

public interface IDIModule
{
    void Load(IServiceCollection services);
}
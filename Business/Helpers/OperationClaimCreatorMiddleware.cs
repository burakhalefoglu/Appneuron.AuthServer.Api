using Business.BusinessAspects;
using Business.Fakes.Handlers.Authorizations;
using Business.Fakes.Handlers.OperationClaims;
using Business.Fakes.Handlers.UserClaims;
using Core.Utilities.IoC;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Business.Helpers
{
    public static class OperationClaimCreatorMiddleware
    {
        public static async Task UseDbOperationClaimCreator(this IApplicationBuilder app)
        {
            var mediator = ServiceTool.ServiceProvider.GetService<IMediator>();
            foreach (var operationName in GetOperationNames())
            {
                await mediator.Send(new CreateOperationClaimInternalCommand
                {
                    ClaimName = operationName
                });
            }
            var operationClaims = (await mediator.Send(new GetOperationClaimsInternalQuery())).Data.ToList();
            var user = await mediator.Send(new RegisterUserInternalCommand
            {
                FullName = "System Admin",
                Password = "(ex9XivwA0sDtc%XX%Mnidj8)JFhLf8cAnaJ&ySqec@^hutPtc9qPA-46qZOZy&l",
                Email = "admin@appneuron.com",
            });
            await mediator.Send(new CreateUserClaimsInternalCommand
            {
                UserId = 1,
                OperationClaims = operationClaims,
                Users = (Core.Entities.Concrete.User)user
            });
        }

        private static IEnumerable<string> GetOperationNames()
        {
            var assemblyNames = Assembly.GetExecutingAssembly().GetTypes()
                            .Where(x =>
                     // runtime generated anonmous type'larin assemblysi olmadigi icin null cek yap
                     x.Namespace != null && x.Namespace.StartsWith("Business.Handlers") &&
                      (x.Name.EndsWith("Command") || x.Name.EndsWith("Query")) &&
                     x.CustomAttributes.All(a => a.AttributeType == typeof(SecuredOperation)))
            .Select(x => x.Name);
            return assemblyNames;
        }
    }
}
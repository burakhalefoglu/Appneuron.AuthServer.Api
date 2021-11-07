using System.Threading.Tasks;
using NUnit.Framework;
using static Business.Handlers.Users.Commands.CreateUserCommand;
using static Business.Handlers.Users.Commands.DeleteUserCommand;
using static Business.Handlers.Users.Commands.UpdateUserCommand;
using static Business.Handlers.Users.Commands.UserChangePasswordCommand;
using static Business.Handlers.Users.Queries.GetUserLookupQuery;
using static Business.Handlers.Users.Queries.GetUserQuery;
using static Business.Handlers.Users.Queries.GetUsersQuery;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class UsersHandlerTests
    {


        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task Handler_XXX_XXX()
        {

        }
    }
}
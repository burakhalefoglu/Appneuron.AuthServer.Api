using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.OperationClaims.Commands;
using Business.Handlers.OperationClaims.Queries;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using static Business.Handlers.OperationClaims.Commands.CreateOperationClaimCommand;
using static Business.Handlers.OperationClaims.Commands.DeleteOperationClaimCommand;
using static Business.Handlers.OperationClaims.Commands.UpdateOperationClaimCommand;
using static Business.Handlers.OperationClaims.Queries.GetOperationClaimLookupQuery;
using static Business.Handlers.OperationClaims.Queries.GetOperationClaimQuery;
using static Business.Handlers.OperationClaims.Queries.GetOperationClaimsQuery;

namespace Tests.Business.Handlers
{
    [TestFixture]
    public class OperationClaimHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _operationClaimRepository = new Mock<IOperationClaimRepository>();

            _createOperationClaimCommandHandler = new CreateOperationClaimCommandHandler(
                _operationClaimRepository.Object);
            _updateOperationClaimCommandHandler =
                new UpdateOperationClaimCommandHandler(_operationClaimRepository.Object);
            _deleteOperationClaimCommandHandler =
                new DeleteOperationClaimCommandHandler(_operationClaimRepository.Object);
            _getOperationClaimLookupQueryHandler =
                new GetOperationClaimLookupQueryHandler(_operationClaimRepository.Object);
            _getOperationClaimQueryHandler = new GetOperationClaimQueryHandler(_operationClaimRepository.Object);
            _getOperationClaimsQueryHandler = new GetOperationClaimsQueryHandler(_operationClaimRepository.Object);
        }

        private Mock<IOperationClaimRepository> _operationClaimRepository;

        private CreateOperationClaimCommandHandler _createOperationClaimCommandHandler;
        private UpdateOperationClaimCommandHandler _updateOperationClaimCommandHandler;
        private DeleteOperationClaimCommandHandler _deleteOperationClaimCommandHandler;

        private GetOperationClaimLookupQueryHandler _getOperationClaimLookupQueryHandler;
        private GetOperationClaimQueryHandler _getOperationClaimQueryHandler;
        private GetOperationClaimsQueryHandler _getOperationClaimsQueryHandler;

        [Test]
        public async Task OperationClaim_Create_ClaimExists()
        {
            var command = new CreateOperationClaimCommand
            {
                ClaimName = "Test"
            };

            _operationClaimRepository.Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .Returns(Task.FromResult(new OperationClaim()));

            var result = await _createOperationClaimCommandHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.OperationClaimExists);
        }


        [Test]
        public async Task OperationClaim_Create_Success()
        {
            var command = new CreateOperationClaimCommand
            {
                ClaimName = "Test"
            };

            _operationClaimRepository.Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .Returns(Task.FromResult<OperationClaim>(null));

            _operationClaimRepository.Setup(x => x.Add(It.IsAny<OperationClaim>()));

            var result = await _createOperationClaimCommandHandler.Handle(command, new CancellationToken());
           
            _operationClaimRepository.Verify(c=> c.SaveChangesAsync());

            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Added);
        }


        [Test]
        public async Task OperationClaim_Update_OperationClaimNotFound()
        {
            var command = new UpdateOperationClaimCommand
            {
                Alias = "Test",
                Description = "Test",
                Id = 1
            };

            _operationClaimRepository.Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .Returns(Task.FromResult<OperationClaim>(null));

            _operationClaimRepository.Setup(x =>
                x.Update(It.IsAny<OperationClaim>()));


            var result = await _updateOperationClaimCommandHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.OperationClaimNotFound);
        }

        [Test]
        public async Task OperationClaim_Update_Success()
        {
            var command = new UpdateOperationClaimCommand
            {
                Alias = "Test",
                Description = "Test",
                Id = 1
            };

            _operationClaimRepository.Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .Returns(Task.FromResult(new OperationClaim()));

            _operationClaimRepository.Setup(x =>
                x.Update(It.IsAny<OperationClaim>()));


            var result = await _updateOperationClaimCommandHandler.Handle(command, new CancellationToken());

            _operationClaimRepository.Verify(c=> c.SaveChangesAsync());

            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Updated);
        }


        [Test]
        public async Task OperationClaim_Delete_Success()
        {
            var command = new DeleteOperationClaimCommand
            {
                Id = 1
            };

            _operationClaimRepository.Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .Returns(Task.FromResult(new OperationClaim()));

            _operationClaimRepository.Setup(x =>
                x.Update(It.IsAny<OperationClaim>()));


            var result = await _deleteOperationClaimCommandHandler.Handle(command, new CancellationToken());
           
            _operationClaimRepository.Verify(c=> c.SaveChangesAsync());

            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Deleted);
        }


        [Test]
        public async Task OperationClaim_Delete_OperationClaimNotFound()
        {
            var command = new DeleteOperationClaimCommand
            {
                Id = 1
            };

            _operationClaimRepository.Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .Returns(Task.FromResult<OperationClaim>(null));

            _operationClaimRepository.Setup(x =>
                x.Update(It.IsAny<OperationClaim>()));


            var result = await _deleteOperationClaimCommandHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.OperationClaimNotFound);
        }


        [Test]
        public async Task OperationClaim_GetOperationClaimLookup_OperationClaimNotFound()
        {
            var command = new GetOperationClaimLookupQuery();

            _operationClaimRepository.Setup(x =>
                    x.GetListAsync(It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .Returns(Task.FromResult<IEnumerable<OperationClaim>>(null));


            var result = await _getOperationClaimLookupQueryHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.OperationClaimNotFound);
        }


        [Test]
        public async Task OperationClaim_GetOperationClaimLookup_Success()
        {
            var command = new GetOperationClaimLookupQuery();

            _operationClaimRepository.Setup(x =>
                    x.GetListAsync(It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .Returns(Task.FromResult<IEnumerable<OperationClaim>>(
                    new List<OperationClaim>
                    {
                        new()
                        {
                            Alias = "Test",
                            Description = "Test",
                            GroupClaims = new List<GroupClaim>(),
                            Id = 1,
                            Name = "Test",
                            UserClaims = new List<UserClaim>()
                        },

                        new()
                        {
                            Alias = "Test1",
                            Description = "Test1",
                            GroupClaims = new List<GroupClaim>(),
                            Id = 2,
                            Name = "Test1",
                            UserClaims = new List<UserClaim>()
                        }
                    }));

            var result = await _getOperationClaimLookupQueryHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Data.Count().Should().BeGreaterThan(1);
        }

        [Test]
        public async Task OperationClaim_GetOperationClaim_Success()
        {
            var command = new GetOperationClaimQuery();

            _operationClaimRepository.Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .Returns(Task.FromResult(
                    new OperationClaim
                    {
                        Alias = "Test",
                        Description = "Test",
                        GroupClaims = new List<GroupClaim>(),
                        Id = 1,
                        Name = "Test",
                        UserClaims = new List<UserClaim>()
                    }));

            var result = await _getOperationClaimQueryHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Data.Name.Should().Be("Test");
        }

        [Test]
        public async Task OperationClaim_GetOperationClaimsQuery_Success()
        {
            var command = new GetOperationClaimsQuery();

            _operationClaimRepository.Setup(x =>
                    x.GetListAsync(It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .Returns(Task.FromResult<IEnumerable<OperationClaim>>(
                    new List<OperationClaim>
                    {
                        new()
                        {
                            Alias = "Test",
                            Description = "Test",
                            GroupClaims = new List<GroupClaim>(),
                            Id = 1,
                            Name = "Test",
                            UserClaims = new List<UserClaim>()
                        },

                        new()
                        {
                            Alias = "Test1",
                            Description = "Test1",
                            GroupClaims = new List<GroupClaim>(),
                            Id = 2,
                            Name = "Test1",
                            UserClaims = new List<UserClaim>()
                        }
                    }));


            var result = await _getOperationClaimsQueryHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Data.Count().Should().BeGreaterThan(1);
        }
    }
}
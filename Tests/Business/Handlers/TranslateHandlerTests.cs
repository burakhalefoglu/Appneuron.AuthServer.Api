using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.Translates.Commands;
using Business.Handlers.Translates.Queries;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using static Business.Handlers.Translates.Commands.CreateTranslateCommand;
using static Business.Handlers.Translates.Commands.DeleteTranslateCommand;
using static Business.Handlers.Translates.Commands.UpdateTranslateCommand;
using static Business.Handlers.Translates.Queries.GetTranslateQuery;
using static Business.Handlers.Translates.Queries.GetTranslatesQuery;

namespace Tests.Business.Handlers
{
    [TestFixture]
    public class TranslateHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _translateRepository = new Mock<ITranslateRepository>();
            _mediator = new Mock<IMediator>();

            _getTranslateQueryHandler = new GetTranslateQueryHandler(_translateRepository.Object, _mediator.Object);
            _getTranslatesQueryHandler = new GetTranslatesQueryHandler(_translateRepository.Object, _mediator.Object);
            _createTranslateCommandHandler =
                new CreateTranslateCommandHandler(_translateRepository.Object);
            _updateTranslateCommandHandler =
                new UpdateTranslateCommandHandler(_translateRepository.Object);
            _deleteTranslateCommandHandler =
                new DeleteTranslateCommandHandler(_translateRepository.Object, _mediator.Object);
        }

        private Mock<ITranslateRepository> _translateRepository;
        private Mock<IMediator> _mediator;

        private GetTranslateQueryHandler _getTranslateQueryHandler;
        private GetTranslatesQueryHandler _getTranslatesQueryHandler;
        private CreateTranslateCommandHandler _createTranslateCommandHandler;
        private UpdateTranslateCommandHandler _updateTranslateCommandHandler;
        private DeleteTranslateCommandHandler _deleteTranslateCommandHandler;

        [Test]
        public async Task Translate_GetQuery_Success()
        {
            //Arrange
            var query = new GetTranslateQuery
            {
                Id = 1
            };

            _translateRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Translate, bool>>>()))
                .ReturnsAsync(new Translate
                    {
                        Code = "TR",
                        Value = "Turkish"
                    }
                );
            //Act
            var x = await _getTranslateQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
        }

        [Test]
        public async Task Translate_GetQueries_Success()
        {
            //Arrange
            var query = new GetTranslatesQuery();

            _translateRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Translate, bool>>>()))
                .ReturnsAsync(new List<Translate>
                {
                    new() {Code = "test", Value = "test"},
                    new() {Code = "test", Value = "Test"}
                }.AsQueryable());

            //Act
            var x = await _getTranslatesQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ToList().Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task Translate_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateTranslateCommand();

            _translateRepository.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Translate, bool>>>()))
                .ReturnsAsync(true);

            _translateRepository.Setup(x => x.AddAsync(It.IsAny<Translate>()));

            var x = await _createTranslateCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task Translate_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateTranslateCommand
            {
                Code = "test",
                Id = 1,
                Value = "Test"
            };

            _translateRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Translate, bool>>>()))
                .ReturnsAsync(new Translate
                {
                    Code = "Test",
                    Value = "Test"
                });

            _translateRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<Translate>()));

            var x = await _updateTranslateCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task Translate_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteTranslateCommand();

            _translateRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Translate, bool>>>()))
                .ReturnsAsync(new Translate());

            _translateRepository.Setup(x => x.UpdateAsync(It.IsAny<Translate>()));

            var x = await _deleteTranslateCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}
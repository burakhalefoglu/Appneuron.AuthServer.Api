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
using Core.Entities.Dtos;
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
using static Business.Handlers.Translates.Queries.GetTranslateListDtoQuery;
using static Business.Handlers.Translates.Queries.GetTranslateWordListQuery;
using static Business.Handlers.Translates.Queries.GetTranslatesByLangQuery;


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
                new CreateTranslateCommandHandler(_translateRepository.Object, _mediator.Object);
            _updateTranslateCommandHandler =
                new UpdateTranslateCommandHandler(_translateRepository.Object, _mediator.Object);
            _deleteTranslateCommandHandler =
                new DeleteTranslateCommandHandler(_translateRepository.Object, _mediator.Object);
            _getTranslateWordListQueryHandler =
                new GetTranslateWordListQueryHandler(_translateRepository.Object, _mediator.Object);
            _getTranslateListDtoQueryHandler =
                new GetTranslateListDtoQueryHandler(_translateRepository.Object, _mediator.Object);
            _getTranslatesByLangQueryHandler =
                new GetTranslatesByLangQueryHandler(_translateRepository.Object, _mediator.Object);
        }

        private Mock<ITranslateRepository> _translateRepository;
        private Mock<IMediator> _mediator;

        private GetTranslateQueryHandler _getTranslateQueryHandler;
        private GetTranslatesQueryHandler _getTranslatesQueryHandler;
        private CreateTranslateCommandHandler _createTranslateCommandHandler;
        private UpdateTranslateCommandHandler _updateTranslateCommandHandler;
        private DeleteTranslateCommandHandler _deleteTranslateCommandHandler;
        private GetTranslateWordListQueryHandler _getTranslateWordListQueryHandler;
        private GetTranslateListDtoQueryHandler _getTranslateListDtoQueryHandler;
        private GetTranslatesByLangQueryHandler _getTranslatesByLangQueryHandler;

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
                        Id = 1,
                        Value = "Türkçe",
                        Languages = new Language(),
                        LangId = 1
                    }
                );
            //Act
            var x = await _getTranslateQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.LangId.Should().Be(1);
        }

        [Test]
        public async Task Translate_GetQueries_Success()
        {
            //Arrange
            var query = new GetTranslatesQuery();

            _translateRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Translate, bool>>>()))
                .ReturnsAsync(new List<Translate>
                {
                    new() { Id = 1, Code = "test", LangId = 1, Value = "Deneme" },
                    new() { Id = 2, Code = "test", LangId = 2, Value = "Test" }
                });

            //Act
            var x = await _getTranslatesQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<Translate>)x.Data).Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task Translate_GetTranslateWordList_Success()
        {
            //Arrange
            var query = new GetTranslateWordListQuery
            {
                Lang = "Test"
            };

            _translateRepository.Setup(x =>
                    x.GetTranslateWordList(It.IsAny<string>()))
                .ReturnsAsync(new Dictionary<string, string>
                {
                    { "Test", "Test" },
                    { "Test2", "Test2" }
                });

            //Act
            var x = await _getTranslateWordListQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task Translate_GetTranslatesByLang_Success()
        {
            //Arrange
            var query = new GetTranslatesByLangQuery
            {
                Lang = "Test"
            };

            _translateRepository.Setup(x =>
                    x.GetTranslatesByLang(It.IsAny<string>()))
                .ReturnsAsync("Test");

            //Act
            var x = await _getTranslatesByLangQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
        }


        [Test]
        public async Task Translate_GetTranslateListDto_Success()
        {
            //Arrange
            var query = new GetTranslateListDtoQuery();

            _translateRepository.Setup(x =>
                    x.GetTranslateDto())
                .ReturnsAsync(new List<TranslateDto>
                {
                    new()
                    {
                        Code = "Tr",
                        Id = 1,
                        Language = "Türkçe",
                        Value = "Türkçe"
                    },

                    new()
                    {
                        Code = "Eng",
                        Id = 2,
                        Language = "ingilizce",
                        Value = "ingilizce"
                    }
                });

            //Act
            var x = await _getTranslateListDtoQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.Count().Should().BeGreaterThan(1);
        }

        [Test]
        public async Task Translate_CreateCommand_Success()
        {
            Translate rt = null;
            //Arrange
            var command = new CreateTranslateCommand
            {
                Code = "TR",
                Value = "Trestdfs",
                LangId = 1
            };


            _translateRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Translate, bool>>>()))
                .ReturnsAsync(rt);

            _translateRepository.Setup(x => x.Add(It.IsAny<Translate>())).Returns(new Translate());

            var x = await _createTranslateCommandHandler.Handle(command, new CancellationToken());

            _translateRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Translate_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateTranslateCommand();
            //propertyler buraya yazılacak
            //command.TranslateName = "test";

            _translateRepository.Setup(x => x.Query())
                .Returns(new List<Translate>
                {
                    new()
                    {
                        /*TODO:propertyler buraya yazılacak TranslateId = 1, TranslateName = "test"*/
                    }
                }.AsQueryable());

            _translateRepository.Setup(x => x.Add(It.IsAny<Translate>())).Returns(new Translate());

            var x = await _createTranslateCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task Translate_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateTranslateCommand();
            command.Code = "test";
            command.Id = 1;
            command.Value = "Test";

            _translateRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Translate, bool>>>()))
                .ReturnsAsync(new Translate
                {
                    LangId = 1,
                    Code = "Test",
                    Id = 1,
                    Languages = new Language(),
                    Value = "Test"
                });

            _translateRepository.Setup(x => x.Update(It.IsAny<Translate>())).Returns(new Translate());

            var x = await _updateTranslateCommandHandler.Handle(command, new CancellationToken());

            _translateRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task Translate_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteTranslateCommand();

            _translateRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Translate, bool>>>()))
                .ReturnsAsync(new Translate
                {
                    Id = 1
                });

            _translateRepository.Setup(x => x.Delete(It.IsAny<Translate>()));

            var x = await _deleteTranslateCommandHandler.Handle(command, new CancellationToken());

            _translateRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}
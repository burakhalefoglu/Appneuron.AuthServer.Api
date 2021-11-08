using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.Languages.Commands;
using Business.Handlers.Languages.Queries;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using static Business.Handlers.Languages.Commands.CreateLanguageCommand;
using static Business.Handlers.Languages.Commands.DeleteLanguageCommand;
using static Business.Handlers.Languages.Commands.UpdateLanguageCommand;
using static Business.Handlers.Languages.Queries.GetLanguageQuery;
using static Business.Handlers.Languages.Queries.GetLanguagesQuery;

namespace Tests.Business.Handlers
{
    [TestFixture]
    public class LanguageHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _languageRepository = new Mock<ILanguageRepository>();
            _mediator = new Mock<IMediator>();

            _createLanguageCommandHandler = new CreateLanguageCommandHandler(_languageRepository.Object);
            _getLanguageQueryHandler = new GetLanguageQueryHandler(_languageRepository.Object, _mediator.Object);
            _getLanguagesQueryHandler = new GetLanguagesQueryHandler(_languageRepository.Object, _mediator.Object);
            _updateLanguageCommandHandler =
                new UpdateLanguageCommandHandler(_languageRepository.Object, _mediator.Object);
            _deleteLanguageCommandHandler =
                new DeleteLanguageCommandHandler(_languageRepository.Object, _mediator.Object);
        }

        private Mock<ILanguageRepository> _languageRepository;
        private Mock<IMediator> _mediator;

        private CreateLanguageCommandHandler _createLanguageCommandHandler;
        private GetLanguageQueryHandler _getLanguageQueryHandler;
        private GetLanguagesQueryHandler _getLanguagesQueryHandler;
        private UpdateLanguageCommandHandler _updateLanguageCommandHandler;
        private DeleteLanguageCommandHandler _deleteLanguageCommandHandler;

        [Test]
        public async Task Language_GetQuery_Success()
        {
            //Arrange
            var query = new GetLanguageQuery();
            query.Id = 1;

            _languageRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<Language, bool>>>()))
                .ReturnsAsync(new Language
                    {
                        Code = "Test",
                        Id = 1,
                        Name = "Test",
                        Translates = new List<Translate>()
                    }
                );

            //Act
            var x = await _getLanguageQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.Id.Should().Be(1);
        }

        [Test]
        public async Task Language_GetQueries_Success()
        {
            //Arrange
            var query = new GetLanguagesQuery();

            _languageRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Language, bool>>>()))
                .ReturnsAsync(new List<Language>
                {
                    new() { Id = 1, Code = "tr-TR", Name = "Türkçe" },
                    new() { Id = 2, Code = "en-US", Name = "English" }
                });


            //Act
            var x = await _getLanguagesQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<Language>)x.Data).Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task Language_CreateCommand_Success()
        {
            Language rt = null;
            //Arrange
            var command = new CreateLanguageCommand
            {
                Code = "Test",
                Name = "Test"
            };

            _languageRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Language, bool>>>()))
                .ReturnsAsync(rt);

            _languageRepository.Setup(x => x.Add(It.IsAny<Language>())).Returns(new Language());

            var x = await _createLanguageCommandHandler.Handle(command, new CancellationToken());

            _languageRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Language_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateLanguageCommand();
            command.Code = "test";
            command.Name = "test";

            _languageRepository.Setup(x => x.Query())
                .Returns(new List<Language>
                {
                    new()
                    {
                        Code = "test",
                        Id = 1,
                        Name = "test",
                        Translates = new List<Translate>()
                    },

                    new()
                    {
                        Code = "test",
                        Id = 2,
                        Name = "test",
                        Translates = new List<Translate>()
                    }
                }.AsQueryable());

            _languageRepository.Setup(x => x.Add(It.IsAny<Language>())).Returns(new Language());

            var x = await _createLanguageCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task Language_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateLanguageCommand
            {
                Code = "test",
                Id = 1,
                Name = "test"
            };


            _languageRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Language, bool>>>()))
                .ReturnsAsync(new Language
                {
                    /*TODO:propertyler buraya yazılacak LanguageId = 1, LanguageName = "deneme"*/
                });

            _languageRepository.Setup(x => x.Update(It.IsAny<Language>())).Returns(new Language());

            var x = await _updateLanguageCommandHandler.Handle(command, new CancellationToken());

            _languageRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task Language_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteLanguageCommand
            {
                Id = 1
            };

            _languageRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Language, bool>>>()))
                .ReturnsAsync(new Language
                {
                    Code = "Test",
                    Id = 1,
                    Name = "Test",
                    Translates = new List<Translate>()
                });

            _languageRepository.Setup(x => x.Delete(It.IsAny<Language>()));

            var x = await _deleteLanguageCommandHandler.Handle(command, new CancellationToken());

            _languageRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}
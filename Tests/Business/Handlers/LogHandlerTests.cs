using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using DataAccess.Abstract;
using MediatR;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Business.Handlers.Logs.Queries;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Core.Entities.Concrete;
using FluentAssertions;
using static Business.Handlers.Logs.Queries.GetLogDtoQuery;
namespace Tests.Business.Handlers
{
    [TestFixture]
    public class LogHandlerTests
    {
        private Mock<ILogRepository> _logRepository;
        private Mock<IMediator> _mediator;

        private GetLogDtoQueryHandler _getLogDtoQueryHandler;
        [SetUp]
        public void Setup()
        {
            _logRepository = new Mock<ILogRepository>();
            _mediator = new Mock<IMediator>();

            _getLogDtoQueryHandler = new GetLogDtoQueryHandler(_logRepository.Object,
                _mediator.Object);

        }

        [Test]
        public async Task Log_GetQuery_Success()
        {
            var query = new GetLogDtoQuery();

            _logRepository.Setup(x => x.GetListAsync(
                    It.IsAny<Expression<Func<Log, bool>>>()))
                .Returns(Task.FromResult<IEnumerable<Log>>(new List<Log>
                {
                    new Log
                    {
                        Exception = "NoContentException",
                        Id = 1,
                        Level = "Test",
                        MessageTemplate ="{'Id':1," +
                                         "'Level':'Test'," +
                                         "'ExceptionMessage':'Test'," +
                                         "'TimeStamp':'0001-01-01T00:00:00+00:00'," +
                                         "'User':'Test'," +
                                         "'Value':'Test'," +
                                         "'Type':'Test'}",
                        TimeStamp = new DateTimeOffset()
                    },
                    new Log
                    {
                        Exception = "ValidContentException",
                        Id = 1,
                        Level = "Test",
                        MessageTemplate ="{'Id':1," +
                                        "'Level':'Test'," +
                                        "'ExceptionMessage':'Test'," +
                                        "'TimeStamp':'0001-01-01T00:00:00+00:00'," +
                                        "'User':'Test'," +
                                        "'Value':'Test'," +
                                        "'Type':'Test'}",
                        TimeStamp = new DateTimeOffset()
                    }
                }.AsQueryable()));

            var result = await _getLogDtoQueryHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Data.Count().Should().BeGreaterThan(1);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Handlers.Logs.Queries;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using static Business.Handlers.Logs.Queries.GetLogDtoQuery;

namespace Tests.Business.Handlers;

[TestFixture]
public class LogHandlerTests
{
    [SetUp]
    public void Setup()
    {
        _logRepository = new Mock<ILogRepository>();
        _mediator = new Mock<IMediator>();

        _getLogDtoQueryHandler = new GetLogDtoQueryHandler(_logRepository.Object,
            _mediator.Object);
    }

    private Mock<ILogRepository> _logRepository;
    private Mock<IMediator> _mediator;

    private GetLogDtoQueryHandler _getLogDtoQueryHandler;

    [Test]
    public async Task Log_GetQuery_Success()
    {
        var query = new GetLogDtoQuery();

        _logRepository.Setup(x => x.GetListAsync(
                It.IsAny<Expression<Func<Log, bool>>>()))
            .ReturnsAsync(new List<Log>
            {
                new()
                {
                    Exception = "NoContentException",
                    Level = "Test",
                    MessageTemplate = "{'Id':1," +
                                      "'Level':'Test'," +
                                      "'ExceptionMessage':'Test'," +
                                      "'TimeStamp':'0001-01-01T00:00:00+00:00'," +
                                      "'User':'Test'," +
                                      "'Value':'Test'," +
                                      "'Type':'Test'}"
                },
                new()
                {
                    Exception = "ValidContentException",
                    Level = "Test",
                    MessageTemplate = "{'Id':1," +
                                      "'Level':'Test'," +
                                      "'ExceptionMessage':'Test'," +
                                      "'TimeStamp':'0001-01-01T00:00:00+00:00'," +
                                      "'User':'Test'," +
                                      "'Value':'Test'," +
                                      "'Type':'Test'}"
                }
            }.AsQueryable());

        var result = await _getLogDtoQueryHandler.Handle(query, new CancellationToken());
        result.Success.Should().BeTrue();
        result.Data.Count().Should().BeGreaterThan(1);
    }
}
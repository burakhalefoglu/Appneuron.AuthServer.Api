using Business.Constants;
using Business.Handlers.UserProjects.Commands;
using Business.Handlers.UserProjects.Queries;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using static Business.Handlers.UserProjects.Commands.CreateUserProjectCommand;
using static Business.Handlers.UserProjects.Commands.DeleteUserProjectCommand;
using static Business.Handlers.UserProjects.Commands.UpdateUserProjectCommand;
using static Business.Handlers.UserProjects.Queries.GetUserProjectQuery;
using static Business.Handlers.UserProjects.Queries.GetUserProjectsQuery;
using static Business.Handlers.UserProjects.Queries.GetUserProjectsByUserIdQuery;

namespace Tests.Business.Handlers
{
    [TestFixture]
    public class UserProjectHandlerTests
    {
        private Mock<IUserProjectRepository> _userProjectRepository;
        private Mock<IMediator> _mediator;
        private Mock<IHttpContextAccessor> _httpContextAccessor;

        private GetUserProjectQueryHandler _getUserProjectQueryHandler;
        private GetUserProjectsQueryHandler _getUserProjectsQueryHandler;
        private GetUserProjectsByUserIdQueryHandler _getUserProjectsByUserIdQueryHandler;

        private CreateUserProjectCommandHandler _createUserProjectCommandHandler;
        private UpdateUserProjectCommandHandler _updateUserProjectCommandHandler;
        private DeleteUserProjectCommandHandler _deleteUserProjectCommandHandler;

        [SetUp]
        public void Setup()
        {
            _userProjectRepository = new Mock<IUserProjectRepository>();
            _mediator = new Mock<IMediator>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _getUserProjectQueryHandler = new GetUserProjectQueryHandler(_userProjectRepository.Object, _mediator.Object);
            _getUserProjectsQueryHandler = new GetUserProjectsQueryHandler(_userProjectRepository.Object, _mediator.Object);
            _getUserProjectsByUserIdQueryHandler = new GetUserProjectsByUserIdQueryHandler(
                _userProjectRepository.Object, _mediator.Object, _httpContextAccessor.Object);

            _createUserProjectCommandHandler = new CreateUserProjectCommandHandler(_userProjectRepository.Object, _mediator.Object);
            _updateUserProjectCommandHandler = new UpdateUserProjectCommandHandler(_userProjectRepository.Object, _mediator.Object);
            _deleteUserProjectCommandHandler = new DeleteUserProjectCommandHandler(_userProjectRepository.Object, _mediator.Object);

        }

        [Test]
        public async Task UserProject_GetQuery_Success()
        {
            //Arrange
            var query = new GetUserProjectQuery();

            _userProjectRepository.Setup(x => x.
                GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync(new UserProject()
                {
                    Id = 1,
                    ProjectKey = "sadsfdsfsad",
                    UserId = 12
                });

            //Act
            var x = await _getUserProjectQueryHandler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.Id.Should().Be(1);
        }

        [Test]
        public async Task UserProject_GetQueries_Success()
        {
            //Arrange
            var query = new GetUserProjectsQuery();

            _userProjectRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                        .ReturnsAsync(new List<UserProject> {

                            new UserProject()
                            {
                                Id = 1,
                                ProjectKey = "sdfsdfsdf",
                                UserId = 1
                            },

                            new UserProject()
                            {
                                Id = 12,
                                ProjectKey = "dasdsa",
                                UserId = 2
                            },

                        });

            //Act
            var x = await _getUserProjectsQueryHandler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<UserProject>)x.Data).Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task UserProject_GetUserProjectsByUserId_Success()
        {
            //Arrange
            var query = new GetUserProjectsByUserIdQuery();

            _userProjectRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                        .ReturnsAsync(new List<UserProject> {

                            new UserProject()
                            {
                                Id = 1,
                                ProjectKey = "sdfsdfsdf",
                                UserId = 1
                            },

                            new UserProject()
                            {
                                Id = 12,
                                ProjectKey = "dasdsa",
                                UserId = 2
                            },

                        });

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEwIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjpbImdQek0rSVhvejVhcjMxaVRPbWZteWc9PSIsIjFTZzY4dzlYSlgxT2ZKZDdnUXBZZ0E9PSIsIlFJQis5bEhXeS9NbityRVFHQmhuemU3Rm5ZWHRZVktab1VKdzdZOUMzTGM9IiwiZVdtTjMwTUk0SDJTb2VPdW55aGVkK0dMdjVZNVdYdXlJQWZ1ZGhEQnNyaz0iLCJtazJYQVpoY3lsN2hOL3pIenNDMEhPOSt4TEMxbWRQNmhwTnR4R0tjamZ3PSIsImFGL1FSNjhHVDB1dzlnWHZrRy9SZ0lZZWZSUStMVVllSXZUUm9jS1ZIL1U9IiwiQlY0QmhCOGpVRE4ycCtLem1jUmlUZjJld0J6RTRRRWpkNFc4c0pzZk5hcz0iLCJQUTF1R0hQcGZ4L0JwMEpNT1kzQVlvVjkxWC95a2RnNk9ibHNTZldoUURzPSIsIjBjb0ljQmdhOG93anQ1ZXg1SWZQb3E4RmE4SUVPSWxJT2RvSWh0UWFQb0E9IiwidE4xRnJtWElIbm42OEtaTW5kY1VYVXBrOHNPRmRyZE90akU3Q0tZcjZCaz0iLCIwRi8xUTVJMEtFVnFYWFNKVXUyWWhBdUFJbkNDemZkdXFUVHR5QmNuZ3c0PSIsIlczclBmWGRJZWNQT0w3SXpCT0t1ZHIrT01hNU5lUWJ1TmRpZ25LSkRnSlE9Iiwia1dzdzcxNzU2NXNqWVRpeVVmOXNhcGV5NThCYUtOREMwTWMvRG5McmZ3ST0iLCJpZEMwOTNQUmNjaHR1L2tQOE5WMkp4U1pHcGVLOXd0Z1VyMVdmNmZRbFNJPSIsIlYvS2Y4TWI3bE95ZW1qS3NrVW0wZm9URFNnWUZ2M1dXS0xOWlUrQ01NQ3c9IiwiVi9LZjhNYjdsT3llbWpLc2tVbTBmdFhwWVZZMXl4Q0xUVGZHeUdocWFNaz0iLCJWMDNTUmRSVDc2MURIVEZLcTZsemFUWU9TMUs1ZDNKOXZPeTU1cmlBNzRJPSIsIkhleGZXYmhvMndJaklncWFPRmZ5ayt4dklaazNuei9QaTN5TU1vbDhUNDg9IiwiMmVxa1JWOFo2RjVyb0Rzc0xnV0NKQ0VWanphUXNVM0R6R1lGSy9hbHl0RT0iLCJYcXd1aUN2VlBwWFpjdkZqQndXamkvUWxUY2pua050QlhWYU9NTmRNMlFFPSIsIktaZWZUUXB5ME1nNHZ3UkVMSDJNbXc2U0Q3WTVuejI3OVVjUi83MWsrNGM9IiwiSTF6Q0RxRUc1aGNmNUFIc2h4YWhHQk1VTFJocUhjOHViNi8wM2N3N0QvST0iLCJJMXpDRHFFRzVoY2Y1QUhzaHhhaEdOTUg0RnRVZkRUWm9JeGZRVWxMa3d3PSIsIlYwM1NSZFJUNzYxREhURktxNmx6YVMrbCt4UGdCb2RzMnJjRmp1STk0dFU9IiwiUTJ5SzFGQ2E4REpqOStoRFlCaXFpRXJZcVBZSkpWclVBakVPQnlQN0NKdz0iLCIwZWNLZTRWVGhlK0NyMjQ2a0prR0o1ZDlGYVJ3TU5IeU9tRElyMDBXZnNHd0RORlR1VkdXN0hCRElYelJvMVJqIiwiSWg3aHlGdEFmbldncElGQnIwYW5jUkl0Ukt2b0x0dkorN3BkZ0RrNDdQS1EwSm9qRWJlQ2lKME5yOU1FOWFJY1djcG5RMy9GaUNEQisrMHZiOCt0RXc9PSIsIlF3Vm1Xbk9sbE44WW4xRWlQR2g2bkZZZVZ3eWJySjFLODFWcllKMUJZTmdOSWJBaHkwOU1QdytqNWFxaktSNTUiLCIrQ1AyL1Zob3IvNGYvOEJiaU9JSzRSR05sTWdpS0psMTlSejQzWFRTWGswPSIsIkRQL0owYWF1SEY2bkJVOFVwYTRibG1Qd2p6WnhyWklxKzljcG5nNVgyQlE9IiwieUF4SEg4Vi9KM0h3V3FHdXBmMlpYTE9HMTRSUitxTThXRFhwMVVEWHN6dWxwRWxDQkZ1NUFDcTdhK0tBY24zZSIsIlJnQWFBS2MrZGt4bUhvN0ZHS21mcmFZSGFzQnNYelpyMEZOek1tV1YzUDZ3Qm5pOTFvV01DVkxqR1JnbDhycWoiLCI0NmZpZjhtV0VUeTVLVEVNTEtXMnBUTlI0alhrUEEzd2ZOM2dnMUY4OWQzMXgwZ1Btek9xLzN5OEs1L3JhMDNsIiwiUDIyRTJadjc4empvNlIyY3NTM0hZVDlVeHVXK1Q3QzNDdlh3Q1hXN3VITjF0T3VFVkJFeU83VHJ2UndhN0tCQiIsIjBlY0tlNFZUaGUrQ3IyNDZrSmtHSjFGTUtDdWJreWlWNVFpOXR2MFhNSXY1UElBVVFpRDhPdUJVUndxSGdLYk4iLCJna0FYTitLa0h1cWNjNzVUVm9lSzFVcDdDeEx5YUdTeEVUTjNjT3BKdUR0R0g0OVZMc1N3SmJva0M3SmQwK2NIIiwiVTh5dDJ1eEd5enlodDNrTlIrVzVOOS9HZW45ZjFWc0dxQUJqZWNCWS90em9qTHU5THlnbzJWeXNvR3R0SjZOUSIsIi9OaWhtQ1B0ZUhiTmlDSEp0OUlxRDRlSDl1VGNqYzFsWVZwYkp3NDZ5L1BSMzAzU1JHVkVUZTlIaUJHZDNEcHkiLCJBZHpodFZ1YlVLRDUyUExQN0s3STd5SmJCNGtYeVV6b0Q4UHJrYWw4QTdZVDc1Yk9QV0hkNFpmd1R0OHNjdmZLclc0aXJvNHd0QWtKT1p0cCtzb3hFZz09IiwiR1E4ZzQwd0krdk9RWHd4YThkMGw0bXJzV1g5ME9ibzRuMFB5MjM3MmtUeDVNa2xqV2JOSTAxdGRQNm1jSGk1WSIsInU3aUFDSVN6UTdKRTBhb0pYbEtMa2ZuUEtOb1gwYmxHYzB6VzFodUpQVERSQTdGTDJpZ0R1ZHVNN1VDWXhhclEiLCJ4Q0dablZaZWpBZDR2aXU1Q29MVUxVM2ZNTkF4czN6S1FVK0I4YTJoU0VtWjBTU3VodjR1MUNJS01aN0t0VXIwIiwiMWVhY2tpZTc5dXpJUFF4ZVBjTkw5WFJZc2xTTndwVkUrd01WTTQxN3g5WT0iLCI2VW55dWdVUUlzK3BrZWVNd0M2YVMzUDFTZzA4OWlUQ2ZNWHVveDY0bjAvTU9pUjRTTVk4WStCMHNIbjVCV1pCIiwidHdkTkxtRXovQ01rTXhYL092ME5oVnhTdjFZV2NHZFBmQWdqK3RsNHYzVT0iLCI2VUhEcjVDWlN3S2MxTFV6Q2xvemRqSHdEZDF5NDE5VXNrOCt4bEVFaFpjPSIsInNNVFMrTVZCSGNldEh4YmJsQm1XdDZ4NG1iYzljb3VtZXJxWmI5bGhsWGM9Il0sIlVuaXF1ZUtleSI6IjIzOUNCRjBGM0EwNDI3RDE0QkU1RUY4NzFDMDJCRENFIiwiUHJvamVjdElkIjoiZjk3NWRhZDJjYzBhMGJmNzczOWQ4ZGQ4ZDdhNDZlYmMzNTBkMWI3MTQwZDNlMTY2Y2I0YzUwYjhkYjQ4MzA1NSIsIm5iZiI6MTYzNDQwNDg2MywiZXhwIjoxNjM0NDA4NDYzLCJpc3MiOiJ3d3cuYXBwbmV1cm9uLmNvbSIsImF1ZCI6ImFwaS5hcHBuZXVyb24uY3VzdG9tZXIifQ.cR5b_M3UDWISVhHpMM37zraJoyWGRHga7ihit5DaND8";

            _httpContextAccessor.SetupGet(x =>
                x.HttpContext).Returns(new DefaultHttpContext());


            //Act
            var x = await _getUserProjectsByUserIdQueryHandler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<UserProject>)x.Data).Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task UserProject_CreateCommand_Success()
        {
            //Arrange
            var command = new CreateUserProjectCommand
            {
                ProjectKey = "fsdfsdffsd",
                UserId = 1
            };

            _userProjectRepository.Setup(x => x
                    .GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .Returns(Task.FromResult<UserProject>(null));

            _userProjectRepository.Setup(x =>
                x.Add(It.IsAny<UserProject>())).Returns(new UserProject
                {
                    Id = 1,
                    ProjectKey = "fsdfsdffsd",
                    UserId = 1
                });

            var x = await _createUserProjectCommandHandler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task UserProject_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateUserProjectCommand
            {
                ProjectKey = "fsdfsdffsd",
                UserId = 1
            };

            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                                           .Returns(Task.FromResult<UserProject>(new UserProject()));

            _userProjectRepository.Setup(x => x.Add(It.IsAny<UserProject>())).Returns(new UserProject());

            var x = await _createUserProjectCommandHandler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task UserProject_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateUserProjectCommand
            {
                Id = 1,
                ProjectKey = "fdsfsdf",
                UserId = 1
            };


            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                        .ReturnsAsync(new UserProject()
                        {
                            Id = 1,
                            ProjectKey = "kjhkj",
                            UserId = 1
                        });

            _userProjectRepository.Setup(x => x.Update(It.IsAny<UserProject>())).Returns(new UserProject());

            var x = await _updateUserProjectCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _userProjectRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task UserProject_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteUserProjectCommand
            {
                Id = 1
            };

            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                        .ReturnsAsync(new UserProject()
                        {
                            Id = 1,
                            ProjectKey = "fsdfsdf",
                            UserId = 1
                        });

            _userProjectRepository.Setup(x => x.Delete(It.IsAny<UserProject>()));

            var x = await _deleteUserProjectCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _userProjectRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}
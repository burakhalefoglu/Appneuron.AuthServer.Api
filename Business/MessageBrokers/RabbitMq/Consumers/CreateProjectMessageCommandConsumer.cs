using Business.Fakes.Handlers.UserProjects;
using Business.MessageBrokers.RabbitMq.Models;
using MassTransit;
using MediatR;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Business.MessageBrokers.RabbitMq.Consumers
{
    public class CreateProjectMessageCommandConsumer : IConsumer<ProjectMessageCommand>
    {
        private readonly IMediator _mediator;

        public CreateProjectMessageCommandConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<ProjectMessageCommand> context)
        {
            Debug.WriteLine($"UserId: {context.Message.UserId} , ProjectKey: {context.Message.ProjectKey}");

            var result = await _mediator.Send(new CreateUserProjectInternalCommand
            {
                UserId = context.Message.UserId,
                ProjectKey = context.Message.ProjectKey
            });

            Debug.WriteLine($"Message: {result.Message} , Success: {result.Success}");
        }
    }
}
using Business.Fakes.Handlers.UserProjects;
using Core.Utilities.IoC;
using Entities.Dtos;
using MediatR;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Business.CustomBackgroundService
{
    public class RabbitMqBackgroundService : BackgroundService
    {
        private readonly IMediator _mediator;
        public RabbitMqBackgroundService(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Debug.WriteLine("çalıştı");
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "123456"
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.BasicQos(0, 1, false);
            channel.QueueDeclare(queue: "DArchQueue",
                                                     durable: true,
                                                     exclusive: false,
                                                     autoDelete: false,
                                                     arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: "DArchQueue",
                                                autoAck: false,
                                                consumer: consumer);

            consumer.Received += async (object sender, BasicDeliverEventArgs e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var project = JsonSerializer.Deserialize<ProjectDto>(message);

                Debug.WriteLine($"UserId: {project.UserId} , ProjectKey: {project.ProjectKey}");

                var result = await _mediator.Send(new CreateUserProjectInternalCommand
                {
                    UserId = project.UserId,
                    ProjectKey = project.ProjectKey

                });

                Debug.WriteLine($"Message: {result.Message} , Success: {result.Success}");

                if (result.Success)
                    channel.BasicAck(e.DeliveryTag, false);
            };


        }
    }
}

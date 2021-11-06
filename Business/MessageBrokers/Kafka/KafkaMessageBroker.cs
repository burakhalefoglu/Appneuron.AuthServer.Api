using Business.MessageBrokers.Kafka.Model;
using Confluent.Kafka;
using Core.Utilities.IoC;
using Core.Utilities.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Business.Fakes.Handlers.GroupClaims;
using Business.Fakes.Handlers.UserClaims;
using Business.Fakes.Handlers.UserProjects;
using Business.Handlers.UserProjects.Queries;
using Business.Services.Authentication;
using Core.Entities.ClaimModels;
using Core.Entities.Concrete;
using Core.Entities.Dtos;
using MassTransit;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Business.MessageBrokers.Models;

namespace Business.MessageBrokers.Kafka
{
    public class KafkaMessageBroker : IKafkaMessageBroker
    {
        IConfiguration Configuration;
        KafkaOptions kafkaOptions;
        private readonly IMediator _mediator;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly ITokenHelper _tokenHelper;
        private readonly IUserRepository _userRepository;

        public KafkaMessageBroker(IMediator mediator,
           ISendEndpointProvider sendEndpointProvider,
           ITokenHelper tokenHelper,
           IUserRepository userRepository)
        {
            _mediator = mediator;
            _sendEndpointProvider = sendEndpointProvider;
            _tokenHelper = tokenHelper;
            _userRepository = userRepository;
            Configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
            kafkaOptions = Configuration.GetSection("ApacheKafka").Get<KafkaOptions>();

        }

        public async Task GetProjectCreationMessage()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = $"{kafkaOptions.HostName}:{kafkaOptions.Port}",
                GroupId = "ProjectCreationConsumerGroup",
                EnableAutoCommit = false,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true,
                PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky
            };


            using (var consumer = new ConsumerBuilder<Ignore, string>(config)
           .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
           .SetStatisticsHandler((_, json) => Console.WriteLine($"Statistics: {json}"))
           .Build())
            {
                consumer.Subscribe("ProjectMessageCommand");

                try
                {
                    while (true)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume();

                            if (consumeResult.IsPartitionEOF)
                            {
                                Console.WriteLine(
                                    $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");

                                continue;
                            }

                            Console.WriteLine($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Message.Value}");



                            var projectMessageCommand = JsonConvert.DeserializeObject<ProjectMessageCommand>(consumeResult.Message.Value,
                            new JsonSerializerSettings
                            {
                                PreserveReferencesHandling = PreserveReferencesHandling.Objects
                            });

                            var result = await _mediator.Send(new CreateUserProjectInternalCommand
                            {
                                UserId = projectMessageCommand.UserId,
                                ProjectKey = projectMessageCommand.ProjectKey
                            });

                            var user = await _userRepository.GetAsync(u => u.UserId == projectMessageCommand.UserId);

                            if (user == null)
                                return;

                            //New Token Creation
                            var GrupClaims = (await _mediator.Send(new GetGroupClaimsLookupByGroupIdInternalQuery()
                            {
                                GroupId = 1
                            }));

                            List<SelectionItem> selectionItems = GrupClaims.Data.ToList();
                            List<OperationClaim> operationClaims = new List<OperationClaim>();

                            foreach (var item in selectionItems)
                            {
                                operationClaims.Add(new OperationClaim
                                {
                                    Id = int.Parse(item.Id),
                                    Name = item.Label
                                });
                            }

                            await _mediator.Send(new CreateUserClaimsInternalCommand
                            {
                                UserId = user.UserId,
                                OperationClaims = operationClaims,
                            });

                            var ProjectIdResult = await _mediator.Send(new GetUserProjectsInternalQuery
                            {
                                UserId = user.UserId,
                            });
                            List<string> ProjectIdList = new List<string>();
                            ProjectIdResult.Data.ToList().ForEach(x =>
                            {
                                ProjectIdList.Add(x.ProjectKey);
                            });

                            var accessToken = _tokenHelper.CreateCustomerToken<DArchToken>(new UserClaimModel
                            {
                                UserId = user.UserId,
                                OperationClaims = operationClaims.Select(x => x.Name).ToArray()
                            }, ProjectIdList);

                            var kafkaResult = await SendMessageAsync(new ProjectCreationResult
                            {

                                Accesstoken = accessToken.Token,
                                UserId = user.UserId
                            });

                            if (kafkaResult.Success)
                            {
                                try
                                {
                                    consumer.Commit(consumeResult);
                                }
                                catch (KafkaException e)
                                {
                                    Console.WriteLine($"Commit error: {e.Error.Reason}");
                                }
                            }

                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Consume error: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Closing consumer.");
                    consumer.Close();
                }
            }

        }



        public async Task<IResult> SendMessageAsync<T>(T messageModel) where T :
         class, new()
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = $"{kafkaOptions.HostName}:{kafkaOptions.Port}",
                Acks = Acks.All
            };

            var message = JsonConvert.SerializeObject(messageModel);
            var topicName = typeof(T).Name;
            using (var p = new ProducerBuilder<Null, string>(producerConfig).Build())
            {
                try
                {
                    var partitionCount = KafkaAdminHelper.SetPartitionCountAsync(topicName);

                    await p.ProduceAsync(new TopicPartition(topicName,
                        new Partition(new Random().Next(0, partitionCount)))
                    , new Message<Null, string>
                    {
                        Value = message

                    });
                    return new SuccessResult();

                }

                catch (ProduceException<Null, string> e)
                {
                    return new ErrorResult();
                }
            }
        }
    }
}

using MediatR;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.MessageBrokers.RabbitMq
{
    public class MqConsumerHelper : IMessageConsumer
    {
        private readonly MessageBrokerOptions _brokerOptions;

        public MqConsumerHelper(IConfiguration configuration)
        {
            _brokerOptions = configuration.GetSection("MessageBrokerOptions").Get<MessageBrokerOptions>();
        }

        public async Task GetQueue()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _brokerOptions.HostName,
                UserName = _brokerOptions.UserName,
                Password = _brokerOptions.Password
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "DArchQueue",
                                                         durable: true,
                                                         exclusive: false,
                                                         autoDelete: false,
                                                         arguments: null);


                channel.BasicQos(0, 1, false);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += async (model, mq) =>
                {

                    var body = mq.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    //işlem başarılı olursa silmeli
                    Debug.WriteLine($"Message: {message}");

                    channel.BasicAck(mq.DeliveryTag, false);

                };

                channel.BasicConsume(queue: "DArchQueue",
                                                      autoAck: false,
                                                      consumer: consumer);

            }
        }
    }
}
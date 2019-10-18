using System;
using System.Text;
using Components.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ComplexPublisher
{
    class ComplexPublisher
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = CommonConsts.HOST_NAME;

            using (var con = factory.CreateConnection())
            {
                using (var channel = con.CreateModel())
                {
                    var replyQueue = channel.QueueDeclare().QueueName;
                    var basicProperties = channel.CreateBasicProperties();
                    basicProperties.Persistent = true;
                    basicProperties.ReplyTo = replyQueue;
                    basicProperties.CorrelationId = Guid.NewGuid().ToString();

                    channel.QueueDeclare(CommonConsts.QUEUE_NAME_WORKER, false, false, false);

                    var body = Encoding.UTF8.GetBytes(CommonConsts.URL);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (sender, e) =>
                    {
                        var response = e.Body;
                        var content = Encoding.UTF8.GetString(response);
                        Console.WriteLine(content);
                    };

                    channel.BasicConsume(replyQueue, true, consumer);

                    while (Console.ReadKey().Key == ConsoleKey.Enter)
                    {
                        channel.BasicPublish(exchange: "",
                            routingKey: CommonConsts.QUEUE_NAME_WORKER,
                            basicProperties: basicProperties,
                            body: body);
                        Console.WriteLine($"Work published ({basicProperties.CorrelationId})!");
                    }
                }
            }
        }
    }
}

using System;
using System.Text;
using Components.RabbitMQ;
using RabbitMQ.Client;

namespace SimplePublisher
{
    class SimplePublisher
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = CommonConsts.HOST_NAME;

            using (var con = factory.CreateConnection())
            {
                using (var channel = con.CreateModel())
                {
                    var basicProperties = channel.CreateBasicProperties();
                    basicProperties.Persistent = true;

                    channel.QueueDeclare(CommonConsts.QUEUE_NAME_HELLO_WORLD, false, false, false);

                    var msg = "Hello World!";
                    var body = Encoding.UTF8.GetBytes(msg);

                    while (Console.ReadKey().Key == ConsoleKey.Enter)
                    {
                        channel.BasicPublish(exchange: "",
                            routingKey: CommonConsts.QUEUE_NAME_HELLO_WORLD,
                            basicProperties: basicProperties,
                            body: body);
                        Console.WriteLine("Message sent!");
                    }
                }
            }
        }
    }
}
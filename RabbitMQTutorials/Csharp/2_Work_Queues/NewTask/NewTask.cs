using System;
using System.Text;
using RabbitMQ.Client;

namespace NewTask
{
    class NewTask
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare("WorkQueues", false, false, false);

                for (int i = 0; i < 100; i++)
                {
                    string message = Guid.NewGuid().ToString();
                    var body = Encoding.UTF8.GetBytes(message);

                    var basicProperties = channel.CreateBasicProperties();
                    basicProperties.Persistent = true;

                    channel.BasicPublish("", "WorkQueues", null, body);
                    Console.WriteLine($" [x] Sent {message} ({i + 1})");
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}

using System;
using System.Text;
using RabbitMQ.Client;

namespace EmitLogs
{
    class EmitLog
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("logs", ExchangeType.Fanout);

                for (int i = 0; i < 100; i++)
                {
                    var message = Guid.NewGuid().ToString();
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish("logs", "", null, body);

                    Console.WriteLine($" [x] Sent {message}");
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}

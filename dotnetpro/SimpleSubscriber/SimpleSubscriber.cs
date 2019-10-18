using System;
using System.Text;
using Components.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SimpleSubscriber
{
    class SimpleSubscriber
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = CommonConsts.HOST_NAME;

            using (var con = factory.CreateConnection())
            {
                using (var channel = con.CreateModel())
                {
                    channel.QueueDeclare(CommonConsts.QUEUE_NAME_HELLO_WORLD, false, false, false);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += Consumer_Received;

                    channel.BasicConsume(CommonConsts.QUEUE_NAME_HELLO_WORLD, true, consumer);

                    Console.ReadLine();
                }
            }
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine($"Received Message: {message}.");
        }
    }
}

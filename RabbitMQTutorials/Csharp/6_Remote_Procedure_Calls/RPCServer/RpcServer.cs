using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RPCServer
{
    class RpcServer
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare("rpc_queue", false, false, false);
                channel.BasicQos(0, 1, false);

                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume("rpc_queue", false, consumer);

                Console.WriteLine(" [x] Awaiting RPC requests");

                consumer.Received += (sender, e) =>
                {
                    string response = null;

                    var body = e.Body;
                    var props = e.BasicProperties;
                    var replyProperties = channel.CreateBasicProperties();
                    replyProperties.CorrelationId = props.CorrelationId;

                    try
                    {
                        var message = Encoding.UTF8.GetString(body);
                        int n = int.Parse(message);
                        Console.WriteLine(" [.] Fibonacci({0})", message);
                        response = Fibonacci(n).ToString();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(" [.] " + ex.Message);
                        response = "";
                    }
                    finally
                    {
                        var responseBytes = Encoding.UTF8.GetBytes(response);
                        channel.BasicPublish("", props.ReplyTo, replyProperties, responseBytes);
                        channel.BasicAck(e.DeliveryTag, false);
                    }
                };

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static int Fibonacci(int n)
        {
            if (n == 0 || n == 1)
            {
                return n;
            }

            return Fibonacci(n - 1) + Fibonacci(n - 2);
        }
    }
}

using System;
using System.Net;
using System.Text;
using Components.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ComplexSubscriber
{
    class ComplexSubscriber
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = CommonConsts.HOST_NAME;

            using (var con = factory.CreateConnection())
            {
                using (var channel = con.CreateModel())
                {
                    channel.QueueDeclare(CommonConsts.QUEUE_NAME_WORKER, false, false, false);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (sender, e) =>
                    {
                        var body = e.Body;
                        var basicProperties = e.BasicProperties;
                        var replyProperties = channel.CreateBasicProperties();
                        replyProperties.CorrelationId = basicProperties.CorrelationId;

                        var url = Encoding.UTF8.GetString(body);
                        Console.WriteLine($"{DateTime.Now}: Processing url {url}. ({basicProperties.CorrelationId})");

                        string content = "";

                        try
                        {
                            content = ProcessUrl(url);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"Failed to read {url}.");
                        }
                        finally
                        {
                            var response = Encoding.UTF8.GetBytes(content);
                            channel.BasicPublish("", basicProperties.ReplyTo, replyProperties, response);
                            channel.BasicAck(e.DeliveryTag, false);
                        }
                    };

                    channel.BasicConsume(CommonConsts.QUEUE_NAME_WORKER, false, consumer);

                    Console.ReadLine();
                }
            }

            Console.ReadLine();

        }

        private static string ProcessUrl(string url)
        {
            string content = "";

            using (WebClient webClient = new WebClient())
            {
                content = webClient.DownloadString(url);
            }

            return content;
        }
    }
}

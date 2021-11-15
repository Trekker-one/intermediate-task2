using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ;
using RabbitMQ.Client;
using RestSharp;
using RestSharp.Serialization.Json;
using producer_service.Models;
using Newtonsoft.Json;

namespace producer_service
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 900000;
            timer.Elapsed += producerEvent;
            timer.Start();

            Console.WriteLine(" Press [enter] to exit.");
            Console.Write(Environment.GetEnvironmentVariable("RABBITMQ_HOST") + ": " +
                Environment.GetEnvironmentVariable("RABBITMQ_PORT"));
            Console.ReadLine();
        }

        static void producerEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("Start Timer!");
            var client = new RestClient("https://api.coindesk.com/");
            var request = new RestRequest("/v1/bpi/currentprice.json", Method.GET);

            IRestResponse response = client.Execute(request);
            var content = response.Content; 
            
            CustomMessage contentJson = new JsonDeserializer().Deserialize<CustomMessage>(response);

            if (contentJson == null)
            {
                Console.WriteLine("Failed to get the json message !!!!!!");
            }

            var factory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
                Port = Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT"))
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Bpi",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(contentJson));

                channel.BasicPublish(exchange: "",
                                     routingKey: "Bpi",
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine(" [x] Sent {0}", JsonConvert.SerializeObject(contentJson));
            }
        }
    }
}

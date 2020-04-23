
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyRabbitMQConsumer
{
    public class MyRabbitMQConsumerHostedService : IHostedService
    {
        private IConnection _conn;
        private IModel _chanel;
        public MyRabbitMQConsumerHostedService() 
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory() 
            {
                UserName = "guest",
                Password = "guest",
                HostName = "localhost"
            };

            _conn = connectionFactory.CreateConnection();

            _chanel = _conn.CreateModel();

            // 事件基本消费者
            var consumer = new EventingBasicConsumer(_chanel);

            consumer.Received +=  (ch, args) =>
            {
                var msg = Encoding.UTF8.GetString(args.Body);
                Console.WriteLine($"收到消息:{msg}");
                
                // 确认消息已被消费
                _chanel.BasicAck(args.DeliveryTag,false);

            };

            // 启动消费者 ，启用手动应答
            _chanel.BasicConsume("myQueue", false, consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _chanel.Dispose();
            _conn.Close();

            return Task.CompletedTask;
        }
    }
}

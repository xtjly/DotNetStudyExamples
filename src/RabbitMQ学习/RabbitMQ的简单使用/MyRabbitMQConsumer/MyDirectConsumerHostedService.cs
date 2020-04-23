using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyRabbitMQConsumer
{
    public class MyDirectConsumerHostedService : IHostedService
    {
        // private static int _tryCount = 3;
        private IConnection _conn;
        private IModel _chanel;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Init();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private Task<bool> Init(bool force = false) 
        {
            if (_conn != null && _chanel != null && !force) 
            {
                return Task.FromResult(true);
            }

            try
            {
                ConnectionFactory connectionFactory = new ConnectionFactory()
                {
                    UserName = "guest",
                    Password = "guest",
                    HostName = "localhost"
                };

                _conn = connectionFactory.CreateConnection();

                string exchangeName = "myDirectExchange";
                string routeKey = "test";

                _chanel = _conn.CreateModel();
                // 事件基本消费者
                var consumer = new EventingBasicConsumer(_chanel);
                
                consumer.Received += (ch, args) =>
                {
                    var msg = Encoding.UTF8.GetString(args.Body);
                    Console.WriteLine($"收到消息:{msg}");

                    // 确认消息已被消费
                    _chanel.BasicAck(args.DeliveryTag, false);

                };

                // 启动消费者 ，启用手动应答
                _chanel.BasicConsume("myQueue", false, consumer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("初始化消费者异常:"+ex.Message);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}

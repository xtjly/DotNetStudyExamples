using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyRabbitMQ.Services
{
    public class RabbitMQConnectionService
    {
        private IConnection Connection => new ConnectionFactory() 
        {
            UserName = "guest",
            Password = "guest",
            HostName = "localhost"
        }.CreateConnection();

        /// <summary>
        /// 获取连接
        /// </summary>
        /// <returns></returns>
        public IConnection GetConnection() 
        {
            return Connection;
        }

    }
}

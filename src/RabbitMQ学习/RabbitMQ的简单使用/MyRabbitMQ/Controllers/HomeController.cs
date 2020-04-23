using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyRabbitMQ.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace MyRabbitMQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        RabbitMQConnectionService _service;

        public HomeController(RabbitMQConnectionService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(new { Message = "Get Info Successed" });
        }

        /// <summary>
        /// 投递普通队列消息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("msg")]
        public IActionResult Post([FromBody]MQMsg data)
        {
            // 创建连接工厂
            //ConnectionFactory connectionFactory = new ConnectionFactory() 
            //{
            //    UserName = "guest",
            //    Password = "guest",
            //    HostName = "localhost"
            //};

            // 创建连接
            using (var conn = _service.GetConnection())
            {
                // 创建通道
                using (var channel = conn.CreateModel())
                {
                    // 声明一个队列
                    channel.QueueDeclare("myQueue", false, false, false);

                    // 定义发送内容
                    string input = JsonConvert.SerializeObject(data);

                    var sendBytes = Encoding.UTF8.GetBytes(input);

                    channel.BasicPublish("", "myQueue", null, sendBytes);
                }
                // conn.Close();
            }

            return new JsonResult(new MQResult{ Message = "Post Info Successed" });
        }

        /// <summary>
        /// 投递Direct消息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("directmsg")]
        public IActionResult PostDirectMsg([FromBody]MQMsg data)
        {
            // 创建连接工厂
            //ConnectionFactory connectionFactory = new ConnectionFactory()
            //{
            //    UserName = "guest",
            //    Password = "guest",
            //    HostName = "localhost"
            //};

            // 创建连接
            using (var conn = _service.GetConnection()) // connectionFactory.CreateConnection();
            {
                // 创建通道
                using var channel = conn.CreateModel();
                string exchangeName = "myDirectExchange";
                string routeKey = "xxxx";

                ///
                channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, false, false);
                //声明一个队列
                var queueDeclareOk =  channel.QueueDeclare("myQueue", false, false, false);
                
                channel.QueueBind("myQueue", exchangeName, routeKey, null);
                
                // 定义发送内容
                string input = JsonConvert.SerializeObject(data);

                var sendBytes = Encoding.UTF8.GetBytes(input);

                channel.BasicPublish(exchangeName, routeKey, null, sendBytes);
            }

            return new JsonResult(new MQResult { Message = "Post Info Successed" });
        }
        
        /// <summary>
        /// 投递Fanout消息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("fanoutmsg")]
        public IActionResult PostFanoutMsg([FromBody]MQMsg data)
        {
            return null;
        }
    }

    public class MQResult 
    {
        public string Message { get; set; }
    }

    public class MQMsg 
    {
        public string Name { get; set; }
        public string Desc { get; set; }
    }
}
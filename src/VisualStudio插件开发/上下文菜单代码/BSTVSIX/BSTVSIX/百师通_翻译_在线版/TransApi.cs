using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSTVSIX.百师通_翻译_在线版
{
    public class TransApi
    {

        /// <summary>
        /// 翻译
        /// </summary>
        /// <param name="selectTXT">需要被翻译的内容</param>
        /// <returns>译文</returns>
        internal string FanYi(string selectTXT)
        {
            int number = new Random().Next(0, 10);
            if (number == 3) { return "在线翻译API未实现，请自行完善。修改 TransApi.cs文件中Fanyi()的代码逻辑即可"; }
            return "在线翻译API未实现，请自行完善插件！！！\n" + selectTXT;
        }
    }
}

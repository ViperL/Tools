using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DataConsole
{
    /// <summary>
    /// UDP网关类
    /// </summary>
    class SocketMethod
    {
        public List<DeviceModel> List = new List<DeviceModel>();
        public List<string> DataStream = new List<string>();//数据流


        /// <summary>
        /// 初始化监听
        /// </summary>
        /// <returns>是否成功</returns>
        public bool InitNet()
        {
            try
            {
                Thread t2 = new Thread(new ThreadStart(RecvFromController));
                t2.IsBackground = true;
                t2.Start();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, 0));

        /// <summary>
        /// 发送字符串
        /// </summary>
        /// <param name="Data">内容</param>
        /// <param name="Port">端口号</param>
        public void SendData(string Data, int Port)
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), Port);
            byte[] buf = Encoding.Default.GetBytes("    "+Data);
            client.Send(buf, buf.Length, endpoint);//发送服务
        }


        public void RecvFromController()//控制器接收处理
        {
            UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, 8003));//绑定端口
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                byte[] buf = client.Receive(ref endpoint);
                string msg = Encoding.UTF8.GetString(buf);
                string tSt;
                tSt = msg.Remove(0, 4);//移除帧头(可删除)
                
            }
        }
        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <returns>IP号</returns>
        public String[] GetLocalIpAddress()
        {
            string hostName = Dns.GetHostName();                    //获取主机名称  
            IPAddress[] addresses = Dns.GetHostAddresses(hostName); //解析主机IP地址  
            string[] IP = new string[addresses.Length];             //转换为字符串形式  

            for (int i = 0; i < addresses.Length; i++) IP[i] = addresses[i].ToString();

            return IP;
        }
    }
}

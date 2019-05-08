using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using System.Threading;

namespace Router
{
    class SerialPort
    {
        DatagramSocket socket = null;//套接字实体对象

        /// <summary>
        /// 创建新通道
        /// </summary>
        /// <param name="Port">端口号</param>
        private async void CreateLine(int Port)
        {
            try
            {
                socket = new DatagramSocket();
                socket.Control.MulticastOnly = true;
                socket.MessageReceived += Socket_MessageReceived1;
                await socket.BindServiceNameAsync(Port.ToString());
                ListSocket.Add(socket);
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// 删除监听通道
        /// </summary>
        /// <param name="Port">端口号</param>
        private void DisposeLine(int Port)
        {
            for (int i = 0; i < ListSocket.Count; i++)
            {
                if (ListSocket[i].Information.LocalPort == Port.ToString())
                {
                    ListSocket[i].Dispose();//销毁对象
                    ListSocket.Remove(ListSocket[i]);
                }
            }
        }//移除监听

        /// <summary>
        /// 监听服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Socket_MessageReceived1(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            string remoteaddr = args.RemoteAddress.DisplayName;//远端IP地址
            if (remoteaddr == LocIp)
                return;//本机IP不做处理
            DataReader reader = args.GetDataReader();
            reader.UnicodeEncoding = UnicodeEncoding.Utf8;// 读长度
            uint len = reader.ReadUInt32();// 读内容
            string msg = reader.ReadString(reader.UnconsumedBufferLength);
            if (oldStr == msg)
                return;//重复过滤器
            else
                oldStr = msg;
            //你的处理函数
        }//数据通道

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)//释放资源
        {
            if (socket != null)
            {
                socket.MessageReceived -= Socket_MessageReceived;
                socket.Dispose();
                socket = null;
            }
        }//释放资源

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="PortNum">端口号</param>
        /// <param name="Cmd">数据</param>
        private async void SendCmd(int PortNum, string Cmd)
        {
            string content = Cmd;
            if (string.IsNullOrEmpty(content)) return;
            using (DatagramSocket socket = new DatagramSocket())
            {
                HostName broardaddr = new HostName(IPAddress.Broadcast.ToString());
                IOutputStream outstream = await socket.GetOutputStreamAsync(broardaddr, PortNum.ToString());
                DataWriter writer = new DataWriter(outstream);
                writer.UnicodeEncoding = UnicodeEncoding.Utf8;
                uint len = writer.MeasureString(content);
                writer.WriteUInt32(len);
                writer.WriteString(content);
                await writer.StoreAsync();
                writer.Dispose();
            }
        }//发送指令
    }
}


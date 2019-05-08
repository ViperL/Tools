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
        DatagramSocket socket = null;//�׽���ʵ�����

        /// <summary>
        /// ������ͨ��
        /// </summary>
        /// <param name="Port">�˿ں�</param>
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
        /// ɾ������ͨ��
        /// </summary>
        /// <param name="Port">�˿ں�</param>
        private void DisposeLine(int Port)
        {
            for (int i = 0; i < ListSocket.Count; i++)
            {
                if (ListSocket[i].Information.LocalPort == Port.ToString())
                {
                    ListSocket[i].Dispose();//���ٶ���
                    ListSocket.Remove(ListSocket[i]);
                }
            }
        }//�Ƴ�����

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Socket_MessageReceived1(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            string remoteaddr = args.RemoteAddress.DisplayName;//Զ��IP��ַ
            if (remoteaddr == LocIp)
                return;//����IP��������
            DataReader reader = args.GetDataReader();
            reader.UnicodeEncoding = UnicodeEncoding.Utf8;// ������
            uint len = reader.ReadUInt32();// ������
            string msg = reader.ReadString(reader.UnconsumedBufferLength);
            if (oldStr == msg)
                return;//�ظ�������
            else
                oldStr = msg;
            //��Ĵ�����
        }//����ͨ��

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)//�ͷ���Դ
        {
            if (socket != null)
            {
                socket.MessageReceived -= Socket_MessageReceived;
                socket.Dispose();
                socket = null;
            }
        }//�ͷ���Դ

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="PortNum">�˿ں�</param>
        /// <param name="Cmd">����</param>
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
        }//����ָ��
    }
}


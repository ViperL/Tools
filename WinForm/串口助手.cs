using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace TempMoniter
{
    class SerialPortAss
    {
        SerialPort ObjPort = new SerialPort();

        //只读属性设置
        public SerialPort SerialPortObject
        {
            get { return ObjPort; }
        }

        /// <summary>
        /// 获取可用串口信息
        /// </summary>
        /// <returns>串口名称序列</returns>
        public List<string> PortInit()
        {
            List<string> ProtName = new List<string>();
            string[] portList = System.IO.Ports.SerialPort.GetPortNames();
            if (portList.Length <= 0)
            {
                ProtName.Add("无串口");
            }
            else
            {
                for (int i = 0; i < portList.Length; ++i)
                {
                    string name = portList[i];
                    ProtName.Add(name);
                }
            }
            return ProtName;
        }

        /// <summary>
        /// 切换串口开启/关闭
        /// </summary>
        /// <param name="IsLinked">是否开启</param>
        /// <param name="PortName">串口号</param>
        /// <param name="Rate">波特率</param>
        /// <param name="PortData">数据位</param>
        /// <param name="PortStop">停止位</param>
        /// <param name="PortParity">校验位</param>
        public void Switch(bool IsLinked, string PortName, string Rate, string PortData, string PortStop, string PortParity)
        {
            if (IsLinked == false)//串口处于关闭状态
            {
                try
                {
                    ObjPort.PortName = PortName;
                    ObjPort.BaudRate = Convert.ToInt32(Rate);
                    ObjPort.DataBits = Convert.ToInt32(PortData);//数据位处理函数
                    switch (PortStop)//停止位
                    {
                        case "0": ObjPort.StopBits = StopBits.None; break;
                        case "1": ObjPort.StopBits = StopBits.One; break;
                        case "1.5": ObjPort.StopBits = StopBits.OnePointFive; break;
                        case "2": ObjPort.StopBits = StopBits.Two; break;
                    }
                    switch (PortParity)//奇偶校验位
                    {
                        case "0": ObjPort.Parity = System.IO.Ports.Parity.None; break;
                        case "1": ObjPort.Parity = System.IO.Ports.Parity.Odd; break;
                        case "2": ObjPort.Parity = System.IO.Ports.Parity.Even; break;
                        case "3": ObjPort.Parity = System.IO.Ports.Parity.Mark; break;
                        case "4": ObjPort.Parity = System.IO.Ports.Parity.Space; break;
                    }
                    ObjPort.Open();
                }
                catch (Exception)
                {
                    throw new Exception("串口设置错误，请检查后重试");
                }
            }
            else
            {
                ObjPort.Close();
            }
        }

        /// <summary>
        /// 数据读取服务，主程序中使用托管调用
        /// </summary>
        /// <returns></returns>
        public string ReceiveData()
        {
            return this.ObjPort.ReadExisting();
        }
        //托管方法在主程序初始化里打“ObjSerial.SerialPortObject.DataReceived+=”再连续按两下Tab

        /// <summary>
        /// 串口发送数据
        /// </summary>
        /// <param name="Str">数据</param>
        public void SendData(string Str)
        {
            ObjPort.Write(Str);//发送数据
        }
    }
}

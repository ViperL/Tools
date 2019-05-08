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
        private SerialDevice serialPort = null;
        DataWriter dataWriteObject = null;
        DataReader dataReaderObject = null;
        private ObservableCollection<DeviceInformation> listOfDevices;
        private CancellationTokenSource ReadCancellationTokenSource;
        private List<DatagramSocket> ListSocket = new List<DatagramSocket>();//���ڴ洢�󶨵Ĵ���

        private async void ListAvailablePorts()//�����ô���
        {
            try
            {
                string aqs = SerialDevice.GetDeviceSelector();
                var dis = await DeviceInformation.FindAllAsync(aqs);
                listOfDevices.Clear();
                for (int i = 0; i < dis.Count; i++)
                {
                    listOfDevices.Add(dis[i]);
                }
                DeviceListSource.Source = listOfDevices;
                ConnectDevices.SelectedIndex = -1;
            }
            catch (Exception)
            { }
        }

        private async void Listen()
        {
            dataReaderObject = new DataReader(serialPort.InputStream);
            while (true)
            {
                try
                {
                    await ReadAsync(ReadCancellationTokenSource.Token);
                }
                catch (Exception)
                {
                    if (dataReaderObject != null)
                    {
                        CloseDevice();
                        dataReaderObject.DetachStream();
                        dataReaderObject = null;
                        break;
                    }
                }
            }
        }//��������

        private async Task ReadAsync(CancellationToken cancellationToken)
        {
            Task<UInt32> loadAsyncTask;
            uint ReadBufferLength = 30;
            cancellationToken.ThrowIfCancellationRequested();
            dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;
            using (var childCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask(childCancellationTokenSource.Token);
                UInt32 bytesRead = await loadAsyncTask;
                if (bytesRead > 0)
                {
                    try
                    {
                        string msg = dataReaderObject.ReadString(bytesRead);//MsgΪ��ȡ��������
                    }
                    catch (Exception){ }
                }
            }
        }//��ȡ����       

        private async void WriteCmd(string Cmd)//���ͷ���
        {
            try
            {
                if (serialPort != null)
                {
                    dataWriteObject = new DataWriter(serialPort.OutputStream);
                    await WriteAsync(Cmd);
                }
            }
            catch (Exception)
            { }
            finally
            {
                if (dataWriteObject != null)
                {
                    dataWriteObject.DetachStream();
                    dataWriteObject = null;
                }
            }
        }

        private async void WriteCmdByte(byte[] Cmd)//���ͷ���
        {
            try
            {
                if (serialPort != null)
                {
                    dataWriteObject = new DataWriter(serialPort.OutputStream);
                    await WriteByte(Cmd);
                }
            }
            catch (Exception)
            { }
            finally
            {
                if (dataWriteObject != null)
                {
                    dataWriteObject.DetachStream();
                    dataWriteObject = null;
                }
            }
        }

        private async Task WriteAsync(string Data)//�����ӷ���
        {
            Task<UInt32> storeAsyncTask;

            if (Data.Length != 0)
            {
                dataWriteObject.WriteString(Data);
                storeAsyncTask = dataWriteObject.StoreAsync().AsTask();
                UInt32 bytesWritten = await storeAsyncTask;
            }
        }

        private async Task WriteByte(byte[] Data)//�����ӷ���
        {
            Task<UInt32> storeAsyncTask;

            if (Data.Length != 0)
            {
                dataWriteObject.WriteBytes(Data);
                storeAsyncTask = dataWriteObject.StoreAsync().AsTask();
                UInt32 bytesWritten = await storeAsyncTask;
            }
        }

        private void CancelReadTask()
        {
            if (ReadCancellationTokenSource != null)
            {
                if (!ReadCancellationTokenSource.IsCancellationRequested)
                {
                    ReadCancellationTokenSource.Cancel();
                }
            }
        }//�ͷŴ���

        private void CloseDevice()
        {
            if (serialPort != null)
            {
                serialPort.Dispose();
            }
            serialPort = null;
            listOfDevices.Clear();
        }//�ر��豸

        private async void BtnAddZigBee_Click(object sender, RoutedEventArgs e)
        {
            #region ���ô���
            var selection = ConnectDevices.SelectedItems;
            if (selection.Count <= 0)
            {
                return;
            }

            DeviceInformation entry = (DeviceInformation)selection[0];

            try
            {
                serialPort = await SerialDevice.FromIdAsync(entry.Id);
                if (serialPort == null) return;
                serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                serialPort.BaudRate = 9600;//������
                serialPort.Parity = SerialParity.None;//
                serialPort.StopBits = SerialStopBitCount.One;//
                serialPort.DataBits = 8;//
                serialPort.Handshake = SerialHandshake.None;//
                // Create cancellation token object to close I/O operations when closing the device
                ReadCancellationTokenSource = new CancellationTokenSource();
                Listen();
            }
            catch (Exception)
            { }
            #endregion
        }
        
    }
}


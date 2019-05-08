using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace ProjectLine
{
    /// <summary>
    /// 陀螺仪6050服务函数
    /// </summary>
    class MPU6050
    {

        AccData accel = new AccData();

        private const byte ACCEL_I2C_ADDR = 0x68; //写入时的地址字节数据，+1为读取\
        private const byte MPU6050_RA_PWR_MGMT_1 = 0x6B;//解除休眠
        private const byte MPU6050_RA_SMPLRT_DIV = 0x19;//陀螺仪采样率
        private const byte MPU6050_RA_CONFIG = 0x1A;//
        private const byte MPU6050_RA_ACCEL_CONFIG = 0x1C;//陀螺仪工作范围
        private const byte MPU6050_RA_GYRO_CONFIG = 0x1B;//陀螺仪自检及测量范围
        private const byte MPU6050_ACC_OUT = 0x3B;//加速度计的数据寄存器地址

        private I2cDevice I2CAccel;

        public async void InitI2CAccel()//初始化陀螺仪
        {
            string aqs = I2cDevice.GetDeviceSelector(); //获取I2C控制器
            var dis = await DeviceInformation.FindAllAsync(aqs);    //找到I2C控制器

            var setting = new I2cConnectionSettings(ACCEL_I2C_ADDR);    //设备信号（SlaveAddress）
            setting.BusSpeed = I2cBusSpeed.FastMode;    //传输速率
            I2CAccel = await I2cDevice.FromIdAsync(dis[0].Id, setting);//创建I2C设备


            //第一个字节存放目标寄存器，第二个字节存放内容
            byte[] WriteBuf_Power = new byte[] { MPU6050_RA_PWR_MGMT_1, 0x00 };//解除休眠
            byte[] WriteBuf_SMPLRT_DIV = new byte[] { MPU6050_RA_SMPLRT_DIV, 0x07 };//陀螺仪采集率
            byte[] WriteBuf_RA_CONFIG = new byte[] { MPU6050_RA_CONFIG, 0x06 };
            byte[] WriteBuf_RA_ACCEL_CONFIG = new byte[] { MPU6050_RA_ACCEL_CONFIG, 0x01 };//陀螺仪工作范围为16G
            byte[] WriteBuf_RA_GYRO_CONFIG = new byte[] { MPU6050_RA_GYRO_CONFIG, 0x18 };//陀螺仪自检及测量范围

            try
            {
                I2CAccel.Write(WriteBuf_Power);//写入数据
                I2CAccel.Write(WriteBuf_SMPLRT_DIV);//写入数据
                I2CAccel.Write(WriteBuf_RA_CONFIG);//写入数据
                I2CAccel.Write(WriteBuf_RA_ACCEL_CONFIG);//写入数据
                I2CAccel.Write(WriteBuf_RA_GYRO_CONFIG);//写入数据
            }
            catch 
            {
                throw new Exception("陀螺仪初始化失败");
            }
        }

        public AccData ReadI2CAccel()//读取陀螺仪数据
        {
            byte[] RegAddrBuf = new byte[] { MPU6050_ACC_OUT };
            byte[] ReadBuf = new byte[6];
            I2CAccel.WriteRead(RegAddrBuf, ReadBuf);
            short AcceleationRawX = BitConverter.ToInt16(ReadBuf, 0);
            short AcceleationRawY = BitConverter.ToInt16(ReadBuf, 2);
            short AcceleationRawZ = BitConverter.ToInt16(ReadBuf, 4);

            accel.X = (double)4 * AcceleationRawX / 32768;
            accel.Y = (double)4 * AcceleationRawY / 32768;
            accel.Z = (double)4 * AcceleationRawZ / 32768;
            return accel;
        }

        public void I2CDispose()//程序结束的时候释放I2C设备
        {
            I2CAccel.Dispose();
        }
    }
}

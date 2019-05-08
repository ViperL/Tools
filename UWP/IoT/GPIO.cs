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

using Windows.Devices.Gpio;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace GPIO_Demo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int LED_PIN = 5;//指定GPIO5
        private GpioPin pin;
        private GpioPinValue pinValue;
        private DispatcherTimer timer;
        private bool TimerFlag = false;//定时器状态

        public MainPage()
        {
            this.InitializeComponent();
            timer = new DispatcherTimer(); //初始化计时器函数
            timer.Interval = TimeSpan.FromMilliseconds(500);//计时0.5s
            timer.Tick += Timer_Tick;
            InitGPIO();
        }

        private void InitGPIO()//设置GPIO口
        {
            var gpio = GpioController.GetDefault();//获取硬件GPIO
            if (gpio == null)
            {
                pin = null;//显示没有硬件
                LED_Staut.Text = "没有硬件";
                return;
            }

            pin = gpio.OpenPin(LED_PIN);
            pinValue = GpioPinValue.High;//高电平
            pin.Write(pinValue);//输出数据
            pin.SetDriveMode(GpioPinDriveMode.Output);//数位输出
            //设置完成
        }

        private void Timer_Tick(object sender, object e)
        {
            if (pinValue == GpioPinValue.High)
            {
                pinValue = GpioPinValue.Low;
                pin.Write(pinValue);
                //响应
                LED_Staut.Text = "设置完成";
            }
            else
            {
                pinValue = GpioPinValue.High;
                pin.Write(pinValue);
            }
        }

        private void LED_Switch_Click(object sender, RoutedEventArgs e)
        {
            if (pin != null)
            {
                if (TimerFlag == false)
                {
                    timer.Start();
                    TimerFlag = true;
                }
                else
                {
                    timer.Stop();
                    TimerFlag = false;
                }
            }
        }
    }
}

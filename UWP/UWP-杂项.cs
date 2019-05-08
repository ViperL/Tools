        public async void Invoke(Action action, Windows.UI.Core.CoreDispatcherPriority Priority = Windows.UI.Core.CoreDispatcherPriority.Normal)//跨线程
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); });
        }

        private string GetDeviceInfo()//获取设备信息
        {
            EasClientDeviceInformation deviceInfo = new EasClientDeviceInformation();
            string x=deviceInfo.SystemFirmwareVersion;
            return deviceInfo.SystemManufacturer+"--"+deviceInfo.SystemProductName;
        }
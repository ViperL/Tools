        public async void Invoke(Action action, Windows.UI.Core.CoreDispatcherPriority Priority = Windows.UI.Core.CoreDispatcherPriority.Normal)//���߳�
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); });
        }

        private string GetDeviceInfo()//��ȡ�豸��Ϣ
        {
            EasClientDeviceInformation deviceInfo = new EasClientDeviceInformation();
            string x=deviceInfo.SystemFirmwareVersion;
            return deviceInfo.SystemManufacturer+"--"+deviceInfo.SystemProductName;
        }
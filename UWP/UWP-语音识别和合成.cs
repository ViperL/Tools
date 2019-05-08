        private async void PlayVoice(string tTS, int voice = 0)
        {
            //await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () => {
            //=====================
            // The media object for controlling and playing audio.           
            // The object for controlling the speech synthesis engine (voice).          
            SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
            //speechSynthesizer.Voice = SpeechSynthesizer.DefaultVoice;
            speechSynthesizer.Voice = SpeechSynthesizer.AllVoices[voice];//0,4,8,12

            // Generate the audio stream from plain text.
            SpeechSynthesisStream spokenStream = await speechSynthesizer.SynthesizeTextToStreamAsync(tTS);

            await mediaElement.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                mediaElement.SetSource(spokenStream, spokenStream.ContentType);
                mediaElement.Play();
            }));

        }//合成声音

        /// <summary>
        /// 前台识别
        /// </summary>
        /// <returns>识别内容</returns>
        public async Task<string> BegiRecongnize()
        {
            string Result = "";
            try
            {
                using (SpeechRecognizer recognizer = new SpeechRecognizer())
                {
                    SpeechRecognitionCompilationResult compilationResult = await recognizer.CompileConstraintsAsync();
                    if (compilationResult.Status == SpeechRecognitionResultStatus.Success)
                    {

                        recognizer.UIOptions.IsReadBackEnabled = false;
                        recognizer.UIOptions.ShowConfirmation = false;
                        recognizer.UIOptions.AudiblePrompt = "我在听,请说...";
                        SpeechRecognitionResult recognitionResult = await recognizer.RecognizeWithUIAsync();
                        if (recognitionResult.Status == SpeechRecognitionResultStatus.Success)
                        {
                            Result = recognitionResult.Text;
                        }
                    }

                }
            }
            catch (Exception)
            {
                Result = "erro";
            }
            return Result;
        }

        /// <summary>
        /// 后台识别
        /// </summary>
        /// <returns>识别内容</returns>
        public async Task<string> BackGroundRec()
        {
            string Result = "";
            try
            {
                using (SpeechRecognizer recognizer = new SpeechRecognizer())
                {
                    SpeechRecognitionCompilationResult compilationResult = await recognizer.CompileConstraintsAsync();
                    if (compilationResult.Status == SpeechRecognitionResultStatus.Success)
                    {
                        SpeechRecognitionResult recognitionResult = await recognizer.RecognizeAsync();
                        if (recognitionResult.Status == SpeechRecognitionResultStatus.Success)
                        {
                            Result = recognitionResult.Text;
                        }
                    }
                }
            }
            catch (Exception){}
            return Result;
        }//后台常驻声音

        /// <summary>
        /// 后台监听线程原型
        /// </summary>
        private async void SoundListen()
        {
            try
            {
                while (true)
                {
                    string Str = await BackGroundRec();
                    this.Invoke(() =>
                    {
                        lblContana.Text = Str;
                    });
                    switch (Str)
                    {
                        case "你好小娜": PlayVoice("What Can I do for you"); await SoundLink(); break;
                        default:break;
                    }
                }
            }
            catch (Exception) { }
        }//监听服务

        /// <summary>
        /// 识别响应
        /// </summary>
        /// <returns>是否成功</returns>
        private async Task<bool> SoundLink()
        {
            string Str = await BegiRecongnize();
            this.Invoke(() =>
            {
                lblContana.Text = Str;
            });
            switch (Str)
            {
                case "切换第一个灯":
                    WriteCmdByte(new byte[] { 0x01 });
                    PlayVoice("Yes Sir,light one Switch"); break;
                case "切换第二个灯":
                    WriteCmdByte(new byte[] { 0x02 });
                    PlayVoice("Yes Sir,light Two Switch"); break;
                default: PlayVoice("hail hydra"); break;
            }
            return true;
        }//识别服务
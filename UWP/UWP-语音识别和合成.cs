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

        }//�ϳ�����

        /// <summary>
        /// ǰ̨ʶ��
        /// </summary>
        /// <returns>ʶ������</returns>
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
                        recognizer.UIOptions.AudiblePrompt = "������,��˵...";
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
        /// ��̨ʶ��
        /// </summary>
        /// <returns>ʶ������</returns>
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
        }//��̨��פ����

        /// <summary>
        /// ��̨�����߳�ԭ��
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
                        case "���С��": PlayVoice("What Can I do for you"); await SoundLink(); break;
                        default:break;
                    }
                }
            }
            catch (Exception) { }
        }//��������

        /// <summary>
        /// ʶ����Ӧ
        /// </summary>
        /// <returns>�Ƿ�ɹ�</returns>
        private async Task<bool> SoundLink()
        {
            string Str = await BegiRecongnize();
            this.Invoke(() =>
            {
                lblContana.Text = Str;
            });
            switch (Str)
            {
                case "�л���һ����":
                    WriteCmdByte(new byte[] { 0x01 });
                    PlayVoice("Yes Sir,light one Switch"); break;
                case "�л��ڶ�����":
                    WriteCmdByte(new byte[] { 0x02 });
                    PlayVoice("Yes Sir,light Two Switch"); break;
                default: PlayVoice("hail hydra"); break;
            }
            return true;
        }//ʶ�����
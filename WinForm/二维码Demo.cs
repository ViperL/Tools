using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using ZXing.QrCode;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;

using AForge;
using AForge.Controls;
using AForge.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;

namespace QRCode_Camer
{
    public partial class FrmMain : Form
    {
        FilterInfoCollection videoDevices;
        VideoCaptureDevice videoSource;
        public int selectedDeviceIndex = 0;//新建摄像头

        public FrmMain()
        {
            InitializeComponent();
            cmb_Type.SelectedIndex = 0;
            Load_Camera();
            videoPlayer.Visible = false;
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseCamera();
        }

        //加载摄像头
        private void Load_Camera()
        {
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            try
                {
                    if (videoDevices.Count == 0)
                        throw new ApplicationException();

                    foreach (FilterInfo device in videoDevices)
                    {
                        cmb_CameraId.Items.Add(device.Name);
                    }

                    cmb_CameraId.SelectedIndex = 0;

                }
                catch (ApplicationException)
                {
                    cmb_CameraId.Items.Add("No local capture devices");
                    videoDevices = null;
                }
    }

        //开启摄像头
        private void OpenCamera()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            selectedDeviceIndex = cmb_CameraId.SelectedIndex;
            videoSource = new VideoCaptureDevice(videoDevices[selectedDeviceIndex].MonikerString);//连接摄像头。
            videoSource.VideoResolution = videoSource.VideoCapabilities[selectedDeviceIndex];
            videoPlayer.VideoSource = videoSource;
            // set NewFrame event handler
            videoPlayer.Start();
        }

        private void CloseCamera()
        {
            videoPlayer.SignalToStop();
            videoPlayer.WaitForStop();
        }

        private void btn_Camera_Switch_Click(object sender, EventArgs e)
        {
            if (btn_Camera_Switch.Text == "打开摄像头")
            {
                videoPlayer.Visible = true;
                OpenCamera();
                btn_Camera_Switch.Text = "关闭摄像头";
                timer_Shoot.Start();
            }
            else
            {
                videoPlayer.Visible = false;
                CloseCamera();
                btn_Camera_Switch.Text = "打开摄像头";
            }
        }

        private void shoot()
        {
            try
            {
                if (videoSource == null)
                    return;
                Bitmap bitmap = videoPlayer.GetCurrentVideoFrame();
                //string fileName = "54250.jpg";//DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ff") + ".jpg";
                //bitmap.Save(@fileName, System.Drawing.Imaging.ImageFormat.Jpeg);

                QRCodeAnayle(bitmap);
                //Pic_RQCode_A.Image = System.Drawing.Image.FromFile(@fileName);

                bitmap.Dispose();
                videoPlayer.Visible = false;

                CloseCamera();
                btn_Camera_Switch.Text = "打开摄像头";
                timer_Shoot.Stop();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }

        }
        private void QRCodeAnayle(Bitmap bitmap)
        {
            BarcodeReader reader = new BarcodeReader();
            if (Pic_RQCode_A == null)
            {
                return;
            }
            Result result = reader.Decode(bitmap); //通过reader解码
            txt_Result.Text = result.ToString();
            System.Diagnostics.Process.Start(result.ToString());
        }

        private  void QRCodeAnayle()
        {
            BarcodeReader reader = new BarcodeReader();
            if ( Pic_RQCode_A== null)
            {
                return;
            }
            Result result = reader.Decode((Bitmap)Pic_RQCode_A.Image); //通过reader解码
            txt_Result.Text = result.ToString();
        }

        private void btn_Catch_Click(object sender, EventArgs e)
        {
            shoot();
        }

        private void btn_AddPic_Click(object sender, EventArgs e)
        {
            Open_Pic_Dialog.ShowDialog();
            Pic_RQCode_A.Image = System.Drawing.Image.FromFile(Open_Pic_Dialog.FileName);
            timer_Analy.Start();
        }

        private void timer_Analy_Tick(object sender, EventArgs e)
        {
            QRCodeAnayle();
            timer_Analy.Stop();
        }

        private void btn_Creat_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmb_Type.SelectedIndex.Equals(0))
                {
                    BarcodeWriter writer = null;
                    EncodingOptions options = null;
                    options = new QrCodeEncodingOptions
                    {
                        DisableECI = true,
                        CharacterSet = "UTF-8",
                        Width = Convert.ToInt32(txt_With.Text.Trim().ToString()),
                        Height = Convert.ToInt32(txt_With.Text.Trim().ToString())
                    };
                    writer = new BarcodeWriter();
                    writer.Format = BarcodeFormat.QR_CODE;
                    Creat(writer, options);
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show("编码方式错误"+ex.Message);
            }
            if (cmb_Type.SelectedIndex.Equals(1))
            {
                try
                {
                    BarcodeWriter writer = null;
                    EncodingOptions options = null;
                    options = new EncodingOptions
                    {
                        //DisableECI = true,  
                        //CharacterSet = "UTF-8",  
                        Width = Convert.ToInt32(txt_With.Text.Trim().ToString()),
                        Height = Convert.ToInt32(txt_With.Text.Trim().ToString())
                    };
                    writer = new BarcodeWriter();
                    writer.Format = BarcodeFormat.ITF;
                    Creat(writer, options);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("编码方式错误"+ex.Message);
                }
            }
        }

        private void Creat(BarcodeWriter writer, EncodingOptions options)
        {
            writer.Options = options;
            if (txt_Address.Text == string.Empty)
            {
                MessageBox.Show("输入内容不能为空！");
                return;
            }
            Bitmap bitmap = writer.Write(txt_Address.Text);
            Pic_QRCode_C.Image = bitmap;
        }

        private void SaveQRcode(System.Windows.Forms.PictureBox Pic)
        {
            bool isSave = true;
            SaveFileDialog obj = new SaveFileDialog();
            string NowTime = DateTime.Now.ToString();
            obj.Title = "保存图片";
            obj.Filter = @"jpeg|*.jpg|bmp|*.bmp";

            if (obj.ShowDialog() == DialogResult.OK)
            {
                #region 保存实现
                string fileName = obj.FileName.ToString();
                if (fileName != "" && fileName != null)
                {
                    string fileExtName = fileName.Substring(fileName.LastIndexOf(".") + 1).ToString();

                    System.Drawing.Imaging.ImageFormat imgformat = null;

                    if (fileExtName != "")
                    {
                        switch (fileExtName)
                        {
                            case "jpg":
                                imgformat = System.Drawing.Imaging.ImageFormat.Jpeg;
                                break;
                            case "bmp":
                                imgformat = System.Drawing.Imaging.ImageFormat.Bmp;
                                break;
                            case "gif":
                                imgformat = System.Drawing.Imaging.ImageFormat.Gif;
                                break;
                            default:
                                MessageBox.Show("只能存取为: jpg,bmp 格式");
                                isSave = false;
                                break;
                        }
                    }
                    //默认保存为JPG格式   
                    if (imgformat == null)
                    {
                        imgformat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    }

                    if (isSave)
                    {
                        try
                        {
                            Pic.Image.Save(fileName, imgformat);
                            //MessageBox.Show("图片已经成功保存!");   
                        }
                        catch
                        {
                            MessageBox.Show("保存失败,你还没有截取过图片或已经清空图片!");
                        }
                    }
                }
                #endregion
            }
            //else if(obj.ShowDialog()==DialogResult.Cancel)
            //    return;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            SaveQRcode(Pic_QRCode_C);
        }

        private void cmb_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
           if(cmb_Type.SelectedIndex.Equals(1))
                txt_With.Text = 70.ToString();
           else
                txt_With.Text = 256.ToString();
        }

        private void timer_Shoot_Tick(object sender, EventArgs e)
        {
            shoot();
        }
    }
}

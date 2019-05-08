using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;

namespace ProjectLine
{
    /// <summary>
    /// 文件处理类
    /// </summary>
    class DataFile
    {
        StorageFolder folder = ApplicationData.Current.LocalFolder;//获取应用文件目录
        private string file_name = DateTime.Now.ToString("yyyy_MM_dd") + "记录.txt";
        private string folder_name = "Data";

        private async void CreateTxt()//创建文件
        {
            try
            {
                await folder.CreateFileAsync(file_name, CreationCollisionOption.ReplaceExisting);//直接创建文件
            }
            catch (Exception ex)
            {
                lbl_Reslut.Text = ex.Message;
            }
        }

        private async void CreateFile()//创建文件夹
        {
            await folder.CreateFolderAsync(folder_name, CreationCollisionOption.ReplaceExisting);
        }

        private async void StreamWrite(string Str)
        {
            try
            {
                StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.GetFileAsync(file_name);
                if (file == null)
                {
                    file = await folder.CreateFileAsync(file_name, CreationCollisionOption.ReplaceExisting);
                }
                await FileIO.WriteTextAsync(file, Str);
            }
            catch (Exception ex)
            {
                lbl_Reslut.Text = ex.Message;
            }
        }

        private async void StreamWriteLine(string Str)
        {
            try
            {
                StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.GetFileAsync(file_name);
                if (file == null)
                {
                    file = await folder.CreateFileAsync(file_name, CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(file, Str);
                }
                //string text = await Windows.Storage.FileIO.ReadTextAsync(file);
                //text = text + "\r\n" + Str;
                //await FileIO.WriteTextAsync(file, text);
                await FileIO.AppendTextAsync(file, "\r\n" + Str);
            }
            catch (Exception ex)
            {
                lbl_Reslut.Text = ex.Message;
            }
        }

        private async void StreamRead()
        {
            try
            {
                StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.GetFileAsync(file_name);
                string text = await Windows.Storage.FileIO.ReadTextAsync(file);
                txt_Output.Text = text;
                txt_Path.Text = file.Path;
            }
            catch (Exception ex)
            {
                lbl_Reslut.Text = ex.Message;
            }
        }

    }
}

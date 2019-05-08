using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LotteryPro
{
    public partial class FrmMain : Form
    {
        private Selector objSelector = new Selector();//创建选号器对象
        public FrmMain()
        {
            InitializeComponent();
            this.btn_Select.Enabled = false;//禁用相关按钮
            this.btn_Print.Enabled = false;
            this.btn_Clear.Enabled = false;
            this.btn_Del.Enabled = false;
            //关联打印对象的事件
            printDoc.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.LotteryPrintPage);
        }

        private void RadomTimer_Tick(object sender, EventArgs e)
        {
            string[] numList = objSelector.CreateNum();//生成随机号码
            //显示随机号
            this.lbl_Num1.Text = numList[0];
            this.lbl_Num2.Text = numList[1];
            this.lbl_Num3.Text = numList[2];
            this.lbl_Num4.Text = numList[3];
            this.lbl_Num5.Text = numList[4];
            this.lbl_Num6.Text = numList[5];
            this.lbl_Num7.Text = numList[6];
        }//定时生成随机号码

        #region 窗体按钮实现

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Min_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        #endregion

        #region 实现窗体拖动
        private Point mouseOff;//鼠标移动位置设为变量
        private bool leftFlag;//标签是否为左键
        private void FrmMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOff = new Point(-e.X, -e.Y);
                leftFlag = true;
            }
        }
        private void FrmMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                Point mouseSet = Control.MousePosition;
                mouseSet.Offset(mouseOff.X, mouseOff.Y);//设置移动后的位置
                Location = mouseSet;
            }
        }

        private void FrmMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                leftFlag = false;//改变标签
            }
        }
        #endregion

        private void btn_Start_Click(object sender, EventArgs e)//启动选号器
        {
            this.RadomTimer.Start();
            this.btn_Start.Enabled = false;
            this.btn_Select.Enabled = true;
        }

        private void btn_Select_Click(object sender, EventArgs e)
        {
            this.RadomTimer.Stop();
            string[] SelectNum = {this.lbl_Num1.Text,
                                  this.lbl_Num2.Text,
                                  this.lbl_Num3.Text,
                                  this.lbl_Num4.Text,
                                  this.lbl_Num5.Text,
                                  this.lbl_Num6.Text,
                                  this.lbl_Num7.Text,
                                 };
            objSelector.SelectNums.Add(SelectNum);//存储数据
            ShowSelectedNums();//显示选中号码
        }

        private void ShowSelectedNums()
        {
            this.lbNumberList.Items.Clear();
            this.lbNumberList.Items.AddRange(objSelector.GetPrintedNums().ToArray());
            //按钮启动
            btn_Start.Enabled = btn_Print.Enabled = btn_Clear.Enabled = btn_Del.Enabled = true;

        }

        private void btn_WriteSelf_Click(object sender, EventArgs e)//手写号码
        {
            //先验证文本框是单个数字

            string[] SelectNum = {this.txt_Num1.Text,
                                  this.txt_Num2.Text,
                                  this.txt_Num3.Text,
                                  this.txt_Num4.Text,
                                  this.txt_Num5.Text,
                                  this.txt_Num6.Text,
                                  this.txt_Num7.Text,
                                 };
            objSelector.SelectNums.Add(SelectNum);//存储数据
            ShowSelectedNums();//显示选中号码
        }

        private void btn_GroupSelect_Click(object sender, EventArgs e)//随机组选
        {
            this.RadomTimer.Stop();//排除随机选号状态
            //验证是否给了数字(略)
            this.objSelector.CreateGroupNum(Convert.ToInt32(this.txt_Group.Text.Trim()));
            ShowSelectedNums();
        }

        private void btn_Del_Click(object sender, EventArgs e)
        {
            if (this.lbNumberList.Items.Count == 0 || this.lbNumberList.SelectedItem == null)
                return;
            int index = this.lbNumberList.SelectedIndex;
            objSelector.SelectNums.RemoveAt(index);//移除后台数据
            ShowSelectedNums();
            if (objSelector.SelectNums.Count == 0)//当全部清除的时候
            {
                this.btn_Clear.Enabled = this.btn_Del.Enabled = this.btn_Print.Enabled = false;
            }
        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            lbNumberList.Items.Clear();//从页面中清除显示
            objSelector.SelectNums.Clear();//清除后台数据
            this.lbl_Num1.Text = this.lbl_Num2.Text = this.lbl_Num3.Text = this.lbl_Num4.Text = this.lbl_Num5.Text =
                this.lbl_Num6.Text = this.lbl_Num7.Text = "0";
            this.txt_Num1.Text = this.txt_Num2.Text = this.txt_Num3.Text = this.txt_Num4.Text = this.txt_Num5.Text =
                this.txt_Num6.Text = this.txt_Num7.Text = "0";
            //复位选择框体
            this.btn_Clear.Enabled = this.btn_Del.Enabled = this.btn_Print.Enabled = false;
        }

        private void btn_Print_Click(object sender, EventArgs e)//实现打印
        {
            this.printDoc.Print();
        }

        private void LotteryPrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            string serialNum = DateTime.Now.ToString("yyyyMMddHHmmssms");//流水号（条码使用）
            this.objSelector.PrintLottery(e, serialNum, objSelector.GetPrintedNums());

            objSelector.Save(serialNum);//同时保存

            btn_Clear_Click(null, null);//全部清空
        }

    }
}

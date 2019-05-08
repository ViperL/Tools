
// 串口助手V0.0Dlg.cpp : 实现文件
//

#include "stdafx.h"
#include "串口助手V0.0.h"
#include "串口助手V0.0Dlg.h"
#include "afxdialogex.h"
#include <tchar.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// 用于应用程序“关于”菜单项的 CAboutDlg 对话框

class CAboutDlg : public CDialogEx
{
public:
	CAboutDlg();

// 对话框数据
#ifdef AFX_DESIGN_TIME
	enum { IDD = IDD_ABOUTBOX };
#endif

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持

// 实现
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialogEx(IDD_ABOUTBOX)
{
	EnableActiveAccessibility();
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialogEx)
END_MESSAGE_MAP()


// C串口助手V00Dlg 对话框



C串口助手V00Dlg::C串口助手V00Dlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(IDD_V00_DIALOG, pParent)
	, m_Receive(_T(""))
	, m_Send(_T(""))
{
	EnableActiveAccessibility();
	m_hIcon = AfxGetApp()->LoadIcon(IDI_ICON1);
}

void C串口助手V00Dlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_RECEIVE, m_Receive);
	DDX_Text(pDX, IDC_SEND, m_Send);
	DDX_Control(pDX, IDC_SERIAL_PORT, m_Comb1);
	DDX_Control(pDX, IDC_BAUD_RATE, m_Comb2);
	DDX_Control(pDX, IDC_MSCOMM1, m_Mscom);
	DDX_Control(pDX, IDC_RECEIVE, m_Edit);
}

BEGIN_MESSAGE_MAP(C串口助手V00Dlg, CDialogEx)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_Open, &C串口助手V00Dlg::OnBnClickedButton1)
	ON_BN_CLICKED(IDC_SENT, &C串口助手V00Dlg::OnBnClickedSent)
	ON_BN_CLICKED(IDC_CLEAN, &C串口助手V00Dlg::OnBnClickedClean)
	ON_BN_CLICKED(IDCANCEL, &C串口助手V00Dlg::OnBnClickedCancel)
	ON_BN_CLICKED(IDC_SAVE, &C串口助手V00Dlg::OnBnClickedSave)
END_MESSAGE_MAP()


// C串口助手V00Dlg 消息处理程序

BOOL C串口助手V00Dlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// 将“关于...”菜单项添加到系统菜单中。

	// IDM_ABOUTBOX 必须在系统命令范围内。
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		BOOL bNameValid;
		CString strAboutMenu;
		bNameValid = strAboutMenu.LoadString(IDS_ABOUTBOX);
		ASSERT(bNameValid);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// 设置此对话框的图标。  当应用程序主窗口不是对话框时，框架将自动
	//  执行此操作
	SetIcon(m_hIcon, TRUE);			// 设置大图标
	SetIcon(m_hIcon, FALSE);		// 设置小图标

	// TODO: 在此添加额外的初始化代码
	//串口组合框设置
	CString str;

	int i;
	for (i = 0; i<15; i++)
	{
		str.Format(_T("com %d"), i + 1);
		m_Comb1.InsertString(i, str);
	}
//********************************************************************//

//********************************************************************//

	m_Comb1.SetCurSel(0);//预置COM口


						 //波特率选择组合框
	CString str1[] = { _T("300"),_T("600"),_T("1200"),_T("2400"),_T("4800"),_T("9600"),
		_T("19200"),_T("50000"),_T("56000"),_T("57600"),_T("115200"),_T("1000000") };
	for (int i = 0; i<12; i++)
	{
		int judge_tf = m_Comb2.AddString(str1[i]);
		if ((judge_tf == CB_ERR) || (judge_tf == CB_ERRSPACE))
			MessageBox(_T("build baud error!"));
	}
	m_Comb2.SetCurSel(5);//预置波特率为"9600"


	return TRUE;  // 除非将焦点设置到控件，否则返回 TRUE
}

void C串口助手V00Dlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialogEx::OnSysCommand(nID, lParam);
	}
}

// 如果向对话框添加最小化按钮，则需要下面的代码
//  来绘制该图标。  对于使用文档/视图模型的 MFC 应用程序，
//  这将由框架自动完成。

void C串口助手V00Dlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // 用于绘制的设备上下文

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// 使图标在工作区矩形中居中
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// 绘制图标
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
	}
}

//当用户拖动最小化窗口时系统调用此函数取得光标
//显示。
HCURSOR C串口助手V00Dlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}



void C串口助手V00Dlg::OnBnClickedButton1()
{

		CString str, str1, n;					//定义字符串
		GetDlgItemText(IDC_Open, str);
		CWnd *h1;
		h1 = GetDlgItem(IDC_Open);		//指向控件的caption

		if (!m_Mscom.get_PortOpen())
		{
			m_Comb2.GetLBText(m_Comb2.GetCurSel(), str1);//取得所选的字符串，并存放在str1里面
			str1 = str1 + ',' + 'n' + ',' + '8' + ',' + '1';			//这句话很关键

			m_Mscom.put_CommPort((m_Comb1.GetCurSel() + 1));	//选择串口
			m_Mscom.put_InputMode(1);			//设置输入方式为二进制方式
			m_Mscom.put_Settings(str1);		//波特率为（波特率组Á合框）无校验，8数据位，1个停止位
			m_Mscom.put_InputLen(1024);		//设置当前接收区数据长度为1024
			m_Mscom.put_RThreshold(1);			//缓冲区一个字符引发事件
			m_Mscom.put_RTSEnable(1);			//设置RT允许

			m_Mscom.put_PortOpen(true);		//打开串口
			if (m_Mscom.get_PortOpen())
			{
				AfxMessageBox(_T("串口已开启！"));
				str = _T("关闭串口");
				UpdateData(true);
				h1->SetWindowText(str);			//改变按钮名称为‘’关闭串口”
			}
		}

		else
		{
			m_Mscom.put_PortOpen(false);
			if (str != _T("打开串口"))
			{
				AfxMessageBox(_T("串口已关闭！"));
				str = _T("打开串口");
				UpdateData(true);
				h1->SetWindowText(str);			//改变按钮名称为打开串口
			}
		}

}


void C串口助手V00Dlg::OnBnClickedSent()
{
	UpdateData(true);							//更新控件数据
	m_Mscom.put_Output(COleVariant(m_Send));//把发送编辑框的数据发送出去
}


void C串口助手V00Dlg::OnBnClickedClean()
{
	m_Receive = _T("");	//给接收编辑框发送空格符
	UpdateData(false);		//更新数据
}



void C串口助手V00Dlg::OnBnClickedCancel()
{
	if (m_Mscom.get_PortOpen())
		m_Mscom.put_PortOpen(false);
	CDialogEx::OnCancel();
}
BEGIN_EVENTSINK_MAP(C串口助手V00Dlg, CDialogEx)
	ON_EVENT(C串口助手V00Dlg, IDC_MSCOMM1, 1, C串口助手V00Dlg::OnCommMscomm1, VTS_NONE)
END_EVENTSINK_MAP()


void C串口助手V00Dlg::OnCommMscomm1()
{
	if (m_Mscom.get_CommEvent() == 2)
	{
		char str[1024] = { 0 };
		long k;
		VARIANT InputData = m_Mscom.get_Input();	//读缓冲区
		COleSafeArray fs;
		fs = InputData;	//VARIANT型变À量转换为COleSafeArray型变量
		for (k = 0; k<fs.GetOneDimSize(); k++)
			fs.GetElement(&k, str + k);	//转换为BYTE型数组

		m_Receive += str;      //	接收到编辑框里面
								   //SetTimer(1,10,NULL);		//延时10ms
		UpdateData(false);

		m_Edit.SetSel(-1, -1);
		this->SetDlgItemTextW(IDC_RECEIVE, m_Receive);//将m_Receive内容显示到ID为IDC_EDIT的编辑框的最后位置
		m_Edit.LineScroll(m_Edit.GetLineCount() - 1, 0);//将垂直滚动条滚动到最后一

	}

}




void C串口助手V00Dlg::OnBnClickedSave()
{
	CStdioFile file;
	CFileException fileException;
	if (file.Open(_T("./接收记录.txt"), CFile::typeText | CFile::modeCreate | CFile::modeReadWrite), &fileException)

	{
		CString str1;
		str1= m_Receive;
		file.WriteString(str1);
		AfxMessageBox(_T("输出成功！"));
	}

	else
	{
		AfxMessageBox(_T("因未知原因无法输出文件"));
	}

	file.Close();
}

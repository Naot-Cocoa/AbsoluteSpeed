using FanController;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

public static class AppController
{
    //ネットで調べた各種イベントの値
    //マウスボタンを押された
    public const int WM_LBUTTONDOWN = 0x201;
    //マウスボタンが放された
    public const int WM_LBUTTONUP = 0x202;
    //マウスボタンを押している
    public const int MK_LBUTTON = 0x0001;
    //ウィンドウハンドルのサイズが必要らしい
    public static int GWL_STYLE = -16;

    // DLLから呼び出す命令群
    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindowEx(IntPtr hWnd, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

    [DllImport("user32")]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    //起動したプロセスを入れておく
    private static Process mProc = null;

    public static void StartProcess()
    {
        if (mProc != null) return;
        var procs = Process.GetProcessesByName("ADIR01P_Trns_CT_v12");
        if (procs.Length > 0)
        {
            mProc = new Process();
            mProc = procs.First();
            return;
        }
        var appPass = "/usb_ir_remote/ADIR01P_Trns_CT_v12";
        var currentPath = new System.IO.DirectoryInfo(System.IO.Directory.GetCurrentDirectory()).ToString();
        //プロジェクトからの絶対パスを生成
        currentPath += appPass;
        Program.Run(currentPath);
    }

    public static void GetProcess()
    {
        if (mProc != null) return;
        mProc = new Process();
        mProc = Process.GetProcessesByName("ADIR01P_Trns_CT_v12")[0];
    }

    public static void KillProcess()
    {
        mProc.Kill();
    }

    /// <summary>
    /// 指定した番号のボタンを探し押す
    /// </summary>
    /// <param name="findNum"></param>
    public static void FindButton(int findNum)
    {
        // トップウィンドウのウィンドウハンドル（※見つかることを前提としている）
        var mainWindowHandle = mProc.MainWindowHandle;
        // 対象のボタンを探す
        var hWnd = FindTargetButton(GetWindow(mainWindowHandle), findNum);
        PushButton(hWnd);
    }

    /// <summary>
    /// ボタンを押す
    /// </summary>
    /// <param name="hWnd">押すボタンのハンドル</param>
    private static void PushButton(IntPtr hWnd)
    {
        // マウスを押して放す
        SendMessage(hWnd, WM_LBUTTONDOWN, MK_LBUTTON, 0x000A000A);
        SendMessage(hWnd, WM_LBUTTONUP, 0x00000000, 0x000A000A);
    }
    /// <summary>
    /// アプリウィンドウの中から指定された番号のボタンのハンドルを返す
    /// </summary>
    /// <param name="top">アプリウィンドウのハンドル</param>
    /// <param name="findNum">探したい番号</param>
    /// <returns></returns>
    public static IntPtr FindTargetButton(Window top, int findNum)
    {
        var all = GetAllChildWindows(top, new List<Window>());
        var button = all.Where(x => x.ClassName.Contains("BUTTON"));
        //foreach(var x in button.Select((x,i) => new {x,i }))
        //{
        //    UnityEngine.Debug.Log("番号:" + x.i + "名前:" + x.x.Title);
        //}
        
        var buttonFind = button.ToArray()[findNum];
        return buttonFind.hWnd;
    }


    // 指定したウィンドウの全ての子孫ウィンドウを取得し、リストに追加する
    public static List<Window> GetAllChildWindows(Window parent, List<Window> dest)
    {
        dest.Add(parent);
        EnumChildWindows(parent.hWnd).ToList().ForEach(x => GetAllChildWindows(x, dest));
        return dest;
    }

    // 与えた親ウィンドウの直下にある子ウィンドウを列挙する（孫ウィンドウは見つけてくれない）
    public static IEnumerable<Window> EnumChildWindows(IntPtr hParentWindow)
    {
        IntPtr hWnd = IntPtr.Zero;
        while ((hWnd = FindWindowEx(hParentWindow, hWnd, null, null)) != IntPtr.Zero) { yield return GetWindow(hWnd); }
    }

    // ウィンドウハンドルを渡すと、ウィンドウテキスト（ラベルなど）、クラス、スタイルを取得してWindowsクラスに格納して返す
    public static Window GetWindow(IntPtr hWnd)
    {
        int textLen = GetWindowTextLength(hWnd);
        string windowText = null;
        if (0 < textLen)
        {
            //ウィンドウのタイトルを取得する
            StringBuilder windowTextBuffer = new StringBuilder(textLen + 1);
            GetWindowText(hWnd, windowTextBuffer, windowTextBuffer.Capacity);
            windowText = windowTextBuffer.ToString();
        }

        //ウィンドウのクラス名を取得する
        StringBuilder classNameBuffer = new StringBuilder(256);
        GetClassName(hWnd, classNameBuffer, classNameBuffer.Capacity);

        // スタイルを取得する
        int style = GetWindowLong(hWnd, GWL_STYLE);
        return new Window() { hWnd = hWnd, Title = windowText, ClassName = classNameBuffer.ToString(), Style = style };
    }

}

public class Window
{
    public string ClassName;
    public string Title;
    public IntPtr hWnd;
    public int Style;
}
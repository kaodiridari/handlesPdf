using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

public interface IWindowStuff
{
    IntPtr _SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);
    int    _GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);
    int    _GetWindowTextLength(IntPtr hWnd);
    bool   _EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
    bool   _IsWindowVisible(IntPtr hWnd);
}

public class RealWindowStuff : IWindowStuff
{ 
    #region extern
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetWindowTextLength(IntPtr hWnd);
    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(IntPtr hWnd);
    #endregion extern 

    public IntPtr _SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam)
    {
        return SendMessage(hWnd, Msg, wParam, lParam);
    }

    public int _GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount)
    {
        return GetWindowText(hWnd, strText, maxCount);
    }

    public int _GetWindowTextLength(IntPtr hWnd)
    {
        return GetWindowTextLength(hWnd);
    }

    public bool _EnumWindows(EnumWindowsProc enumProc, IntPtr lParam)
    {
        return EnumWindows(enumProc, lParam);
    }

    public bool _IsWindowVisible(IntPtr hWnd)
    {
        return IsWindowVisible(hWnd);
    }
}

public class FakeWindowStuff : IWindowStuff
{
    public class FakedWindow
    {
        public string text;
        public bool isVisible;
        public IntPtr hwnd;

        public FakedWindow(IntPtr hwnd, string text,  bool isVisible)
        {
            this.hwnd = hwnd;
            this.text = text;
            this.isVisible = isVisible;
        } 
    }

    List<FakedWindow> fws;// = new List<FakedWindow>();

    public FakeWindowStuff(List<FakedWindow> fws)
    {
        this.fws = fws;
    }

    public void setWindows(List<FakedWindow> fws)
    {
        this.fws = fws;
    }

    /// <summary>
    /// Iterates over a list of faked windows.
    /// </summary>
    /// <param name="enumProc">must return always true.</param>
    /// <param name="lParam">always IntPtr.Zero</param>
    /// <returns>Allways true.</returns>
    public bool _EnumWindows(EnumWindowsProc enumProc, IntPtr lParam)
    {
        foreach (FakedWindow fw in this.fws) {
            enumProc(fw.hwnd, IntPtr.Zero);
        }
        return true; 
    }

    public int _GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount)
    {
        string wt = pickSingleFromList(hWnd).text;
        strText.Append(wt);
        return wt.Length;
    }

    public int _GetWindowTextLength(IntPtr hWnd)
    {
        return pickSingleFromList(hWnd).text.Length;
    }

    public bool _IsWindowVisible(IntPtr hWnd)
    {
        return pickSingleFromList(hWnd).isVisible;    
    }

    private FakedWindow pickSingleFromList(IntPtr hWnd)
    {
        var res = from w in fws where w.hwnd == hWnd select w;
        if (res.Count() != 1)
        {
            throw new Exception("Ups. More than one result or none.");
        }
        FakedWindow fw = res.First();
        return fw;
    }

    /// <summary>
    /// Here: Returns a handle to a icon.
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="Msg"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <returns>A handle to an icon (always the same)</returns>
    public IntPtr _SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam)
    {
        Bitmap myBitmap = new Bitmap(@"C:\Users\user\Documents\Visual Studio 2015\Projects\handlesPdf\WindowsFormsApplication2\Resources\blue_qm.jpg");

        IntPtr Hicon = myBitmap.GetHicon();

        return Hicon;
    }
}

/// <summary>
/// Which are the application windows like adobe reader, firefox? 
/// </summary>
public class MainWindows  {
   
    #region types
    /// <summary>
    /// Delegate for getting a window-list.
    /// Called when new (main) windows are created or existing windows are closed.
    ///  Also immediatly after the constructor is called.
    /// </summary>
    /// <param name="mws" Updated window-list.></param>
    public delegate void MainWindowsDelegate(List<MainWindow> mws);

    /// <summary>
    /// Structure for a window: handle, window-title and a icon (if available).
    /// </summary>
    public class MainWindow
    {
        public string title {get;}
        public Icon ic {get;}
        public IntPtr hWnd {get;}
        public enum flags {VANISHED, NEW, SAME, NONE}
        public flags flag { get; set; }

        public MainWindow(Icon ic, string v, IntPtr hWnd, flags f) 
        {
            this.ic = ic;
            this.title = v;
            this.hWnd = hWnd;
            this.flag = f;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("hwnd: ").Append(hWnd).Append(",");
            sb.Append("title: ").Append(title).Append(",");
            sb.Append("icon: ").Append(ic).Append(";");
            return sb.ToString();
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            // TODO: write your implementation of Equals() here
            MainWindow inObj = (MainWindow)obj;
            if (hWnd.Equals(inObj.hWnd))
            {
                return true;
            }
            return false;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            return hWnd.GetHashCode();            
        }
    }
    #endregion types
    #region members
    private List<MainWindow> _windows = new List<MainWindow>();
    private int _refreshMillis;     
    private System.Threading.Timer _timer;
    
    private List<MainWindow> _handlesOfOldWindows;
    private int refreshMillis;
    private List<MainWindowsDelegate> clients;
    private IWindowStuff _ws;
    private bool _busy = false;
    #endregion

    public MainWindows(int refreshMillis, List<MainWindowsDelegate> clients, IWindowStuff ws)
    {
        this.refreshMillis = refreshMillis;
        this.clients = clients;
        _timer = new System.Threading.Timer(timetimer, "Timer", 0, refreshMillis);
        _ws = ws;
    }

    public void setWindowStuff(IWindowStuff ws)  //called from "outside-thread"
    {
        if (_busy)
            throw new Exception("Can't change windows while busy.");
        //lock (ws)
        { 
            _ws = ws;
        }
    }

    private void timetimer(object state)
    {
        try
        {
           setBusy(true);
            if (getMainWindows())
            {
                //call "view"
                Console.WriteLine("Timer new windows. Calling views.");
                foreach (MainWindowsDelegate aDelegate in clients)
                {
                    aDelegate.Invoke(_windows);
                }
            }
        } finally
        {
            setBusy(false);
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void setBusy(bool busy)
    {
        _busy = busy;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool isBusy()
    {
        return _busy;
    }

    //public static void start(int refreshMillis) {
    //    ;
    //}

    /// <summary>
    /// Callback for EnumWindows in user32.dll.
    /// Uses _GetWindowTextLength, _IsWindowVisible, _GetWindowText
    /// </summary>
    /// <param name="hWnd">Windows handle.</param>
    /// <param name="lParam">Not used here.</param>
    /// <returns>Always true.</returns>
    private bool EnumTheWindows(IntPtr hWnd, IntPtr lParam)
    {

        int size = _ws._GetWindowTextLength(hWnd);
        if (size > 0 && _ws._IsWindowVisible(hWnd))
        {
            MainWindow win =_handlesOfOldWindows.Find(w => w.hWnd.Equals(hWnd));
            if (win != null)          //old window
            {
                win.flag = (MainWindow.flags.SAME);
                _windows.Add(win);
            }
            else     //new window -> query details
            {
                StringBuilder sb = new StringBuilder(size + 1);
                _ws._GetWindowText(hWnd, sb, size + 1);
                Icon ic = getIconFromHandle(hWnd);
                var mw = new MainWindow(ic, sb.ToString(), hWnd, MainWindow.flags.NEW);
                _windows.Add(mw);
            }
        } 
        
        return true;  //would stop enumerating windows if false is returned
    }
    
    /// <summary>
    /// Trys to get an icon from the windows-handle.
    /// </summary>
    /// <param name="hWnd"></param>
    /// <returns>The windows-icon, if there is one. May be null.</returns>
    private Icon getIconFromHandle(IntPtr hWnd)
    {
        UInt32 WM_GETICON = 0x7F;
        Int32 ICON_SMALL = 1; //(16x16)

        IntPtr IconHandle = _ws._SendMessage(hWnd, WM_GETICON, ICON_SMALL, 0);
        Icon icn = null;
        
        if (IconHandle != null && IconHandle.ToInt32() != 0)
        {
            try {
                icn = Icon.FromHandle(IconHandle);
            } catch (Exception e)
            {
                //ignore
            }
        }
        return icn;
    }

    /// <summary>
    /// Retrieve visible windows. -> _windows
    /// </summary>
    /// <returns>true _windows has changed.</returns>
    private bool getMainWindows()
    {
        //fast because of few windows 
        //beim umkopieren vanished windows löschen.
        _handlesOfOldWindows = (new List<MainWindow>(_windows.Count));
        foreach (MainWindow mw in _windows)
        {
            if (!mw.flag.Equals(MainWindow.flags.VANISHED))
            {
                _handlesOfOldWindows.Add(mw);
            }
        }

        _windows = new List<MainWindow>(_handlesOfOldWindows.Count + 10 );
        _ws._EnumWindows(new EnumWindowsProc(EnumTheWindows), IntPtr.Zero);

        //sort by handle
        _windows = _windows.OrderBy(o => o.hWnd.ToInt32()).ToList();

        //mark vanished windows
        var vanishedWindows = _handlesOfOldWindows.Except(_windows);
        foreach(MainWindow vanished in vanishedWindows)
        {
            vanished.flag = MainWindow.flags.VANISHED;             
        }
        if (vanishedWindows.Count() > 0)
        {
            _windows.AddRange(vanishedWindows);
            return true;
        }

        if (_handlesOfOldWindows.Count != _windows.Count)
        {
            return true;
        }  else
        {
            for(int i = 0; i < _windows.Count; i++)
            {
                if (_handlesOfOldWindows[i].hWnd != _windows[i].hWnd)
                {
                    return true;
                }
            }
            return false;
        }                 
    }

    
    public void stop()
    {  
        _timer.Dispose();
    }
}


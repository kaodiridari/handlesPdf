using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Snipping_OCR
{
    public  class MainForm : Form
    {
        private bool _capturing;
        private IntPtr _clipboardViewerNext;
        private bool _isSnipping = false;

        public MainForm()
        {           
                                 
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RegisterClipboardViewer();        
            Hide();
        }

        private void RegisterClipboardViewer()
        {
            _clipboardViewerNext = Win32.User32.SetClipboardViewer(this.Handle);
        }

        private void UnregisterClipboardViewer()
        {
            Win32.User32.ChangeClipboardChain(this.Handle, _clipboardViewerNext);
        }

        protected override void WndProc(ref Message m)
        {
            switch ((Win32.Msgs)m.Msg)
            {
                case Win32.Msgs.WM_DRAWCLIPBOARD:
                    OnClipboardData();
                    Win32.User32.SendMessage(_clipboardViewerNext, m.Msg, m.WParam, m.LParam);
                    break;
                case Win32.Msgs.WM_CHANGECBCHAIN:
                    Debug.WriteLine("WM_CHANGECBCHAIN: lParam: " + m.LParam, "WndProc");
                    if (m.WParam == _clipboardViewerNext)
                    {
                        _clipboardViewerNext = m.LParam;
                    }
                    else
                    {
                        Win32.User32.SendMessage(_clipboardViewerNext, m.Msg, m.WParam, m.LParam);
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        // Called when there is new data in clipboard
        public void OnClipboardData()
        {
            if (_capturing)
            {
                ProcessClipboardData();
            }
        }

        private void ProcessClipboardData()
        {
            if (Clipboard.ContainsImage())
            {
                var img = Clipboard.GetImage();                    
            }
        }

        private void SaveImage(Image image)
        {
            //TODO        
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void Exit()
        {            
            UnregisterClipboardViewer();        
            Application.Exit();
            Environment.Exit(0);
        }

        //private void mnuMonitorClipboard_Click(object sender, EventArgs e)
        //{
        //    mnuMonitorClipboard.Checked = !mnuMonitorClipboard.Checked;
        //    if (mnuMonitorClipboard.Checked)
        //    {
        //        _capturing = true;
        //        OnClipboardData();
        //    }
        //    else
        //    {
        //        _capturing = false;
        //    }
        //}

        private void StartSnipping()
        {
            if (!_isSnipping)
            {
                _isSnipping = true;
                ChooseAreaTool.Snip();
            }
        }       

        

        private void AreaToolOnAreaSelected(object sender, EventArgs e)
        {
            _isSnipping = false;
            SaveImage(ChooseAreaTool.Image);            
        }

        private void mnuArea_Click(object sender, EventArgs e)
        {

        }

        private void mnuButtonToClick_Click(object sender, EventArgs e)
        {

        }
    }
}

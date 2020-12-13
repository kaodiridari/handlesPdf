using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

//ohne Designer: https://msdn.microsoft.com/en-us/library/5s3ce6k8(v=vs.110).aspx
//namespace WindowsFormsApplication2
//{
public partial class WindowsForm : Form
{ 
    private readonly int refreshMillis = 1000;
    private MainWindows mw;
    

    public WindowsForm()
    {
        InitializeComponent();
        MainWindows.MainWindowsDelegate x = new MainWindows.MainWindowsDelegate(populateWindowsGridListView);
        List<MainWindows.MainWindowsDelegate> lc = new List<MainWindows.MainWindowsDelegate>(1);
        lc.Add(x);
        RealWindowStuff ws = new RealWindowStuff();
        mw = new MainWindows(refreshMillis, lc, ws);        
    }

    /// <summary>
    /// 
    /// </summary>
    /// 
    private void populateWindowsGridListView(List<MainWindows.MainWindow> mws)
    {
        //Console.WriteLine("populateWindowsGridListView " + mws.Count);
        //dataSet1.Clear();
        //foreach (MainWindows.MainWindow amw in mws)
        //{   
        //  //  WindowsFormsApplication2.DataSet1.MainWindowDataRow newRow = dataSet1.MainWindowData.NewMainWindowDataRow();
        //    newRow.handle = amw.hWnd;
        //    newRow.title = amw.title;
        //    if (amw.ic != null)
        //    {
        //        newRow.icon = amw.ic.ToBitmap();
        //    }
        //    else
        //    {                
        //        Bitmap im = (Bitmap)global::WindowsFormsApplication2.Properties.Resources.ResourceManager.GetObject("qm");
        //        newRow.icon = im;
        //    }
        //    dataSet1.MainWindowData.Rows.Add(newRow);
        //}
        //dataSet1.AcceptChanges();
    }

    private void windowChoosenButton_Click(object sender, EventArgs e)
    {
        //ausgewählte Zeile übernehmen
        IntPtr handle = (IntPtr)dataGridView1.SelectedRows[0].Cells[1].Value;
        textBox1.Text = handle.ToString();
    }
}
//}

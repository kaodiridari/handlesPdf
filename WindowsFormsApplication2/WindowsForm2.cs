using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class WindowsForm2 : Form
    {
        private delegate void SetTextCallback(MainWindows.MainWindow mw);
        private delegate void SetTextCallbackCount(int count);
        private delegate void ClearCallback();
        private delegate void ClearGridCallback();
        private delegate void SetRowCallback(MainWindows.MainWindow mw);

        private MainWindows mw;
        private int refreshMillis = 500;

        public WindowsForm2()
        {
            InitializeComponent();
            List < MainWindows.MainWindowsDelegate > clients = new List<MainWindows.MainWindowsDelegate>(2);
            MainWindows.MainWindowsDelegate x = new MainWindows.MainWindowsDelegate(populateTextBox);
            MainWindows.MainWindowsDelegate xx = new MainWindows.MainWindowsDelegate(populateGridbox);
            clients.Add(x);
            clients.Add(xx);
            mw = new MainWindows(refreshMillis, clients, new RealWindowStuff());
        }

        private void populateGridbox(List<MainWindows.MainWindow> mws)
        {
            if (this.dataGridView1.InvokeRequired)
            {
                this.Invoke(new ClearGridCallback(ClearGrid));
            } else
            {
                ClearGrid();
            }

            Console.WriteLine("populateGridbox " + mws.Count);
            foreach (var amw in mws) {
                if (this.dataGridView1.InvokeRequired)
                {
                    SetRowCallback scb = new SetRowCallback(setGridRow);
                    Invoke(scb, new object[] {amw});
                } else
                {
                    setGridRow(amw);
                }
            }
        } 

        private void ClearGrid()
        {
            //this.dataGridView1.DataSource = null;   //no DataSource
            this.dataGridView1.Rows.Clear();
            this.dataGridView1.Refresh();
        }  

        private void populateTextBox(List<MainWindows.MainWindow> mws)
        {
            Console.WriteLine("populateWindowsGridListView " + mws.Count);

            if (this.textBox2.InvokeRequired)
            {
               ClearCallback d = new ClearCallback(clearTextBoxText);
               this.Invoke(d, new object[] {  });
            }
            else
            {
                clearTextBoxText();
            }

            foreach (MainWindows.MainWindow amw in mws)
            {
                if (this.textBox2.InvokeRequired)
                {                    
                    SetTextCallback d = new SetTextCallback(setTextBoxText);
                    this.Invoke(d, new object[] { amw });
                }
                else
                {
                    setTextBoxText(amw);
                }   
            }

            if (this.textBox3.InvokeRequired)
            {
                SetTextCallbackCount d = new SetTextCallbackCount(setTextBox3Text);
                this.Invoke(d, new object[] { mws.Count });
            }   else
            {
                setTextBox3Text(mws.Count);
            }
            
        }

        private void clearTextBoxText()
        {
            textBox2.Clear();
        }

        private void setTextBox3Text(int count)
        {
            textBox3.Clear();
            textBox3.AppendText(count.ToString());
        }

        private void setTextBoxText(MainWindows.MainWindow amw)
        {           
            textBox2.AppendText("handle: " + amw.hWnd + " title: " + amw.title + "\n");
        }

        private void setGridRow(MainWindows.MainWindow amw)
        {
            string[] aRow = new string[] {amw.hWnd.ToString(), amw.title};
            dataGridView1.Rows.Add(aRow);
        }
    } 
}

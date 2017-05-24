////////////////////////////////////////////////////////////////////////////
//	Copyright 2016 : Vladimir Novick    https://www.linkedin.com/in/vladimirnovick/  
//
//    NO WARRANTIES ARE EXTENDED. USE AT YOUR OWN RISK. 
//
// To contact the author with suggestions or comments, use  :vlad.novick@gmail.com
//
////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Com.SGcombo.DataSynchronization.Utils;
using System.Threading;
using System.Security.Principal;
using System.Reflection;

namespace Com.SGcombo.DataSynchronization
{
    public partial class frmMain : Form
    {
     

        protected DataSynchronizationhronization _Sync = new DataSynchronizationhronization();
        protected DataSynchronizationConfiguration _conf = null;
        protected bool _ForceClose = false;
        protected NotifyIcon _Icon = null;
        protected bool _ReportError = true;

        public frmMain()
        {
            InitializeComponent();
            _conf = new DataSynchronizationConfiguration();
            LoadConfiguration();
    
                   
           
        }




      

        protected void LoadConfiguration()
        {
            List<ListViewItem> items = _conf.GetListViewItems();
            listView1.Items.Clear();
            listView1.Items.AddRange(items.ToArray());
            lblSyncQueueCount.Text = _Sync.QueueSyncCount.ToString();
            lblScanQueueCount.Text = _Sync.QueueScanCount.ToString();
            lblMonitorInQueueCount.Text = _Sync.QueueMonitorCount.ToString();
        }       



        private void _Icon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(_Icon, null);
            }
        }
          
        protected void EndApplication()
        {
 
            Environment.Exit(0);
        }

        #region Splash Screen
        protected void ShowProcessIndicator()
        {
            pictureBoxProcess.Show();
        }

        protected void HideProcessIndicator()
        {
            pictureBoxProcess.Hide();
        }




        #endregion

        #region Button Click Events
        private void btnAbout_Click(object sender, EventArgs e)
        {
            frmAbout about = new frmAbout();
            about.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int index = GetSelectedIndex();
            if (index == -1) return;
            frmDetail det = new frmDetail(_conf, index);
            det.ShowDialog();
            if (det.Reload) LoadConfiguration();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            _conf.Flush();
  
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmDetail det = new frmDetail(_conf, -1);
            det.ShowDialog();
            if (det.Reload)
            {

                int opt = 1;
                if (det.SyncOption == DataSynchronizationhorizationOption.Destination) { opt = 2; }
                _conf.AddConfiguration(det.Source, det.Destination, opt, det.Monitor);
                _conf.Flush();
                LoadConfiguration(); 
            }
            det.Dispose();
        }




        private void btnDel_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices == null || listView1.SelectedIndices.Count != 1)
            {
                MessageBox.Show("Please select one source and destination for deletion");
                return;
            }

            DialogResult dr = MessageBox.Show(string.Format("Are you sure you want to delete this\r\nSource = '{0}'\r\nDestination = '{1}'", GetSelectedSource(), GetSelectedDestination()), "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dr == DialogResult.Yes)
            {
                _conf.DeleteConfiguration(listView1.SelectedIndices[0]);
                LoadConfiguration();
            }
        }
        #endregion

        #region Form Events
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Maximize();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EndApplication();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_ForceClose == false)
            {
                e.Cancel = true;
                Minimize();
            }
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Minimize();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblSyncQueueCount.Text = _Sync.QueueSyncCount.ToString();
            lblScanQueueCount.Text = _Sync.QueueScanCount.ToString();
            lblMonitorInQueueCount.Text = _Sync.QueueMonitorCount.ToString();

            if (string.IsNullOrEmpty(_Sync.Status))
            {
                lblStatus.ForeColor = Color.Black;
                lblStatus.Text = "Sync is working fine";
                _ReportError = true;
            }
            else
            {
                if (_ReportError == true)
                {
                    _ReportError = false;
                    DialogResult dr = MessageBox.Show("Folder Sync is having exception : \r\n" + _Sync.Status, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (dr == DialogResult.Yes)
                    {
                        EndApplication();
                    }                    
                }
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = _Sync.Status;
            }
        }
        
        private void frmMain_Deactivate(object sender, EventArgs e)
        {
            //Minimize();
        }
        #endregion

        #region Selected Index Protected Methods
        protected int GetSelectedIndex()
        {
            if (listView1.SelectedIndices == null || listView1.SelectedIndices.Count != 1) return -1;
            return listView1.SelectedIndices[0];
        }

        protected string GetSelectedSource()
        {
            int index = GetSelectedIndex();
            if (index == -1) return string.Empty;
            ListViewItem lvi = listView1.Items[index];
            return lvi.SubItems[0].ToString();
        }

        protected string GetSelectedDestination()
        {
            int index = GetSelectedIndex();
            if (index == -1) return string.Empty;
            ListViewItem lvi = listView1.Items[index];
            return lvi.SubItems[1].ToString();
        }
        #endregion        

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            int index = GetSelectedIndex();
            if (index == -1) return;
            frmDetail det = new frmDetail(_conf, index);
            det.ShowDialog();
            if (det.Reload) LoadConfiguration();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            _Sync.Start();
            LoadChanges();  
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            HideProcessIndicator();
        }
    }
}


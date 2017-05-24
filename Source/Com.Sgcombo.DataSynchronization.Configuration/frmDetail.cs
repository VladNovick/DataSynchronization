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
using System.IO;
using Com.SGcombo.DataSynchronization.Utils;

namespace Com.SGcombo.DataSynchronization
{
    public partial class frmDetail : Form
    {
        protected DataSynchronizationConfiguration _Conf;
        protected bool _Reload = false;
        protected int _Index = -1;
        public bool Reload
        {
            get { return _Reload; }
        }

        public string Source
        {
            get { return txtSource.Text; }
        }

        public string Destination
        {
            get { return txtDestination.Text; }
        }

        public DataSynchronizationhorizationOption SyncOption
        {
            get { if (rbBoth.Checked) return DataSynchronizationhorizationOption.Both ; return DataSynchronizationhorizationOption.Destination; }
        }

        public bool Monitor
        {
            get { return chkMonitor.Enabled; }
        }

        public frmDetail(DataSynchronizationConfiguration conf, int index)
        {
            _Conf = conf;
            _Index = index;
            InitializeComponent();
            LoadDetail();
        }

        public void LoadDetail()
        {
            if (_Index == -1) return;
            btnAdd.Visible = false;
            txtSource.Enabled = true;
            txtDestination.Enabled = true;
            rbBoth.Enabled = true;
            rbDestination.Enabled = true;
            chkMonitor.Enabled = true;

            List<string> items = _Conf.GetItem(_Index);
            txtSource.Text = items[0];
            txtDestination.Text = items[1];
            if (items[2] == "1") rbBoth.Checked = true;
            else rbDestination.Checked = true;
            if (items[3] == "1") chkMonitor.Checked = true;
            else chkMonitor.Checked = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSource.Text))
            {
                MessageBox.Show("Source is empty");
                return;
            }
            if (string.IsNullOrEmpty(txtDestination.Text))
            {
                MessageBox.Show("Source is empty");
                return;
            }
            if (GeneralLib.StringCompare(txtSource.Text, txtDestination.Text))
            {
                MessageBox.Show("Source and Destination folders are the same");
                return;
            }
            if (Directory.Exists(txtSource.Text) == false)
            {
                MessageBox.Show("Source folder does not exist");
                return;
            }
            if (Directory.Exists(txtDestination.Text) == false)
            {
                MessageBox.Show("Source folder does not exist");
                return;
            }
            if (rbBoth.Checked == false && rbDestination.Checked == false)
            {
                MessageBox.Show("You must check Sync Options either Both or Destination");
                return;
            }
            if (_Index == -1)
            {
                _Conf.AddConfiguration(txtSource.Text, txtDestination.Text, rbBoth.Checked ? 1 : 2, chkMonitor.Checked);
                _Reload = true;
            }
            else
            {
                _Conf.ModifyConfiguration(_Index, txtSource.Text, txtDestination.Text, rbBoth.Checked ? 1 : 2, chkMonitor.Checked);
                _Reload = true;
            }
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSource_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = txtSource.Text;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtSource.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnDestination_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = txtDestination.Text;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtDestination.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}

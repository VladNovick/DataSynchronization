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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.SGcombo.DataSynchronization.Utils;
using System.ComponentModel;
using System.Threading;

namespace Com.SGcombo.DataSynchronization.Scheduler
{
    public class SchedulerProcess
    {
        protected DataSynchronizationhronization _Sync = new DataSynchronizationhronization();
        protected DataSynchronizationConfiguration _conf = null;
        protected bool _ForceClose = false;
        BackgroundWorker m_oWorker;

        public void Load()
        {
            _conf = new DataSynchronizationConfiguration();
           
        }

        private List<DataSynchronizationItem> items;
        

        void m_oWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //If it was cancelled midway
            if (e.Cancelled)
            {
                Console.WriteLine("Task Cancelled.");
            }
            else if (e.Error != null)
            {
                  Console.WriteLine("Error while performing background operation.");
            }
            else
            {
                 Console.WriteLine("Task Completed...");
            }
            resetEvent.Set(); // signal that worker is done
           
        }


        void m_oWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Here you play with the main UI thread
            int status  = e.ProgressPercentage;
            Console.WriteLine("Processing {0} of {1}", status, items.Count);
        }


        void m_oWorker_DoWork(object sender, DoWorkEventArgs e)
        {

         
            items = (List<DataSynchronizationItem>)e.Argument;

            int i = 0;
           foreach ( DataSynchronizationItem item in items ) 
            {
              
                m_oWorker.ReportProgress(i);
                i++;

                //If cancel button was pressed while the execution is in progress
                //Change the state from cancellation ---> cancel'ed
                if (m_oWorker.CancellationPending)
                {
                    e.Cancel = true;
                    m_oWorker.ReportProgress(0);
                    return;
                }

            }

            //Report 100% completion on operation completed
            m_oWorker.ReportProgress(100);
        }


        public AutoResetEvent resetEvent ;

        public void Start()
        {
            m_oWorker = new BackgroundWorker();
            m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWork);
            m_oWorker.ProgressChanged += new ProgressChangedEventHandler(m_oWorker_ProgressChanged);
            m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_oWorker_RunWorkerCompleted);
            m_oWorker.WorkerReportsProgress = true;
            m_oWorker.WorkerSupportsCancellation = true;
            resetEvent = new AutoResetEvent(false);

            List<DataSynchronizationItem> listScanItems;
            listScanItems = _conf.GetDataSynchronizationItems();

            m_oWorker.RunWorkerAsync(listScanItems);
        }

        public void Cancel()
        {
            if (m_oWorker.IsBusy)
            {
               m_oWorker.CancelAsync();
            }
        }


    }
}

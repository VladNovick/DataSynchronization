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
using System.IO;
using System.Threading;

namespace Com.SGcombo.DataSynchronization.Utils
{
    public class DataSynchronizationhronization
    {
        protected AutoResetEvent _SyncIsPaused = new AutoResetEvent(false);
        protected AutoResetEvent _ScanIsPaused = new AutoResetEvent(false);
        protected AutoResetEvent _MonitorIsPaused = new AutoResetEvent(false);
        protected Queue<FileOperation> _SyncQueue = new Queue<FileOperation>();
        protected Queue<DataSynchronizationhronizationScannerItem> _ScanQueue = new Queue<DataSynchronizationhronizationScannerItem>();
        protected Queue<DataSynchronizationhronizationScannerItem> _MonitorQueue = new Queue<DataSynchronizationhronizationScannerItem>();
        protected bool _IsRunning = false;
        protected bool _IsPausedSync = false;
        protected bool _IsPausedScan = false;
        protected bool _IsPausedMonitor = false;
        protected Thread _SyncThread = null;
        protected Thread _ScanThread = null;
        protected Thread _MonitorThread = null;
        protected string _Status = string.Empty;

        public string Status
        {
            get { return _Status; }
        }

        public int QueueSyncCount
        {
            get { return _SyncQueue.Count; }
        }

        public int QueueScanCount
        {
            get { return _ScanQueue.Count; }
        }

        public int QueueMonitorCount
        {
            get { return _MonitorQueue.Count; }
        }

        public DataSynchronizationhronization()
        {
            try
            {
                GeneralLib.AddAccess(AppDomain.CurrentDomain.BaseDirectory);
            }
            catch { }
        }

        #region Start/Stop/Pause/Resume
        public void Start()
        {
            if (_IsRunning == false)
            {
                _IsRunning = true;
                StartSyncingThread();
                StartScanningThread();
                StartMonitorThread();
            }
        }

        public void Stop()
        {
            if (_IsRunning)
            {
                _IsRunning = false;
            }
        }

        public void PauseMonitor()
        {
            _IsPausedMonitor = true;
        }

        public void PauseScan()
        {
            _IsPausedScan = true;
        }

        public void PauseSync()
        {
            _IsPausedSync = true;
        }

        public void ResumeMonitor()
        {
            if (_IsRunning)
            {
                _IsPausedMonitor = false;
                _MonitorIsPaused.Set();
            }
        }

        public void ResumeScan()
        {
            if (_IsRunning)
            {
                _IsPausedScan = false;
                _ScanIsPaused.Set();
            }
        }

        public void ResumeSync()
        {
            if (_IsRunning)
            {
                _IsPausedSync = false;
                _SyncIsPaused.Set();
            }
        }
        #endregion

        protected void StartMonitorThread()
        {
            ThreadStart ts = new ThreadStart(StartMonitor);
            _MonitorThread = new Thread(ts);
            _MonitorThread.Start();
        }

        protected void StartScanningThread()
        {
            ThreadStart ts = new ThreadStart(StartScanning);
            _ScanThread = new Thread(ts);
            _ScanThread.Start();
        }

        protected void StartSyncingThread()
        {
            ThreadStart ts = new ThreadStart(StartSyncing);
            _SyncThread = new Thread(ts);
            _SyncThread.Start();
        }

        protected void StartMonitor()
        {
            DataSynchronizationhronizationScannerItem _op = null;
            while (true)
            {
                if (_IsPausedMonitor) _MonitorIsPaused.WaitOne();
                if (_IsRunning == false) break;
                if (_MonitorQueue.Count == 0) continue;

                _op = _MonitorQueue.Dequeue();
                if (_op != null)
                {
                    try
                    {
                        if (Directory.Exists(_op.Source) && Directory.Exists(_op.Destination))
                        {
                            lock (_ScanQueue)
                            {
                                _ScanQueue.Enqueue(_op);
                            }
                        }
                        else
                        {
                            if (_op.Monitor) _MonitorQueue.Enqueue(_op);
                            Thread.Sleep(1000);
                        }
                        _Status = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        _Status = ex.Message;
                    }
                }
            }
            _MonitorThread.Join();
        }

        protected void StartScanning()
        {
            DataSynchronizationhronizationScannerItem _op = null;
            while (true)
            {
                if (_IsPausedScan) _ScanIsPaused.WaitOne();
                if (_IsRunning == false) break;
                if (_ScanQueue.Count == 0) continue;

                _op = _ScanQueue.Dequeue();
                if (_op != null)
                {
                    try
                    {
                        if (Directory.Exists(_op.Source) == false || Directory.Exists(_op.Destination) == false)
                        {
                            if (_op.Monitor)
                            {
                                lock (_MonitorQueue)
                                {
                                    _MonitorQueue.Enqueue(_op);
                                }
                            }                            
                        }
                        else
                        {
                            DataSynchronizationhronizationScanner fss = new DataSynchronizationhronizationScanner(_op.Source, _op.Destination, _op.Option);
                            fss.Sync();
                            if (fss.SyncCollection.Objects.Count > 0)
                            {
                                lock (_SyncQueue)
                                {
                                    foreach (FileOperation fo in fss.SyncCollection.Objects)
                                    {
                                        _SyncQueue.Enqueue(fo);
                                    }
                                }
                            }
                        }
                        _Status = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        _Status = ex.Message;
                    }
                }
            }
            _ScanThread.Join();
        }

        protected void StartSyncing()
        {
            FileOperation _op = null;
            while(true)
            {
                if (_IsPausedSync) _SyncIsPaused.WaitOne();
                if (_IsRunning == false) break;
                if (_SyncQueue.Count == 0) continue;

                _op = _SyncQueue.Dequeue();
                if (_op != null)
                {
                    try
                    {
                        _op.DoOperation();
                        _Status = string.Empty;
                    }
                    catch(Exception ex)
                    {
                        _Status = ex.Message;
                    }
                }
            }
            _SyncThread.Join();
        }

        public void AddScan(DataSynchronizationhronizationScannerItem item)
        {
            lock (_ScanQueue)
            {
                _ScanQueue.Enqueue(item);
            }
        }

        //public void StartFolderScanner(string source, string destination, DataSynchronizationhorizationOption option, bool monitor)
        //{
        //    DataSynchronizationhronizationScanner fss = new DataSynchronizationhronizationScanner(source, destination, option);
        //    fss.Sync();
        //    if (fss.SyncCollection.Objects.Count <= 0) return;
        //    foreach (FileOperation fo in fss.SyncCollection.Objects)
        //    {
        //        AddQueue(fo);
        //    }
        //}
    }
}

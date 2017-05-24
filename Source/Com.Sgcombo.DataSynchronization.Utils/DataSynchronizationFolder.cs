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

namespace Com.SGcombo.DataSynchronization.Utils
{
    public enum DataSynchronizationhronizationItemFolderOption
    {
        CreateFolder, NoOperation
    }

    public class DataSynchronizationhronizationItemFolder : FileOperation
    {
        protected string _SourceFileName = string.Empty;
        protected string _DestinationFileName = string.Empty;
        protected DataSynchronizationhronizationItemFolderOption _Option = DataSynchronizationhronizationItemFolderOption.NoOperation;

        #region Properties
        public string SourceFileName
        {
            get { return _SourceFileName; }
            set { _SourceFileName = value; }
        }

        public string DestinationFileName
        {
            get { return _DestinationFileName; }
            set { _DestinationFileName = value; }
        }

        public DataSynchronizationhronizationItemFolderOption Option
        {
            get { return _Option; }
            set { _Option = value; }
        }
        #endregion

        public DataSynchronizationhronizationItemFolder(string foldername, DataSynchronizationhronizationItemFolderOption option)
        {
            _SourceFileName = foldername;
            _Option = option;
        }

        public override void DoOperation()
        {
            if (_Option == DataSynchronizationhronizationItemFolderOption.CreateFolder)
            {
                if (Directory.Exists(_SourceFileName) == false)
                { Directory.CreateDirectory(_SourceFileName); }
            }
        }
    }
}

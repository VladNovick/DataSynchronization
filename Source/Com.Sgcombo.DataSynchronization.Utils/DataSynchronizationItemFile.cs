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
    public enum DataSynchronizationhronizationItemFileOption
    {
        SourceCreate, SourceOverwrite, DestinationCreate, DestinationOverwrite, NoOperation
    }

    public class DataSynchronizationhronizationItemFile : FileOperation
    {
        protected string _SourceFileName;
        protected string _DestinationFileName;
        protected DataSynchronizationhronizationItemFileOption _Option = DataSynchronizationhronizationItemFileOption.NoOperation;

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

        public DataSynchronizationhronizationItemFileOption Option
        {
            get { return _Option; }
            set { _Option = value; }
        }
        #endregion

        public DataSynchronizationhronizationItemFile(string source, string destination, DataSynchronizationhronizationItemFileOption option)
        {
            _SourceFileName = source;
            _DestinationFileName = destination;
            _Option = option;
        }

        public override void DoOperation()
        {
            if (_Option == DataSynchronizationhronizationItemFileOption.DestinationCreate)
            {
                if (File.Exists(_DestinationFileName) == false)
                { File.Copy(_SourceFileName, _DestinationFileName); }
            }
            else if (_Option == DataSynchronizationhronizationItemFileOption.DestinationOverwrite)
            {
                if (File.Exists(_DestinationFileName) == true)
                { File.Copy(_SourceFileName, _DestinationFileName, true); }
            }
            else if (_Option == DataSynchronizationhronizationItemFileOption.SourceCreate)
            {
                if (File.Exists(_SourceFileName) == false)
                { File.Copy(_DestinationFileName, _SourceFileName); }
            }
            else if (_Option == DataSynchronizationhronizationItemFileOption.SourceOverwrite)
            {
                if (File.Exists(_SourceFileName) == true)
                { File.Copy(_DestinationFileName, _SourceFileName, true); }
            }
        }
    }
}

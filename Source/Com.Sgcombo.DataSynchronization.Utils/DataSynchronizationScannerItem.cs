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

namespace Com.SGcombo.DataSynchronization.Utils
{
    public class DataSynchronizationhronizationScannerItem
    {
        protected string _Source;
        protected string _Destination;
        protected DataSynchronizationhorizationOption _Option;
        protected bool _Monitor;

        public string Source
        {
            get { return _Source; }
            set { _Source = value; }
        }

        public string Destination
        {
            get { return _Destination; }
            set { _Destination = value; }
        }

        public DataSynchronizationhorizationOption Option
        {
            get { return _Option; }
            set { _Option = value; }
        }

        public bool Monitor
        {
            get { return _Monitor; }
            set { _Monitor = value; }
        }
    }
}

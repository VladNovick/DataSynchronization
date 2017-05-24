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

namespace Com.SGcombo.DataSynchronization.Scheduler
{
    public class Program
    {
      
        static void Main(string[] args)
        {
              SchedulerProcess schedulerProcee;
              schedulerProcee = new SchedulerProcess();
              schedulerProcee.Start();
              schedulerProcee.resetEvent.WaitOne();
              Console.WriteLine("Completed");
            
        }
    }
}

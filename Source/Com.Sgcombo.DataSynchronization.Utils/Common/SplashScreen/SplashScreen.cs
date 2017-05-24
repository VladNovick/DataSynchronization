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
using System.Windows.Forms;
using System.Threading;

namespace Com.SGcombo.DataSynchronization.Utils
{
    public class SplashScreen
    {
        protected bool _Done = false;
        Thread _Thread = null;
        Form _Form = null;

        public SplashScreen(Form _frm)
        {
            _Form = _frm;
        }

        public void Show()
        {
            ThreadStart ts = new ThreadStart(ShowForm);
            _Thread = new Thread(ts);
            _Thread.Start();
        }

        protected void ShowForm()
        {
            _Form.Show();
            _Form.Update();
            while(!_Done)
            {
                Application.DoEvents();
            }
            _Form.Close();
            _Form.Dispose();
        }

        public void Close()
        {
            _Done = true;
        }
    }
}

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
using System.Collections;
using System.Reflection;

namespace Com.SGcombo.DataSynchronization.Utils
{
    public class ReflectionObject
    {
        public void SetProperty(string propertyName, object value)
        {
            PropertyInfo pi = this.GetType().GetProperty(propertyName);
            pi.SetValue(this, value, System.Reflection.BindingFlags.SetProperty, null, new object[] { }, null);
        }

        public object GetProperty(string propertyName)
        {
            PropertyInfo pi = this.GetType().GetProperty(propertyName);
            object value = pi.GetValue(this, null);
            return value;
        }
    }
}

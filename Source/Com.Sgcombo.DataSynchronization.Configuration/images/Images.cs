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
using System.Drawing;
using System.Reflection;

namespace Com.SGcombo.DataSynchronization
{
    public class Images
    {
        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static string assemblyPath = assembly.GetName().Name.Replace(' ', '_');

        public static System.IO.Stream GetResource(string fileName)
        {
            return assembly.GetManifestResourceStream(assemblyPath + '.' + fileName);
        }

        public static Image SplashScreen = Image.FromStream(Images.GetResource("Logo.png"));
    }
}

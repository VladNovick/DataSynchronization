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
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Com.SGcombo.DataSynchronization.Utils
{
    public class GeneralLib
    {
        public static bool StringCompare(string str1, string str2)
        {
            return str1.Equals(str2, StringComparison.CurrentCultureIgnoreCase);
        }

        public static byte[] ReadFully(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            byte[] result = br.ReadBytes((int)fs.Length);
            br.Close();
            fs.Close();
            return result;
        }

        public static Assembly LoadAssemblyFromBinaries(byte[] binaries)
        {
            Assembly asm = null;
            try
            {
                asm = Assembly.Load(binaries);
            }
            catch { }
            return asm;
        }

        public static Assembly LoadAssemblyFromFile(string filename)
        {
            byte[] binaries = ReadFully(filename);
            return LoadAssemblyFromBinaries(binaries);
        }

        public static void AddAccess(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            DirectorySecurity security = info.GetAccessControl();
            WindowsIdentity wi = WindowsIdentity.GetCurrent();

            security.AddAccessRule(new FileSystemAccessRule(wi.User, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
            security.AddAccessRule(new FileSystemAccessRule(wi.User, FileSystemRights.FullControl, InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
            info.SetAccessControl(security);

            File.SetAttributes(path, FileAttributes.Normal);
        }
    }
}

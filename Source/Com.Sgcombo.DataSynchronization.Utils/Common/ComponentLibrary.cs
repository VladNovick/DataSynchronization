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

namespace Com.SGcombo.DataSynchronization.Utils
{
    public class ComponentLibrary<T>
    {
        protected string _ComponentFolder;
        protected List<T> _Components = new List<T>();
        protected bool _LoadRecursive;

        #region Properties
        public List<T> Components
        {
            get { return _Components; }
        }

        public string ComponentFolder
        {
            get { return _ComponentFolder; }
        }
        #endregion

        public ComponentLibrary() : this(true) { }

        public ComponentLibrary(string componentFolder) : this(true, componentFolder) { }

        public ComponentLibrary(bool recursive) : this (recursive, AppDomain.CurrentDomain.BaseDirectory) { }

        public ComponentLibrary(bool recursive, string componentFolder)
        {
            _LoadRecursive = recursive;
            _ComponentFolder = componentFolder;
            LoadComponents(_ComponentFolder);
        }

        protected void LoadComponents(string folder)
        {
            if (Directory.Exists(folder) == false) return;
            foreach (string str in Directory.GetFiles(folder, "*.dll"))
            {
                if (GeneralLib.StringCompare(Path.GetExtension(str), ".dll"))
                {
                    LoadComponentsFromAsm(GeneralLib.LoadAssemblyFromFile(str));
                }
            }
            if (_LoadRecursive)
            {
                foreach (string subfolder in Directory.GetDirectories(folder))
                {
                    LoadComponents(subfolder);
                }
            }
        }

        protected void LoadComponentsFromAsm(Assembly asm)
        {
            if (asm == null) return;
            Type[] types = asm.GetTypes();
            foreach (Type type in types)
            {
                object obj = null;
                try
                {
                    obj = Activator.CreateInstance(type);
                }
                catch { }
                
                if (obj != null)
                {
                    T result = default(T);
                    try
                    {
                        result = (T)obj;
                        _Components.Add(result);
                    }
                    catch (Exception ex) 
                    {
                        Console.Write(ex.Message);
                    }
                }
            }
        }
    }
}

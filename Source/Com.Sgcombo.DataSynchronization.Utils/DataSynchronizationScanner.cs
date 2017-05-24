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
using System.Collections;

namespace Com.SGcombo.DataSynchronization.Utils
{
    public enum DataSynchronizationhorizationOption
    {
        Both, Destination
    }

    public class DataSynchronizationhronizationScanner
    {
        protected string _Source = string.Empty;
        protected string _Destination = string.Empty;
        protected DataSynchronizationhorizationOption _Options = DataSynchronizationhorizationOption.Both;
        protected MultiKeyCollection<FileOperation> _SyncCollection = null;        
        protected MultiFileComparer<IFileComparer> _Comparer = null;

        public MultiKeyCollection<FileOperation> SyncCollection
        {
            get { return _SyncCollection; }
        }

        public DataSynchronizationhronizationScanner(string source, string destination, DataSynchronizationhorizationOption option)
        {
            if (Directory.Exists(source) == false || Directory.Exists(destination) == false) return;

            _SyncCollection = new MultiKeyCollection<FileOperation>(new string[] { "SourceFileName", "DestinationFileName" });
            ComponentLibrary<IFileComparer> libraries = new ComponentLibrary<IFileComparer>();
            _Comparer = new MultiFileComparer<IFileComparer>(libraries);

            _Source = source;
            _Destination = destination;
            _Options = option;
        }

        public void Sync()
        {            
            StartFolder("", -1);
        }

        protected void StartFolder(string folder, int level)
        {
            string sourcePath = Path.Combine(_Source, folder);
            string destinationPath = Path.Combine(_Destination, folder);

            if (Directory.Exists(sourcePath) == false) AddCreateFolderTask(sourcePath);
            if (Directory.Exists(destinationPath) == false) AddCreateFolderTask(destinationPath);

            if (Directory.Exists(sourcePath) == true) LoadFilesInFirstPath(sourcePath, destinationPath, false);
            if (_Options == DataSynchronizationhorizationOption.Both && Directory.Exists(destinationPath) == true) LoadFilesInFirstPath(destinationPath, sourcePath, true);

            if (Directory.Exists(sourcePath) == true)
            {
                foreach (string subfolder in Directory.GetDirectories(sourcePath))
                {
                    string shortfoldername = subfolder.GetLastPathName(level + 1);
                    StartFolder(shortfoldername, level + 1);
                }
            }

            if (_Options == DataSynchronizationhorizationOption.Both && Directory.Exists(destinationPath) == true)
            {
                foreach (string subfolder in Directory.GetDirectories(destinationPath))
                {
                    string shortfoldername = subfolder.GetLastPathName(level + 1);
                    StartFolder(shortfoldername, level + 1);
                }
            }
        }

        protected void LoadFilesInFirstPath(string sourcePath, string destinationPath, bool flip)
        {
            string[] files = Directory.GetFiles(sourcePath);
            foreach (string file in files)
            {
                string shortfilename = Path.GetFileName(file);
                string sourcefilename = Path.Combine(sourcePath, shortfilename);
                string destinationfilename = Path.Combine(destinationPath, shortfilename);

                bool sourceExist = false;
                bool destinationExist = false;

                sourceExist = File.Exists(sourcefilename); // Source must be exist
                destinationExist = File.Exists(destinationfilename);

                DataSynchronizationhronizationItemFile item = null;
                if (destinationExist == false)
                {
                    if (flip == false)
                        item = new DataSynchronizationhronizationItemFile(sourcefilename, destinationfilename, DataSynchronizationhronizationItemFileOption.DestinationCreate);
                    else
                        item = new DataSynchronizationhronizationItemFile(destinationfilename, sourcefilename, DataSynchronizationhronizationItemFileOption.SourceCreate);
                    
                }
                else
                {
                    FileInfo sourceInfo = new FileInfo(sourcefilename);
                    FileInfo destinationInfo = new FileInfo(destinationfilename);
                    int result = _Comparer.Compare(sourceInfo, destinationInfo);

                    DataSynchronizationhronizationItemFileOption option = DataSynchronizationhronizationItemFileOption.NoOperation;
                    if (result < 0)
                    {
                        if (flip == false) option = DataSynchronizationhronizationItemFileOption.SourceOverwrite;
                        else option = DataSynchronizationhronizationItemFileOption.DestinationOverwrite;
                    }
                    else if (result > 0)
                    {
                        if (flip == false) option = DataSynchronizationhronizationItemFileOption.DestinationOverwrite;
                        else option = DataSynchronizationhronizationItemFileOption.SourceOverwrite;
                    }

                    if (option != DataSynchronizationhronizationItemFileOption.NoOperation)
                    {                        
                        if (flip == false)
                            item = new DataSynchronizationhronizationItemFile(sourceInfo.FullName, destinationInfo.FullName, option);
                        else
                            item = new DataSynchronizationhronizationItemFile(destinationInfo.FullName, sourceInfo.FullName, option);
                    }                    
                }
                if (item != null) AddFileTask(item);
            }
        }

        public void AddCreateFolderTask(string folder)
        {
            DataSynchronizationhronizationItemFolder item = new DataSynchronizationhronizationItemFolder(folder, DataSynchronizationhronizationItemFolderOption.CreateFolder);
            DataSynchronizationhronizationItemFolder existingitem = (DataSynchronizationhronizationItemFolder) _SyncCollection.GetAddObject((FileOperation) item);
            if (existingitem.Option != item.Option)
            {
                existingitem.Option = item.Option;
            }
        }

        public void AddFileTask(DataSynchronizationhronizationItemFile item)
        {
            DataSynchronizationhronizationItemFile existingitem = (DataSynchronizationhronizationItemFile) _SyncCollection.GetAddObject((FileOperation) item);
            if (existingitem.Option != item.Option)
            {
                existingitem.Option = item.Option;
            }
        }
    }
}

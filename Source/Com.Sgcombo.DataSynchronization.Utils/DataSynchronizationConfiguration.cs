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
using System.Xml;
using System.Windows.Forms;
using Com.SGcombo.DataSynchronization.Utils;

namespace Com.SGcombo.DataSynchronization.Utils
{
    public class DataSynchronizationConfiguration
    {
        protected string _ConfigurationFileName = string.Empty;
        protected XmlDocument _XmlDoc = null;
        public DataSynchronizationConfiguration()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DataSynchronization");
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
            _ConfigurationFileName = Path.Combine(path, "conf.xml");
            if (File.Exists(_ConfigurationFileName) == false)
                CreateConfiguration();
            else
                LoadConfiguration();
        }

        protected void LoadConfiguration()
        {
            if (File.Exists(_ConfigurationFileName) == false) return;
            _XmlDoc = new XmlDocument();
            _XmlDoc.Load(_ConfigurationFileName);
        }

        public void DeleteConfiguration(int index)
        {
            XmlNodeList list = _XmlDoc.GetElementsByTagName("DataSynchronizationInstance");
            if (index >= 0 && index < list.Count)
            {
                XmlNode node = list[index];
                XmlNodeList list2 = _XmlDoc.GetElementsByTagName("DataSynchronization");
                if (list2.Count == 1)
                {
                    list2[0].RemoveChild(node);
                }
            }
        }

        public int ItemCount
        {
            get
            {
                XmlNodeList list = _XmlDoc.GetElementsByTagName("DataSynchronizationInstance");
                if (list == null || list.Count <= 0) return 0;
                return list.Count;
            }
        }

        public List<string> GetItem(int index)
        {
            List<string> result = new List<string>();
            XmlNodeList list = _XmlDoc.GetElementsByTagName("DataSynchronizationInstance");
            if (list == null || list.Count <= 0) return result;
            if (index >= 0 && index < list.Count)
            {
                result.Add(list[0].ChildNodes[0].ChildNodes[0].InnerText);
                result.Add(list[0].ChildNodes[1].ChildNodes[0].InnerText);
                result.Add(list[0].ChildNodes[2].ChildNodes[0].InnerText);
                result.Add(list[0].ChildNodes[3].ChildNodes[0].InnerText);
            }
            return result;
        }



        public void ModifyConfiguration(int index, string source, string destination, int option, bool auto)
        {
            List<ListViewItem> result = new List<ListViewItem>();
            XmlNodeList list = _XmlDoc.GetElementsByTagName("DataSynchronizationInstance");
            if (list == null || list.Count <= 0) return;
            if (index >= 0 && index < list.Count)
            {
                list[0].ChildNodes[0].ChildNodes[0].InnerText = source;
                list[0].ChildNodes[1].ChildNodes[0].InnerText = destination;
                list[0].ChildNodes[2].ChildNodes[0].InnerText = option.ToString();
                list[0].ChildNodes[3].ChildNodes[0].InnerText = auto ? "1" : "0";
            }
        }

        public void AddConfiguration(string source, string destination, int option, bool auto)
        {
            XmlElement parentNode = _XmlDoc.CreateElement("DataSynchronizationInstance");
            _XmlDoc.DocumentElement.PrependChild(parentNode);

            XmlElement sourceFolder = _XmlDoc.CreateElement("SourceFolder");
            XmlElement destFolder = _XmlDoc.CreateElement("DestinationFolder");
            XmlElement syncOption = _XmlDoc.CreateElement("SyncOption");
            XmlElement autoSync = _XmlDoc.CreateElement("Monitor");

            XmlText sourceFolderText = _XmlDoc.CreateTextNode(source);
            XmlText destFolderText = _XmlDoc.CreateTextNode(destination);
            XmlText syncOptionText = _XmlDoc.CreateTextNode(option.ToString());
            XmlText autoSyncText = _XmlDoc.CreateTextNode(auto ? "1" : "0");

            // append the nodes to the parentNode without the value
            parentNode.AppendChild(sourceFolder);
            parentNode.AppendChild(destFolder);
            parentNode.AppendChild(syncOption);
            parentNode.AppendChild(autoSync);

            // save the value of the fields into the nodes
            sourceFolder.AppendChild(sourceFolderText);
            destFolder.AppendChild(destFolderText);
            syncOption.AppendChild(syncOptionText);
            autoSync.AppendChild(autoSyncText);
        }


        public List<DataSynchronizationItem> GetDataSynchronizationItems()
        {
            List<DataSynchronizationItem> result = new List<DataSynchronizationItem>();
            XmlNodeList list = _XmlDoc.GetElementsByTagName("DataSynchronizationInstance");
            if (list == null || list.Count <= 0) return result;
            foreach (XmlNode node in list)
            {
                DataSynchronizationItem item = new DataSynchronizationItem();
                string src = node.ChildNodes[0].ChildNodes[0].InnerText;
                string dst = node.ChildNodes[1].ChildNodes[0].InnerText;
                src = PathExtension.GetLastPathName(src, 2);
                dst = PathExtension.GetLastPathName(dst, 2);

                item.sourceFolder = node.ChildNodes[0].ChildNodes[0].InnerText;
                item.destinationFolder = node.ChildNodes[1].ChildNodes[0].InnerText;
                item.SyncOption = node.ChildNodes[2].ChildNodes[0].InnerText;
                item.Monitor = node.ChildNodes[3].ChildNodes[0].InnerText;

                result.Add(item);
            }
            return result;
        }


        public List<ListViewItem> GetListViewItems()
        {
            List<ListViewItem> result = new List<ListViewItem>();
            XmlNodeList list = _XmlDoc.GetElementsByTagName("DataSynchronizationInstance");
            if (list == null || list.Count <= 0) return result;
            foreach (XmlNode node in list)
            {
                string src = node.ChildNodes[0].ChildNodes[0].InnerText;
                string dst = node.ChildNodes[1].ChildNodes[0].InnerText;
                src = PathExtension.GetLastPathName(src, 2);
                dst = PathExtension.GetLastPathName(dst, 2);
                ListViewItem item = new ListViewItem(new string[] { src, dst });
                result.Add(item);
            }
            return result;
        }

        public void Flush()
        {
            if (File.Exists(_ConfigurationFileName))
                File.Delete(_ConfigurationFileName);
            FileStream fs = new FileStream(_ConfigurationFileName, FileMode.CreateNew, FileAccess.Write);
            _XmlDoc.Save(fs);
            fs.Close();
        }

        protected void CreateConfiguration()
        {
            if (File.Exists(_ConfigurationFileName) == true) return;

            _XmlDoc = new XmlDocument();
            XmlDeclaration xmlDeclaration = _XmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            XmlElement rootNode = _XmlDoc.CreateElement("DataSynchronization");
            _XmlDoc.InsertBefore(xmlDeclaration, _XmlDoc.DocumentElement);
            _XmlDoc.AppendChild(rootNode);
        }
    }
}

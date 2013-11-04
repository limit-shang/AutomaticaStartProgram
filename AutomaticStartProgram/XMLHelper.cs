using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;

namespace AutomaticStartProgram
{
    /// <summary>
    /// 对Data.xml文件进行操作
    /// </summary>
    public class XMLHelper
    {
        /// <summary>
        /// Data.xml文件路径
        /// </summary>
        private static string dataXMLPath = System.Environment.CurrentDirectory + @"\Data.xml";

        /// <summary>
        /// 获取Data.xml文件的DataSet形式
        /// </summary>
        /// <returns>存储Data.xml的DataSet</returns>
        public static DataSet GetDataSetOfXmlDocument()
        {
            DataSet ds = new DataSet();
            ds.ReadXml(dataXMLPath);
            return ds;
        }

        /// <summary>
        /// 获取常用程序路径
        /// </summary>
        /// <returns>路径集合</returns>
        public static List<string> GetPaths()
        {
            List<string> pathsList = new List<string>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(dataXMLPath);
            XmlNodeList pathsNodeList = xmlDoc.SelectNodes("/Data/Paths/Path/Value");
            int i = 0;
            
            for (; i < pathsNodeList.Count; i++)
            {
                pathsList.Add(pathsNodeList.Item(i).InnerText);
            }            

            return pathsList;
        }

        /// <summary>
        /// 向Data.xml文件中添加新路径
        /// </summary>
        /// <param name="path">新路径</param>
        public static void InsertPathIntoXmlDocument(string newPathText)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(dataXMLPath);

            XmlNode root = xmlDoc.SelectSingleNode("Data");
            XmlNode paths = root.SelectSingleNode("Paths");
            XmlNode newPath = xmlDoc.CreateElement("Path");
            XmlNode value = xmlDoc.CreateElement("Value");

            value.InnerText = (newPathText);
            newPath.AppendChild(value);
            paths.AppendChild(newPath);

            xmlDoc.Save(dataXMLPath);
        }

        /// <summary>
        /// 修改工作状态信息
        /// </summary>
        /// <param name="index">要修改的工作状态信息行索引</param>
        /// <param name="state">工作状态</param>
        public static void UpdateJobStateInXmlDocument(int index, string state)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(dataXMLPath);
            XmlNode root = xmlDoc.SelectSingleNode("Data");
            XmlNode jobs = root.SelectSingleNode("Jobs");
            XmlNodeList jobsList = jobs.ChildNodes;
            XmlNode jobState = null;
            //List<ComboBox> comboBoxList = null;

            //jobsList[index].SelectSingleNode("State").InnerText = DataTableJobs.Rows[index][2].ToString();
            jobState = jobsList[index].SelectSingleNode("State");
            //jobState.InnerText = DataTableJobs.DefaultView.Table.Rows[index]["State"].ToString();
            //comboBoxList = GetChildObjects<ComboBox>(dataGridJobsView, "comboBoxJobState");
            jobState.InnerText = state;
            xmlDoc.Save(dataXMLPath);
        }

        /// <summary>
        /// 删除工作信息数据
        /// </summary>
        /// <param name="jobsIndexes">需要删除的工作信息的索引</param>
        public static void DeleteJobsInXmlDocument(int[] jobsIndexes)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(dataXMLPath);
            XmlNode root = xmlDoc.SelectSingleNode("Data");
            XmlNode jobs = root.SelectSingleNode("Jobs");
            XmlNodeList jobsList = jobs.ChildNodes;

            for (int i = 0; i < jobsIndexes.Length; i++)
            {
                jobs.RemoveChild(jobsList.Item(jobsIndexes[i] - i));
            }

            xmlDoc.Save(dataXMLPath);
        }

        /// <summary>
        /// 删除应用程序的路径信息
        /// </summary>
        /// <param name="pathsIndexs">需要删除路径的索引</param>
        public static void DeletePathsInXmlDocument(int[] pathsIndexes)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(dataXMLPath);
            XmlNode root = xmlDoc.SelectSingleNode("Data");
            XmlNode paths = root.SelectSingleNode("Paths");
            XmlNodeList pathsList = paths.ChildNodes;

            for (int i = 0; i < pathsIndexes.Length; i++)
            {
                paths.RemoveChild(pathsList.Item(pathsIndexes[i] - i));
            }

            xmlDoc.Save(dataXMLPath);
        }

        /// <summary>
        /// 向Data.Xml文件中添加新工作内容
        /// </summary>
        /// <param name="newJobText">新工作内容</param>
        public static void InsertJobIntoXmlDocument(string newJobText)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(dataXMLPath);

            XmlNode root = xmlDoc.SelectSingleNode("Data");
            XmlNode jobs = root.SelectSingleNode("Jobs");
            XmlNode newJob = xmlDoc.CreateElement("Job");
            XmlNode content = xmlDoc.CreateElement("Content");
            XmlNode state = xmlDoc.CreateElement("State");
            content.InnerText = newJobText;
            state.InnerText = "未开始";

            newJob.AppendChild(content);
            newJob.AppendChild(state);
            jobs.AppendChild(newJob);

            xmlDoc.Save(dataXMLPath);
        }

        /// <summary>
        /// 创建新的Data.xml文件
        /// </summary>
        public static void CreatePathsXmlDocument()
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "GB2312", null);
            doc.AppendChild(dec);
            //创建一个根节点（一级）
            XmlElement root = doc.CreateElement("Data");
            doc.AppendChild(root);
            //创建节点（二级）
            XmlNode paths = doc.CreateElement("Paths");
            XmlNode jobs = doc.CreateElement("Jobs");

            //创建节点（三级）
            XmlNode path = doc.CreateElement("Path");
            //创建节点（四级）
            XmlNode value = doc.CreateElement("Value");
            value.InnerText = @"C:\Users\Administrator\Desktop\周记.txt";
            path.AppendChild(value);
            paths.AppendChild(path);

            path = doc.CreateElement("Path");
            value = doc.CreateElement("Value");
            value.InnerText = @"C:\Users\Administrator\Desktop\日志记录.txt";
            path.AppendChild(value);
            paths.AppendChild(path);

            path = doc.CreateElement("Path");
            value = doc.CreateElement("Value");
            value.InnerText = @"C:\Users\Administrator\Desktop\tfs连接.txt";
            path.AppendChild(value);
            paths.AppendChild(path);

            path = doc.CreateElement("Path");
            value = doc.CreateElement("Value");
            value.InnerText = @"C:\Users\Administrator\Desktop\Microsoft Visual Studio 2010";
            path.AppendChild(value);
            paths.AppendChild(path);

            path = doc.CreateElement("Path");
            value = doc.CreateElement("Value");
            value.InnerText = @"C:\Users\Administrator\Desktop\Microsoft Visual Studio 2010";
            path.AppendChild(value);
            paths.AppendChild(path);

            path = doc.CreateElement("Path");
            value = doc.CreateElement("Value");
            value.InnerText = @"C:\Users\Administrator\Desktop\Microsoft Visual Studio 2010";
            path.AppendChild(value);
            paths.AppendChild(path);

            path = doc.CreateElement("Path");
            value = doc.CreateElement("Value");
            value.InnerText = @"E:\Program Files\360\360Chrome\360Chrome\Chrome\Application\360chrome.exe";
            path.AppendChild(value);
            paths.AppendChild(path);

            path = doc.CreateElement("Path");
            value = doc.CreateElement("Value");
            value.InnerText = @"E:\Program Files\Dict\YodaoDict.exe";
            path.AppendChild(value);
            paths.AppendChild(path);

            path = doc.CreateElement("Path");
            value = doc.CreateElement("Value");
            value.InnerText = @"E:\Program Files\Wiz\Wiz.exe";
            path.AppendChild(value);
            paths.AppendChild(path);

            path = doc.CreateElement("Path");
            value = doc.CreateElement("Value");
            value.InnerText = @"G:\Projects\G-工具\AutomaticStartProgram\AutomaticStartProgram.sln";
            path.AppendChild(value);
            paths.AppendChild(path);

            root.AppendChild(paths);
            root.AppendChild(jobs);
            doc.Save(dataXMLPath);
        }
    }
}

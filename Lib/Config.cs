using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;

namespace Lib
{
    public class Config : IDisposable
    {

        #region CONST

        const string varname_configuration = "CONFIGURATION";

        #endregion

        #region STRUCTURES

        public struct SNode
        {
            public string path;
            public string value;
            public string default_value;
        }

        #endregion

        #region VARIABLES

        private Thread thread;
        private bool thread_execution_flag = true;
        private bool disposedValue;

        #endregion

        #region PROPERTIES

        private string path_file;
        public string PathFile { get { return path_file; } set { path_file = value; } }


        private List<SNode> nodes = new List<SNode>();
        public List<SNode> Nodes { get { return nodes; } set { nodes = value; } }

        #endregion

        #region CONSTRUCTOR
        public Config()
        {
            try
            {
                thread = new Thread(new ThreadStart(Handler)) { IsBackground = true, Name = "Config" };
                thread.Start();
            }
            catch (Exception ex)
            {
                throw new Exception("Error creation object", ex);
            }
        }

        #endregion

        #region PUBLIC



        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region PRIVATE

        private void Handler()
        {
            while (thread_execution_flag)
            {
#if DEBUG
                if (DateTime.Now.Second % 5 == 0)
#else
                if (DateTime.Now.Second == 0)
#endif
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();

                        if (!File.Exists(path_file))
                        {
                            CreateTemplate(doc);
                            doc.Save(path_file);
                        }

                        doc.Load(path_file);


                        for (int i = 0; i < nodes.Count; i++)
                        {
                            string result = GetElementOrDefault(doc, $"/{varname_configuration}/{nodes[i].path}/".ToUpper(), nodes[i].default_value);
                            if (result != nodes[i].value)
                            {
                                SNode node = nodes[i];
                                node.value = result;
                                nodes[i] = node;
                                Logger.WriteMessage($"Config {nodes[i].path} = {nodes[i].value}");
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.WriteMessage("Error read nodes", ex);
                    }
                    Thread.Sleep(1000);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        private void CreateTemplate(XmlDocument doc)
        {
            try
            {
                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmlDeclaration, root);

                XmlElement element = doc.CreateElement(string.Empty, varname_configuration, string.Empty);
                doc.AppendChild(element);


            }
            catch (Exception ex)
            {
                throw new Exception("Error create template", ex);
            }
        }

        private XmlNode GetElement(XmlDocument doc, string xpath)
        {
            try
            {
                return makeXPath(doc, doc as XmlNode, xpath);
            }
            catch (Exception ex)
            {
                throw new Exception("Error get element", ex);
            }

        }
        private string GetElementOrDefault(XmlDocument doc, string xpath, string default_value)
        {
            try
            {
                XmlElement contentElement = (XmlElement)GetElement(doc, xpath);

                if (contentElement.InnerText == String.Empty)
                {
                    contentElement.InnerText = default_value;
                    Uri url = new Uri(doc.BaseURI);
                    doc.Save(url.AbsolutePath.Replace("%20", " "));
                }



                return contentElement.InnerText;
            }
            catch (Exception ex)
            {

                throw new Exception("Error get element or default", ex);
            }

        }
        private XmlNode makeXPath(XmlDocument doc, XmlNode parent, string xpath)
        {
            try
            {
                // grab the next node name in the xpath; or return parent if empty
                string[] partsOfXPath = xpath.Trim('/').Split('/');
                string nextNodeInXPath = partsOfXPath.First();
                if (string.IsNullOrEmpty(nextNodeInXPath))
                    return parent;

                // get or create the node from the name
                XmlNode node = parent.SelectSingleNode(nextNodeInXPath);
                if (node == null)
                    node = parent.AppendChild(doc.CreateElement(nextNodeInXPath));

                // rejoin the remainder of the array as an xpath expression and recurse
                string rest = String.Join("/", partsOfXPath.Skip(1).ToArray());
                return makeXPath(doc, node, rest);
            }
            catch (Exception ex)
            {

                throw new Exception("Error make X path", ex);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    thread_execution_flag = false;
                    thread.Join();
                    thread = null;
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                // TODO: установить значение NULL для больших полей
                disposedValue = true;
            }
        }



        #endregion

    }
}

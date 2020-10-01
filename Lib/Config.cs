using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;

namespace Lib
{
    public class Config
    {

        #region CONST

        const string varname_configuration = "CONFIGURATION";

        #endregion

        #region STRUCTURES

        private struct SNode
        {
            public string path;
            public string value;
            public string default_value;
        }

        #endregion

        #region PROPERTIES

        private string path_file;
        public string PathFile { get { return path_file; } set { path_file = value; } }


        private List<SNode> nodes = new List<SNode>();

        #endregion

        #region PUBLIC

        public void Add(string path, string default_value)
        {
            try
            {
                nodes.Add(new SNode() { path = path, default_value = default_value });
            }
            catch (Exception ex)
            {

                throw new Exception("Error add", ex);
            }
        }

        public string Get(string path)
        {
            try
            {
                return nodes.Find(x => x.path.Equals(path)).value;
            }
            catch (Exception ex)
            {
                throw new Exception("Error get", ex);
            }
        }

        public void Read()
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
                throw new Exception("Error read nodes", ex);
            }
        }


        #endregion

        #region PRIVATE


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

        #endregion

    }
}

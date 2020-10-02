using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;

namespace Lib
{
    public class XML
    {

        #region CONST

        const string varname_configuration = "CONFIGURATION";

        #endregion


        public class Node
        {

            private string path;
            public string Path { get { return path; } set { path = value; } }

            private string value;
            public string Value { get { return value; } set { if (value != this.value) { this.value = value; Logger.WriteMessage($"Config {path} = {this.value}"); }; } }

            private string default_value;
            public string DefaultValue { get { return default_value; } set { default_value = value; } }


        }


        #region PROPERTIES

        private string path;
        public string Path { get { return path; } set { path = value; } }

        #endregion

        #region VARIABLES

        private List<Node> nodes = new List<Node>();

        #endregion

        #region PUBLIC

        public Node AddNode(string path, string default_value)
        {
            try
            {
                Node node = new Node() { Path = path, DefaultValue = default_value };
                nodes.Add(node);
                return node;
            }
            catch (Exception ex)
            {
                throw new Exception("Error add", ex);
            }
        }
        public Node FindNode(string path)
        {
            try
            {
                return nodes.Find(x => x.Path.Equals(path));
            }
            catch (Exception ex)
            {
                throw new Exception("Error find node", ex);
            }
        }

        public bool ExistNode(string path)
        {
            try
            {
                return FindNode(path) != null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error check for exist", ex);
            }
        }





        public string GetValue(string path)
        {
            try
            {
                return nodes.Find(x => x.Path.Equals(path)).Value;
            }
            catch (Exception ex)
            {
                throw new Exception("Error get value", ex);
            }
        }

        public void ReadAllValues()
        {

            try
            {



                for (int i = 0; i < nodes.Count; i++)
                {

                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error read nodes", ex);
            }
        }

        public string ReadValue(string path, string default_value = null)
        {
            try
            {
                Node result = FindNode(path);

                if (result == null)
                {
                    result = AddNode(path, default_value);
                }

                XmlDocument doc = new XmlDocument();

                if (!File.Exists(this.path))
                {
                    CreateTemplate(doc);
                    doc.Save(this.path);
                }

                doc.Load(this.path);

                result.Value = GetElementOrDefault(doc, $"/{varname_configuration}/{path}/".ToUpper(), default_value);
                return result.Value;

            }
            catch (Exception ex)
            {

                throw new Exception("Error read node", ex);
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

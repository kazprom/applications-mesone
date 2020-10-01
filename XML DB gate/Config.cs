using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace XML_DB_gate
{
    class Config
    {

        private Logger logger = Logger.GetInstance();

        #region VARIABLE

        string path_to_file = $@"{Program.PathExeFolder}{Program.NameExeFile.Split('.')[0]}.xml";
        string path_to_file_xml = $@"{Program.PathExeFolder}{Path.DirectorySeparatorChar}Xml.xml";
        private object xmlName, xmlValue, xmlNameEndElement;

        #endregion

        #region ENUMS
        public enum e_DB_Type
        {
            Unknown = 0,
            Microsoft_SQL_Server = 1,
            MySQL = 2,
            PostgreSQL = 3
        }
        #endregion

        #region CONSTRUCTOR

        private static Config instance;
        public static Config GetInstance()
        {
            if (instance == null)
                instance = new Config();
            return instance;
        }

        public Config()
        {
            MainActions();
            Console.WriteLine(path_to_file_xml);
            Thread thread = new Thread(new ThreadStart(Handler)) { IsBackground = true, Name = "Config" };
            thread.Start();
        }

        #endregion

        #region PROPERTIES

        public DataTable DataTable { get { return table; } }

        List<string> bufferNames = new List<string>();

        #endregion

        #region CONST

        private const string varname_configuration = "CONFIGURATION";
        private const string varname_db_type = "DB_TYPE";
        const string varname_depth_log_day = "DEPTH_LOG_DAY";
        private const string varname_connection_string = "CONNECTION_STRING";

        const string varname_db_mssql = "microsoft sql server";
        const string varname_db_MySQL = "mysql";
        const string varname_db_postGreSQL = "postgresql";

        private const string varname_id = "id";

        private const string varname_system_type_int32 = "System.Int32";
        private const string varname_system_type_string = "System.String";

        #endregion

        #region PROPERTIES

        private bool flag = false;

        private DataTable table = new DataTable();
        public DataTable TableXML
        {
            get { return table; }
            set { table = value; }
        }

        private string connection_string = "Driver={mySQL ODBC 8.0 ANSI Driver};Server=myServerAddress;Option=131072;Stmt=;Database=myDataBase;User=myUsername;Password=myPassword;";
        public string ConnectionString { get { return connection_string; } private set { if (connection_string != value) connection_string = value; logger.WriteMessage(0, $"CONFIG {varname_connection_string} = {connection_string}"); } }

        private e_DB_Type db_type = e_DB_Type.Unknown;
        public e_DB_Type DB_Type { get { return db_type; } private set { if (db_type != value) { db_type = value; logger.WriteMessage(0, $"DB_TYPE {varname_db_type} = {db_type}"); } } }

        private int depth_log_day = 2;
        public int DepthLogDay { get { return depth_log_day; } private set { if (depth_log_day != value) depth_log_day = value; logger.WriteMessage(0, $"CONFIG {varname_depth_log_day} = {depth_log_day}"); } }

        private int id;
        public int ID { get { return id; } private set { if (id != value) id = value; } }

        #endregion

        #region ENUMS

        #endregion

        private void MainActions()
        {
            ReadData();
            ReadOutSideXML();

        }

        private void Handler()
        {
            while (true)
            {
                if (DateTime.Now.Second != 0)
                {
                    Thread.Sleep(100);
                }
                else
                {
                    MainActions();
                    Thread.Sleep(1000);
                }
            }
        }

        #region  Read outside XML

        private void ReadOutSideXML()
        {
            try
            {
                bool flag = false;
                int count = 0;
                XmlTextReader reader = new XmlTextReader(path_to_file);
                List<object> values = new List<object>();

                if (!flag)
                {
                    if (!File.Exists((path_to_file_xml)))
                    {
                        logger.WriteMessage(0, $"Error! Can`t find xml file from {path_to_file}");
                    }
                    else
                    {
                        XDocument doc = XDocument.Load(path_to_file_xml);

                        foreach (var name in doc.Root.DescendantNodes().OfType<XElement>().Select(x => x.Name).Distinct())
                        {
                            foreach (var root in doc.Root.Element(name).DescendantNodes().OfType<XElement>().Select(x => x.Name).Distinct())
                            {
                                xmlName = root;
                                DataColumnCollection columns = table.Columns;

                                count++;

                                if (!columns.Contains(xmlName.ToString()))
                                {
                                    CreateDataTable(xmlName);
                                }

                                // Console.WriteLine("<" + xmlName + ">");
                            }

                            break;
                        }

                        var elements = (from nn in doc.Descendants() select nn.Value).Skip(2).ToList();

                        EditDT(elements, count);

                        Console.WriteLine(table);
                        Console.WriteLine();

                        #region Old


                        //table.Rows.Add(elements[])


                        //foreach (var el in elements)
                        //{
                        //    if (xmlValue != el)
                        //    {
                        //        xmlValue = el;

                        //        values.Add(xmlValue);
                        //        Console.WriteLine("\n" + xmlValue);
                        //    }
                        //}

                        //for (int i = 0; i < elements.Count; i++)
                        //{
                        //    if (xmlValue != elements[i])
                        //    {
                        //        xmlValue = elements[i];

                        //        values.Add(xmlValue);
                        //        Console.WriteLine("\n" + xmlValue);
                        //    }

                        //}

                        //while (reader.Read())
                        //{
                        //    switch (reader.NodeType)
                        //    {

                        //        case XmlNodeType.Element:
                        //        {
                        //            xmlName = reader.Name;
                        //            DataColumnCollection columns = table.Columns;

                        //            if (!columns.Contains(xmlName.ToString()))
                        //            {
                        //                CreateDataTable(xmlName);
                        //            }

                        //            Console.WriteLine("<" + xmlName + ">");
                        //            break;
                        //        }
                        //        case XmlNodeType.Text:
                        //            {
                        //                if (xmlValue != reader.Value)
                        //                {
                        //                    xmlValue = reader.Value;

                        //                    //DataRow row = table.NewRow();
                        //                    //row[xmlName.ToString()] = xmlValue.ToString();

                        //                    values.Add(xmlValue);
                        //                    Console.WriteLine("\n" + xmlValue);
                        //                }

                        //                break;
                        //            }
                        //            //case XmlNodeType.EndElement:
                        //            //    {
                        //            //        xmlNameEndElement = reader.Name;
                        //            //        break;
                        //            //    }
                        //    }
                        //}


                        // EditDT(values);

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteMessage(0, ex.ToString());
            }
        }

        private void EditDT(List<string> values, int count)
        {
            //UPDATE and DELETE
            if (table.Rows.Count != 0)
            {


                foreach (DataRow row in table.Rows)
                {
                    foreach (DataColumn col in table.Columns)
                    {
                        var list = values.Find(x => x == row[col]);


                        Console.WriteLine($"{col.ColumnName} - {row[col]}");

                    }
                }

                Console.WriteLine();
            }

            if (table.Rows.Count == 0)
            {
                //Fill datatable
                for (int i = 0; i <= values.Count + 1; i++)
                {
                    table.Rows.Add(null, values[0], values[1], values[2], values[3]);
                    values = values.Skip(count + 1).ToList();

                    // buffer = table.Copy();
                }
            }
        }

        private void CreateID()
        {
            try
            {
                //ID
                DataColumn idColumn = new DataColumn(varname_id, Type.GetType(varname_system_type_int32));
                idColumn.AutoIncrement = true;
                idColumn.Unique = true;
                idColumn.AllowDBNull = false;
                idColumn.AutoIncrementSeed = 1;
                idColumn.AutoIncrementStep = 1;

                table.Columns.Add(idColumn);

                table.PrimaryKey = new DataColumn[] { table.Columns[varname_id] };
            }
            catch (Exception ex)
            {
                logger.WriteMessage(0, $"Exception! Can't create DataTable {ex}");
            }
        }

        private bool CreateDataTable(object values)
        {
            try
            {
                DataColumn idColumn = null;

                if (!flag)
                {
                    CreateID();
                    flag = true;
                }

                if (flag)
                {
                    DataColumn db_type_column = new DataColumn($"{values.ToString().ToLower()}", Type.GetType(varname_system_type_string));

                    if (!table.Columns.Contains(db_type_column.ColumnName))
                    {
                        table.Columns.Add(db_type_column);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteMessage(0, $"A column already belongs to this DataTable {ex}");
                return false;
            }
            return true;
        }

        #endregion

        #region read Config XML

        private void ReadData()
        {
            try
            {
                var document = new XmlDocument();
                if (!File.Exists(path_to_file))
                {
                    logger.WriteMessage(0, $"Config not found {path_to_file}");
                    XmlDeclaration xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
                    XmlElement root = document.DocumentElement;
                    document.InsertBefore(xmlDeclaration, root);

                    XmlElement element = document.CreateElement(string.Empty, varname_configuration, string.Empty);
                    document.AppendChild(element);

                    document.Save(path_to_file);
                }

                document.Load(path_to_file);

                string XMLpath = string.Empty;

                XMLpath = $"/{varname_configuration}/{varname_db_type}";

                string type_db = GetElementOrDefault(document, XMLpath, e_DB_Type.MySQL.ToString());

                switch (type_db.ToLower())
                {
                    case varname_db_mssql:
                        {
                            DB_Type = e_DB_Type.Microsoft_SQL_Server;
                            break;
                        }
                    case varname_db_MySQL:
                        {
                            DB_Type = e_DB_Type.MySQL;
                            break;
                        }
                    case varname_db_postGreSQL:
                        {
                            DB_Type = e_DB_Type.PostgreSQL;
                            break;
                        }
                    default:
                        {
                            DB_Type = e_DB_Type.Unknown;
                        }
                        break;
                }

                XMLpath = $"/{varname_configuration}/{varname_connection_string}/";
                connection_string = GetElementOrDefault(document, XMLpath, connection_string);

                XMLpath = $"/{varname_configuration}/{varname_depth_log_day}/";
                if (!int.TryParse(GetElementOrDefault(document, XMLpath, depth_log_day.ToString()), out depth_log_day))
                {
                    logger.WriteMessage(0, $"Error read XML value {XMLpath}");
                }

            }
            catch (Exception ex)
            {
                logger.WriteMessage(0, "Config error read data", ex);
            }
        }

        static private string GetElementOrDefault(XmlDocument doc, string xpath, string default_value)
        {
            XmlElement contentElement = (XmlElement)GetElement(doc, xpath);

            if (contentElement.InnerText == string.Empty)
            {
                contentElement.InnerText = default_value;
                Uri url = new Uri(doc.BaseURI);
                doc.Save(url.AbsolutePath);
            }
            return contentElement.InnerText;
        }

        static private XmlNode GetElement(XmlDocument doc, string xpath)
        {
            return makeXPath(doc, doc as XmlNode, xpath);
        }

        static private XmlNode makeXPath(XmlDocument doc, XmlNode parent, string xpath)
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

        #endregion
    }
}

using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace OPC_DB_gate_server
{
    class Database
    {

        #region VARIABLES

        private Lib.Database database = new Lib.Database();
        private Thread thread;
        private bool execution = true;

        #endregion

        #region PROPERTIES

        private Lib.Parameter<Lib.Database.EType> type;
        public Lib.Parameter<Lib.Database.EType> TYPE { get { return type; } }

        private Lib.Parameter<string> connection_string;
        public Lib.Parameter<string> CONNECTION_STRING { get { return connection_string; } }


        private Settings settings;
        private Clients clients;
        private Tags tags;
        private RT_values rt_values;
        private History history;


        #endregion



        #region CONSTRUCTOR

        public Database(Lib.Parameter<Lib.Database.EType> type,
                        Lib.Parameter<string> connection_string,
                        Settings settings,
                        Clients client,
                        Tags tags,
                        RT_values rt_values,
                        History history)
        {

            this.type = type;
            Type_ValueChanged(this.type.Value);
            this.type.ValueChanged += Type_ValueChanged;

            this.connection_string = connection_string;
            Connection_string_ValueChanged(this.connection_string.Value);
            this.connection_string.ValueChanged += Connection_string_ValueChanged;

            this.settings = settings;
            this.clients = client;
            this.tags = tags;
            this.rt_values = rt_values;
            this.history = history;
            

            ReadAction();
            thread = new Thread(new ThreadStart(Handler)) { IsBackground = true, Name = "Database" };
            thread.Start();

        }



        #endregion


        #region PRIVATE

        private void Handler()
        {
            while (execution)
            {
                try
                {

#if DEBUG
                    if (DateTime.Now.Second % 5 == 0)
#else
                    if(DateTime.Now.Second == 0)
#endif
                    {

                        ReadAction();
                        Thread.Sleep(1000);

                    }

                    if (DateTime.Now.Second % 3 == 0)
                    {

                        WriteAction();
                        Thread.Sleep(1000);
                    }



                }
                catch (Exception ex)
                {
                    Logger.WriteMessage("Error database", ex);
                }

                Thread.Sleep(100);
            }
        }

        private void ReadAction()
        {
            try
            {

                if (database != null)
                {
                    if (settings != null)
                        database.Read(settings.Source.Table);

                    if (clients != null)
                        database.Read(clients.Source.Table);

                    if (tags != null)
                        database.Read(tags.Source.Table);

                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error read", ex);
            }
        }

        private void WriteAction()
        {

            try
            {

                if (database != null)
                {
                    if (rt_values != null)
                        database.Write(rt_values.Source.Table, false, true);


                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error write", ex);
            }

        }


        private void Connection_string_ValueChanged(string value)
        {
            database.ConnectionString = value;
        }

        private void Type_ValueChanged(Lib.Database.EType value)
        {
            database.Type = value;
        }


        #endregion





    }
}

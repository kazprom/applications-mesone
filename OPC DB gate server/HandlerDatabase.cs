using Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace OPC_DB_gate_server
{
    class HandlerDatabase
    {

        #region VARIABLES

        private Thread thread;
        private bool execution = true;

        private Settings settings;
        private Clients clients;
        private Tags tags;
        private RT_values rt_values;
        private History history;
        private Application application;
        private Diagnostics diagnostics;

        #endregion

        #region PROPERTIES

        private Lib.Parameter<Lib.Database.EType> type;
        public Lib.Parameter<Lib.Database.EType> TYPE { get { return type; } }

        private Lib.Parameter<string> connection_string;
        public Lib.Parameter<string> CONNECTION_STRING { get { return connection_string; } }


        private Lib.Database database = new Lib.Database();
        public Lib.Database Database { get { return database; } }


        #endregion



        #region CONSTRUCTOR

        public HandlerDatabase(Lib.Parameter<Lib.Database.EType> type,
                        Lib.Parameter<string> connection_string,
                        Settings settings,
                        Clients client,
                        Tags tags,
                        RT_values rt_values,
                        History history,
                        Application application,
                        Diagnostics diagnostics)
        {

            try
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
                this.application = application;
                this.diagnostics = diagnostics;

                ReadAction();
                thread = new Thread(new ThreadStart(Handler)) { IsBackground = true, Name = "Database" };
                thread.Start();
            }
            catch (Exception ex)
            {
                throw new Exception("Error constructor", ex);
            }

            

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
                    Lib.Message.Make("Error database", ex);
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
                        database.Read(settings.Source);

                    if (clients != null)
                        database.Read(clients.Source);

                    if (tags != null)
                        database.Read(tags.Source);

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
                    {
                        lock (database) lock (rt_values)
                            {
                                database.Write(rt_values.Source, false, true);
                            }
                    }

                    if (history != null)
                    {
                        lock (database) lock (history)
                            {
                                database.Write(history.Source);
                                history.Clear();
                            }
                    }

                    if(application != null)
                    {
                        lock (database) lock (application)
                            {
                                database.Write(application.Source, false, true);
                            }
                    }

                    if (diagnostics != null)
                    {
                        lock (database) lock (diagnostics)
                            {
                                database.Write(diagnostics.Source, false, true);
                            }
                    }

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

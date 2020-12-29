using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using LibMESone.Tables.Core;
using System.Timers;

namespace LibMESone
{
    public class CCORE<SrvCustom> : CSrvDB where SrvCustom : CCUSTOM, new()
    {

        #region PROPERTIES

        public IEnumerable<CServiceType> TServiceTypes { get; set; }

        public IEnumerable<CService> TServices { get; set; }

        public IEnumerable<CDatabase> TDatabases { get; set; }

        public IEnumerable<CHost> THosts { get; set; }

        public CServiceDiagnostic TServiceDiagnostic { get; set; }

        public NLog.Targets.FileTarget Target_TextLog { get; set; }

        #endregion

        #region CONTRUCTOR

        public CCORE()
        {
            //Id = 0;
            //Name = "CORE";

            Logger = NLog.LogManager.GetLogger("CORE");

            try
            {
                TServiceDiagnostic = new CServiceDiagnostic() { Version = Lib.Common.AppVersion() };

                Target_TextLog = (NLog.Targets.FileTarget)NLog.LogManager.Configuration.FindTargetByName("file");

            }
            catch (Exception ex)
            {
                if (Logger != null) Logger.Error(ex);
            }
        }

        #endregion



        #region PRIVATES

        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {

            try
            {
                if (DB != null && DB.State == Lib.CDatabase.EState.Connected)
                {

                    //----------read---------------

                    switch (DB.CompareTableSchema<CServiceType>(CServiceType.TableName))
                    {
                        case true:
                            TServiceTypes = DB.WhereRead<CServiceType>(CServiceType.TableName, new { Guid = Lib.Common.AppGUID() });
                            break;

                    }

                    if (TServiceTypes != null && TServiceTypes.Count() == 0)
                    {
                        Logger.Warn("Can't find id of service");
                    }
                    else if (TServiceTypes != null)
                    {
                        ulong id = TServiceTypes.First().Id;

                        switch (DB.CompareTableSchema<CService>(CService.TableName))
                        {
                            case true:
                                TServiceDiagnostic.Service_types_id = id;

                                TServices = DB.WhereRead<CService>(CService.TableName, new
                                {
                                    Service_types_id = id,
                                    Enabled = true
                                });

                                break;

                        }

                    }


                    switch (DB.CompareTableSchema<CDatabase>(CDatabase.TableName))
                    {
                        case true:
                            TDatabases = DB.WhereRead<CDatabase>(CDatabase.TableName, new { Enabled = true });
                            break;
                    }



                    switch (DB.CompareTableSchema<CHost>(CHost.TableName))
                    {
                        case true:
                            THosts = DB.WhereRead<CHost>(CHost.TableName, new { Enabled = true });
                            break;
                    }



                    if (TServices != null && TDatabases != null && THosts != null)
                    {

                        var data = from db in TDatabases
                                   join host in THosts on db.Hosts_id equals host.Id
                                   join service in TServices on db.Id equals service.Databases_id
                                   select new
                                   {
                                       db.Id,
                                       service.Name,
                                       DBprops = new Dictionary<string, string>()
                                       {
                                           { Lib.CDatabase.EPropKeys.Driver.ToString(), db.Driver},
                                           { Lib.CDatabase.EPropKeys.Host.ToString(), host.Ip },
                                           { Lib.CDatabase.EPropKeys.Port.ToString(), db.Port.ToString() },
                                           { Lib.CDatabase.EPropKeys.Charset.ToString(), db.Charset },
                                           { Lib.CDatabase.EPropKeys.BaseName.ToString(), db.Database },
                                           { Lib.CDatabase.EPropKeys.User.ToString(), db.Username },
                                           { Lib.CDatabase.EPropKeys.Password.ToString(), db.Password }
                                       }
                                   }
                                   ;


                        Dictionary<ulong, Dictionary<string, object>> children_props = data.ToDictionary(o => o.Id,
                                                                                                         o => o.
                                                                                                              GetType().
                                                                                                              GetProperties().ToDictionary(z => z.Name,
                                                                                                                                           z => z.GetValue(o)));

                        CUD<SrvCustom>(children_props);

                        //----------write---------------

                        TServiceDiagnostic.Sys_ts = DateTime.Now;
                        DB.Update(CServiceDiagnostic.TableName, TServiceDiagnostic);

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            base.Timer_Handler(sender, e);
        }

        public void LoadSettingFromConfigFile(CConfigFile config)
        {
            try
            {
                DBprops.Clear();

                DBprops.Add(Lib.CDatabase.EPropKeys.Driver.ToString(), config.DB_Driver);
                DBprops.Add(Lib.CDatabase.EPropKeys.Host.ToString(), config.DB_Host);
                DBprops.Add(Lib.CDatabase.EPropKeys.Port.ToString(), config.DB_Port.ToString());
                DBprops.Add(Lib.CDatabase.EPropKeys.Charset.ToString(), config.DB_Charset);
                DBprops.Add(Lib.CDatabase.EPropKeys.BaseName.ToString(), config.DB_BaseName);
                DBprops.Add(Lib.CDatabase.EPropKeys.User.ToString(), config.DB_User);
                DBprops.Add(Lib.CDatabase.EPropKeys.Password.ToString(), config.DB_Password);

                DB.LoadSettings(DBprops);

                if (Target_TextLog != null)
                    Target_TextLog.MaxArchiveFiles = (int)config.LOG_DepthDay;


            }
            catch (Exception ex)
            {
                if (Logger != null) Logger.Error(ex);
            }

        }


        #endregion

    }
}

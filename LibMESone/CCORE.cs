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

        #region CONSTANTS

#if DEBUG
        private const int period = 5000;
#else
        private const int period = 60000;
#endif

        #endregion



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
            Name = "CORE";

            try
            {
                TServiceDiagnostic = new CServiceDiagnostic() { Version = Lib.Common.AppVersion() };
                CycleRate = period;

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
                if (Database != null)
                {

                    //----------read---------------

                    switch (Database.CompareTableSchema<CServiceType>(CServiceType.TableName))
                    {
                        case null:
                        case false:
                            return;

                    }

                    TServiceTypes = Database.WhereRead<CServiceType>(CServiceType.TableName, new { Guid = Lib.Common.AppGUID() });


                    if (TServiceTypes == null || TServiceTypes.Count() == 0)
                    {
                        Logger.Warn("Can't find id of service");
                        return;
                    }

                    switch (Database.CompareTableSchema<CService>(CService.TableName))
                    {
                        case null:
                        case false:
                            return;

                    }
                    ulong id = TServiceTypes.First().Id;
                    TServiceDiagnostic.Service_types_id = id;

                    TServices = Database.WhereRead<CService>(CService.TableName, new
                    {
                        Service_types_id = id,
                        Enabled = true
                    });


                    switch (Database.CompareTableSchema<CDatabase>(CDatabase.TableName))
                    {
                        case null:
                        case false:
                            return;
                    }

                    TDatabases = Database.WhereRead<CDatabase>(CDatabase.TableName, new { Enabled = true });


                    switch (Database.CompareTableSchema<CHost>(CHost.TableName))
                    {
                        case null:
                        case false:
                            return;
                    }

                    THosts = Database.WhereRead<CHost>(CHost.TableName, new { Enabled = true });


                    if (TServices != null && TDatabases != null && THosts != null)
                    {

                        var data = from db in TDatabases
                                   join host in THosts on db.Hosts_id equals host.Id
                                   join service in TServices on db.Id equals service.Databases_id
                                   select new 
                                   {
                                       Id = db.Id,
                                       Name = service.Name,
                                       Database = db.Database,
                                       Driver = db.Driver,
                                       Host = host.Ip,
                                       Port = db.Port,
                                       Charset = db.Charset,
                                       Username = db.Username,
                                       Password = db.Password
                                   }
                                   ;

                        IDictionary<ulong, IDictionary<string, object>> children_props = data.ToDictionary<ulong, Dictionary<string, object>>( o => o.Id, o => o.);

                        CUD<SrvCustom>(children_props);


                        //----------write---------------

                        TServiceDiagnostic.Sys_ts = DateTime.Now;
                        Database.Update(CServiceDiagnostic.TableName, TServiceDiagnostic);

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

                var props = new Dictionary<string, object>();

                props.Add(Lib.CDatabase.EPropKeys.Driver.ToString(), config.DB_Driver);
                props.Add(Lib.CDatabase.EPropKeys.Host.ToString(), config.DB_Host);
                props.Add(Lib.CDatabase.EPropKeys.Port.ToString(), config.DB_Port);
                props.Add(Lib.CDatabase.EPropKeys.Charset.ToString(), config.DB_Charset);
                props.Add(Lib.CDatabase.EPropKeys.BaseName.ToString(), config.DB_BaseName);
                props.Add(Lib.CDatabase.EPropKeys.User.ToString(), config.DB_User);
                props.Add(Lib.CDatabase.EPropKeys.Password.ToString(), config.DB_Password);


                Database.LoadSettings(props);

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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using LibMESone.Tables.Core;
using LibMESone.Structs;
using System.Timers;

namespace LibMESone
{
    public class CSrvCORE<SrvCustom> : CCyclycService where SrvCustom : CSrvCUSTOM, new()
    {

        #region CONSTANTS

#if DEBUG
        private const int period = 5000;
#else
        private const int period = 60000;
#endif

        #endregion

        #region VARIABLES

        private Lib.Database database;

        #endregion

        #region PROPERTIES

        public IEnumerable<CServiceType> TServiceTypes { get; set; }

        public IEnumerable<CService> TServices { get; set; }

        public IEnumerable<CDatabase> TDatabases { get; set; }

        public IEnumerable<CHost> THosts { get; set; }

        public CServiceDiagnostic TServiceDiagnostic { get; set; }

        #endregion

        #region CONTRUCTOR

        public CSrvCORE()
        {
            Logger = NLog.LogManager.GetLogger("CORE");

            try
            {
                TServiceDiagnostic = new CServiceDiagnostic() { Version = Lib.Common.AppVersion() };
                CycleRate = period;

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        #endregion



        #region PRIVATES

        public override void Timer_Handler(object sender, ElapsedEventArgs e)
        {

            try
            {
                if (database != null)
                {

                    //----------read---------------

                    switch (database.CompareTableSchema<CServiceType>(CServiceType.TableName))
                    {
                        case null:
                        case false:
                            return;

                    }

                    TServiceTypes = database.WhereRead<CServiceType>(CServiceType.TableName, new { Guid = Lib.Common.AppGUID() });


                    if (TServiceTypes == null || TServiceTypes.Count() == 0)
                    {
                        Logger.Warn("Can't find id of service");
                        return;
                    }

                    switch (database.CompareTableSchema<CService>(CService.TableName))
                    {
                        case null:
                        case false:
                            return;

                    }
                    ulong id = TServiceTypes.First().Id;
                    TServiceDiagnostic.Service_types_id = id;

                    TServices = database.WhereRead<CService>(CService.TableName, new
                    {
                        Service_types_id = id,
                        Enabled = true
                    });


                    switch (database.CompareTableSchema<CDatabase>(CDatabase.TableName))
                    {
                        case null:
                        case false:
                            return;
                    }

                    TDatabases = database.WhereRead<CDatabase>(CDatabase.TableName, new { Enabled = true });


                    switch (database.CompareTableSchema<CHost>(CHost.TableName))
                    {
                        case null:
                        case false:
                            return;
                    }

                    THosts = database.WhereRead<CHost>(CHost.TableName, new { Enabled = true });


                    if (TServices != null && TDatabases != null && THosts != null)
                    {

                        IEnumerable<CSetCUSTOM> settings = (IEnumerable<CSetCUSTOM>)(
                                                                            from db in TDatabases
                                                                            join host in THosts on db.Hosts_id equals host.Id
                                                                            join service in TServices on db.Id equals service.Databases_id
                                                                            select new CSetCUSTOM()
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
                                                                          );

                        CUD<SrvCustom>(settings);


                        //----------write---------------

                        TServiceDiagnostic.Sys_ts = DateTime.Now;
                        database.Update(CServiceDiagnostic.TableName, TServiceDiagnostic);

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
                CSetCORE setting = new CSetCORE()
                {
                    DB_Driver = config.DB_Driver,
                    DB_Host = config.DB_Host,
                    DB_Port = config.DB_Port,
                    DB_Charset = config.DB_Charset,
                    DB_BaseName = config.DB_BaseName,
                    DB_User = config.DB_User,
                    DB_Password = config.DB_Password,

                    LOG_DepthDay = config.LOG_DepthDay
                };

                LoadSetting(setting);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }


        public override void LoadSetting(ISetting setting)
        {
            try
            {
                CSetCORE _setting = setting as CSetCORE;

                ID = _setting.Id;
                Name = "Core";


                if (database == null) database = new Lib.Database(0);

                database.LoadSettings(_setting.DB_Driver,
                                      _setting.DB_Host,
                                      _setting.DB_Port,
                                      _setting.DB_Charset,
                                      _setting.DB_BaseName,
                                      _setting.DB_User,
                                      _setting.DB_Password);

                NLog.Targets.FileTarget target = (NLog.Targets.FileTarget)NLog.LogManager.Configuration.FindTargetByName("file");
                if (target != null)
                    target.MaxArchiveFiles = (int)_setting.LOG_DepthDay;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


        }

        /*
        private class ServicesDatabasesHosts : DatabasesHosts
        {

            public string Name { get; set; }


        }

        private class DatabasesHosts : Tables.BaseID
        {

            public string Database { get; set; }

            public string Driver { get; set; }

            public string Host { get; set; }

            public uint Port { get; set; }

            public string Charset { get; set; }

            public string Username { get; set; }

            public string Password { get; set; }


        }
        */

        #endregion

    }
}

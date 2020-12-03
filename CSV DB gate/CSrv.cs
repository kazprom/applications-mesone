using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSV_DB_gate
{
    public class CSrv : LibDBgate.CSrvCUSTOM
    {

        #region PROPERTIES

        public IEnumerable<Tables.CConverter> TConverters { get; set; }

        public IEnumerable<Tables.CField> TFields { get; set; }

        #endregion


        #region PUBLICS

        public override void Timer_Handler(object state)
        {

            base.Timer_Handler(state);

            try
            {

                if (Database != null)
                {

                    switch (Database.CheckExistTable(Tables.CConverter.TableName))
                    {
                        case null:
                        case false:
                            return;
                    }

                    switch (Database.CompareTableSchema<Tables.CConverter>(Tables.CConverter.TableName))
                    {
                        case null:
                        case false:
                            return;
                    }

                    TConverters = Database.WhereRead<Tables.CConverter>(Tables.CConverter.TableName, new { Enabled = true });

                    switch (Database.CheckExistTable(Tables.CField.TableName))
                    {
                        case null:
                        case false:
                            return;
                    }

                    switch (Database.CompareTableSchema<Tables.CField>(Tables.CField.TableName))
                    {
                        case null:
                        case false:
                            return;
                    }

                    TFields = Database.WhereRead<Tables.CField>(Tables.CField.TableName, new { Enabled = true });

                    IEnumerable<Structs.CSetConverter> settings = default;

                    if (TConverters != null && TFields != null)
                    {
                        settings = from convs in TConverters
                                   where convs.Enabled == true
                                   select new Structs.CSetConverter()
                                   {
                                       Id = convs.Id,
                                       Name = convs.Name,
                                       File_path = convs.File_path,
                                       File_delete = convs.File_delete,
                                       Table_clear = convs.Table_clear,
                                       Start_timestamp = convs.Start_timestamp,
                                       Frequency_sec = convs.Frequency_sec,
                                       Timeout_sec = convs.Timeout_sec,
                                       Fields = from fields in TFields
                                                where fields.Enabled == true && fields.Converters_id == convs.Id
                                                select new Structs.CField()
                                                {
                                                    NameSource = fields.Name_src,
                                                    NameDestination = fields.Name_dst,
                                                    DataType = Type.GetType($"System.{fields.Data_type}", false, true)
                                                }
                                   };
                    }

                    CUD<CConverter>(settings);

                }

                /*
                if (Database != null)
                {

                    IEnumerable<Structs.File> files = null;
                    if (Database.CompareTableSchema<Structs.File>(Structs.File.TableName))
                        files = Database.WhereRead<Structs.File>(Structs.File.TableName, new { Enabled = true });

                    if (files != null)
                    {

                        IEnumerable<ulong> fresh_ids = files.Select(x => (ulong)x.Id);
                        IEnumerable<ulong> existing_ids = this.SubServices.Keys;

                        IEnumerable<ulong> waste = existing_ids.Except(fresh_ids);
                        IEnumerable<ulong> modify = fresh_ids.Intersect(existing_ids);
                        IEnumerable<ulong> missing = fresh_ids.Except(existing_ids);

                        foreach (ulong point_id in waste)
                        {
                            Client srv = (Client)this.Clients[point_id];
                            srv.Dispose();
                            this.Clients.Remove(point_id);
                        }

                        foreach (ulong point_id in modify)
                        {
                            Structs.Client set_point = clients.First(x => x.Id == point_id);
                            Client srv = (Client)Clients[point_id];

                            srv.LoadSettings(set_point.Name,
                                             set_point.Cpu_type,
                                             set_point.Ip,
                                             set_point.Port,
                                             set_point.Rack,
                                             set_point.Slot);
                        }

                        foreach (ulong point_id in missing)
                        {
                            Structs.Client set_point = clients.First(x => x.Id == point_id);
                            Client inst_point = new Client(this, point_id);

                            inst_point.LoadSettings(set_point.Name,
                                                    set_point.Cpu_type,
                                                    set_point.Ip,
                                                    set_point.Port,
                                                    set_point.Rack,
                                                    set_point.Slot);

                            Clients.Add(set_point.Id, inst_point);
                        }
                    }
                    else
                    {
                        foreach (Client client in Clients.Values)
                        {
                            client.Dispose();
                        }
                        Clients.Clear();
                    }
                }
                */
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


        }

        #endregion



    }
}

using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lib
{
    public class CChild
    {


        #region EVENTS

        public delegate void LoggerMakedNotify(Logger logger);  // delegate
        public event LoggerMakedNotify LoggerMaked; // event

        #endregion

        private Logger logger = LogManager.GetCurrentClassLogger();
        public virtual Logger Logger
        {
            get { return logger; }
            set
            {
                if (!Equals(logger, value))
                {
                    logger = value;
                    LoggerMaked?.Invoke(logger);
                }
            }
        }

        public virtual CParent Parent { get; set; }

        private ulong? id;
        public virtual ulong Id
        {
            get { return id != null ? (ulong)id : default; }
            set
            {
                if (!Equals(id, value))
                {
                    id = value;

                    if (Parent != null && Parent.Logger != null)
                    {
                        Logger = LogManager.GetLogger($"{Parent.Logger.Name}.{Id}");
                    }
                    else
                    {
                        Logger = LogManager.GetLogger($"{Id}");
                    }

                    Logger.Info($"Id = {id}");

                    LoggerMaked?.Invoke(Logger);
                }


            }
        }

        public string Name { get; set; }

    }
}

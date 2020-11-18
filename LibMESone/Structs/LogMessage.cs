using Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibMESone.Structs
{
    public class LogMessage : LibMESone.Structs.BaseID
    {

        public const string TablePrefix = "l_";

        [Field(TYPE = Field.Etype.TimeStamp, SIZE = 3, NN = true)]
        public DateTime Timestamp { get; set; }

        [Field(TYPE = Field.Etype.Text, NN = true)]
        public string Message { get; set; }

        public static string GetTableName(DateTime ts)
        {
            return $"{TablePrefix}{ts:yyyy_MM_dd}";
        }

        public static DateTime GetTimeStamp(string name)
        {
            DateTime result = default;

            try
            {
                string[] parts = name.Split('_');
                if (parts.Length != 4)
                    throw new Exception("Wrong name format");

                result = new DateTime(int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]));

            }
            catch (Exception ex)
            {
                throw new Exception("Error convert name to timestamp", ex);
            }

            return result;
        }

    }
}

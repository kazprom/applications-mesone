using Lib;
using System;

namespace LibMESone.Tables.Custom
{
    public class CLogMessage : CBaseID
    {

        public const string TablePrefix = "l_";

        [Field(TYPE = Field.EDoctrine.DateTime, SIZE = 3, NN = true)]
        public DateTime Timestamp { get; set; }

        [Field(TYPE = Field.EDoctrine.Text, NN = true)]
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

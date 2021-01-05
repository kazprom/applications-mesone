using Lib;
using System;

namespace LibPlcDBgate.Tables
{
    public class CHisValue : LibMESone.Tables.CBaseID
    {

        public const string TablePrefix = "h_";

        [Field(TYPE = Field.EDoctrine.UnsignedBigInteger, NN = true)]
        public ulong Tags_id { get; set; }

        [Field(TYPE = Field.EDoctrine.DateTime, SIZE = 3, NN = true)]
        public DateTime Timestamp { get; set; }

        [Field(TYPE = Field.EDoctrine.Binary, SIZE = 8)]
        public byte[] Value { get; set; }

        [Field(TYPE = Field.EDoctrine.UnsignedSmallInteger, NN = true)]
        public byte Quality { get; set; }


        public static string GetTableName(DateTime ts)
        {
            return $"{TablePrefix}{ts:yyyy_MM_dd_HH}";
        }

        public static DateTime GetTimeStamp(string name)
        {
            DateTime result = default;

            try
            {
                string[] parts = name.Split('_');
                if (parts.Length != 5)
                    throw new Exception("Wrong name format");

                result = new DateTime(int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]), int.Parse(parts[4]), 0, 0);

            }
            catch (Exception ex)
            {
                throw new Exception("Error convert name to timestamp", ex);
            }

            return result;
        }

    }
}

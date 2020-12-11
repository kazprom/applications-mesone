using System;
using System.Collections.Generic;
using System.Text;

namespace Lib
{
    public class Field : Attribute
    {

        public enum EDoctrine
        {
            None = 0,
            SmallInt,
            Integer,
            BigInt,
            Decimal,
            Float,
            String,
            ASCII_String,
            Text,
            Binary,
            Blob,
            Boolean,
            Date,
            DateTime,
            DateTimeZ,
            Time,
            Array
        }


        /// <summary>
        /// database type
        /// </summary>
        public EDoctrine TYPE;

        /// <summary>
        /// database lenght
        /// </summary>
        public uint SIZE;

        /// <summary>
        /// primary key
        /// </summary>
        public bool PK = false;

        /// <summary>
        /// auto increment
        /// </summary>
        public bool AI = false;

        /// <summary>
        /// not null
        /// </summary>
        public bool NN = false;

        /// <summary>
        /// unique
        /// </summary>
        public bool UQ = false;


        public string GetType_MySQL()
        {
            string result = "";

            switch (TYPE)
            {
                case EDoctrine.SmallInt:
                    result = "SMALLINT UNSIGNED"; break;
                case EDoctrine.Integer:
                    result = "INT UNSIGNED"; break;
                case EDoctrine.BigInt:
                    result = "BIGINT UNSIGNED"; break;
                case EDoctrine.Decimal:
                    throw new Exception();
                case EDoctrine.Float:
                    //result = "DOUBLE PRECISION UNSIGNED"; break;
                    result = "DOUBLE UNSIGNED"; break;
                case EDoctrine.String:
                    result = "VARCHAR()"; break;
                case EDoctrine.ASCII_String:
                    throw new Exception();
                case EDoctrine.Text:
                    result = "TEXT"; break;
                case EDoctrine.Binary:
                    throw new Exception();
                case EDoctrine.Blob:
                    throw new Exception();
                case EDoctrine.Boolean:
                    result = "TINYINT(1)"; break;
                case EDoctrine.Date:
                    throw new Exception();
                case EDoctrine.DateTime:
                    result = "DATETIME()"; break;
                case EDoctrine.DateTimeZ:
                    throw new Exception();
                case EDoctrine.Time:
                    throw new Exception();
                case EDoctrine.Array:
                    throw new Exception();
            }

            if (result.Contains("()"))
            {
                if (SIZE > 0)
                {
                    result = result.Replace("()", $"({SIZE})");
                }
                else
                {
                    if (TYPE == EDoctrine.String)
                        result = result.Replace("()", $"(255)");
                    else
                        result = result.Replace("()", $"");
                }
            }

            return result;
        }

        public Type GetType_System()
        {
            Type result = null;

            switch (TYPE)
            {
                case EDoctrine.SmallInt:
                    result = typeof(short);
                    break;
                case EDoctrine.Integer:
                    result = typeof(int);
                    break;
                case EDoctrine.BigInt:
                    result = typeof(long);
                    break;
                case EDoctrine.Decimal:
                    result = typeof(string);
                    break;
                case EDoctrine.Float:
                    result = typeof(float);
                    break;
                case EDoctrine.String:
                    result = typeof(string);
                    break;
                case EDoctrine.ASCII_String:
                    result = typeof(string);
                    break;
                case EDoctrine.Text:
                    result = typeof(string);
                    break;
                case EDoctrine.Binary:
                    break;
                case EDoctrine.Blob:
                    break;
                case EDoctrine.Boolean:
                    result = typeof(bool);
                    break;
                case EDoctrine.Date:
                case EDoctrine.DateTime:
                case EDoctrine.DateTimeZ:
                case EDoctrine.Time:
                    result = typeof(DateTime);
                    break;
                case EDoctrine.Array:
                    break;
            }

            return result;
        }

        public static EDoctrine? TypeParce(string value)
        {
            EDoctrine result = default;
            if (Enum.TryParse<EDoctrine>(value, true, out result))
                return result;
            else
                return null;
        }

    }
}

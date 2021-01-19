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

            BigIncrements,
            BigInteger,
            Binary,
            Boolean,
            Char,
            DateTimeTz,
            DateTime,
            Date,
            Decimal,
            Double,
            Enum,
            Float,
            ForeignId,
            GeometryCollection,
            Geometry,
            Id,
            Increments,
            Integer,
            IpAddress,
            Json,
            Jsonb,
            LineString,
            LongText,
            MacAddress,
            MediumIncrements,
            MediumInteger,
            MediumText,
            Morphs,
            MultiLineString,
            MultiPoint,
            MultiPolygon,
            NullableMorphs,
            NullableTimestamps,
            NullableUuidMorphs,
            Point,
            Polygon,
            RememberToken,
            Set,
            SmallIncrements,
            SmallInteger,
            SoftDeletesTz,
            SoftDeletes,
            String,
            Text,
            TimeTz,
            Time,
            TimestampTz,
            Timestamp,
            TimestampsTz,
            Timestamps,
            TinyIncrements,
            TinyInteger,
            UnsignedBigInteger,
            UnsignedDecimal,
            UnsignedInteger,
            UnsignedMediumInteger,
            UnsignedSmallInteger,
            UnsignedTinyInteger,
            UuidMorphs,
            Uuid,
            Year

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
                case EDoctrine.None:
                    break;
                case EDoctrine.BigIncrements:
                    break;
                case EDoctrine.BigInteger:
                    result = "BIGINT"; break;
                case EDoctrine.Binary:
                    result = "BINARY()"; break;
                case EDoctrine.Boolean:
                    result = "TINYINT(1)"; break;
                case EDoctrine.Char:
                    break;
                case EDoctrine.DateTimeTz:
                    break;
                case EDoctrine.DateTime:
                    result = "DATETIME()"; break;
                case EDoctrine.Date:
                    break;
                case EDoctrine.Decimal:
                    break;
                case EDoctrine.Double:
                    break;
                case EDoctrine.Enum:
                    break;
                case EDoctrine.Float:
                    result = "DOUBLE"; break;
                case EDoctrine.ForeignId:
                    break;
                case EDoctrine.GeometryCollection:
                    break;
                case EDoctrine.Geometry:
                    break;
                case EDoctrine.Id:
                    break;
                case EDoctrine.Increments:
                    break;
                case EDoctrine.Integer:
                    result = "INT"; break;
                case EDoctrine.IpAddress:
                    break;
                case EDoctrine.Json:
                    break;
                case EDoctrine.Jsonb:
                    break;
                case EDoctrine.LineString:
                    break;
                case EDoctrine.LongText:
                    break;
                case EDoctrine.MacAddress:
                    break;
                case EDoctrine.MediumIncrements:
                    break;
                case EDoctrine.MediumInteger:
                    break;
                case EDoctrine.MediumText:
                    break;
                case EDoctrine.Morphs:
                    break;
                case EDoctrine.MultiLineString:
                    break;
                case EDoctrine.MultiPoint:
                    break;
                case EDoctrine.MultiPolygon:
                    break;
                case EDoctrine.NullableMorphs:
                    break;
                case EDoctrine.NullableTimestamps:
                    break;
                case EDoctrine.NullableUuidMorphs:
                    break;
                case EDoctrine.Point:
                    break;
                case EDoctrine.Polygon:
                    break;
                case EDoctrine.RememberToken:
                    break;
                case EDoctrine.Set:
                    break;
                case EDoctrine.SmallIncrements:
                    break;
                case EDoctrine.SmallInteger:
                    result = "SMALLINT"; break;
                case EDoctrine.SoftDeletesTz:
                    break;
                case EDoctrine.SoftDeletes:
                    break;
                case EDoctrine.String:
                    result = "VARCHAR()"; break;
                case EDoctrine.Text:
                    result = "TEXT"; break;
                case EDoctrine.TimeTz:
                    break;
                case EDoctrine.Time:
                    break;
                case EDoctrine.TimestampTz:
                    break;
                case EDoctrine.Timestamp:
                    break;
                case EDoctrine.TimestampsTz:
                    break;
                case EDoctrine.Timestamps:
                    break;
                case EDoctrine.TinyIncrements:
                    break;
                case EDoctrine.TinyInteger:
                    result = "TINYINT"; break;
                case EDoctrine.UnsignedBigInteger:
                    result = "BIGINT UNSIGNED"; break;
                case EDoctrine.UnsignedDecimal:
                    break;
                case EDoctrine.UnsignedInteger:
                    result = "INT UNSIGNED"; break;
                case EDoctrine.UnsignedMediumInteger:
                    result = "MEDIUMINT UNSIGNED"; break;
                case EDoctrine.UnsignedSmallInteger:
                    result = "SMALLINT UNSIGNED"; break;
                case EDoctrine.UnsignedTinyInteger:
                    result = "TINYINT UNSIGNED"; break;
                case EDoctrine.UuidMorphs:
                    break;
                case EDoctrine.Uuid:
                    break;
                case EDoctrine.Year:
                    break;
                default:
                    break;
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
                case EDoctrine.SmallInteger:
                    result = typeof(short);
                    break;
                case EDoctrine.Integer:
                    result = typeof(int);
                    break;
                case EDoctrine.BigInteger:
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
                case EDoctrine.Text:
                    result = typeof(string);
                    break;
                case EDoctrine.Binary:
                    break;
                case EDoctrine.Boolean:
                    result = typeof(bool);
                    break;
                case EDoctrine.Date:
                case EDoctrine.DateTime:
                case EDoctrine.Time:
                    result = typeof(DateTime);
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

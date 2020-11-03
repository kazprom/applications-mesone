using System;
using System.Collections.Generic;
using System.Text;

namespace Lib
{
    public class Field : Attribute
    {

        public enum Etype
        {
            BigInt,
            Int,
            VarChar,
            TimeStamp,
            TinyInt,
            SmallInt
        }

        /// <summary>
        /// database type
        /// </summary>
        public Etype? TYPE = null;

        /// <summary>
        /// database lenght
        /// </summary>
        public uint? SIZE = null;

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
        public bool UN = false;


        public Field(Etype type)
        {
            TYPE = type;
        }

        public Field(Etype type, uint size) : this(type)
        {
            SIZE = size;
        }

        public Field(Etype type, uint size, bool pk = false, bool ai = false, bool nn = false, bool un = false) : this(type, size)
        {
            PK = pk;
            AI = ai;
            NN = nn;
            UN = un;
        }

        public Field(Etype type, bool pk = false, bool ai = false, bool nn = false, bool un = false) : this(type)
        {
            PK = pk;
            AI = ai;
            NN = nn;
            UN = un;
        }
    }
}

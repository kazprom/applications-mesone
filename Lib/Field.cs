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
            SmallInt,
            Binary
        }

        /// <summary>
        /// database type
        /// </summary>
        public Etype TYPE;

        /// <summary>
        /// database lenght
        /// </summary>
        public uint SIZE;

        /// <summary>
        /// ignore
        /// </summary>
        public bool IGNORE = false;

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

        /// <summary>
        /// unsigned
        /// </summary>
        public bool UN = false;

      
    }
}

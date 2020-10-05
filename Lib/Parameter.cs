﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Lib
{
    public class Parameter<T>
    {

        #region PROPERTIES

        private string name;
        public string Name { get { return name; } }

        private T value;
        public T Value
        {
            get { return value; }
            set
            {
                if (this.value == null || !this.value.Equals(value))
                {
                    this.value = value;
                    Lib.Logger.WriteMessage($"{name} = {this.value}");
                    ValueChanged?.Invoke(this.value);
                }
            }
        }



        #endregion

        #region EVENTS

        public delegate void ValueChangedNotify(T value);  // delegate
        public event ValueChangedNotify ValueChanged; // event

        #endregion


        public Parameter(string name)
        {
            this.name = name;
        }

    }
}

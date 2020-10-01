﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Lib
{
    public class Protocol
    {
        private static byte[] start_of_package = { 0, 0, 0, 0, 0, 0 };

        private static byte[] size_package = new byte[sizeof(int)];
        private static Queue<byte> package = new Queue<byte>();

        private static int counter_start_of_package = 0;
        private static int counter_size_of_package = 0;

        public const string MSG_OK = "OK";

        public enum ePackageTypes : byte
        {
            UNKNOWN = 0,
            KEY = 1,
            INFO = 2,
            DATA = 3,
            DIAG = 4
        }

        public static byte[] BuildPackage(byte[] data, ePackageTypes e)
        {

            try
            {
                if (data == null)
                    return null;

                byte[] title = start_of_package;
                byte[] type = /*new byte[] {*/ BitConverter.GetBytes((byte)e) /*}*/;
                byte[] size = BitConverter.GetBytes(type.Length + data.Length);

                byte[] buf = new byte[title.Length + size.Length + type.Length + data.Length];

                System.Buffer.BlockCopy(title, 0, buf, 0, title.Length);
                System.Buffer.BlockCopy(size, 0, buf, title.Length, size.Length);
                System.Buffer.BlockCopy(type, 0, buf, title.Length + size.Length, type.Length);
                System.Buffer.BlockCopy(data, 0, buf, title.Length + size.Length + type.Length, data.Length);

                return buf;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<byte[]> CatchPackage(byte[] data)
        {

            List<byte[]> result = new List<byte[]>();

            try
            {
                foreach (byte item in data)
                {
                    if (counter_start_of_package < start_of_package.Length)
                    {
                        if (start_of_package[counter_start_of_package] == item)
                        {
                            counter_start_of_package += 1;
                        }
                    }
                    else if (counter_size_of_package < sizeof(int))
                    {
                        size_package[counter_size_of_package] = item;
                        counter_size_of_package += 1;
                    }
                    else if (package.Count < BitConverter.ToInt32(size_package, 0))
                    {
                        package.Enqueue(item);
                    }

                    if (package.Count > 1024)
                    {
                        counter_start_of_package = 0;
                        counter_size_of_package = 0;
                        package.Clear();
                    }

                    if (counter_start_of_package == start_of_package.Length &&
                       counter_size_of_package == sizeof(int) &&
                       package.Count == BitConverter.ToInt32(size_package, 0))
                    {
                        result.Add(package.ToArray());
                        counter_start_of_package = 0;
                        counter_size_of_package = 0;
                        package.Clear();
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error catch package", ex);
            }

            return result;

        }

        public static ePackageTypes GetTypePackage(ref byte[] data)
        {
            ePackageTypes result = ePackageTypes.UNKNOWN;


            try
            {
                if (data.Length > 0)
                {
                    result = (ePackageTypes)BitConverter.ToInt16(data,0);
                    data = data.Skip(sizeof(short)).ToArray();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error get type package", ex);
            }

            return result;


        }

        public static byte[] ConvertObjToByteArr(object obj)
        {
            if (obj != null)
            {
                var binFormatter = new BinaryFormatter();
                var mStream = new MemoryStream();
                binFormatter.Serialize(mStream, obj);

                //This gives you the byte array.
                return mStream.ToArray();
            }
            return null;
        }

        public static object ConvertByteArrToObj(byte[] arr)
        {
            if (arr != null)
            {
                var mStream = new MemoryStream();
                var binFormatter = new BinaryFormatter();

                // Where 'objectBytes' is your byte array.
                mStream.Write(arr, 0, arr.Length);
                mStream.Position = 0;

                //var myObject = binFormatter.Deserialize(mStream) as YourObjectType;
                return binFormatter.Deserialize(mStream);
            }
            return null;
        }

    }
}

using System;
using System.IO;
using System.Security.Cryptography;

namespace Lib
{
    public class Encryption
    {

        #region STRUCTURES

        [Serializable]
        public struct SSafetyKeys
        {
            public byte[] key;
            public byte[] iv;
        }

        #endregion

        #region PROPERTIES

        private SSafetyKeys safety_keys;
        public SSafetyKeys SafetyKeys { get { return safety_keys; } set { safety_keys = value; } }

        #endregion

        #region VARIABLES

        private DES algorithm = DES.Create();


        #endregion

        #region CONSTRUCTOR

        public Encryption()
        {
            try
            {
                GenerateSecurityKey();
            }
            catch (Exception ex)
            {

                throw new Exception("Error creation object", ex);
            }

        }

        #endregion

        #region PUBLICS

        public void GenerateSecurityKey()
        {
            try
            {
                DES DESalg = DES.Create();
                safety_keys.key = DESalg.Key;
                safety_keys.iv = DESalg.IV;
            }
            catch (Exception ex)
            {
                throw new Exception("Error generate security key", ex);
            }

        }

        public byte[] Encrypt(byte[] data)
        {
            try
            {
                if (SafetyKeys.key == null || SafetyKeys.iv == null)
                    return null;

                using (var encryptor = algorithm.CreateEncryptor(SafetyKeys.key, SafetyKeys.iv))
                {
                    return PerformCryptography(encryptor, data);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error encrypt", ex);
            }
        }

        public byte[] Decrypt(byte[] data)
        {
            try
            {
                if (SafetyKeys.key == null || SafetyKeys.iv == null)
                    return null;

                using (var decryptor = algorithm.CreateDecryptor(SafetyKeys.key, SafetyKeys.iv))
                {
                    return PerformCryptography(decryptor, data);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error decrypt", ex);
            }
        }

        #endregion

        #region PRIVATES

        private byte[] PerformCryptography(ICryptoTransform cryptoTransform, byte[] data)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(data, 0, data.Length);
                        cryptoStream.FlushFinalBlock();
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error cryptography", ex);
            }
        }

        #endregion

    }
}

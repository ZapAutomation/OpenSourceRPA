using System;
using System.Globalization;
using System.IO;
using Zappy.Properties;

namespace Zappy.Decode.Helper
{
    internal static class EncodeDecode
    {
        private const string checkPhrase = "pnr_etadilav";
        private static bool isInitialized;
        private static int s_customKeyLength = 0x10;
        private static string s_customKeyLocation;

        public static string DecodeString(string dataToDecode)
        {
            if (string.IsNullOrEmpty(dataToDecode))
            {
                return dataToDecode;
            }
            if (!isInitialized && !string.IsNullOrEmpty(s_customKeyLocation))
            {
                ReadKeyFile();
            }
            string str = string.Empty;
            try
            {
                str = EncodeDecodeImpl.DecryptString(dataToDecode);
            }
            catch (Exception exception)
            {
                object[] objArray1 = { exception.Message };
                            }
            if (str.EndsWith("pnr_etadilav", StringComparison.Ordinal))
            {
                return str.Substring(0, str.Length - "pnr_etadilav".Length);
            }
                                    return str;
        }

        public static string EncodeString(string dataToEncode)
        {
            if (dataToEncode == null)
            {
                return dataToEncode;
            }
            if (!isInitialized && !string.IsNullOrEmpty(s_customKeyLocation))
            {
                ReadKeyFile();
            }
            dataToEncode = dataToEncode + "pnr_etadilav";
            return EncodeDecodeImpl.EncryptString(dataToEncode);
        }

        internal static void InitializeDefaultSettings()
        {
            s_customKeyLocation = null;
            isInitialized = false;
        }

        private static void ReadKeyFile()
        {
            try
            {
                EncodeDecodeImpl.ReadKeyFile(s_customKeyLocation, s_customKeyLength);
                isInitialized = true;
            }
            catch (InvalidDataException)
            {
                            }
            catch (FileNotFoundException exception)
            {
                object[] args = { exception.Message };
                throw new FileNotFoundException(string.Format(CultureInfo.CurrentCulture, Resources.EncryptionKeyLocationNotFoundMessage, args));
            }
        }

        public static void SetEncryptionKeyLocation(string location, int keySize)
        {
            if (keySize < 0x10)
            {
                keySize = 0x10;
            }
            else if (keySize > 0x10)
            {
                keySize = 0x18;
            }
            s_customKeyLocation = location;
            s_customKeyLength = keySize;
            isInitialized = false;
        }
    }
}
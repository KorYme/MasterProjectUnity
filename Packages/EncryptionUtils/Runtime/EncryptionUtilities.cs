namespace EncryptionUtils
{
    public static class EncryptionUtilities
    {
        public enum EncryptionType
        {
            None,
            XOR,
        }

        public static string Encrypt(string data, EncryptionType encryptionType, bool isEncrypting, string encrytionString = "")
        {
            switch (encryptionType)
            {
                case EncryptionType.None:
                    return data;
                case EncryptionType.XOR:
                    return XOREncryption(data, encrytionString);
                default:
                    return "";
            }
        }

        public static string XOREncryption(string data, string encrytionString)
        {
            string modifiedData = "";
            for (int i = 0; i < data.Length; i++)
            {
                modifiedData += (char)(data[i] ^ encrytionString[i % encrytionString.Length]);
            }
            return modifiedData;
        }
    }
}
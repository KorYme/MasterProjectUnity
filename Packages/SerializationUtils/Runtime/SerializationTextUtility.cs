namespace SerializationUtils
{
    public static class SerializationTextUtility
    {
        public const string ALLOWED_CHARACTERS = "_";

        public static string RemoveNonSerializableCharacters(this string text)
        {
            string newString = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].IsSerializableFriendly())
                {
                    newString += text[i];
                }
            }
            return newString;
        }

        public static bool IsSerializableFriendly(this string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (!text[i].IsSerializableFriendly()) return false;
            }
            return true;
        }

        public static bool IsSerializableFriendly(this char character)
            => char.IsLetterOrDigit(character) || ALLOWED_CHARACTERS.Contains(character);
    }
}
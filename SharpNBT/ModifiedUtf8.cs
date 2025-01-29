namespace SharpNBT;
public static class ModifiedUtf8
{
    public static byte[] GetBytes(string str)
    {
        if (string.IsNullOrEmpty(str))
            return [];

        int byteCount = 0;
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            if (c == 0)
                byteCount += 2;
            else if (c >= 0x0001 && c <= 0x007F)
                byteCount += 1;
            else if (c <= 0x07FF)
                byteCount += 2;
            else
            {
                if (char.IsHighSurrogate(c) && i + 1 < str.Length && char.IsLowSurrogate(str[i + 1]))
                {
                    byteCount += 6;
                    i++;
                }
                else
                {
                    byteCount += 3;
                }
            }
        }

        byte[] bytes = new byte[byteCount];
        int position = 0;

        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];

            if (c == 0)
            {
                bytes[position++] = 0xC0;
                bytes[position++] = 0x80;
            }
            else if (c >= 0x0001 && c <= 0x007F)
            {
                bytes[position++] = (byte)c;
            }
            else if (c <= 0x07FF)
            {
                bytes[position++] = (byte)(0xC0 | ((c >> 6) & 0x1F));
                bytes[position++] = (byte)(0x80 | (c & 0x3F));
            }
            else
            {
                if (char.IsHighSurrogate(c) && i + 1 < str.Length && char.IsLowSurrogate(str[i + 1]))
                {
                    bytes[position++] = (byte)(0x80 | ((c >> 6) & 0x3F));
                    bytes[position++] = (byte)(0x80 | (c & 0x3F));

                    char lowSurrogate = str[++i];
                    bytes[position++] = (byte)(0xE0 | ((lowSurrogate >> 12) & 0x0F));
                    bytes[position++] = (byte)(0x80 | ((lowSurrogate >> 6) & 0x3F));
                    bytes[position++] = (byte)(0x80 | (lowSurrogate & 0x3F));
                }
                else
                {
                    bytes[position++] = (byte)(0xE0 | ((c >> 12) & 0x0F));
                    bytes[position++] = (byte)(0x80 | ((c >> 6) & 0x3F));
                    bytes[position++] = (byte)(0x80 | (c & 0x3F));
                }
            }
        }

        return bytes;
    }
}
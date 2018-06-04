using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;
using System;

namespace Uno.Text
{
    public static class Base64
    {
        const string Base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        public static byte[] GetBytes(string value)
        {
            if defined(DOTNET)
                return Convert.FromBase64String(value);
            else
            {
                var addidionalSymbols = 0;
                var ret = new List<byte>();
                var charArray3 = new int[3];
                var charArray4 = new int[4];

                for (var position = 0; position < value.Length; position++)
                {
                    if (value[position] == '=' || Base64Chars.IndexOf(value[position]) < 0)
                        break;

                    charArray4[addidionalSymbols] = value[position];
                    addidionalSymbols++;
                    if (addidionalSymbols == 4)
                    {
                        for (var j = 0; j < 4; j++)
                            charArray4[j] = Base64Chars.IndexOf((char)charArray4[j]);

                        charArray3[0] = (charArray4[0] << 2) + ((charArray4[1] & 0x30) >> 4);
                        charArray3[1] = ((charArray4[1] & 0xf) << 4) + ((charArray4[2] & 0x3c) >> 2);
                        charArray3[2] = ((charArray4[2] & 0x3) << 6) + charArray4[3];

                        for (var j = 0; j < 3; j++)
                            ret.Add((byte)charArray3[j]);

                        addidionalSymbols = 0;
                    }
                }

                if (addidionalSymbols > 0)
                {
                    for (var j = addidionalSymbols; j < 4; j++)
                        charArray4[j] = 0;

                    for (var j = 0; j < 4; j++)
                        charArray4[j] = Base64Chars.IndexOf((char)charArray4[j]);

                    charArray3[0] = (charArray4[0] << 2) + ((charArray4[1] & 0x30) >> 4);
                    charArray3[1] = ((charArray4[1] & 0xf) << 4) + ((charArray4[2] & 0x3c) >> 2);
                    charArray3[2] = ((charArray4[2] & 0x3) << 6) + charArray4[3];

                    for (var j = 0; j < addidionalSymbols - 1; j++)
                        ret.Add((byte)charArray3[j]);
                }

                return ret.ToArray();
            }
        }

        public static string GetString(byte[] value)
        {
            if defined(CPLUSPLUS)
            @{
                static const char* base64_chars =
                    "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
                    "abcdefghijklmnopqrstuvwxyz"
                    "0123456789+/";

                unsigned char* bytes_to_encode =(unsigned char*)$0->Ptr();
                unsigned int in_len = $0->Length();
                unsigned char* encoded_buffer = new unsigned char[in_len*2 + 3];

                int i = 0;
                int j = 0;
                unsigned char char_array_3[3] = { 0, 0, 0 };
                unsigned char char_array_4[4] = { 0, 0, 0, 0 };

                unsigned int out_len = 0;
                while (in_len--)
                {
                    char_array_3[i++] = *(bytes_to_encode++);
                    if (i == 3)
                    {
                        char_array_4[0] = (char_array_3[0] & 0xfc) >> 2;
                        char_array_4[1] = ((char_array_3[0] & 0x03) << 4) + ((char_array_3[1] & 0xf0) >> 4);
                        char_array_4[2] = ((char_array_3[1] & 0x0f) << 2) + ((char_array_3[2] & 0xc0) >> 6);
                        char_array_4[3] = char_array_3[2] & 0x3f;

                        for (i = 0; i < 4 ; i++)
                        {
                            encoded_buffer[out_len++] = base64_chars[char_array_4[i]];
                        }
                        i = 0;
                    }
                }

                if (i)
                {
                    for (j = i; j < 3; j++)
                    {
                        char_array_3[j] = '\0';
                    }

                    char_array_4[0] = (char_array_3[0] & 0xfc) >> 2;
                    char_array_4[1] = ((char_array_3[0] & 0x03) << 4) + ((char_array_3[1] & 0xf0) >> 4);
                    char_array_4[2] = ((char_array_3[1] & 0x0f) << 2) + ((char_array_3[2] & 0xc0) >> 6);
                    char_array_4[3] = char_array_3[2] & 0x3f;

                    for (j = 0; j < (i + 1); j++)
                    {
                        encoded_buffer[out_len++] = base64_chars[char_array_4[j]];
                    }

                    while (i++ < 3)
                    {
                        encoded_buffer[out_len++] = '=';
                    }
                }

                uString* res = uString::Ansi((char const *)encoded_buffer, out_len);
                delete [] encoded_buffer;
                return res;
            @}
            else if defined(DOTNET)
                return Convert.ToBase64String(value);
            else
            {
                var addidionalSymbols = 0;
                var ret = string.Empty;
                var charArray3 = new int[3];
                var charArray4 = new int[4];

                for (var position = 0; position < value.Length; position++)
                {
                    charArray3[addidionalSymbols] = value[position];
                    addidionalSymbols++;
                    if (addidionalSymbols == 3)
                    {
                        charArray4[0] = (charArray3[0] & 0xfc) >> 2;
                        charArray4[1] = ((charArray3[0] & 0x03) << 4) + ((charArray3[1] & 0xf0) >> 4);
                        charArray4[2] = ((charArray3[1] & 0x0f) << 2) + ((charArray3[2] & 0xc0) >> 6);
                        charArray4[3] = charArray3[2] & 0x3f;

                        for (var j = 0; j < 4; j++)
                        {
                            ret += Base64Chars[charArray4[j]];
                        }
                        addidionalSymbols = 0;
                    }
                }

                if (addidionalSymbols > 0)
                {
                    for (var j = addidionalSymbols; j < 3; j++)
                        charArray3[j] = '\0';

                    charArray4[0] = (charArray3[0] & 0xfc) >> 2;
                    charArray4[1] = ((charArray3[0] & 0x03) << 4) + ((charArray3[1] & 0xf0) >> 4);
                    charArray4[2] = ((charArray3[1] & 0x0f) << 2) + ((charArray3[2] & 0xc0) >> 6);
                    charArray4[3] = charArray3[2] & 0x3f;

                    for (var j = 0; j < addidionalSymbols + 1; j++)
                        ret += Base64Chars[charArray4[j]];

                    while (addidionalSymbols < 3)
                    {
                        ret += "=";
                        addidionalSymbols++;
                    }
                }

                return ret;
            }
        }
    }
}

namespace System
{
    [DotNetType]
    extern(DOTNET) static class Convert
    {
        public static extern byte[] FromBase64String(string s);
        public static extern string ToBase64String(byte[] inArray);
    }
}

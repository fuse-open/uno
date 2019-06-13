using Uno;

namespace Uno.Text
{
    public static class Ascii
    {
        public static byte[] GetBytes(string value)
        {
            if defined(DOTNET)
                return Encoding.ASCII.GetBytes(value);
            else
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                @{
                    uArray* res = uArray::New(@{byte[]:TypeOf}, value->_length);

                    for (size_t i = 0; i < value->_length; i++)
                        res->Unsafe<uint8_t>(i) = (uint8_t)(value->_ptr[i] < 128 ? value->_ptr[i] : '?');

                    return res;
                @}
            }
        }

        public static string GetString(byte[] value)
        {
            if defined(DOTNET)
                return Encoding.ASCII.GetString(value);
            else
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                @{
                    uString* res = uString::New(value->_length);
                    
                    for (size_t i = 0; i < value->_length; i++)
                        res->_ptr[i] = value->Unsafe<uint8_t>(i) < 128 ? value->Unsafe<uint8_t>(i) : '?';

                    return res;
                @}
            }
        }

        public static string GetString(byte[] value, int index, int count)
        {
            if defined(DOTNET)
                return Encoding.ASCII.GetString(value, index, count);
            else
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (index < 0 || index >= value.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));
                if (count < 0 || index + count > value.Length)
                    throw new ArgumentOutOfRangeException(nameof(count));

                @{
                    uString* res = uString::New(count);
                    
                    for (size_t i = 0; i < count; i++)
                        res->_ptr[i] = value->Unsafe<uint8_t>(i + index) < 128 ? value->Unsafe<uint8_t>(i + index) : '?';

                    return res;
                @}
            }
        }
    }
}

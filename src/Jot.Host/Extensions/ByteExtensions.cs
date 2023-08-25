using System.Text;

namespace Jot.Extensions
{
    public static class ByteExtensions
    {
        public static byte[] AddObjectType(this byte[] input, string type)
        {
            var result = input.ToList();

            result.Add(byte.MinValue);
            result.AddRange(Encoding.UTF8.GetBytes(type));

            return result.ToArray();
        }

        public static string GetObjectType(this byte[] input)
        {
            var result = input.ToList();

            var delimeterIndex = result.LastIndexOf(byte.MinValue);

            var typeBytes = result.Skip(delimeterIndex + 1);

            return Encoding.UTF8.GetString(typeBytes.ToArray());
        }

        public static byte[] StripType(this byte[] input)
        {
            var inputList = input.ToList();

            var delimeter = inputList.LastIndexOf(byte.MinValue);

            var result = inputList.Take(delimeter);

            return result.ToArray();
        }
    }
}
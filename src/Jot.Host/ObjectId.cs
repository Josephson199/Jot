using System.Security.Cryptography;
using System.Text;

namespace Jot
{
    public class NullObjectId : ObjectId
    {
        public NullObjectId() : base(value: string.Empty)
        {
        }
    }



    public class ObjectId : IEquatable<ObjectId>
    {
        public ObjectId(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override string ToString() { return Value; }

        public static ObjectId CreateInstance(IEnumerable<byte> input)
        {
            var data = SHA256.HashData(input.ToArray());

            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            var hash = sBuilder.ToString();

            return new ObjectId(hash);
        }

        public static NullObjectId Null => new ();

        public FilePath CreateObjectFilePath() => JotPaths.CreateObjectPath(Value);

        public bool Equals(ObjectId? other)
        {
            if (other is null) return false;

            return string.Equals(Value, other.Value);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            return Equals(obj as ObjectId);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static implicit operator string(ObjectId id) => id.Value;

        public static implicit operator ObjectId(string? value)
            => string.IsNullOrWhiteSpace(value) ? Null : new ObjectId(value);

        //public static explicit operator ObjectId(string value) => new (value);
    }
}
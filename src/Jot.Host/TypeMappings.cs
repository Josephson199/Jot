using Jot.Attributes;
using System.Reflection;

namespace Jot
{
    public static class TypeMappings
    {
        private static readonly IDictionary<string, Type> _cache = new Dictionary<string, Type>();

        public static IDictionary<string, Type> Get()
        {
            if (!_cache.Any())
            {
                Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(type => type.IsDefined(typeof(JotObjectTypeAttribute)))
                    .Select(type => new
                    {
                        Type = type,
                        TypeKey = ((JotObjectTypeAttribute)Attribute.GetCustomAttribute(type, typeof(JotObjectTypeAttribute))!)!.Type
                    })
                    .ToList()
                    .ForEach(tm => _cache.Add(tm.TypeKey, tm.Type));
            }

            return _cache;
        }
    }
}
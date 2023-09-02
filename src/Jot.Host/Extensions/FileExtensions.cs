using System.CommandLine.Help;
using System.IO.Abstractions;
using System.Text.Json;

namespace Jot.Extensions
{
    public static class FileExtensions
    {
        public static void WriteAllBytes(this IFile file, ObjectId objectId, byte[] bytes)
        {
            file.WriteAllBytes(objectId.CreateObjectFilePath().Value, bytes);
        }

        public static T GetOptions<T>(this IFile file) where T : IConfig
        {
            var path = JotPaths.OptionPaths[typeof(T)];
            
            var config = file.ReadAllText(path!);

            return JsonSerializer.Deserialize<T>(config, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
        }

        public static bool OptionsExists<T>(this IFile file) where T : IConfig
        {
            var path = JotPaths.OptionPaths[typeof(T)];

            if (file.Exists(path))
            {
                return true;
            }

            return false;
        }

        public static void WriteOptions<T>(this IFile file, T @object) where T : IConfig
        {
            var exists = JotPaths.OptionPaths.TryGetValue(typeof(T), out var path);

            if (!exists)
            {
                throw new NotImplementedException(nameof(@object));
            }

            var @string = JsonSerializer.Serialize(@object, new JsonSerializerOptions
            {
                WriteIndented = true,
            });

            file.WriteAllText(path!, @string);
        }
    }
}

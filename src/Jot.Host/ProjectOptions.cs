namespace Jot
{
    public class ProjectOptions : IConfig
    {
        public string Version { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string[] IngoredDirectories { get; set; } = Array.Empty<string>();
    }

    public interface IConfig { }
}
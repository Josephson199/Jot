using Jot;

internal static class JotPaths
{
    internal static readonly string JotBasePath = Path.Combine(Environment.CurrentDirectory, ".jot");
    
    internal static readonly string ObjectsDirPath = Path.Combine(JotBasePath, "objects");
    
    internal static readonly string ProjectOptionsFilePath = Path.Combine(JotBasePath, ".config");

    internal static readonly IDictionary<Type, string> OptionPaths = new Dictionary<Type, string>
    {
        { typeof(ProjectOptions), ProjectOptionsFilePath }
    };

    internal static FilePath CreateObjectPath(string hash) => new(Path.Combine(ObjectsDirPath, hash));
}
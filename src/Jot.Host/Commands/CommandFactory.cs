using System.CommandLine;
using System.Reflection;

namespace Jot.Commands
{
    public class CommandFactory
    {
        public static RootCommand CreateRoot()
        {
            var root = new RootCommand("Sample implementation of a source control tool in c#");

            Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(type => type.IsSubclassOf(typeof(Command)) && !type.IsAbstract)
                .Select(type => Activator.CreateInstance(type, null) as Command)
                .ToList()
                .ForEach(root.Add!);

            return root;
        }
    }


    
}

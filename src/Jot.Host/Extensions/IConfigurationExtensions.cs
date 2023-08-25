using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jot.Extensions
{
    public static class IConfigurationExtensions
    {
        public static T MustGet<T>(this IConfiguration configuration)
        {
            return configuration.Get<T>() ?? throw new InvalidOperationException("T must exists in configuration");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandboxPolicy.Services.Extensions
{
    public static class QueryExtensions
    {
        public static IEnumerable<T> Page<T>(this IEnumerable<T> source, int pageNum, int pageSize)
        {
            return source.Skip(pageNum * pageSize).Take(pageSize);
        }
    }
}

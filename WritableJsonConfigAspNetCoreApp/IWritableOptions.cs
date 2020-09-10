using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace WritableJsonConfigAspNetCoreApp
{
    public interface IWritableOptions<out T> : IOptionsSnapshot<T> where T : class, new()
    {
        Task Update(Action<T> applyChanges);
    }
}

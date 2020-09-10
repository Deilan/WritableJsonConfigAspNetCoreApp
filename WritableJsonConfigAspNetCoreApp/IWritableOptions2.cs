using Microsoft.Extensions.Options;

namespace WritableJsonConfigAspNetCoreApp
{
    public interface IWritableOptions2<TOptions> : IOptionsSnapshot<TOptions> where TOptions : class, new()
    {
        void Update(string key, string val);
        public void Update(TOptions instance);
    }
}

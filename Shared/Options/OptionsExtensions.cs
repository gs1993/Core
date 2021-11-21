using Microsoft.Extensions.Configuration;

namespace Shared.Options
{
    public static class OptionsExtensions
    {
        public static TOptions GetOptions<TOptions>(this IConfiguration configuration, string sectionName = null)
            where TOptions : new()
        {
            sectionName ??= nameof(TOptions);

            var options = new TOptions();
            configuration.GetSection(sectionName).Bind(options);
            return options;
        }
    }
}

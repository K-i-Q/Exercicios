using Microsoft.AspNetCore.Builder;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Extensions
{
    public static class DatabaseBootstrapExtensions
    {
        public static IApplicationBuilder UseDatabaseSetup(this IApplicationBuilder app)
        {
            var databaseBootstrap = app.ApplicationServices.GetService<IDatabaseBootstrap>();

            databaseBootstrap?.Setup();

            return app;
        }
    }
}
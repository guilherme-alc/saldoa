
using Microsoft.AspNetCore.Authorization;

namespace MeuBolso.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Habilita validacao de escopo para servicos
            builder.Host.UseDefaultServiceProvider(config =>
            {
                config.ValidateScopes = true;
            });

            // Remove o cabecalho "Server" das respostas HTTP
            builder.WebHost.UseKestrel(options => options.AddServerHeader = false);

            // Fallback policy para exigir autenticacaoo em todas as rotas por padrao
            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            builder.Services.AddOpenApi();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            // Configuracao de cabecalhos de seguranca HTTP
            var policy = new HeaderPolicyCollection()
                .AddFrameOptionsDeny()
                .AddXssProtectionBlock()
                .AddContentTypeOptionsNoSniff()
                .AddReferrerPolicyStrictOriginWhenCrossOrigin()
                .AddCrossOriginOpenerPolicy(builder => builder.SameOrigin())
                .AddPermissionsPolicy(policy =>
                {
                    policy.AddCamera().None();
                    policy.AddMicrophone().None();
                    policy.AddGeolocation().None();
                });
            app.UseSecurityHeaders(policy);

            app.UseAuthorization();

            app.Run();
        }
    }
}

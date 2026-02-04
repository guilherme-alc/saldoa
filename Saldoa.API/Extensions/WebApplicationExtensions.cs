using Saldoa.API.Middlewares;

namespace Saldoa.API.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Configura o pipeline HTTP da API, centralizando middlewares,
    /// segurança, documentação e mapeamento de endpoints.
    /// </summary>
    public static WebApplication UseApi(this WebApplication app)
    {
        // Tratamento global de exceções
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        
        // HTTPS
        app.UseHttpsRedirection();
        
        // Security Headers
        var policy = new HeaderPolicyCollection()
            .AddFrameOptionsDeny()
            .AddXssProtectionBlock()
            .AddContentTypeOptionsNoSniff()
            .AddReferrerPolicyStrictOriginWhenCrossOrigin()
            .AddCrossOriginOpenerPolicy(policyBuilder => policyBuilder.SameOrigin())
            .AddPermissionsPolicy(policy =>
            {
                policy.AddCamera().None();
                policy.AddMicrophone().None();
                policy.AddGeolocation().None();
            });
        app.UseSecurityHeaders(policy);
        
        // Swagger em dev
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "docs/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "docs";
                c.SwaggerEndpoint("/docs/v1/swagger.json", "Saldoa API v1");
                c.EnablePersistAuthorization();
            });
        }

        // Segurança
        app.UseAuthentication();
        app.UseAuthorization();
        
        // Endpoints
        app.MapEndpoints();
        
        return app;
    }
}
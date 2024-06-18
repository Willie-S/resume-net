using Microsoft.AspNetCore.Authorization;
using ResuMeAPI.Data;
using ResuMeAPI.Utilities;
using System.Net;

namespace ResuMeAPI.Authentication
{
    public class ApiKeyAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _apiKeyHeaderName = "x-api-key";

        public ApiKeyAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ResuMeApiDbContext dbContext)
        {
            // Check for the AllowAnonymous attribute
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var endpointAttributes = endpoint.Metadata.GetOrderedMetadata<AllowAnonymousAttribute>();
                if (endpointAttributes != null && endpointAttributes.Any())
                {
                    await _next(context);
                    return;
                }
            }

            if (!context.Request.Headers.TryGetValue(_apiKeyHeaderName, out var extractedApiKey) || String.IsNullOrWhiteSpace(extractedApiKey))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("API Key is missing or empty");
                return;
            }

            var hashedKey = ApiKeyHelper.HashApiKey(extractedApiKey);
            var apiKey = dbContext.ApiKeys.SingleOrDefault(k => k.Key == hashedKey && !k.IsExpired);

            if (apiKey == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            await _next(context);
        }
    }
}

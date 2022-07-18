using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;
using SimpleWeatherApp.Api.Models;
using SimpleWeatherApp.Api.Attributes;
using SimpleWeatherApp.Api.Extensions;
using System.Net;
using Microsoft.AspNetCore.Builder;

namespace SimpleWeatherApp.Api.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly IDistributedCache _cache;
        private readonly RequestDelegate _next;

        public RateLimitMiddleware(IDistributedCache cache, RequestDelegate next)
        {
            _cache = cache;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var requestLimit = endpoint?.Metadata.GetMetadata<RequestLimit>();

            var key = "some key"; //TODO: Add API key
            var clientDetails = await GetClientDetailsByKey(key); 

            if(clientDetails != null
                && DateTime.UtcNow < clientDetails.LastSuccessfulReponseTime.AddMinutes(requestLimit.TimeLimit)
                && clientDetails.NumberOfRequestsCompleted == requestLimit.MaxRequests)
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                return;
            }

            await UpdateClientDetailStorage(key, requestLimit.MaxRequests);
            await _next(context);
        }

        private async Task<ClientDetails> GetClientDetailsByKey(string key)
        {
            try
            {
                byte[] result = await _cache.GetAsync(key, default(CancellationToken));
                var clientDetails = result.FromByteArray<ClientDetails>();

                return clientDetails;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving client details from the cache...", ex.InnerException);
            }
            
        }

        private async Task UpdateClientDetailStorage(string key, int maxRequests)
        {
            try
            {
                var clientDetails = await GetClientDetailsByKey(key);

                if (clientDetails != null)
                {
                    clientDetails.LastSuccessfulReponseTime = DateTime.UtcNow;

                    if (clientDetails.NumberOfRequestsCompleted == maxRequests)
                        clientDetails.NumberOfRequestsCompleted = 1;
                    else
                        clientDetails.NumberOfRequestsCompleted++;

                    await _cache.SetAsync(key, clientDetails.ToByteArray(), default);

                }
                else
                {
                    var newClientDetails = new ClientDetails
                    {
                        LastSuccessfulReponseTime = DateTime.UtcNow,
                        NumberOfRequestsCompleted = 1
                    };

                    await _cache.SetAsync(key, newClientDetails.ToByteArray(), default);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving client details to the cache...", ex.InnerException);
            }
        }
    }

    /// <summary>
    /// Extension method to expose the rate limiting middleware through IApplicationBuilder
    /// </summary>
    public static class RateLimitingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitMiddleware>();
        }
    }
}
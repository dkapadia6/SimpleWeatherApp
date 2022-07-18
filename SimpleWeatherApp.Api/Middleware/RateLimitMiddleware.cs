using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;
using SimpleWeatherApp.Api.Models;
using SimpleWeatherApp.Api.Attributes;
using SimpleWeatherApp.Api.Extensions;
using System.Net;

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
    }
}
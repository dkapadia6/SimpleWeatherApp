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
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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

            if (endpoint != null)
            {
                var requestLimit = endpoint?.Metadata.GetMetadata<RequestLimit>();

                if (requestLimit != null)
                {
                    var key = "some key"; //TODO: Add API key
                    var clientDetails = await GetClientDetailsByKey(key);

                    if (clientDetails != null
                        && DateTime.UtcNow < clientDetails.LastSuccessfulReponseTime.AddMinutes(requestLimit.TimeLimit)
                        && clientDetails.NumberOfRequestsCompleted == requestLimit.MaxRequests)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                        return;
                    }

                    await UpdateClientDetailStorage(key, requestLimit.MaxRequests);
                }
            }

            await _next(context);
        }

        private async Task<ClientDetails> GetClientDetailsByKey(string key)
        {
            try
            {
                byte[] value = await _cache.GetAsync(key);

                if (value is null)
                    return null;
                else
                {
                    var clientDetails = ByteArrayToObject(value) as ClientDetails;

                    return clientDetails;
                }
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

                    byte[] value = ObjectToByteArray(clientDetails);
                    await _cache.SetAsync(key, value);
                }
                else
                {
                    var newClientDetails = new ClientDetails
                    {
                        LastSuccessfulReponseTime = DateTime.UtcNow,
                        NumberOfRequestsCompleted = 1
                    };

                    byte[] value = ObjectToByteArray(newClientDetails);
                    await _cache.SetAsync(key, value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving client details to the cache...", ex.InnerException);
            }
        }

        private byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        private object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            object obj = (object)binForm.Deserialize(memStream);
            return obj;
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
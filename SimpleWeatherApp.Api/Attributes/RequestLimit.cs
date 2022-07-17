using System;

namespace SimpleWeatherApp.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequestLimit : Attribute
    {
        public int TimeLimit { get; set; } //Time limit in minutes e.g. 60 for 1 hour
        public int MaxRequests { get; set; } //Max number of requests allowed within defined time limit
    }
}
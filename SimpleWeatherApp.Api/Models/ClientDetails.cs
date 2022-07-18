using System;

namespace SimpleWeatherApp.Api.Models
{
    [Serializable]
    public class ClientDetails
    {
        public DateTime LastSuccessfulReponseTime { get; set; }
        public int NumberOfRequestsCompleted { get; set; }
    }
}
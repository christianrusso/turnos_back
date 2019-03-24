using System;
using System.Collections.Generic;
using RestSharp.Deserializers;

namespace SistemaTurnos.WebApplication.WebApi.Dto.OneSignal
{
    internal class OsNotificationResponseDto
    {
        public string id { get; set; }

        public int recipients { get; set; }

        public string external_id { get; set; }
    }
}
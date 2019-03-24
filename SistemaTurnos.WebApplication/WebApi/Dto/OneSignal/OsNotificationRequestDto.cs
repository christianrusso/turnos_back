using System;
using System.Collections.Generic;
using RestSharp.Deserializers;

namespace SistemaTurnos.WebApplication.WebApi.Dto.OneSignal
{
    internal class OsNotificationRequestDto
    {
        public string app_id { get; set; }

        public List<OsNotificationFilterDto> filters { get; set; }

        // TODO: This is used if we want to pass information when the user gets
        // the notification.
        public object data { get; set; }

        public OsNotificationContentsDto contents { get; set; }

        public string send_after { get; set; }
    }

    internal class OsNotificationFilterDto
    {
        public string field { get; set; }

        public string key { get; set; }

        public string relation { get; set; }

        public string value { get; set; }
    }

    internal class OsNotificationContentsDto
    {
        public string en { get; set; }

        public string es { get; set; }
    }
}
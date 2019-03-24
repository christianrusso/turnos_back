using System;
using System.Collections.Generic;
using RestSharp;
using SistemaTurnos.WebApplication.WebApi.Dto.OneSignal;

namespace SistemaTurnos.WebApplication.WebApi.Services
{
    public static class OneSignalService
    {
        /// <summary>
        /// Send notification por a specific user
        /// </summary>
        public static bool SendNotification(int userId, string msg)
        {
            return ScheduleNotification(userId, null, msg);
        }

        /// <summary>
        /// Schedule a notification for a specific user.
        /// </summary>
        public static bool ScheduleNotification(int userId, DateTime? timestamp, string msg)
        {
            string ts = "";
            if (timestamp.HasValue)
            {
                var dt = timestamp.Value;
                if (timestamp.Value.Kind != DateTimeKind.Utc)
                {
                    dt = timestamp.Value.ToUniversalTime();
                }
                ts = dt.ToString("yyyy-MM-ddThh:mm:ssZ");
            }
            var content = new OsNotificationRequestDto()
            {
                // TODO: Get from configuration.
                app_id = "88269b56-66d3-4f0d-b8c0-6e4bd464a92e",
                filters = new List<OsNotificationFilterDto>()
                {
                    new OsNotificationFilterDto()
                    {
                        field = "tag",
                        key = "userid",
                        relation = "=",
                        value = userId.ToString()
                    }
                },
                data = new object(),
                contents = new OsNotificationContentsDto()
                {
                    // TODO: english and spanish notification?
                    en = msg,
                    es = msg
                },
                send_after = ts
            };

            var client = new RestClient("https://onesignal.com");
            var request = new RestRequest("api/v1/notifications", Method.POST, DataFormat.Json);
            request.AddJsonBody(content);
            var response = client.Execute<OsNotificationResponseDto>(request);
            return response.Data.recipients == 1;
        }

        // TODO: This is business logic and must be outside this class.
        public static void ScheduleNotifications(int userId, DateTime appointment)
        {
            // TODO: Put all messages in the same place. ARG or ES language?
            var msg = "Recordá! Tenés un turno en una hora!";
            var timestamp = appointment.AddHours(-1);
            if (!OneSignalService.ScheduleNotification(userId, timestamp, msg))
            {
                // TODO: Maybe send the tag, and retry.
                Console.WriteLine("Schedule OneSignal notification (1h) failed for user: " + userId);
            }

            msg = "Recordá! Tenés un turno en 24hs!";
            timestamp = appointment.AddDays(-1);
            if (!OneSignalService.ScheduleNotification(userId, timestamp, msg))
            {
                // TODO: Maybe send the tag, and retry.
                Console.WriteLine("Schedule OneSignal notification (24h) failed for user: " + userId);
            }

            msg = "Turno reservado con éxito!";
            if (!OneSignalService.SendNotification(userId, msg))
            {
                // TODO: Maybe send the tag, and retry.
                Console.WriteLine("Send OneSignal notification failed for user: " + userId);
            }
        }
    }
}

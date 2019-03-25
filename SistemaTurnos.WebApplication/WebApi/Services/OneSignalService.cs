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
            var ts = "";
            if (timestamp.HasValue)
            {
                ts = timestamp.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
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
            request.AddHeader("Authorization", "Basic YTI5NDEyNmQtNDhhYi00YTc5LWFiMGItY2RkNTM4OTM0Y2M5");
            request.AddJsonBody(content);
            var response = client.Execute<OsNotificationResponseDto>(request);
            return response.Data.recipients == 1;
        }

        // TODO: This is business logic and must be outside this class.
        public static void ScheduleNotifications(int userId, DateTime appointment)
        {
            DateTime dt = appointment;
            if (appointment.Kind != DateTimeKind.Utc)
            {
                dt = appointment.ToUniversalTime();
            }

            // TODO: Put all messages in the same place. ARG or ES language?
            var timestamp = dt.AddHours(-1);
            if (timestamp < appointment)
            {
                var msg = "Recordá! Tenés un turno en una hora!";
                if (!OneSignalService.ScheduleNotification(userId, timestamp, msg))
                {
                    // TODO: Maybe send the tag, and retry.
                    Console.WriteLine("Schedule OneSignal notification (1h) failed for user: " + userId);
                }
            }

            timestamp = dt.AddDays(-1);
            if (timestamp < appointment)
            {
                var msg = "Recordá! Tenés un turno en 24hs!";
                if (!OneSignalService.ScheduleNotification(userId, timestamp, msg))
                {
                    // TODO: Maybe send the tag, and retry.
                    Console.WriteLine("Schedule OneSignal notification (24h) failed for user: " + userId);
                }
            }

            if (!OneSignalService.SendNotification(userId, "Turno reservado con éxito!"))
            {
                // TODO: Maybe send the tag, and retry.
                Console.WriteLine("Send OneSignal notification failed for user: " + userId);
            }
        }
    }
}

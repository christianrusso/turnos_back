﻿namespace SistemaTurnos.WebApplication.WebApi.Dto.Statistics
{
    public class StatisticsFullDto : BaseDto
    {
        public int Professionals { get; set; }

        public int Patients { get; set; }

        public int Specialties { get; set; }

        public int MedicalInsurances { get; set; }

        public int Appointments { get; set; }

        public int CompletedAppointments { get; set; }

        public int CanceledAppointments { get; set; }

        public int TodayAppointments { get; set; }
    }
}

using System;
using System.Collections.Generic;
using SistemaTurnos.Database.Enums;
using SistemaTurnos.WebApplication.WebApi.Dto.Common;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Hairdressing
{
    public class FilterHairdressingDto : BaseDto
    {
        public int? HairdressingId { get; set; }

        public List<int> Specialties { get; set; }
    
        public List<int> Subspecialties { get; set; }

        public List<int> Cities { get; set; }

        public GeoLocationDto Location { get; set; }

        public double? Score { get; set; }

        public int? ScoreQuantity { get; set; }

        public DateTime? AvailableAppointmentStartDate { get; set; }

        public DateTime? AvailableAppointmentEndDate { get; set; }

        public string SortField { get; set; }

        public bool? AscendingOrder { get; set; }

        public int? From { get; set; }

        public int? To { get; set; }

        public List<int> Stars { get; set; }

        public int? BusinessTypeId { get; set; }
    }
}

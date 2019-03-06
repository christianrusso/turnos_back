using System;
using System.Collections.Generic;
using SistemaTurnos.Database.Enums;
using SistemaTurnos.WebApplication.WebApi.Dto.Common;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Hairdressing
{
    public class HairdressingDto : BaseDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int HairdressingId { get; set; }

        public string Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double DistanceToUser { get; set; }

        public string City { get; internal set; }

        public string Logo { get; internal set; }

        public double Score { get; set; }

        public BusinessType? BusinessType { get; set; }
    }
}

﻿using SistemaTurnos.WebApplication.WebApi.Dto.Rating;
using System.Collections.Generic;
using SistemaTurnos.WebApplication.WebApi.Dto.Common;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Hairdressing
{
    public class FullHairdressingDto : BaseDto
    {
        public int HairdressingId { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double DistanceToUser { get; set; }

        public List<string> Specialties { get; set; }

        public List<string> Subspecialties { get; set; }

        public double Score { get; set; }

        public int ScoreQuantity { get; set; }

        public string Logo {get; set;}

        public List<RatingDto> Ratings { get; set; }

        public List<OpenCloseHoursDto> OpenCloseHours { get; set; }
    }
}
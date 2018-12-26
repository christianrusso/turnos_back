﻿using SistemaTurnos.WebApplication.WebApi.Dto.Common;
using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Account
{
    public class ProfileDto : BaseDto
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }

        public string City { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Logo { get; set; }

        public List<OpenCloseHoursDto> OpenCloseHours { get; set; }

        public List<string> Images { get; set; }

        public bool Require { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}

﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Clinic
{
    public class OpenCloseHoursDto
    {
        [Required]
        [Range(0, 6)]
        public DayOfWeek DayNumber { get; set; }

        [Required]
        public TimeSpan Start { get; set; }

        [Required]
        public TimeSpan End { get; set; }
    }
}

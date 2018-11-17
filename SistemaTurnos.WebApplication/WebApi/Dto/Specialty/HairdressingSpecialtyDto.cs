﻿using SistemaTurnos.WebApplication.WebApi.Dto.Subspecialty;
using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Specialty
{
    public class HairdressingSpecialtyDto
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public int Doctors { get; set; }

        public int Professionales { get; set; }

        public List<HairdressingSubspecialtyDto> Subspecialties { get; set; }
    }
}

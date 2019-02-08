using SistemaTurnos.WebApplication.WebApi.Dto.Common;
using System;
using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Doctor
{
    public class DoctorSubspecialtyBlockedInfoDto
    {
        public int SubspecialtyId { get; set; }

        public string SubspecialtyDescription { get; set; }

        public int Doctor { get; set; }
    }
}

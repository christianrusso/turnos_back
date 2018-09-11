using SistemaTurnos.WebApplication.WebApi.Dto.Common;
using System;
using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingProfessional
{
    public class HairdressingProfessionalDto : BaseDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public int SpecialtyId { get; set; }

        public string SpecialtyDescription { get; set; }

        public int? SubspecialtyId { get; set; }

        public string SubspecialtyDescription { get; set; }

        public uint ConsultationLength { get; set; }

        public bool State { get; set; }

        public List<WorkingHoursDto> WorkingHours { get; set; }

        public List<DateTime> Appointments { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaTurnos.WebApplication.WebApi.Dto.Appointment;
using SistemaTurnos.WebApplication.WebApi.Dto.HairdressingAppointment;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Common
{
    public class WeekForClientDto
    {
        public List<HairdressingClientDayDto> Hairdressing_GetWeekForClient { get; internal set; }
    }
}

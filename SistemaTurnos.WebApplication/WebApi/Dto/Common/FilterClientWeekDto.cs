using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Common
{
    public class FilterClientWeekDto
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}

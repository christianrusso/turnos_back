using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaTurnos.WebApplication.WebApi.Dto.Clinic;
using SistemaTurnos.WebApplication.WebApi.Dto.Hairdressing;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Common
{
    public class FavoritesDto : BaseDto
    {
        public List<HairdressingDto> HairdressingFavorites { get; internal set; }
        public List<ClinicDto> ClinicFavorites { get; internal set; }
    }
}

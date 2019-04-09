using System.Collections.Generic;
using SistemaTurnos.WebApplication.WebApi.Dto.Clinic;
using SistemaTurnos.WebApplication.WebApi.Dto.Hairdressing;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Common
{
    public class FavoritesDto : BaseDto
    {
        public List<HairdressingDto> HairdressingFavorites { get; internal set; }
    }
}

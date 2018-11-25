using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Dto
{
    public class OptionalIdDtoAndRubro : BaseDto
    {
        public List<int> Ids { get; set; }

        public int Rubro { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto
{
    public class OptionalIdDtoAndRubro : BaseDto
    {
        public int? Id { get; set; }

        public int Rubro { get; set; }
    }
}

namespace SistemaTurnos.WebApplication.WebApi.Dto
{
    public class RequireDto : BaseDto
    {
        public bool Require { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}

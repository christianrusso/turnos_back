namespace SistemaTurnos.WebApplication.WebApi.Dto.Account
{
    public class LogOnDto : BaseDto
    {
        public string Token { get; set; }

        public string Logo { get; set; }

        public int UserId { get; set; }
    }
}

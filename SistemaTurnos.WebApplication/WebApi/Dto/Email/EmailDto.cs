using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Email
{
    public class EmailDto : BaseDto
    {
        public string From { get; set; }

        public List<string> To { get; set; }
        
        public string Subject { get; set; }

        public string Message { get; set; }
    }
}

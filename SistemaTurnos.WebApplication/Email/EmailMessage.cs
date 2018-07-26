using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.Email
{
    public class EmailMessage
    {
        public IEnumerable<string> To { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }
    }
}

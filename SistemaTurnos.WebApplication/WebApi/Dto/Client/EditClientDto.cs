using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Client
{
    public class EditClientDto : BaseDto
    {
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string Address { get; set; }
        
        public string PhoneNumber { get; set; }
       
        public string Dni { get; set; }
    }
}

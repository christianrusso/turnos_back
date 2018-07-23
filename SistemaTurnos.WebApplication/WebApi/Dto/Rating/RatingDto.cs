using System;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Rating
{
    public class RatingDto : BaseDto
    {
        public string User { get; set; }

        public uint Score { get; set; }
        
        public string Comment { get; set; }

        public DateTime DateTime { get; set; }
    }
}

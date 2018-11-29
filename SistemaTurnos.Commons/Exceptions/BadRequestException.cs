using System;

namespace SistemaTurnos.Commons.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException() : base(ExceptionMessages.BadRequest)
        { }

        public BadRequestException(string message) : base(message)
        { }

        public BadRequestException(Exception innerException): base(ExceptionMessages.BadRequest, innerException)
        { }
    }
}

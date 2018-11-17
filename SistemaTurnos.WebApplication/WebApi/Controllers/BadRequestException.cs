using System;
using System.Runtime.Serialization;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Serializable]
    internal class BadRequestException : Exception
    {
        private object badRequest;

        public BadRequestException()
        {
        }

        public BadRequestException(object badRequest)
        {
            this.badRequest = badRequest;
        }

        public BadRequestException(string message) : base(message)
        {
        }

        public BadRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BadRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
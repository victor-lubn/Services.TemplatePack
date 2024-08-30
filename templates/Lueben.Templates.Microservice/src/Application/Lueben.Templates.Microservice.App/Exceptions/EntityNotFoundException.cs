using System;
using Lueben.Templates.Microservice.App.Constants;

namespace Lueben.Templates.Microservice.App.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException() : base(ErrorMessages.EntityNotFoundError)
        {
        }
    }
}
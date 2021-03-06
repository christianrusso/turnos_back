﻿namespace SistemaTurnos.WebApplication.WebApi.Exceptions
{
    public static class ExceptionMessages
    {
        public const string BadRequest = "Los valores ingresados son incorrectos.";
        public const string UsernameAlreadyExists = "El usuario ingresado ya se encuentra registrado.";
        public const string LoginFailed = "Nombre de usuario o contraseña invalidos.";
        public const string InternalServerError = "Ocurrio un error al realizar la operacion, por favor intente mas tarde.";
        public const string UserDoesNotExists = "El usuario no existe.";
        public const string AppointmentAlreadyTaken = "El turno solicitado no esta disponible.";
        public const string AppointmentCantBeCanceled = "Los turnos pueden cancelarse hasta 24 horas antes.";
        public const string AppointmentCantBeCompleted = "El turno aun no puede ser completado.";
        public const string AppointmentCantBeRequested = "No es posible solicitar turnos para fechas anteriores a la fecha actual.";
        public const string RolesHaveNotBeenCreated = "Los roles aun no han sido creados.";
    }
}

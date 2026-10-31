namespace Inventory.Domain.Exceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("Usuario o contraseña incorrectos.")
    {
    }
}

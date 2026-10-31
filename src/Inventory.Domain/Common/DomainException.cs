using System;

namespace Inventory.Domain.Common;

// [DDD] Excepción base para todas las excepciones de dominio.
// A diferencia de las exceptions de .NET, estas representan violaciones de
// invariantes de negocio y deben ser esperadas y manejadas.
// Se recomienda derivar excepciones específicas para cada caso (ver GLOSARIO.md).
public class DomainException : Exception
{
    public DomainException() { }
    public DomainException(string message) : base(message) { }
    public DomainException(string message, System.Exception inner) : base(message, inner) { }
}
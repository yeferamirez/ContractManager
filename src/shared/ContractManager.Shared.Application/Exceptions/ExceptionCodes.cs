using System.ComponentModel;

namespace ContractManager.Shared.Application.Exceptions;
public enum ExceptionCodes
{
    [Description("Error al intentar conectar a la base de datos")]
    DatabaseError = 1
}

using ContractManager.Shared.Core;

namespace ContractManager.Data.Entities;
public class UserRole : IEntity
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}

using ContractManager.Shared.Core;

namespace ContractManager.Data.Entities;
public class Permission : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}

using ContractManager.Shared.Core;

namespace ContractManager.Data.Entities;
public class Document : IEntity, IFileEntity
{
    public Guid Id { get; set; }

    public Guid ContractId { get; set; }

    public required string FileName { get; set; }

    public required string Name { get; set; }

    public required string MimeType { get; set; }

    public string? TempFileName { get; set; }

    public DateTime CreationDate { get; set; }

    public static string[] GetValidExtensions()
    {
        return new string[] {
            "jpg",
            "png",
            "jpeg",
            "csv",
            "pdf"
        };
    }
}

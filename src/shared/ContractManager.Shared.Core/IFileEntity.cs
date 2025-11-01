namespace ContractManager.Shared.Core;

public interface IFileEntity
{
    Guid Id { get; }

    string FileName { get; }

    string Name { get; }

    string MimeType { get; }
}

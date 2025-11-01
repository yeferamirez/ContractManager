namespace ContractManager.Integration.Tests.Models;
public class ApiResponseModel<T>
{
    public T? Content { get; set; }

    public HttpResponseMessage HttpResponse { get; set; } = default!;
}

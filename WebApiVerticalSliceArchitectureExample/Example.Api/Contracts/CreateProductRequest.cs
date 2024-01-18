namespace Example.Api.Contracts;

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
}

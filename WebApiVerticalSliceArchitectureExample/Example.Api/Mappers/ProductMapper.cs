using Example.Api.Contracts;
using Example.Api.Entities;

namespace Example.Api.Mappers;

public class ProductMapper
{
    internal static ProductResponse MapToResponse(Product product)
    {
        return new ProductResponse
        {
            ProductId = product.ProductId,
            Name = product.Name,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}

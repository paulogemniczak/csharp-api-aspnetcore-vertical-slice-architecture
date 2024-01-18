using Example.Api.Entities;
using Example.Api.Features.Categories;

namespace Example.Api.Mappers;

public class CategoryMapper
{
    public static Category ToEntityUdate(Category category, UpdateCategory.Command request)
    {
        category.Name = request.Name;
        category.UpdatedAt = DateTime.UtcNow;
        return category;
    }
}

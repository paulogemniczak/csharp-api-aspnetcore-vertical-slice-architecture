using Carter;
using Example.Api.Contracts;
using Example.Api.Database;
using Example.Api.Shared;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Example.Api.Features.Categories;

public class GetCategory
{
    public class Query : IRequest<Result<CategoryResponse>>
    {
        public int CategoryId { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<CategoryResponse>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<CategoryResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var category = await _dbContext
                .Categories
                .FirstOrDefaultAsync(p => p.CategoryId == request.CategoryId, cancellationToken);

            if (category is null)
                return Result<CategoryResponse>.Failure("Category not found");

            return Result.SuccessResult(category.Adapt<CategoryResponse>());
        }
    }
}

public class GetCategoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/categories/{categoryId:int}", async ([AsParameters] GetCategory.Query query, ISender sender) =>
        {
            var result = await sender.Send(query);

            if (!result.Success)
                return Results.BadRequest(result.Errors);

            return Results.Ok(result.Value);
        });
    }
}

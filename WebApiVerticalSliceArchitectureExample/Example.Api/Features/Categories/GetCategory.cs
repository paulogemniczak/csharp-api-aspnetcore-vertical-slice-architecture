using Carter;
using Example.Api.Database;
using Example.Api.Entities;
using Example.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Example.Api.Features.Categories;

public class GetCategory
{
    public class Query : IRequest<Result<Category>>
    {
        public int CategoryId { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<Category>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Category>> Handle(Query request, CancellationToken cancellationToken)
        {
            var category = await _dbContext
                .Categories
                .FirstOrDefaultAsync(p => p.CategoryId == request.CategoryId, cancellationToken);

            if (category is null)
                return Result<Category>.Failure("Category not found");

            return Result.SuccessResult(category);
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

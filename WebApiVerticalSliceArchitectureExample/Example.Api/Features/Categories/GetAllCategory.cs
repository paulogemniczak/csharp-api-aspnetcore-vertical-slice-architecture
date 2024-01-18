using Carter;
using Example.Api.Contracts;
using Example.Api.Database;
using Example.Api.Shared;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Example.Api.Features.Categories;

public class GetAllCategory
{
    public class Query : IRequest<Result<List<CategoryResponse>>>
    {
        public string? CategoryName { get; set; } = null;
    }

    public class Handler : IRequestHandler<Query, Result<List<CategoryResponse>>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<CategoryResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var categories = await _dbContext
                .Categories
                .Where(p => request.CategoryName == null || p.Name.ToLower().Contains(request.CategoryName.ToLower()))
                .ToListAsync(cancellationToken);

            return Result.SuccessResult(categories.Adapt<List<CategoryResponse>>());
        }
    }
}

public class GetAllCategoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/categories", async ([AsParameters] GetAllCategory.Query query, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(query);

            if (!result.Success)
                return Results.BadRequest(result.Errors);

            return Results.Ok(result.Value);
        });
    }
}

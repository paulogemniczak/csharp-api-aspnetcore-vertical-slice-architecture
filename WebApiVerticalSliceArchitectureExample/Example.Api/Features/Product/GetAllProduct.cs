using Carter;
using Example.Api.Contracts;
using Example.Api.Database;
using Example.Api.Mappers;
using Example.Api.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Example.Api.Features.Product;

public class GetAllProduct
{
    public class Query : IRequest<Result<List<ProductResponse>>>
    {
    }

    public class Handler : IRequestHandler<Query, Result<List<ProductResponse>>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<ProductResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var products = await _dbContext
                .Products
                .Include(p => p.Category)
                .ToListAsync(cancellationToken);

            return Result.SuccessResult(products.Select(ProductMapper.MapToResponse).ToList());
        }
    }
}

public class GetAllProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products", async ([AsParameters] GetAllProduct.Query query, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(query);

            if (!result.Success)
                return Results.BadRequest(result.Errors);

            return Results.Ok(result.Value);
        });
    }
}
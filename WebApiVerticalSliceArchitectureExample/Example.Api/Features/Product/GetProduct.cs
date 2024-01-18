using Carter;
using Example.Api.Contracts;
using Example.Api.Database;
using Example.Api.Mappers;
using Example.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Example.Api.Features.Product;

public class GetProduct
{
    public class Query : IRequest<Result<ProductResponse>>
    {
        public int ProductId { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<ProductResponse>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<ProductResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.ProductId == request.ProductId, cancellationToken);

            if (product is null)
                return Result<ProductResponse>.Failure("Product not found");


            return Result.SuccessResult(ProductMapper.MapToResponse(product));
        }
    }
}

public class GetProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products/{productId:int}", async([AsParameters] GetProduct.Query query, ISender sender) =>
        {
            var result = await sender.Send(query);

            if (!result.Success)
                return Results.BadRequest(result.Errors);

            return Results.Ok(result.Value);
        });
    }
}

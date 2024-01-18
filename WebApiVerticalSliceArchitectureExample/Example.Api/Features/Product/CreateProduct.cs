using Carter;
using Example.Api.Contracts;
using Example.Api.Database;
using Example.Api.Shared;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Example.Api.Features.Product;

public class CreateProduct
{
    public class Command : IRequest<Result>
    {
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var category = await _dbContext
                .Categories
                .FirstOrDefaultAsync(p => p.CategoryId == request.CategoryId, cancellationToken);

            if (category is null)
                return Result.Failure("Category not found");

            var product = new Example.Api.Entities.Product
            {
                Name = request.Name,
                CategoryId = request.CategoryId
            };

            await _dbContext.Products.AddAsync(product, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.SuccessResult();
        }
    }
}

public class CreateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/products", async([FromBody] CreateProductRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateProduct.Command>();
            var result = await sender.Send(command);

            if (!result.Success)
                return Results.BadRequest(result.Errors);

            return Results.Ok();
        });
    }
}

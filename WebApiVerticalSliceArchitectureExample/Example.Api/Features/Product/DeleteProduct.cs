using Carter;
using Example.Api.Database;
using Example.Api.Shared;
using MediatR;

namespace Example.Api.Features.Product;

public class DeleteProduct
{
    public class Command : IRequest<Result>
    {
        public int ProductId { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.FindAsync(request.ProductId);

            if (product is null)
                return Result.Failure("Product not found");

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.SuccessResult();
        }
    }
}

public class DeleteProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/products/{productId:int}", async([AsParameters] DeleteProduct.Command command, ISender sender) =>
        {
            var result = await sender.Send(command);

            if (!result.Success)
                return Results.BadRequest(result.Errors);

            return Results.Ok();
        });
    }
}

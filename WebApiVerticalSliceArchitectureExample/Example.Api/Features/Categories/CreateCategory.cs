using Carter;
using Example.Api.Database;
using Example.Api.Entities;
using Example.Api.Shared;
using Mapster;
using MediatR;

namespace Example.Api.Features.Categories;

public static class CreateCategory
{
    public class Command : IRequest<Result<int>>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class Handler : IRequestHandler<Command, Result<int>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
        {
            var category = new Category
            {
                Name = request.Name
            };

            await _dbContext.Categories.AddAsync(category, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.SuccessResult(category.CategoryId);
        }
    }
}

public class CreateCategoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/categories", async (CreateCategoryRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateCategory.Command>();
            var result = await sender.Send(command);

            if (!result.Success)
                return Results.BadRequest(result.Errors);

            return Results.Ok(result.Value);
        });
    }
}

using Carter;
using Example.Api.Database;
using Example.Api.Mappers;
using Example.Api.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Example.Api.Features.Categories;

public class UpdateCategory
{
    public class Command : IRequest<Result>
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
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

            category = CategoryMapper.ToEntityUdate(category, request);

            _dbContext.Categories.Update(category);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.SuccessResult();
        }
    }
}

public class UpdateCategoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/categories/{categoryId:int}", async(int categoryId, [FromBody] UpdateCategory.Command command, [FromServices] ISender sender) =>
        {
            command.CategoryId = categoryId;
            var result = await sender.Send(command);

            if (!result.Success)
                return Results.BadRequest(result.Errors);

            return Results.Ok();
        });
    }
}

﻿using Carter;
using Example.Api.Database;
using Example.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Example.Api.Features.Categories;

public class DeleteCategory
{
    public class Command : IRequest<Result>
    {
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

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.SuccessResult();
        }
    }
}

public class DeleteCategoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/categories/{categoryId:int}", async([AsParameters] DeleteCategory.Command command, ISender sender) =>
        {
            var result = await sender.Send(command);

            if (!result.Success)
                return Results.BadRequest(result.Errors);

            return Results.Ok();
        });
    }
}

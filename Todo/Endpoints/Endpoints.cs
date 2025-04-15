using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Dtos;
using Todo.Services;

namespace Todo.Endpoints;

public static class Endpoints
{
	public static void MapEndpoints(this WebApplication app)
	{
		app.MapGet("/todos", async (
				[FromQuery] DateTime? date,
				[FromServices] ITodoService todoService,
				[FromServices] IValidator<DateTime?> validator) =>
			{
				if (date.HasValue)
				{
					var validationResult = await validator.ValidateAsync(date);
					if (!validationResult.IsValid)
					{
						return Results.ValidationProblem(ToDictionary(validationResult));
					}	
				}
				
				var todos = await todoService.GetAllAsync(date);
				return Results.Ok(todos);
			})
			.Produces<List<ReadTodoItemDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem();
		
		app.MapGet("/todos/{id}", async (Guid id,
			[FromServices] ITodoService todoService) =>
		{
			var todo = await todoService.GetByIdAsync(id);
			return todo is not null ? Results.Ok(todo) : Results.NotFound();
		})
		.Produces<ReadTodoItemDto>()
		.Produces(StatusCodes.Status404NotFound);
		
		app.MapPost("/todos", async (
			CreateTodoItemDto dto,
			[FromServices] ITodoService todoService,
			[FromServices] IValidator<CreateTodoItemDto> validator) =>
		{
			var validationResult = await validator.ValidateAsync(dto);

			if (!validationResult.IsValid)
			{
				return Results.ValidationProblem(ToDictionary(validationResult));
			}

			var result = await todoService.CreateAsync(dto);
			return Results.Created($"/todos/{result.Id}", result);
		})
		.Produces<ReadTodoItemDto>(StatusCodes.Status200OK)
		.ProducesValidationProblem();
		
		app.MapPut("/todos/{id}", async (
			Guid id, 
			UpdateTodoItemDto dto,
			[FromServices] ITodoService todoService,
			[FromServices] IValidator<UpdateTodoItemDto> validator) => 
		{
			var validationResult = await validator.ValidateAsync(dto);

			if (!validationResult.IsValid)
			{
				return Results.ValidationProblem(ToDictionary(validationResult));
			}

			var updated = await todoService.UpdateAsync(id, dto);
			return updated ? Results.NoContent() : Results.NotFound();
		})
		.Produces(StatusCodes.Status204NoContent)
		.Produces(StatusCodes.Status404NotFound)
		.ProducesValidationProblem();;
		
		app.MapDelete("todos/{id}", async (
			Guid id,
			[FromServices] ITodoService todoService,
			[FromServices] ApplicationDataContext context) =>
		{
			var deleted = await todoService.DeleteAsync(id);
			return deleted ? Results.NoContent() : Results.NotFound();
		})
		.Produces(StatusCodes.Status204NoContent)
		.Produces(StatusCodes.Status404NotFound);
	}

	private static Dictionary<string, string[]> ToDictionary(ValidationResult validationResult)
	{
		return validationResult.Errors
			.GroupBy(x => x.PropertyName)
			.ToDictionary(
				g => g.Key,
				g => g.Select(x => x.ErrorMessage).ToArray());
	}
}
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Dtos;
using Todo.Models.Identity;
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
			.Produces(StatusCodes.Status401Unauthorized)
			.Produces(StatusCodes.Status403Forbidden)
			.ProducesValidationProblem()
			.RequireAuthorization(policy => 
				policy
					.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
					.RequireRole(Roles.User.ToString()));
		
		app.MapGet("/todos/{id}", async (Guid id,
			[FromServices] ITodoService todoService) =>
		{
			var todo = await todoService.GetByIdAsync(id);
			return todo is not null ? Results.Ok(todo) : Results.NotFound();
		})
		.Produces<ReadTodoItemDto>()
		.Produces(StatusCodes.Status401Unauthorized)
		.Produces(StatusCodes.Status403Forbidden)
		.Produces(StatusCodes.Status404NotFound)
		.RequireAuthorization(policy => 
			policy
				.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
				.RequireRole(Roles.User.ToString()));
		
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
		.ProducesValidationProblem()
		.Produces(StatusCodes.Status403Forbidden)
		.Produces(StatusCodes.Status404NotFound)
		.RequireAuthorization(policy => 
			policy
				.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
				.RequireRole(Roles.User.ToString()));
		
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
		.ProducesValidationProblem()
		.Produces(StatusCodes.Status403Forbidden)
		.Produces(StatusCodes.Status404NotFound)
		.RequireAuthorization(policy => 
			policy
				.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
				.RequireRole(Roles.User.ToString()));
		
		app.MapDelete("todos/{id}", async (
			Guid id,
			[FromServices] ITodoService todoService,
			[FromServices] ApplicationDataContext context) =>
		{
			var deleted = await todoService.DeleteAsync(id);
			return deleted ? Results.NoContent() : Results.NotFound();
		})
		.Produces(StatusCodes.Status204NoContent)
		.Produces(StatusCodes.Status404NotFound)
		.Produces(StatusCodes.Status403Forbidden)
		.Produces(StatusCodes.Status404NotFound)
		.RequireAuthorization(policy => 
			policy
				.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
				.RequireRole(Roles.User.ToString()));

		app.MapPost("users/login", async (
			LoginRequest loginRequest,
			[FromServices] UserManager<User> userManager,
			[FromServices] SignInManager<User> signInManager,
			[FromServices] IJwtGeneratorService jwtGeneratorService) =>
		{
			var user = await userManager.FindByEmailAsync(loginRequest.Email);

			if (user == null)
			{
				return Results.Unauthorized();
			}

			var result = await signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);

			if (result.Succeeded)
			{
				return Results.Ok(new UserResponse(await jwtGeneratorService.CreateToken(user), user.UserName));
			}

			return Results.Unauthorized();
		})
		.Produces(StatusCodes.Status200OK)
		.Produces(StatusCodes.Status401Unauthorized)
		.AllowAnonymous();
		
		app.MapPost("users/register", async (
				RegisterRequest registerRequest,
				[FromServices] UserManager<User> userManager,
				[FromServices] IJwtGeneratorService jwtGeneratorService) =>
			{
				var existingUser = await userManager.FindByEmailAsync(registerRequest.Email);

				if (existingUser != null)
				{
					return Results.BadRequest(new { Message = "User with this email already exists." });
				}

				var user = new User
				{
					Email = registerRequest.Email,
					UserName = registerRequest.UserName
				};
				
				var result = await userManager.CreateAsync(user, registerRequest.Password);

				if (!result.Succeeded)
				{
					var errors = result.Errors.Select(x => x.Description);
					return Results.BadRequest(new { Errors = errors });
				}

				await userManager.AddToRoleAsync(user, Roles.User.ToString());
				var token = await jwtGeneratorService.CreateToken(user);
				return Results.Ok(new UserResponse(token, user.UserName));
			})
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest)
			.AllowAnonymous();
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
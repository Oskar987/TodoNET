using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Dtos;
using Todo.Models;

namespace Todo.Endpoints;

public static class Endpoints
{
	public static void MapEndpoints(this WebApplication app)
	{
		app.MapGet("/todos", ([FromServices] ApplicationDataContext context) =>
		{
			var todos = context.TodoItems.ToList();
			var todoItemDtos = new List<ReadTodoItemDto>();
			foreach (var todo in todos)
			{
				 todoItemDtos.Add(new ReadTodoItemDto(todo.Id, todo.Title, todo.Description, todo.IsDone));
			}
			return Results.Ok(todoItemDtos);
		});
		
		app.MapGet("/todos/{id}", (Guid id, 
			[FromServices] ApplicationDataContext context) => 
		{
			var todo = context.TodoItems.FirstOrDefault(x => x.Id == id);
			return todo is not null ? 
				Results.Ok(new ReadTodoItemDto(todo.Id, todo.Title, todo.Description, todo.IsDone)) 
				: Results.NotFound();
		});
		
		app.MapPost("/todos", (CreateTodoItemDto dto, [FromServices] ApplicationDataContext context) =>
		{
			var todoItem = new TodoItem
			{
				Title = dto.Title,
				Description = dto.Description
			};
			
			context.TodoItems.Add(todoItem);
			context.SaveChanges();
			
			return Results.Created($"/todos/{todoItem.Id}",
				new ReadTodoItemDto(todoItem.Id, todoItem.Title, todoItem.Description, todoItem.IsDone));
		});
		
		app.MapPut("/todos/{id}", (Guid id, Models.TodoItem updatedTodo, [FromServices] ApplicationDataContext context) => 
		{
			var todo = context.TodoItems.FirstOrDefault(x => x.Id == id);
			
			if (todo is not null)
			{
				context.TodoItems.Remove(todo);
				context.TodoItems.Add(updatedTodo);

				context.SaveChanges();
				
				return Results.NoContent();
			}

			return Results.NotFound();
		});
		
		app.MapDelete("todos/{id}", (Guid id, [FromServices] ApplicationDataContext context) => 
		{
			var todo = context.TodoItems.FirstOrDefault(x => x.Id == id);
			if (todo is not null)
			{
				context.TodoItems.Remove(todo);
				context.SaveChanges();
				
				return Results.NoContent();
			}

			return Results.NotFound();            
		});
	}
}
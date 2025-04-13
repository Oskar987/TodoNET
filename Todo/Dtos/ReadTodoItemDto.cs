namespace Todo.Dtos;

public record ReadTodoItemDto(Guid Id, string Title, string Description, bool IsDone);
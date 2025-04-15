using Todo.Dtos;

namespace Todo.Services;

public interface ITodoService
{
	public Task<ReadTodoItemDto> CreateAsync(CreateTodoItemDto dto);
	public Task<List<ReadTodoItemDto>> GetAllAsync(DateTime? fromDate);
	public Task<ReadTodoItemDto?> GetByIdAsync(Guid id);
	public Task<bool> DeleteAsync(Guid id);
	public Task<bool> UpdateAsync(Guid id, UpdateTodoItemDto dto);
}
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Todo.Data;
using Todo.Dtos;
using Todo.Models;

namespace Todo.Services;

public class TodoService : ITodoService
{
	private readonly ApplicationDataContext _applicationDataContext;
	private readonly IMapper _mapper;
	private readonly IDateTimeService _dateTimeService;

	public TodoService(IDateTimeService dateTimeService, IMapper mapper, ApplicationDataContext applicationDataContext)
	{
		_dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		_applicationDataContext = applicationDataContext ?? throw new ArgumentNullException(nameof(applicationDataContext));
	}
	
	
	public async Task<ReadTodoItemDto> CreateAsync(CreateTodoItemDto dto)
	{
		var entity = _mapper.Map<TodoItem>(dto);
		await _applicationDataContext.TodoItems.AddAsync(entity);
		await _applicationDataContext.SaveChangesAsync();

		return _mapper.Map<ReadTodoItemDto>(entity);
	}

	public async Task<List<ReadTodoItemDto>> GetAllAsync(DateTime? fromDate)
	{
		var dateToCompare = fromDate ?? _dateTimeService.UtcNow.Date;

		var todoItems = await _applicationDataContext.TodoItems
			.Where(x => x.CreatedAt >= dateToCompare)
			.OrderByDescending(x => x.CreatedAt)
			.ToListAsync();

		return _mapper.Map<List<ReadTodoItemDto>>(todoItems);
	}

	public async Task<ReadTodoItemDto?> GetByIdAsync(Guid id)
	{
		var entity = await _applicationDataContext.TodoItems.FindAsync(id);
		return entity == null ? null : _mapper.Map<ReadTodoItemDto>(entity);
	}

	public async Task<bool> DeleteAsync(Guid id)
	{
		var entity = await _applicationDataContext.TodoItems.FindAsync(id);
		if (entity == null)
		{
			return false;
		}

		_applicationDataContext.TodoItems.Remove(entity);
		await _applicationDataContext.SaveChangesAsync();
		return true;
	}

	public async Task<bool> UpdateAsync(Guid id, UpdateTodoItemDto dto)
	{
		var entity = await _applicationDataContext.TodoItems.FindAsync(id);
		if (entity == null)
		{
			return false;
		}

		entity.Description = dto.Description;
		entity.Title = dto.Title;
		entity.IsDone = dto.IsDone;

		_applicationDataContext.TodoItems.Update(entity);
		await _applicationDataContext.SaveChangesAsync();
		return true;
	}
}
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Todo.Data;
using Todo.Dtos;
using Todo.Models;

namespace Todo.Services;

public class TodoService : ITodoService
{
	private readonly ApplicationDataContext _applicationDataContext;
	private readonly IUserContext _userContext;
	private readonly IMapper _mapper;
	private readonly IDateTimeService _dateTimeService;

	public TodoService(IDateTimeService dateTimeService, IMapper mapper, ApplicationDataContext applicationDataContext, IUserContext userContext)
	{
		_dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		_applicationDataContext = applicationDataContext ?? throw new ArgumentNullException(nameof(applicationDataContext));
		_userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
	}
	
	
	public async Task<ReadTodoItemDto> CreateAsync(CreateTodoItemDto dto)
	{
		var entity = _mapper.Map<TodoItem>(dto);

		entity.UserId = _userContext.UserId;
		
		await _applicationDataContext.TodoItems.AddAsync(entity);
		await _applicationDataContext.SaveChangesAsync();

		return _mapper.Map<ReadTodoItemDto>(entity);
	}

	public async Task<List<ReadTodoItemDto>> GetAllAsync(DateTime? fromDate)
	{
		var dateToCompare = fromDate ?? _dateTimeService.UtcNow.Date;

		var todoItems = await _applicationDataContext.TodoItems
			.Where(x => x.CreatedAt >= dateToCompare && x.UserId == _userContext.UserId)
			.OrderByDescending(x => x.CreatedAt)
			.ToListAsync();

		return _mapper.Map<List<ReadTodoItemDto>>(todoItems);
	}

	public async Task<ReadTodoItemDto?> GetByIdAsync(Guid id)
	{
		var entity = await _applicationDataContext.TodoItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == _userContext.UserId);
		return entity == null ? null : _mapper.Map<ReadTodoItemDto>(entity);
	}

	public async Task<bool> DeleteAsync(Guid id)
	{
		var entity = await _applicationDataContext.TodoItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == _userContext.UserId);
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
		var entity = await _applicationDataContext.TodoItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == _userContext.UserId);
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
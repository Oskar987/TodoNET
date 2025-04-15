using AutoMapper;
using Todo.Dtos;
using Todo.Models;

namespace Todo.Mapping;

public class TodoMappingProfile : Profile
{
	public TodoMappingProfile()
	{
		CreateMap<CreateTodoItemDto, TodoItem>()
			.ForMember(dest => dest.Id, opt => opt.Ignore())
			.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
			.ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
			.ForMember(dest => dest.IsDone, opt => opt.Ignore());

		CreateMap<UpdateTodoItemDto, TodoItem>()
			.ForMember(dest => dest.Id, opt => opt.Ignore())
			.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
			.ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

		CreateMap<TodoItem, ReadTodoItemDto>();
	}
}
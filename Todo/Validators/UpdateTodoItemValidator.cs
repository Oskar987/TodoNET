using FluentValidation;
using Todo.Dtos;

namespace Todo.Validators;

public class UpdateTodoItemValidator : AbstractValidator<UpdateTodoItemDto>
{
	public UpdateTodoItemValidator()
	{
		RuleFor(x => x.Title)
			.NotEmpty()
			.WithMessage("Title is required")
			.Length(10, 100)
			.WithMessage("Title must be between 10 and 100 characters.");
		
		RuleFor(x => x.Description)
			.NotEmpty()
			.WithMessage("Description is required")
			.Length(10, 500)
			.WithMessage("Description must be between 10 and 500 characters.");

		RuleFor(x => x.IsDone)
			.NotNull()
			.WithMessage("IsDone cannot be null.");
	}
}
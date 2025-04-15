using FluentValidation;

namespace Todo.Validators;

public class GetAllTodoItemsValidator : AbstractValidator<DateTime?>
{
	public GetAllTodoItemsValidator()
	{
		RuleFor(x => x)
			.Must(date => date == null || date <= DateTime.UtcNow.Date)
			.WithMessage("Date cannot be in the future.");
	}
}
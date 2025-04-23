using Todo.Models.Identity;

namespace Todo.Services;

public interface IJwtGeneratorService
{
	Task<string> CreateToken(User user);
}
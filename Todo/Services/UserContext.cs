using System.Security.Claims;

namespace Todo.Services;

public class UserContext : IUserContext
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public UserContext(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}
	
	public Guid UserId {
		get
		{
			var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			return Guid.TryParse(userId, out var id)
				? id 
				: throw new UnauthorizedAccessException("User ID not found");
		}
	}
}
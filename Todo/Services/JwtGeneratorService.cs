using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Todo.Models.Identity;

namespace Todo.Services;

public class JwtGeneratorService : IJwtGeneratorService
{
	private readonly IConfiguration _configuration;
	private readonly UserManager<User> _userManager;
	private readonly SymmetricSecurityKey _key;

	public JwtGeneratorService(IConfiguration configuration, UserManager<User> userManager)
	{
		_configuration = configuration;
		_userManager = userManager;
		_key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
	}
	public async Task<string> CreateToken(User user)
	{
		var claims = new List<Claim>
		{
			new(ClaimTypes.Name, user.UserName),
			new(ClaimTypes.NameIdentifier, user.Id.ToString())
		};

		var roles = await _userManager.GetRolesAsync(user);
		claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

		var token = new JwtSecurityToken(
			issuer: _configuration["Jwt:Issuer"],
			audience: _configuration["Jwt:Audience"],
			claims: claims,
			expires: DateTime.UtcNow.AddHours(3),
			signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256)
		);

		return new JwtSecurityTokenHandler().WriteToken(token);

	}
}
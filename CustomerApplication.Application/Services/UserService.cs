using AutoMapper;
using CustomerApplication.CustomerApplication.Application.DTOs.User.Commands.CreateOrUpdate;
using CustomerApplication.CustomerApplication.Application.DTOs.User.Commands.Login;
using CustomerApplication.CustomerApplication.Domain.Entities;
using CustomerApplication.CustomerApplication.Domain.Repositories;
using CustomerApplication.Data.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly JwtOptions _jwtOptions;

    public UserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IMapper mapper,
        IPasswordHasher<User> passwordHasher,
        IOptions<JwtOptions> jwtOptions)   
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _jwtOptions = jwtOptions.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
    }


    public async Task<LoginResult> LoginAsync(LoginCommand command)
    {
        var user = await _userRepository.GetByUsernameAsync(command.Username);

        if (user == null)
            throw new Exception("Invalid username or password.");

        var passwordVerification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, command.Password);
        if (passwordVerification == PasswordVerificationResult.Failed)
            throw new Exception("Invalid username or password.");

        var token = GenerateJwtToken(user);

        return new LoginResult
        {
            Token = token,
            Username = user.Username,
            UserId=user.Id
        };
    }
    public async Task<Guid> CreateAsync(CreateOrUpdateUserCommand command)
    {
        var usernameTaken = await _userRepository.UsernameExistsAsync(command.Username, command.Id);
        if (usernameTaken)
            throw new Exception($"Username '{command.Username}' is already in use.");

        Role? assignedRole = null;

        if (command.RoleId.HasValue)
        {
            assignedRole = await _roleRepository.GetByIdAsync(command.RoleId.Value)
                           ?? throw new Exception($"Role with ID {command.RoleId} not found.");
        }
        else
        {
            var defaultRoleName = "USER";
            assignedRole = await _roleRepository.GetByNameAsync(defaultRoleName)
                            ?? throw new Exception($"Default role '{defaultRoleName}' not found in database.");
        }

        User user;

        if (command.Id == null || command.Id == Guid.Empty)
        {
            if (string.IsNullOrWhiteSpace(command.Password))
                throw new Exception("Password is required for new users.");

            user = _mapper.Map<User>(command);
            user.Id = Guid.NewGuid();
            user.PasswordHash = _passwordHasher.HashPassword(user, command.Password);

            // Assign role
            user.RoleId = assignedRole.Id;

            await _userRepository.AddAsync(user);
        }
        else
        {
            user = await _userRepository.GetByIdAsync(command.Id.Value)
                   ?? throw new Exception($"User with ID {command.Id} not found.");

            _mapper.Map(command, user);

            if (!string.IsNullOrWhiteSpace(command.Password))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, command.Password);
            }

            if (assignedRole != null)
                user.RoleId = assignedRole.Id;
        }

        await _userRepository.SaveChangesAsync();
        return user.Id;
    }

    public async Task<User?> GetByIdAsync(Guid id, bool includeRole = false)
        => includeRole
            ? await _userRepository.GetByIdWithRoleAsync(id)
            : await _userRepository.GetByIdAsync(id);

    public async Task<IEnumerable<User>> GetListAsync(bool includeRoles = false)
        => includeRoles
            ? await _userRepository.GetAllWithRolesAsync()
            : await _userRepository.GetAllAsync();

    public Task<User?> GetByUsernameAsync(string username)
        => _userRepository.GetByUsernameAsync(username);

    public async Task DeleteAsync(Guid id)
    {
        await _userRepository.DeleteAsync(id);

    }



    public async Task<User?> GetByUsernameAsync(string username, bool includeRole = false)
    {
        var entity = await _userRepository.GetByUsernameAsync(username);
        if (entity == null) return null;

        if (includeRole && entity.Role == null)
        {
            entity = await _userRepository.GetByIdWithRoleAsync(entity.Id) ?? entity;
        }

        return _mapper.Map<User>(entity);
    }
    private string GenerateJwtToken(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        string secretKey = _jwtOptions.SecretKey;

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            secretKey = JwtKeyGenerator.GenerateKey();
            Console.WriteLine("Generated temporary JWT SecretKey: " + secretKey);
        }

        byte[] keyBytes;

        try
        {
            keyBytes = Convert.FromBase64String(secretKey);
        }
        catch (FormatException)
        {
            keyBytes = JwtKeyGenerator.GenerateKeyBytes();
            Console.WriteLine("Invalid Base64 in config, generated new key: " + Convert.ToBase64String(keyBytes));
        }

        var key = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        string roleName = user.Role?.Name ?? "User";

        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.Username ?? string.Empty),
        new Claim(ClaimTypes.Role, roleName)
    };

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


}

namespace CustomerApplication.CustomerApplication.Application.DTOs.User.Commands.Login
{
    public class LoginCommand
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
    public class LoginResult
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = default!;
        public string? Token { get; set; }
        public string? RoleName { get; set; }
    }


}

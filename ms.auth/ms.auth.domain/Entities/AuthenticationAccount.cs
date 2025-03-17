namespace ms.auth.domain.Entities
{
    public class AuthenticationAccount
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }
        public Roles Role { get; private set; }
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private AuthenticationAccount() { }

        public AuthenticationAccount(string username, string passwordHash)
        {
            Id = Guid.NewGuid();
            Username = username.ToLower();
            PasswordHash = passwordHash;
            Role = Roles.USER;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        public AuthenticationAccount(string username, string passwordHash, Roles role)
        {
            Id = Guid.NewGuid();
            Username = username.ToLower();
            PasswordHash = passwordHash;
            Role = role;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeRole(Roles role)
        {
            Role = role;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

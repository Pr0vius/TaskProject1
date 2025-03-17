namespace ms.user.domain.Entities
{

    public class User
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; }
        public string? Email { get; private set; }
        public Guid AccountId { get; private set; }
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private User() { }

        public User(string username, Guid accountId, string email)
        {
            Id = Guid.NewGuid();
            Username = username;
            AccountId = accountId;
            Email = email;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateEmail(string newEmail)
        {
            Email = newEmail;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
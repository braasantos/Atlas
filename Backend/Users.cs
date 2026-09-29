namespace Atlas.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? PhotoUrl { get; set; }
        // ── Apple Sign-In ─────────────────────────────────────────────────────
        public string? AppleSub { get; set; }
        public List<Books> Book { get; set; } = [];
        public User() { }
        public User(string name, string username, string email, string pass)
        {
            Name = name;
            Username = username;
            Email = email;
            PasswordHash = pass;
        }
    }
}

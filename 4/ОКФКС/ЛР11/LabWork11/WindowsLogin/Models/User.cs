namespace WindowsLogin.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] EncryptedToken { get; set; }
        public string Role { get; set; }
    }

    public enum UserRole
    {
        Admin,
        Moderator,
        User
    }
}

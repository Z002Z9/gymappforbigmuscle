namespace api.Models
{
    public class TokenUpdater
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public DateTime Expires { get; set; }
        public bool Expired { get; set; }
    }
}

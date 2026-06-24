namespace League_Backend.Models
{
    public class User : DatabaseEntry
    {
        public Guid Id { get; set; }
        public string? UserPuuid { get; set; } //Riot id
        public string? Name {  get; set; }

    }
}

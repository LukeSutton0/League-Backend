namespace League_Backend.Models
{
    public class DatabaseEntry
    {
        public DateTime CreatedDateUtc { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? LastModifiedDateUtc { get; set; }

        public string? LastModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedDateUtc { get; set; }

        public string? DeletedBy { get; set; }
    }
}

namespace NetCoreIntermediate.Models
{
    public class EmailNotification
    {
        public required string Email { get; set; }
        public required string Subject { get; set; }
        public required string Message { get; set; }

        public required bool IsAccountCreated { get; set; }
    }
}

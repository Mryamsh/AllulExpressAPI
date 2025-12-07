using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllulExpressApi.Models
{
    [Table("ValidTokenClients")]
    public class ValidTokenClients
    {
        public int Id { get; set; }              // Primary Key
        public int ClientId { get; set; }          // Link to Users table
        public required string Token { get; set; }        // JWT string
        public DateTime ExpiresAt { get; set; }  // Expiry time
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default now

        // Navigation property (if you have a User entity)

    }
}

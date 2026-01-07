public class DbLog
{
    public int Id { get; set; }
    public string TableName { get; set; }
    public string Action { get; set; }   // INSERT / UPDATE / DELETE
    public string KeyValues { get; set; }
    public string OldValues { get; set; }
    public string NewValues { get; set; }
    public int? UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

namespace NETCoreBase.Database
{
    /// <summary>
    /// Marks an entity as soft-deletable via a Status column ('D' = deleted).
    /// </summary>
    public interface ISoftDeletable
    {
        string Status { get; }
    }
}

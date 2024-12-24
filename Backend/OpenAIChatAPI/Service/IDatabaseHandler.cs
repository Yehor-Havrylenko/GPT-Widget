namespace Service;

public interface IDatabaseHandler
{
    Task SaveSessionThreadAsync(string sessionId, string threadId, DateTime createdAt, DateTime lastAccessedAt);
    Task<string?> GetThreadBySessionAsync(string sessionId);
    Task SaveCommunicationHistoryAsync(string threadId, string message, string role, DateTime timestamp);
}
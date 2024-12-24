using MySql.Data.MySqlClient;

namespace Service.Databases
{
    public class MariaDB: IDatabaseHandler
    {
        private readonly string _connectionString;

        public MariaDB(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task SaveSessionThreadAsync(string sessionId, string threadId, DateTime createdAt, DateTime lastAccessedAt)
        {
            const string query = @"
                INSERT INTO SessionThread (SessionId, ThreadId, CreatedAt, LastAccessedAt)
                VALUES (@SessionId, @ThreadId, @CreatedAt, @LastAccessedAt)
                ON DUPLICATE KEY UPDATE
                    LastAccessedAt = @LastAccessedAt;";

            await using var connection = new MySqlConnection(_connectionString);
            await using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@SessionId", sessionId);
            command.Parameters.AddWithValue("@ThreadId", threadId);
            command.Parameters.AddWithValue("@CreatedAt", createdAt);
            command.Parameters.AddWithValue("@LastAccessedAt", lastAccessedAt);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<string?> GetThreadBySessionAsync(string sessionId)
        {
            const string query = @"
                SELECT ThreadId
                FROM SessionThread
                WHERE SessionId = @SessionId;";

            await using var connection = new MySqlConnection(_connectionString);
            await using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@SessionId", sessionId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result as string;
        }

        public async Task SaveCommunicationHistoryAsync(string threadId, string message, string role, DateTime timestamp)
        {
            const string query = @"
                INSERT INTO CommunicationHistory (ThreadId, Message, Role, Timestamp)
                VALUES (@ThreadId, @Message, @Role, @Timestamp);";

            await using var connection = new MySqlConnection(_connectionString);
            await using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@ThreadId", threadId);
            command.Parameters.AddWithValue("@Message", message);
            command.Parameters.AddWithValue("@Role", role);
            command.Parameters.AddWithValue("@Timestamp", timestamp);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}

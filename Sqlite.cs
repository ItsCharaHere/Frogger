using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    public static class Sqlite
    {
        public struct TicketRow
        {
            public int Id { get; }
            public int ChannelId { get; }
            public int CreatorId { get; }
            public bool IsClosed { get; }
            public TicketRow(int id, int channelId, int creatorId, bool isClosed)
            {
                Id = id;
                ChannelId = channelId;
                CreatorId = creatorId;
                IsClosed = isClosed;
            }
        }
        public static SqliteConnection connection = null;
        private static string DataSource = $"Data Source ={Config._config.DBLocation};";
        private readonly static string DefaultInitTable = @"
            CREATE TABLE IF NOT EXISTS tickets (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            channel_id TEXT NOT NULL,
            creator_id TEXT,
            is_closed BOOLEAN
            );";

        public static void InitDB() {
            if (connection == null) {
                connection = new SqliteConnection(DataSource);
                connection.Open();
            } else
            {
                return;
            }  
            using (SqliteCommand command = new SqliteCommand(DefaultInitTable, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Attempted to Init DB file.");
            }
        }
        public static bool CreateRow(string channelId, string? creatorId)
        {
            if (creatorId == null)
            {
                creatorId = $"NULL";
            }else if (creatorId != null) {
                creatorId = $"'{creatorId}'";
            }
            string InsertStatement = @$"INSERT INTO tickets (channel_id, creator_id, is_closed)
                                        VALUES ('{channelId}', {creatorId}, FALSE);";
            using (SqliteCommand command = new SqliteCommand(InsertStatement, connection))
            {
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool CloseTicket(int channelId)
        {
            string UpdateStatement = $@"UPDATE tickets SET is_closed = TRUE where channel_id = {channelId}";
            using (SqliteCommand command = new SqliteCommand(UpdateStatement,connection))
            {
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static TicketRow RetrieveRowFromChannelId(int channelId)
        {
            string FetchStatement = $@"SELECT * FROM tickets WHERE channel_id = {channelId}";
            using (SqliteCommand command = new SqliteCommand(FetchStatement, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new TicketRow(
                            reader.GetInt32(0),
                            reader.GetInt32(1),
                            reader.GetInt32(2),
                            reader.GetBoolean(3)
                            );
                    }
                }
                return default(TicketRow);
            }
        }
        public static int GetRowCount()
        {
            string CountStatement = $@"SELECT COUNT(*) FROM tickets";
            using (SqliteCommand command = new SqliteCommand(CountStatement, connection))
            {
                 return Convert.ToInt32((long)command.ExecuteScalar()) + 1;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace FlashcardApp.Services
{
    public static class DbService
    {
        private static SQLiteAsyncConnection _connection;

        public static SQLiteAsyncConnection GetConnection()
        {
            if (_connection == null)
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "flashcards.db");
                _connection = new SQLiteAsyncConnection(dbPath);
            }
            return _connection;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using FlashcardApp.Models;

namespace FlashcardApp.Services
{
    public class FlashcardService
    {
        private readonly SQLiteAsyncConnection _db;

        public FlashcardService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "flashcards.db");
            _db = DbService.GetConnection();  // Reuse your database connection
            _db.CreateTableAsync<FlashcardModel>().Wait();
        }

        public async Task<List<FlashcardModel>> GetFlashcardsBySubjectIdAsync(int subjectId)
        {
            return await _db.Table<FlashcardModel>().Where(f => f.SubjectId == subjectId).ToListAsync();
        }

        public async Task<FlashcardModel> GetFlashcardByIdAsync(int id)
        {
            return await _db.FindAsync<FlashcardModel>(id);
        }

        public async Task<int> AddFlashcardAsync(FlashcardModel flashcard)
        {
            return await _db.InsertAsync(flashcard);
        }

        public async Task<int> UpdateFlashcardAsync(FlashcardModel flashcard)
        {
            return await _db.UpdateAsync(flashcard);
        }

        public async Task<int> DeleteFlashcardByIdAsync(int id)
        {
            return await _db.DeleteAsync<FlashcardModel>(id);
        }

        public async Task<int> DeleteFlashcardAsync(FlashcardModel flashcard)
        {
            return await _db.DeleteAsync(flashcard);
        }

        public async Task<List<FlashcardModel>> GetAllFlashcardsAsync()
        {
            return await _db.Table<FlashcardModel>().ToListAsync();
        }

    }
}

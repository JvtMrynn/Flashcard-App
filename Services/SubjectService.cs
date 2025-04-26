using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using FlashcardApp.Models;

namespace FlashcardApp.Services
{
    public class SubjectService
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly FlashcardService _flashcardService;

        public SubjectService()
        {
            _database = DbService.GetConnection(); // <-- Use the shared connection
            _database.CreateTableAsync<SubjectModel>().Wait();
            _flashcardService = new FlashcardService();
        }

        public async Task<List<SubjectModel>> GetAllSubjectsAsync()
        {
            return await _database.Table<SubjectModel>().ToListAsync();
        }

        public async Task<SubjectModel?> GetSubjectByIdAsync(int id)
        {
            return await _database.Table<SubjectModel>().FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<int> AddSubjectAsync(SubjectModel subject)
        {
            return await _database.InsertAsync(subject);
        }

        public async Task<int> UpdateSubjectAsync(SubjectModel subject)
        {
            return await _database.UpdateAsync(subject);
        }

        public async Task<int> DeleteSubjectAsync(int id)
        {
            return await _database.DeleteAsync<SubjectModel>(id);
        }

        public async Task<bool> IsSubjectNameUniqueAsync(string name)
        {
            var existing = await _database.Table<SubjectModel>().FirstOrDefaultAsync(s => s.Name == name);
            return existing == null;
        }

        public async Task<List<SubjectModel>> GetAllSubjectsWithFlashcardCountAsync()
        {
            var subjects = await _database.Table<SubjectModel>().ToListAsync();
            var allFlashcards = await _flashcardService.GetAllFlashcardsAsync();

            foreach (var subject in subjects)
            {
                subject.FlashcardCount = allFlashcards.Count(f => f.SubjectId == subject.Id);
            }

            return subjects;
        }


    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace FlashcardApp.Models
{
    public class FlashcardModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int SubjectId { get; set; }  // Link to the Subject
        public string Definition { get; set; }  // The Question/Definition
        public string ChoiceA { get; set; }
        public string ChoiceB { get; set; }
        public string ChoiceC { get; set; }
        public string ChoiceD { get; set; }
        public string CorrectAnswer { get; set; }  // Must match one of the choices
        public bool IsLearned { get; set; }  // Optional: for marking flashcards as learned
    }
}

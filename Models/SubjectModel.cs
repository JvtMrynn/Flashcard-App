using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace FlashcardApp.Models
{
    public class SubjectModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ColorTag { get; set; }
        public int FlashcardCount { get; set; }

    }
}

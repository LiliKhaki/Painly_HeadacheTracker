using System.Collections.Generic;
using SQLite;

namespace HeadacheTracker.Domain.Entities { 
public class HeadacheEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public System.DateTime Date { get; set; }

        public int Intensity { get; set; }  // от 1 до 10

        [Ignore]
        public List<MedicationEntry> Medications { get; set; } = new List<MedicationEntry>();
        public string? Notes { get; set; } // дополнительные заметки

    }
}
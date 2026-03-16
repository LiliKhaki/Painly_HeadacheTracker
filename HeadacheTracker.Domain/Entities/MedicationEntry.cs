using SQLite;
using System;

namespace HeadacheTracker.Domain.Entities
{
    public class MedicationEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

            public int HeadacheEntryId { get; set; } // внешний ключ

            public string? Medication { get; set; }         // название лекарства
            public double? Dose { get; set; }         // доза (например, мг)
        }

    }


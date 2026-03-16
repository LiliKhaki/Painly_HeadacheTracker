using HeadacheTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadacheTracker.Infrastructure
{
    internal class HeadacheMedicationDto
    {
        public int HeadacheId { get; set; }
        public DateTime Date { get; set; }
        public int Intensity { get; set; }
        public string? Notes { get; set; }

        public int? MedicationId { get; set; }
        public int? HeadacheEntryId { get; set; }
        public string? Medication { get; set; }
        public double? Dose { get; set; }

        public (HeadacheEntry, MedicationEntry?) ToTuple()
        {
            var headache = new HeadacheEntry
            {
                Id = HeadacheId,
                Date = Date,
                Intensity = Intensity,
                Notes = Notes
            };

            MedicationEntry? med = null;

            if (MedicationId.HasValue)
            {
                med = new MedicationEntry
                {
                    Id = MedicationId.Value,
                    HeadacheEntryId = HeadacheEntryId!.Value,
                    Medication = Medication,
                    Dose = Dose
                };
            }

            return (headache, med);
        }
    }

}

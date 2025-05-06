using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkrun.MVVM.Models
{
    public class ParkrunData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string CourseName { get; set; } // Name der Laufstrecke
        public double DistanceKm { get; set; } // Kilometeranzahl

        public override string ToString()
        {
            return $"{Date.ToShortDateString()} - {Time:mm\\:ss}";
        }
    }
}

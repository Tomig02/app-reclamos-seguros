using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace app_reclamos_seguros.Model
{
    public class ClaimReportEntry
    {
        public string Comment { get; private set; }
        public int ClaimNumber { get; private set; }
        public DateTime DateAndTime { get; private set; }

        public ClaimReportEntry(string comment, int claimNumber) 
        {
            this.Comment = comment;
            this.ClaimNumber = claimNumber;
            this.DateAndTime = DateTime.Now;
        }
    }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace app_reclamos_seguros.Model
{
    public class ClaimReportEntryDTO
    {
        [Required]
        public string Comment { get; set; }
        [Required]
        public int ClaimNumber { get; set; }
        public DateTime DateAndTime { get; set; }

        public ClaimReportEntryDTO(string Comment, int ClaimNumber, DateTime DateAndTime) 
        {
            this.Comment = Comment;
            this.ClaimNumber = ClaimNumber;
            this.DateAndTime = DateAndTime;
        }
    }
}
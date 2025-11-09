using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace app_reclamos_seguros.Model
{
    /// <summary>
    /// Data transfer object for the claim reports, requires a comment and claim to be validated
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <param name="Comment"> !Required! This is the comment of the entry </param> 
    /// <param name="ClaimNumber"> !Required! This is identifying number of the claim</param>
    /// <param name="DateAndTime"> This param will be set by the database, if set it will be overwriten </param>
    public class ClaimReportEntryDTO(string Comment, int ClaimNumber, DateTime DateAndTime)
    {
        [Required]
        public string Comment { get; set; } = Comment;
        [Required]
        public int ClaimNumber { get; set; } = ClaimNumber;
        public DateTime DateAndTime { get; set; } = DateAndTime;
    }
}
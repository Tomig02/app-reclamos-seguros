using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace app_reclamos_seguros.Model
{
    /// <summary>
    /// Data transfer object representing the search result of a query for all claims, 
    /// it holds reduced information meant for identifying the selected claims
    /// </summary>
    public class ClaimSearchResultDTO
    {
        [Required]
        public List<ClaimSearchItem> ResultsList { get; set; }

        public ClaimSearchResultDTO(List<ClaimSearchItem> newItemsList)
        {
            ResultsList = newItemsList;
        }

        public void Combine(ClaimSearchResultDTO newResults) 
        {
            ResultsList = [.. ResultsList, .. newResults.ResultsList];
        }
    }

    public class ClaimSearchItemDTO
    {
        [Required]
        public int ClaimID { get; set; }
        [Required]
        public DateTime DateAndHour { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public bool Archived { get; set; }

        /// <summary>
        /// Creates a search result item holding specific data of a claim for later id
        /// </summary>
        public ClaimSearchItemDTO(int claimNumber, DateTime dateAndHour, string name, string surname, bool archived)
        {
            this.ClaimID = claimNumber;
            this.DateAndHour = dateAndHour;
            this.Name = name;
            this.Surname = surname;
            this.Archived = archived;
        }
    }
}

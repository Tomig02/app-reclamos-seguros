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

        /// <summary>
        /// Receives a string formated JSON array, creating and populating a list of claim search results
        /// </summary>
        /// <param name="jsonString"> string formated JSON array of claim search results </param>
        public ClaimSearchResultDTO(string jsonString)
        {
            ResultsList = new List<ClaimSearchItem>();
            JArray jsonData = JArray.Parse(jsonString);
            
            for (int i = 0; i < jsonData.Count; i++)
            {
                var item = jsonData[i];
                
                ResultsList.Add(new ClaimSearchItem(
                    (int) item["claim_number"]!,
                    (DateTime) item["date_and_hour"]!,
                    (string) item["name"]!,
                    (string) item["surname"]!
                ));
            }
        }
    }

    /// <summary>
    /// Represents a search result holding identifying data for a claim
    /// </summary>
    public class ClaimSearchItem
    {
        [Required]
        public int ClaimID { get; set; }
        [Required]
        public DateTime DateAndHour { get; set; }
        [Required] 
        public string Name { get; set; }
        [Required] 
        public string Surname { get; set; }

        /// <summary>
        /// Creates a search result item holding specific data of a claim for later id
        /// </summary>
        public ClaimSearchItem(int claimNumber, DateTime dateAndHour, string name, string surname)
        {
            this.ClaimID = claimNumber;
            this.DateAndHour = dateAndHour;
            this.Name = name;
            this.Surname = surname;
        }
    }
}

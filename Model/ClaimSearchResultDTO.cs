using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace app_reclamos_seguros.Model
{
    public class ClaimSearchResultDTO
    {
        [Required]
        public List<ClaimSearchItem> ResultsList { get; set; }

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

        public ClaimSearchItem(int claimID, DateTime dateAndHour, string name, string surname)
        {
            this.ClaimID = claimID;
            this.DateAndHour = dateAndHour;
            this.Name = name;
            this.Surname = surname;
        }
    }
}

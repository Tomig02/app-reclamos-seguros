using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace app_reclamos_seguros.Model
{
    /// <summary>
    /// Data transfer object representing the search result of a query for all claims, 
    /// it holds reduced information meant for identifying the selected claims
    /// </summary>
    public class ClaimSearchResult
    {
        // add info of the total results, if contains archived, etc.

        public List<ClaimSearchItem> ResultsList { get; private set; }

        public ClaimSearchResult(List<ClaimSearchItem> newItemsList)
        {
            ResultsList = newItemsList;
        }

        public void Combine(ClaimSearchResult newResults)
        {
            ResultsList = [.. ResultsList, .. newResults.ResultsList];
        }
    }

    /// <summary>
    /// Represents a search result holding identifying data for a claim
    /// </summary>
    public class ClaimSearchItem
    {
        public int ClaimID { get; private set; }
        public DateTime DateAndHour { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public bool Archived { get; private set; }

        /// <summary>
        /// Creates a search result item holding specific data of a claim for later id
        /// </summary>
        public ClaimSearchItem(int claimNumber, DateTime dateAndHour, string name, string surname, bool archived)
        {
            this.ClaimID = claimNumber;
            this.DateAndHour = dateAndHour;
            this.Name = name;
            this.Surname = surname;
            this.Archived = archived;
        }
    }
}

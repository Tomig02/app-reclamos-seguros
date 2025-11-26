
namespace app_reclamos_seguros.Model
{
    /// <summary>
    /// Class for representing a comented event related to a claim identified by the claim's number, 
    /// with date and time saved at creation
    /// </summary>
    public class ClaimReportEntry
    {
        public string Comment { get; private set; }
        public int ClaimNumber { get; private set; }
        public DateTime DateAndTime { get; private set; }

        /// <summary>
        /// Constructor for creating entries of events related to a claim.
        /// DateTime will be the current local time
        /// </summary>
        public ClaimReportEntry(string comment, int claimNumber) 
        {
            this.Comment = comment;
            this.ClaimNumber = claimNumber;
            this.DateAndTime = DateTime.Now;
        }
    }
}
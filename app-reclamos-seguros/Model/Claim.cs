using Newtonsoft.Json.Linq;

namespace app_reclamos_seguros.Model
{
    /// <summary>
    /// Base class for any claim type
    /// </summary>
    public abstract class Claim
    {
        // claim data
        public int ClaimNumber { get; private set; }
        public string Description { get; private set; }
        public string Direction { get; private set; }
        public string City { get; private set; }
        public DateTime DateAndHour { get; private set; }
        public bool Archived { get; private set; }

        // client data
        public int ClientDNI { get; private set; }
        public string ClientName { get; private set; }
        public string ClientSurname { get; private set; }
        public int PhoneNumber { get; private set; }
        public string Email { get; private set; }

        // policy data
        public int PolicyNumber { get; private set; }
        public string CompanyName { get; private set; }
        public string Coverage  { get; private set; }

        /// <summary>
        /// The constructor meant for the creation of a claim with data coming from the controller/s
        /// </summary>
        public Claim(int claimNumber, string description, string direction, string city, DateTime dateAndHour, 
            int clientDNI, string clientName, string clientSurname, int phoneNumber, string email, int policyNumber, 
            string companyName, string coverage, bool archived)
        {
            this.ClaimNumber = claimNumber;
            this.Description = description;
            this.Direction = direction;
            this.City = city;
            this.DateAndHour = dateAndHour;
            this.ClientDNI = clientDNI;
            this.ClientName = clientName;
            this.ClientSurname = clientSurname;
            this.PhoneNumber = phoneNumber;
            this.Email = email;
            this.PolicyNumber = policyNumber;
            this.CompanyName = companyName;
            this.Coverage = coverage;
            this.Archived = archived;
        }
    }
}

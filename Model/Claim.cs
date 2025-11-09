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
        /// Constructor meant for [frombody] deserialization
        /// </summary>
        public Claim() { }

        /// <summary>
        /// The constructor meant for the creation of a claim with data coming from the database
        /// </summary>
        /// <param name="jsonString"> JSON formated string with the data of the claim</param>
        public Claim(string jsonString)
        {
            JArray token = JArray.Parse(jsonString);
            JObject claimObj = (JObject)token[0];

            this.ClaimNumber = claimObj.Value<int>("claim_number")!;
            this.Description = claimObj.Value<string>("description")!;
            this.Direction = claimObj.Value<string>("direction")!;
            this.City = claimObj.Value<string>("city")!;
            this.DateAndHour = claimObj.Value<DateTime>("date_and_hour")!;
            this.ClientDNI = claimObj.Value<int>("dni")!;
            this.ClientName = claimObj.Value<string>("name")!;
            this.ClientSurname = claimObj.Value<string>("surname")!;
            this.PhoneNumber = claimObj.Value<int>("phone_number")!;
            this.Email = claimObj.Value<string>("email")!;
            this.PolicyNumber = claimObj.Value<int>("policy_number")!;
            this.CompanyName = claimObj.Value<string>("company")!;
            this.Coverage = claimObj.Value<string>("coverage")!;
        }

        /// <summary>
        /// The constructor meant for the creation of a claim with data coming from the controller/s
        /// </summary>
        public Claim(int claimNumber, string description, string direction, string city, DateTime dateAndHour, 
            int clientDNI, string clientName, string clientSurname, int phoneNumber, string email, int policyNumber, 
            string companyName, string coverage)
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
        }
    }
}

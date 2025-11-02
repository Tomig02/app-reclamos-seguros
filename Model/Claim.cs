using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace app_reclamos_seguros.Model
{
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


        public Claim(string jsonString) 
        {
            JObject token = JObject.Parse(jsonString);

            this.ClaimNumber = (int) token.SelectToken("claim_number");
            this.Description = (string) token.SelectToken("description");
            this.Direction = (string) token.SelectToken("direction");
            this.City = (string) token.SelectToken("city");
            this.DateAndHour = (DateTime) token.SelectToken("date_and_hour");
            this.ClientDNI = (int) token.SelectToken("dni");
            this.ClientName = (string) token.SelectToken("name");
            this.ClientSurname = (string) token.SelectToken("surname");
            this.PhoneNumber = (int) token.SelectToken("phone_number");
            this.Email = (string) token.SelectToken("email");
            this.PolicyNumber = (int) token.SelectToken("policy_number");
            this.CompanyName = (string) token.SelectToken("company");
            this.Coverage = (string) token.SelectToken("coverage");
        }

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

using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;

namespace app_reclamos_seguros.Model
{
    public class VehicleClaim : Claim
    {
        // car data
        public string vehicleBrand;
        public string vehicleModel;
        public string licensePlate;
        public string registeredOwner;


        public VehicleClaim() : base() { }

        public VehicleClaim(string jsonString) : base(jsonString) {
            JArray token = JArray.Parse(jsonString);
            JObject firstClaim = (JObject)token[0];

            this.vehicleBrand = firstClaim.Value<string>("brand");
            this.vehicleModel = firstClaim.Value<string>("model");
            this.licensePlate = firstClaim.Value<string>("license_plate");
            this.registeredOwner = firstClaim.Value<string>("registered_owner");
        }

        [JsonConstructor]
        public VehicleClaim(int claimNumber, string description, string direction, string city, DateTime dateAndHour,
            int clientDNI, string clientName, string clientSurname, int phoneNumber, string email, int policyNumber,
            string companyName, string coverage, string vehicleBrand, string vehicleModel, string licensePlate, 
            string registeredOwner) 
            : base(claimNumber, description, direction, city, dateAndHour,
            clientDNI, clientName, clientSurname, phoneNumber, email, policyNumber,
            companyName, coverage)
        {
            this.vehicleBrand = vehicleBrand;
            this.vehicleModel = vehicleModel;
            this.licensePlate = licensePlate;
            this.registeredOwner = registeredOwner;
        }
    }
}

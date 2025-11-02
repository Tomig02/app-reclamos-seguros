using Newtonsoft.Json.Linq;

namespace app_reclamos_seguros.Model
{
    public class VehicleClaim : Claim
    {
        // car data
        public string vehicleBrand;
        public string vehicleModel;
        public string licensePlate;
        public string registeredOwner;

        public VehicleClaim(string jsonString) : base(jsonString) {
            JObject token = JObject.Parse(jsonString);

            this.vehicleBrand = (string) token.SelectToken("brand");
            this.vehicleModel = (string) token.SelectToken("model");
            this.licensePlate = (string) token.SelectToken("license_plate");
            this.registeredOwner = (string) token.SelectToken("registered_owner");
        }

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

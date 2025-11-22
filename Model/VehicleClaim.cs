using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;

namespace app_reclamos_seguros.Model
{
    /// <summary>
    /// Claim type for any type of vehicular claim
    /// </summary>
    public class VehicleClaim : Claim
    {
        // car data
        public string vehicleBrand;
        public string vehicleModel;
        public string licensePlate;
        public string registeredOwner;

        /// <summary>
        /// Constructor meant for [frombody] deserialization
        /// </summary>
        public VehicleClaim() : base() { }

        /// <summary>
        /// Constructor meant for instantiation by the database, using a JON formated string
        /// </summary>
        /// <param name="jsonString"> JSON formated string with the data of the claim</param>
        public VehicleClaim(string jsonString) : base(jsonString) {
            JArray token = JArray.Parse(jsonString);
            JObject firstClaim = (JObject)token[0];

            this.vehicleBrand = firstClaim.Value<string>("brand")!;
            this.vehicleModel = firstClaim.Value<string>("model")!;
            this.licensePlate = firstClaim.Value<string>("license_plate")!;
            this.registeredOwner = firstClaim.Value<string>("registered_owner")!;
        }

        /// <summary>
        /// Constructor meant for instantiation by the controller
        /// </summary>
        [JsonConstructor]
        public VehicleClaim(int claimNumber, string description, string direction, string city, DateTime dateAndHour,
            int clientDNI, string clientName, string clientSurname, int phoneNumber, string email, int policyNumber,
            string companyName, string coverage, string vehicleBrand, string vehicleModel, string licensePlate, 
            string registeredOwner, bool archived) 
            : base(claimNumber, description, direction, city, dateAndHour,
            clientDNI, clientName, clientSurname, phoneNumber, email, policyNumber,
            companyName, coverage, archived)
        {
            this.vehicleBrand = vehicleBrand;
            this.vehicleModel = vehicleModel;
            this.licensePlate = licensePlate;
            this.registeredOwner = registeredOwner;
        }
    }
}

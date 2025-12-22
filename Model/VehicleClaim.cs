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
        public string VehicleBrand { get; private set; }
        public string VehicleModel { get; private set; }
        public string LicensePlate { get; private set; }
        public string RegisteredOwner { get; private set; }

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
            this.VehicleBrand = vehicleBrand;
            this.VehicleModel = vehicleModel;
            this.LicensePlate = licensePlate;
            this.RegisteredOwner = registeredOwner;
        }
    }
}

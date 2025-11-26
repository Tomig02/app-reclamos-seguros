using System.ComponentModel.DataAnnotations;

namespace app_reclamos_seguros.Model
{
    /// <summary>
    /// Data transfer object for any type of vehicular claim
    /// </summary>
    public class VehicleClaimDTO
    {
        [Required]
        [Range(1, int.MaxValue)]
        public required int ClaimNumber { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Direction { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public DateTime DateAndHour { get; set; }
        [Required]
        public int ClientDNI { get; set; }
        [Required]
        public string ClientName { get; set; }
        [Required] 
        public string ClientSurname { get; set; }
        [Required] 
        public int PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required] 
        public int PolicyNumber { get; set; }
        [Required] 
        public string CompanyName { get; set; }
        [Required] 
        public string Coverage { get; set; }
        [Required] 
        public string VehicleBrand { get; set; }
        [Required] 
        public string VehicleModel { get; set; }
        [Required] 
        public string LicensePlate { get; set; }
        [Required] 
        public string RegisteredOwner { get; set; }
        [Required]
        public bool Archived { get; set; }


        public VehicleClaimDTO() { }
    }
}

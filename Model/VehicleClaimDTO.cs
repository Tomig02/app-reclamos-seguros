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
        public required string Description { get; set; }
        [Required]
        public required string Direction { get; set; }
        [Required]
        public required string City { get; set; }
        [Required]
        public required DateTime DateAndHour { get; set; }
        [Required]
        public required int ClientDNI { get; set; }
        [Required]
        public required string ClientName { get; set; }
        [Required]
        public required string ClientSurname { get; set; }
        [Required]
        public required int PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required int PolicyNumber { get; set; }
        [Required]
        public required string CompanyName { get; set; }
        [Required]
        public required string Coverage { get; set; }
        [Required]
        public required string VehicleBrand { get; set; }
        [Required]
        public required string VehicleModel { get; set; }
        [Required]
        public required string LicensePlate { get; set; }
        [Required]
        public required string RegisteredOwner { get; set; }
        [Required]
        public bool Archived { get; set; }


        public VehicleClaimDTO() { }
    }
}

using app_reclamos_seguros.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;


namespace app_reclamos_seguros.Controllers
{

    /// <summary>
    /// Controller handling all the interactions related to the insurance claims
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ClaimController : ControllerBase
    {
        private IClaimsRepository dbManager;
        private readonly ILogger<ClaimController> _logger;
        public ClaimController(ILogger<ClaimController> logger, IClaimsRepository repository)
        {
            dbManager = repository;
            _logger = logger;
        }

        /// <summary>
        /// Searchs for a claim using it's assigned number and returns it's data in JSON format
        /// </summary>
        /// <param name="claimID">The claim number asigned by the insurance company</param>
        /// <returns></returns>
        [HttpGet] [Route("Claim/{claimID}")]
        public ActionResult<VehicleClaimDTO> GetClaimByID(int? claimID)
        {
            if(claimID == null) 
            {
                return BadRequest("No id was present in the request"); 
            }
            else
            {
                string jsonString = dbManager.GetByID((int)claimID);

                if (jsonString == "" || jsonString == "[]")
                {
                    return BadRequest("Claim ID doesn't exist");
                }
                else 
                {
                    VehicleClaim vClaim = new VehicleClaim(jsonString);
                    VehicleClaimDTO vClaimDTO = new VehicleClaimDTO() { 
                        ClaimNumber = vClaim.ClaimNumber,
                        City = vClaim.City,
                        ClientDNI = vClaim.ClientDNI,
                        ClientName = vClaim.ClientName,
                        ClientSurname = vClaim.ClientSurname,
                        CompanyName = vClaim.CompanyName,
                        Coverage = vClaim.Coverage,
                        DateAndHour = vClaim.DateAndHour,
                        Description = vClaim.Description,
                        Direction = vClaim.Direction,
                        Email = vClaim.Email,
                        LicensePlate = vClaim.licensePlate,
                        PhoneNumber = vClaim.PhoneNumber,
                        PolicyNumber = vClaim.PolicyNumber,
                        RegisteredOwner = vClaim.registeredOwner,
                        VehicleBrand = vClaim.vehicleBrand,
                        VehicleModel = vClaim.vehicleModel,
                        Archived = vClaim.Archived
                    };

                    return Ok(vClaimDTO);
                }
            }
        }

        /// <summary>
        /// Returns identifying data of all saved claims
        /// </summary>
        /// <returns> JSON data of the search result dto </returns>
        [HttpGet] [Route("AllClaims")]
        public ActionResult<ClaimSearchResultDTO> GetAllClaims()
        {
            ClaimSearchResultDTO searchFirst = new ClaimSearchResultDTO(dbManager.GetClaimsList(false));
            ClaimSearchResultDTO searchSecond = new ClaimSearchResultDTO(dbManager.GetClaimsList(true));
            searchFirst.Combine(searchSecond);
            
            return Ok(searchFirst);
        }

        /// <summary>
        ///  Returns identifying data of all saved claims that are active
        /// </summary>
        /// <returns> JSON data of the search result dto </returns>
        [HttpGet] [Route("AllClaims/Active")]
        public ActionResult<ClaimSearchResultDTO> GetAllClaimsActive()
        {
            ClaimSearchResultDTO search = new ClaimSearchResultDTO(dbManager.GetClaimsList(false));
            return Ok(search);
        }

        /// <summary>
        ///  Returns identifying data of all saved claims that are archived
        /// </summary>
        /// <returns> JSON data of the search result dto </returns>
        [HttpGet] [Route("AllClaims/Archived")]
        public ActionResult<ClaimSearchResultDTO> GetAllClaimsArchived()
        {
            ClaimSearchResultDTO search = new ClaimSearchResultDTO(dbManager.GetClaimsList(true));
            return Ok(search);
        }

        /// <summary>
        /// Receive the complete data related to a vehicle claim and save it into the database
        /// </summary>
        /// <param name="dto"> The DTO for vehicle claims filled from the request's body </param>
        /// <returns> The resulting state of the requested action </returns>
        [HttpPost] [Route("NewClaim")]
        public IActionResult AddNewCarClaim([FromBody] VehicleClaimDTO dto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest("The data model couldn't be validated" + ModelState.ToString());
            }

            var newClaim = new VehicleClaim(
                dto.ClaimNumber,
                dto.Description,
                dto.Direction,
                dto.City,
                dto.DateAndHour,
                dto.ClientDNI,
                dto.ClientName,
                dto.ClientSurname,
                dto.PhoneNumber,
                dto.Email,
                dto.PolicyNumber,
                dto.CompanyName,
                dto.Coverage,
                dto.VehicleBrand,
                dto.VehicleModel,
                dto.LicensePlate,
                dto.RegisteredOwner,
                archived: false
            );

            try
            {
                dbManager.SetNewClaim(newClaim);
                return Ok("Saved succesfully");
            }
            catch (DatabaseException ex)
            {
                return BadRequest($"The database couldnt process the request: {ex.Message}");
            }
            catch (Exception ex) {
                return BadRequest($"There was an unexpected error during the request: {ex.Message}");
            }
        }

        /// <summary>
        /// Mark an existing claim as archived
        /// </summary>
        /// <param name="claimNum"> the insurance-assigned number of the claim to archive </param>
        /// <returns> The resulting state of the requested action </returns>
        [HttpPost][Route("ArchiveClaim/{claimNum}")]
        public IActionResult ArchiveClaim( int claimNum)
        {
            try
            {
                dbManager.SetArchived(claimNum, true);
                return Ok("Archived succesfully");
            }
            catch(DatabaseException ex)
            {
                return BadRequest($"The database couldnt process the request: {ex.Message}");
            }
        }
    }
}

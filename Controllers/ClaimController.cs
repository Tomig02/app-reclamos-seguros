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
                VehicleClaim? vClaim = dbManager.GetByID((int)claimID);

                if (vClaim != null)
                {
                    VehicleClaimDTO vClaimDTO = new VehicleClaimDTO()
                    {
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
                        LicensePlate = vClaim.LicensePlate,
                        PhoneNumber = vClaim.PhoneNumber,
                        PolicyNumber = vClaim.PolicyNumber,
                        RegisteredOwner = vClaim.RegisteredOwner,
                        VehicleBrand = vClaim.VehicleBrand,
                        VehicleModel = vClaim.VehicleModel,
                        Archived = vClaim.Archived
                    };

                    return Ok(vClaimDTO);
                }
                else
                    return BadRequest("No registry matches the specified ID");
            }
        }

        /// <summary>
        /// Returns identifying data of all saved claims
        /// </summary>
        /// <returns> JSON data of the search result dto </returns>
        [HttpGet] [Route("AllClaims")]
        public ActionResult<ClaimSearchResultDTO> GetAllClaims()
        {
            ClaimSearchResult searchFirst = dbManager.GetClaimsList(false);
            ClaimSearchResult searchSecond = dbManager.GetClaimsList(true);

            ClaimSearchResultDTO searchFirstDTO = new ClaimSearchResultDTO(searchFirst.ResultsList);
            ClaimSearchResultDTO searchSecondDTO = new ClaimSearchResultDTO(searchSecond.ResultsList);
            searchFirstDTO.Combine(searchSecondDTO);
            
            return Ok(searchFirstDTO);
        }

        /// <summary>
        ///  Returns identifying data of all saved claims that are active
        /// </summary>
        /// <returns> JSON data of the search result dto </returns>
        [HttpGet] [Route("AllClaims/Active")]
        public ActionResult<ClaimSearchResultDTO> GetAllClaimsActive()
        {
            ClaimSearchResult result = dbManager.GetClaimsList(false);
            ClaimSearchResultDTO search = new ClaimSearchResultDTO(result.ResultsList);

            return Ok(search);
        }

        /// <summary>
        ///  Returns identifying data of all saved claims that are archived
        /// </summary>
        /// <returns> JSON data of the search result dto </returns>
        [HttpGet] [Route("AllClaims/Archived")]
        public ActionResult<ClaimSearchResultDTO> GetAllClaimsArchived()
        {
            ClaimSearchResult result = dbManager.GetClaimsList(true);
            ClaimSearchResultDTO search = new ClaimSearchResultDTO(result.ResultsList);
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

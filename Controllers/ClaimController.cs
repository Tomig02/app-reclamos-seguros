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
        private DBManager dbManager = new DBManager();
        private readonly ILogger<ClaimController> _logger;
        public ClaimController(ILogger<ClaimController> logger)
        {
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
                string jsonString = dbManager.SelectCarClaimByNumber((int)claimID);

                if (jsonString == "")
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
                        VehicleModel = vClaim.vehicleModel
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
            ClaimSearchResultDTO search = new ClaimSearchResultDTO(dbManager.SelectListAllCarClaims());
            return Ok(search);
        }

        /// <summary>
        /// Search for all the event entries related to the specified claim number
        /// </summary>
        /// <param name="claimNum"> The number asigned to the claim by the insurance company </param>
        /// <returns></returns>
        [HttpGet] [Route("ClaimEntries/{claimNum}")]
        public ActionResult<ClaimReportEntryDTO[]> GetAllClaimEntries(int claimNum)
        {
            try
            {
                string entriesJson = dbManager.SelectClaimEntries(claimNum);
                JArray a = JArray.Parse(entriesJson);

                // parse the json array into the claim entry dto
                List<ClaimReportEntryDTO> entryList = new List<ClaimReportEntryDTO>();
                foreach (JObject item in JArray.Parse(entriesJson))
                {
                    entryList.Add(new ClaimReportEntryDTO(
                        (string) item.GetValue("comment")!, 
                        claimNum, 
                        (DateTime)item.GetValue("date_and_time")!)
                    );
                }

                return Ok(entryList);
            }
            catch (DatabaseException ex)
            {
                return BadRequest($"The database couldnt process the request: {ex.Message}");
            }
            
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
                dto.RegisteredOwner
            );

            try 
            { 
                dbManager.InsertNewCarClaim(newClaim);
                return Ok("Saved succesfully");
            }
            catch(DatabaseException ex) 
            { 
                return BadRequest($"The database couldnt process the request: {ex.Message}"); 
            }
        }

        /// <summary>
        /// Receive the data of a new report entry of a claim, and save it into the database
        /// </summary>
        /// <param name="dto"> The data of a claim's new report entry </param>
        /// <returns> The resulting state of the requested action </returns>
        [HttpPost][Route("NewReportEntry")]
        public IActionResult AddNewEntryToClaim([FromBody] ClaimReportEntryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newEntry = new ClaimReportEntry(
                dto.Comment,
                dto.ClaimNumber
            );

            try
            {
                dbManager.InsertNewClaimReportEntry(newEntry);
                return Ok("Saved succesfully");
            }
            catch (DatabaseException ex)
            {
                return BadRequest($"The database couldnt process the request: {ex.Message}");
            }
        }
    }
}

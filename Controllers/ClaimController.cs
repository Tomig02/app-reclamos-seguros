using app_reclamos_seguros.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;


namespace app_reclamos_seguros.Controllers
{
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

        [HttpGet] [Route("Claim/{claimID}")]
        public ActionResult<VehicleClaimDTO> GetClaimByID(int? claimID)
        {
            if(claimID == null) 
            {
                return BadRequest("No id was present in the request"); 
            }
            else
            {
                string jsonString = dbManager.SelectCarClaimByID((int)claimID);

                Console.WriteLine(jsonString);
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

        [HttpGet] [Route("AllClaims")]
        public ActionResult<ClaimSearchResultDTO> GetAllClaims()
        {
            ClaimSearchResultDTO search = new ClaimSearchResultDTO(dbManager.SelectListAllCarClaims());
            return Ok(search);
        }

        [HttpPost] [Route("NewClaim")]
        public IActionResult AddNewCarClaim([FromBody] VehicleClaimDTO dto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
    }
}

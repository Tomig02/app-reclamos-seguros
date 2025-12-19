using app_reclamos_seguros.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace app_reclamos_seguros.Controllers
{

    /// <summary>
    /// Controller handling all the interactions related to the insurance claims report entries
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ClaimReportController : Controller
    {

        private IClaimsRepository dbManager;
        private readonly ILogger<ClaimController> _logger;
        public ClaimReportController(ILogger<ClaimController> logger, IClaimsRepository repository)
        {
            dbManager = repository;
            _logger = logger;
        }

        /// <summary>
        /// Receive the data of a new report entry of a claim, and save it into the database
        /// </summary>
        /// <param name="dto"> The data of a claim's new report entry </param>
        /// <returns> The resulting state of the requested action </returns>
        [HttpPost]
        [Route("NewReportEntry")]
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
                dbManager.SetNewReport(newEntry);
                return Ok("Saved succesfully");
            }
            catch (DatabaseException ex)
            {
                return BadRequest($"The database couldnt process the request: {ex.Message}");
            }
        }

        /// <summary>
        /// Search for all the event entries related to the specified claim number
        /// </summary>
        /// <param name="claimNum"> The number asigned to the claim by the insurance company </param>
        /// <returns></returns>
        [HttpGet]
        [Route("ClaimEntries/{claimNum}")]
        public ActionResult<ClaimReportEntryDTO[]> GetAllClaimEntries(int claimNum)
        {
            try
            {
                string entriesJson = dbManager.GetAllReportsByID(claimNum);
                JArray a = JArray.Parse(entriesJson);

                // parse the json array into the claim entry dto
                List<ClaimReportEntryDTO> entryList = new List<ClaimReportEntryDTO>();
                foreach (JObject item in JArray.Parse(entriesJson))
                {
                    entryList.Add(new ClaimReportEntryDTO(
                        (string)item.GetValue("comment")!,
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
    }
}

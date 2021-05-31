using System;
using System.Threading.Tasks;
using ClamAvProxy.Services;
using Microsoft.AspNetCore.Mvc;
using nClam;

namespace ClamAvProxy.Controllers
{
    public class ScanResult
    {
        public ScanResult()
        {
            VirusDetected = false;
            VirusName = null;
        }
        
        public bool VirusDetected { get; set; }
        public string VirusName { get; set; }
    }
    
    [ApiController]
    [Route("[controller]")]
    public class ClamController : ControllerBase
    {
        private readonly IClamAv _clamAv;
        
        public ClamController(IClamAv clamAv)
        {
            _clamAv = clamAv;
        }

        /// <summary>
        /// Pings The Clam AV
        /// </summary>
        /// <returns>200 if the ping was successful</returns>
        [HttpGet]
        public async Task<IActionResult> Ping()
        {
            try {
                await _clamAv.PingAsync();
                return new OkResult();
            }
            catch (Exception ex) {
                return StatusCode(500, new {ex.Message});
            }
        }

        /// <summary>
        /// Scans a given file from the request body
        /// </summary>
        /// <returns>The virus name is a virus is detected</returns>
        [HttpPost]
        [FileContent]
        public async Task<IActionResult> ScanFile()
        {
            ClamScanInfectedFile infectedFile;
            try {
                infectedFile = await _clamAv.ScanFileAsync(HttpContext.Request.Body);
            }
            catch (Exception ex) {
                return StatusCode(500, new {ex.Message});
            }

            return infectedFile == null
                ? new OkObjectResult(new ScanResult())
                : new OkObjectResult(new ScanResult {VirusDetected = true, VirusName = infectedFile.VirusName.Trim()});
        }
    }
}
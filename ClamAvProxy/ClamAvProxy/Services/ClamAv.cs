using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using nClam;

namespace ClamAvProxy.Services
{
    public class ClamAv : IClamAv
    {
        private readonly ClamClient _clam;
        
        public ClamAv(IConfiguration configuration)
        {
            var clamConfig = configuration.GetSection("Clam");

            var host = clamConfig[nameof(ClamConfig.Host)];
            var okPort = int.TryParse(clamConfig[nameof(ClamConfig.Port)], out var port);

            if (string.IsNullOrWhiteSpace(host) || !okPort || port is < 0 or > 65535) {
                throw new ArgumentException(
                    $"Clam Setting '{nameof(ClamConfig.Host)}' cannot be null ({host}) and Clam Setting '{nameof(ClamConfig.Port)}' must be a valid port number ({port})");
            }

            _clam = new ClamClient(host, port) {MaxStreamSize = 104857600}; //100 MB
        }
        
        public async Task<bool> PingAsync()
        {
            return await _clam.PingAsync();
        }

        public async Task<ClamScanInfectedFile> ScanFileAsync(Stream file)
        {
            var scanResult = await _clam.SendAndScanFileAsync(file);

            return scanResult.Result switch {
                ClamScanResults.Error or ClamScanResults.Unknown => throw new Exception(
                    $"Error Scanning File: {scanResult.RawResult}"),
                ClamScanResults.VirusDetected => scanResult.InfectedFiles.First(),
                _ => null
            };
        } 
    }
}
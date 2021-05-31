using System.IO;
using System.Threading.Tasks;
using nClam;

namespace ClamAvProxy.Services
{
    public interface IClamAv
    {
        public Task<bool> PingAsync();

        public Task<ClamScanInfectedFile> ScanFileAsync(Stream file);
    }
}
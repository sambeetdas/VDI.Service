using Microsoft.AspNetCore.Mvc;
using Renci.SshNet;

namespace VDI.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TunnelController : ControllerBase
    {
        private static SshClient _sshClient;
        private static ForwardedPortLocal _portForwarded;

        [HttpPost("start")]
        public IActionResult StartTunnel()
        {
            if (_sshClient != null && _sshClient.IsConnected)
            {
                return BadRequest("Tunnel is already running.");
            }

            string sshHost = "your.ssh.server";
            string sshUsername = "sshUsername";
            string sshPassword = "sshPassword";
            string vdiHost = "vdi.server";
            int vdiPort = 3389; // Default VDI port
            int localPort = 12345;

            try
            {
                _sshClient = new SshClient(sshHost, sshUsername, sshPassword);
                _sshClient.Connect();

                if (_sshClient.IsConnected)
                {
                    _portForwarded = new ForwardedPortLocal("127.0.0.1", (uint)localPort, vdiHost, (uint)vdiPort);
                    _sshClient.AddForwardedPort(_portForwarded);
                    _portForwarded.Start();

                    return Ok("Tunnel started. Access VDI via localhost:" + localPort);
                }
                else
                {
                    return StatusCode(500, "SSH connection failed.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("stop")]
        public IActionResult StopTunnel()
        {
            if (_sshClient == null || !_sshClient.IsConnected)
            {
                return BadRequest("Tunnel is not running.");
            }

            try
            {
                _portForwarded.Stop();
                _sshClient.Disconnect();
                _sshClient.Dispose();
                _sshClient = null;

                return Ok("Tunnel stopped.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

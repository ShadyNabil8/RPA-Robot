using FluentFTP;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rpaService
{
    public class FtpClient
    {
        public static void DownloadFiles()
        {
            using (var ftp = new FluentFTP.FtpClient("192.168.1.4", "shady", "shady"))
            {
                try 
                {
                    ftp.Connect();
                }
                catch 
                {
                    Log.Information("Can not connet to the FTP server!!");
                }
                try 
                {
                    ftp.DownloadFiles(@"D:\test\GET\",
                        new[] {
                        @"/ftp_fol/car.jpg"
                        });
                }
                catch 
                {
                    Log.Information("Error while downloading!");
                }  
            }
        }
    }
}

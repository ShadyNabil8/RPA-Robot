using FluentFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rpaService
{
    public class FtpClient
    {
        public void DownloadFiles()
        {
            using (var ftp = new FluentFTP.FtpClient("192.168.1.4", "shady", "shady"))
            {
                ftp.Connect();

                // download many files, skip if they already exist on disk
                ftp.DownloadFiles(@"D:\test\GET\",
                    new[] {
                        @"/ftp_fol/car.jpg"
                    }, FtpLocalExists.Skip);

            }
        }
    }
}

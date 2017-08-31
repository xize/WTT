﻿/*
    A security toolkit for windows    

    Copyright(C) 2016-2017 Guido Lucassen

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.If not, see<http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using windows_security_tweak_tool.src.certificates;
using windows_security_tweak_tool.src.utils;

namespace windows_security_tweak_tool.src.ninite
{
    class NiniteCreator : HashSet<NiniteOption>
    {
        
        public string GetNiniteURL()
        {
            string url = "https://ninite.com/";
            NiniteOption[] options = this.ToArray();
            
            for (int index = 0; index < options.Length; index++)
            {
                if(index == (options.Length-1))
                {
                    NiniteOption option = options[index];
                    url += ((NiniteOption)option).GetName();
                } else
                {
                    NiniteOption option = options[index];
                    url += ((NiniteOption)option).GetName()+"-";
                }
            }
            return url+"/ninite.exe";
        }

        public void DownloadNiniteInstaller(string url)
        {
            if(!Directory.Exists(GetDataFolder() + @"\ninite"))
            {
                Directory.CreateDirectory(GetDataFolder() + @"\ninite");
            }
            if (File.Exists(GetDataFolder() + @"\ninite\ninite.exe"))
            {
                File.Delete(GetDataFolder() + @"\ninite\ninite.exe");
            }
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.59 Safari/537.36");
            try
            {
                client.DownloadFile(new Uri(url), GetDataFolder() + @"\ninite\ninite.exe");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                MessageBox.Show("Unable to download ninite!, perhaps the file has been removed?\nplease visit: " + url);
                Process proc1 = Process.Start(url);
                proc1.Dispose();
                Directory.Delete(GetDataFolder() + @"\ninite");
                return;
            }

            while(!VerifyInstaller())
            {
                MessageBox.Show("Re-downloading ninite installer since the certificate did not match with the current hash of the certificate!", "warning ninite installer is invalid or being tampered with!");
                File.Delete(GetDataFolder() + @"\ninite\ninite.exe");
                client.DownloadFile(new Uri(url), GetDataFolder() + @"\ninite\ninite.exe");
            }

            Process proc = Process.Start(GetDataFolder()+@"\ninite\ninite.exe");

            proc.WaitForExit();
            proc.Dispose();
        }

        private string GetDataFolder()
        {
            return Config.GetConfig().GetDataFolder();
        }

        private bool VerifyInstaller()
        {
            string certhash = CertProvider.NINITE.getCertificate().getHash();
            X509Certificate cert = X509Certificate.CreateFromSignedFile(GetDataFolder() + @"\ninite\ninite.exe");
            return AuthenticodeTools.IsTrusted(GetDataFolder() + @"\ninite\ninite.exe") && certhash == cert.GetCertHashString();
        }

    }
}

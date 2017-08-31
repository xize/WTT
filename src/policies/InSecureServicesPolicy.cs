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
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace windows_security_tweak_tool.src.policies
{
    class InSecureServicesPolicy : SecurityPolicy
    {

        private Dictionary<String, ServiceType> services = new Dictionary<string, ServiceType>();

        public InSecureServicesPolicy()
        {
            services.Add("Browser", ServiceType.MANUAL);
            services.Add("TrkWks", ServiceType.AUTOMATIC);
            services.Add("Dnscache", ServiceType.AUTOMATIC);
            services.Add("fdPHost", ServiceType.MANUAL);
            services.Add("FDResPub", ServiceType.MANUAL);
            services.Add("HomeGroupProvider", ServiceType.MANUAL);
            services.Add("SharedAccess", ServiceType.DISABLED);
            services.Add("iphlpsvc", ServiceType.AUTOMATIC);
            services.Add("KtmRm", ServiceType.MANUAL);
            services.Add("lltdsvc", ServiceType.MANUAL);
            services.Add("NetTcpPortSharing", ServiceType.DISABLED);
            services.Add("Netlogon", ServiceType.MANUAL);
            services.Add("NcdAutoSetup", ServiceType.MANUAL);
            services.Add("NcaSvc", ServiceType.MANUAL);
            services.Add("PNRPsvc", ServiceType.MANUAL);
            services.Add("p2psvc", ServiceType.MANUAL);
            services.Add("p2pimsvc", ServiceType.MANUAL);
            services.Add("PerfHost", ServiceType.MANUAL);
            services.Add("pla", ServiceType.MANUAL);
            services.Add("PNRPAutoReg", ServiceType.MANUAL);
            services.Add("QWAVE", ServiceType.MANUAL);
            services.Add("RasMan", ServiceType.MANUAL);
            services.Add("SessionEnv", ServiceType.MANUAL);
            services.Add("TermService", ServiceType.MANUAL);
            services.Add("UmRdpService", ServiceType.MANUAL);
            services.Add("RemoteAccess", ServiceType.DISABLED);
            services.Add("seclogon", ServiceType.MANUAL);
            services.Add("SstpSvc", ServiceType.MANUAL);
            services.Add("LanmanServer", ServiceType.AUTOMATIC);
            services.Add("SNMPTRAP", ServiceType.MANUAL);
            services.Add("SSDPSRV", ServiceType.MANUAL);
            services.Add("lmhosts", ServiceType.MANUAL);
            services.Add("TapiSrv", ServiceType.MANUAL);
            services.Add("upnphost", ServiceType.MANUAL);
            services.Add("WerSvc", ServiceType.MANUAL);
            services.Add("Wecsvc", ServiceType.MANUAL);
            services.Add("WMPNetworkSvc", ServiceType.MANUAL);
            services.Add("WinRM", ServiceType.MANUAL);
            services.Add("wmiApSrv", ServiceType.MANUAL);
            services.Add("workfolderssvc", ServiceType.MANUAL);
            services.Add("LanmanWorkstation", ServiceType.AUTOMATIC);
            services.Add("irmon", ServiceType.MANUAL);
            services.Add("BthHFSrv", ServiceType.MANUAL);
        }

        public override string getName()
        {
            return getType().getName();
        }

        public override SecurityPolicyType getType()
        {
            return SecurityPolicyType.IN_SECURE_SERVICES_POLICY;
        }

        public override string getDescription()
        {
            return "disables all the services not needed by Windows";
        }

        public override bool isEnabled()
        {
            return Config.getConfig().getBoolean("insecure-services");
        }

        public override void apply()
        {
            getButton().Enabled = false;

            foreach (KeyValuePair<string, ServiceType> service in services)
            {
                if(this.IsServiceStarted(service.Key))
                {
                        this.StopService(service.Key, this);
                }
                    this.SetServiceType(service.Key, ServiceType.DISABLED);
            }
            Config.getConfig().put("insecure-services", true);
            setGuiEnabled(this);
            getButton().Enabled = true;
        }

        public override void unapply()
        {
            getButton().Enabled = false;

            foreach (KeyValuePair<string, ServiceType> service in services)
            {
                this.SetServiceType(service.Key, service.Value);
            }
            Config.getConfig().put("insecure-services", false);

            getButton().Enabled = true;
            setGuiDisabled(this);

        }

        public override bool hasIncompatibilityIssues()
        {
            return false;
        }

        [Obsolete]
        public override bool isLanguageDepended()
        {
            return false;
        }

        public override bool isMacro()
        {
            return false;
        }

        public override bool isSafeForBussiness()
        {
            return true;
        }

        public override bool isSecpolDepended()
        {
            return false;
        }

        public override bool isUserControlRequired()
        {
            return false;
        }

        public override Button getButton()
        {
            return gui.insecureservicesbtn;
        }

        public override ProgressBar getProgressbar()
        {
            return gui.insecureserviceprogress;
        }
    }
}

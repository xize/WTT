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
using System.Windows.Forms;
using windows_security_tweak_tool.src.optionalpolicies;

namespace windows_security_tweak_tool.src.policies
{
    class SecurityPolicyType {

        protected static HashSet<SecurityPolicyType> types = new HashSet<SecurityPolicyType>();

        public static SecurityPolicyType TEMP_POLICY = new SecurityPolicyType("temp_policy", new TempPolicy());
        public static SecurityPolicyType WSCRIPT_POLICY = new SecurityPolicyType("wscript_policy", new WscriptPolicy());
        public static SecurityPolicyType UPDATE_POLICY = new SecurityPolicyType("update_policy", new UpdatePolicy());
        public static SecurityPolicyType UAC_POLICY = new SecurityPolicyType("uac_policy", new UacPolicy());
        public static SecurityPolicyType NETBIOS_POLICY = new SecurityPolicyType("netbios_policy", new NetbiosPolicy());
        public static SecurityPolicyType RENAME_POLICY = new SecurityPolicyType("rename_policy", new RenamePolicy());
        public static SecurityPolicyType REMOTE_REGISTRY_POLICY = new SecurityPolicyType("remote_registry_policy", new RemoteRegistryPolicy());
        public static SecurityPolicyType RDP_POLICY = new SecurityPolicyType("rdp_policy", new RDPPolicy());
        public static SecurityPolicyType NTLM_POLICY = new SecurityPolicyType("ntlm_policy", new NTLMPolicy());
        public static SecurityPolicyType MBR_POLICY = new SecurityPolicyType("mbr_policy", new MBRPolicy());
        public static SecurityPolicyType CERT_POLICY = new SecurityPolicyType("cert_policy", new CertPolicy());
        public static SecurityPolicyType NETSHARE_POLICY = new SecurityPolicyType("netshare_policy", new NetSharePolicy());
        public static SecurityPolicyType LLMNR_POLICY = new SecurityPolicyType("llmnr_policy", new LLMNRPolicy());
        public static SecurityPolicyType IN_SECURE_SERVICES_POLICY = new SecurityPolicyType("in_secure_services_policy", new InSecureServicesPolicy());
        public static SecurityPolicyType UNSIGNED_POLICY = new SecurityPolicyType("unsigned_policy", new UnsignedPolicy());
        public static SecurityPolicyType SMB_SHARING_POLICY = new SecurityPolicyType("smb_sharing_policy", new SMBSharingPolicy());
        public static SecurityPolicyType AUTOPLAY_POLICY = new SecurityPolicyType("autoplay_policy", new AutoPlayPolicy());
        public static SecurityPolicyType REGSERVR32_PROXY_POLICY = new SecurityPolicyType("regsvr32_proxy_policy", new Regsvr32ProxyPolicy());
        public static SecurityPolicyType POWERSHELL_POLICY = new SecurityPolicyType("powershell_policy", new PowershellPolicy());

        private string name;
        private SecurityPolicy pol;
        private int priority = 1;

        public SecurityPolicyType(string name, SecurityPolicy pol)
        {
            types.Add(this);
            this.name = name;
            this.pol = pol;

            this.priority = (this.pol.isAsync() ? 1 : 0);

        }

        public string GetName()
        {
            return name;
        }

        public SecurityPolicy GetPolicy(Window wind)
        {
            pol.SetGui(wind);
            return pol;
        }

        public int GetPriority()
        {
            return this.priority;
        }

        public static SecurityPolicyType ValueOf(string name)
        {
            foreach(SecurityPolicyType type in Values())
            {
                if(type.startsWith(name.ToUpper(), type.GetName().ToUpper()))
                {
                    return type;
                }
            }
            return null;
        }

        public static SecurityPolicyType[] Values()
        {
            SecurityPolicyType[] t = types.ToArray();
            Array.Sort(t, new PolicyComparable());
            return t;
        }

        private bool startsWith(string text, string fulltext)
        {
            return fulltext.IndexOf(text) > -1;
        }
    }

    class PolicyComparable : IComparer<SecurityPolicyType>
    {
        public int Compare(SecurityPolicyType a, SecurityPolicyType b)
        {
            //higher the priority for policies which are not macros and are async and does not need user control.
            return (a.GetPriority() as IComparable).CompareTo(b.GetPriority());
        }

        public int GetHashCode(SecurityPolicyType obj)
        {
            return StringComparer.InvariantCultureIgnoreCase.GetHashCode(obj.GetName());
        }
    }

}

﻿using System.Collections.Generic;
using System.Linq;

namespace windows_tweak_tool.src.policies
{
    class PolicyType {

        protected static HashSet<PolicyType> types = new HashSet<PolicyType>();

        public static PolicyType TEMP_POLICY = new PolicyType("temp_policy", new TempPolicy());
        public static PolicyType WSCRIPT_POLICY = new PolicyType("wscript_policy", new WscriptPolicy());
        public static PolicyType UPDATE_POLICY = new PolicyType("update_policy", new UpdatePolicy());

        private string name;
        private Policy pol;

        private PolicyType(string name, Policy pol)
        {
            types.Add(this);
            this.name = name;
            this.pol = pol;
        }

        public string getName()
        {
            return name;
        }

        public Policy getPolicy(window wind)
        {
            pol.setGui(wind);
            return pol;
        }

        public static PolicyType valueOf(string name)
        {
            foreach(PolicyType type in values())
            {
                if(type.getName().ToUpper() == name.ToUpper())
                {
                    return type;
                }
            }
            return null;
        }

        public static PolicyType[] values()
        {
            return types.ToArray();
        }
    }
}

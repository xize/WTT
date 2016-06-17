﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace windows_tweak_tool.policies
{
    class TempExecution : Policy
    {

        public string getName()
        {
            return getType().getName();
        }

        public PolicyType getType()
        {
            return PolicyType.POLICY_TEMP_EXECUTION;
        }

        public string getDescription()
        {
            return "prevents execution on the %temp% folder for normal users\r\ninstallers use trustedinstaller as user this mean that malware has almost zero chance to be installed on the system";
        }

        public bool isEnabled()
        {
            return Config.getConfig().getBoolean("softrestriction");
        }

        public void setEnabled()
        {
            PolicyManager.getRegPolicy().setPolicyByRegFile("reg/softrestriction_add.reg");
            Config.getConfig().put("softrestriction", true);
        }

        public void setDisabled()
        {
            PolicyManager.getRegPolicy().setPolicyByRegFile("reg/softrestriction_add.reg");
            Config.getConfig().put("softrestriction", false);
         }
    }
}

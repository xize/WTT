﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace windows_tweak_tool
{
    interface Policy
    {

        string getName();

        PolicyType getType();

        string getDescription();

        bool isEnabled();

        bool setEnabled();

        object[] getRegistryObject();

    }
}

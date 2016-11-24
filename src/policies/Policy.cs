﻿/*
    A security toolkit for windows    

    Copyright(C) 2016 Guido Lucassen

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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using Microsoft.Win32;
using System.ServiceProcess;
using AutoIt;

namespace windows_tweak_tool.src.policies
{
    abstract class Policy
    {
        private int version = -1;
        protected window gui;

        protected Policy()
        {
            
        }

        public void setGui(window win)
        {
            if(this.gui != win)
            {
                this.gui = win;
            }
        }

        public void setGuiEnabled(Policy p)
        {
            p.getProgressbar().Value = 100;
            p.getButton().Text = "Undo";
        }

        public void setGuiDisabled(Policy p)
        {
            p.getProgressbar().Value = 0;
            p.getButton().Text = "Apply";
        }

        public bool isServiceStarted(string service)
        {
            ServiceController controller = new ServiceController(service);
            if(controller.Status == ServiceControllerStatus.Running)
            {
                return true;
            }
            return false;
        }

        public void startService(string service)
        {
            ServiceController controller = new ServiceController(service);
            if(controller.CanStop)
            {
                controller.Start();
            }
        }

        public void stopService(string service)
        {
            ServiceController controller = new ServiceController(service);
            if(controller.CanStop) {
                controller.Stop();
            }
        }

        public void setServiceType(string service, ServiceType type)
        {
            int stype = 4;
            switch(type)
            {
                case ServiceType.AUTOMATIC_SLOWED:
                    stype = 1;
                    break;
                case ServiceType.AUTOMATIC:
                    stype = 2;
                    break;
                case ServiceType.MANUAL:
                    stype = 3;
                    break;
                case ServiceType.DISABLED:
                    stype = 4;
                    break;
                default:
                    stype = 4;
                    break;
            }
            RegistryKey key = getRegistry(@"SYSTEM\CurrentControlSet\Services\"+service, REG.HKLM);
            key.SetValue("Start", stype);
            key.Close();
        }

        public enum ServiceType {
            AUTOMATIC_SLOWED,
            AUTOMATIC,
            MANUAL,
            DISABLED
        }

        public ServiceType getServiceStatus(string service)
        {
            RegistryKey key = getRegistry(@"SYSTEM\CurrentControlSet\Services\" + service, REG.HKLM);
            int status = (int)key.GetValue("Start");
            switch(status)
            {
                case 1:
                    return ServiceType.AUTOMATIC_SLOWED;
                case 2:
                    return ServiceType.AUTOMATIC;
                case 3:
                    return ServiceType.MANUAL;
                case 4:
                    return ServiceType.DISABLED;
                default:
                    return ServiceType.MANUAL;
            }
        }

        public abstract bool isUserControlRequired();

        public abstract bool isSecpolDepended();

        public bool isSecpolEnabled()
        {
                ManagementObjectSearcher search = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
                var name = (from x in search.Get().OfType<ManagementObject>() select x.GetPropertyValue("Caption")).FirstOrDefault();
                search.Dispose();

                if (name != null)
                {
                    string[] OS = name.ToString().Split(' ');
                    string OS_NAME = OS[3];
                    switch (OS_NAME)
                    {
                        case "Pro":
                            return true;
                        case "Ultimate":
                            return true;
                        case "Professional":
                            return true;
                        default:
                            return false;
                    }
                }
            return false;
        }

        public int getWindowsVersion()
        {
            if(version > -1)
            {
                return version;
            } else
            {
                ManagementObjectSearcher search = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
                var name = (from x in search.Get().OfType<ManagementObject>() select x.GetPropertyValue("Caption")).FirstOrDefault();
                search.Dispose();

                if (name != null)
                {
                    string[] OS = name.ToString().Split(' ');
                    return Convert.ToInt32(OS[2]);
                }
            }
            throw new Exception("Failed to get Windows version, maybe WMI is broken?");
        }

        public abstract bool isMacro();

        public abstract bool isSafeForBussiness();

        /**
         * <summary>
         *      <para>this method has been deprecated, since we are not sure if we can make AutoIT macros language independed, this method will be a fallback.</para>
         *      <para></para>
         *      <para>returns true if the policy is language depended otherwise false.</para>
         * </summary>
         *
         * <returns>bool</returns>
         * */
        [Obsolete]
        public abstract bool isLanguageDepended();

        public abstract bool hasIncompatibilityIssues();

        public bool isAutoItInstalled()
        {

            string b32 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\AutoIt3\AutoItX\AutoItX3.dll";
            string b64 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\AutoIt3\AutoItX\AutoItX3.dll";

            if (File.Exists(b32) || File.Exists(b64))
            {
                return true;
            }
            return false;
        }

        public abstract string getName();

        public abstract PolicyType getType();

        public abstract string getDescription();

        public abstract bool isEnabled();

        public abstract void apply();

        public abstract void unapply();

        public abstract ProgressBar getProgressbar();

        public abstract Button getButton();

        protected String generateGUID()
        {
            return "{"+Guid.NewGuid().ToString()+"}";
        }

        public string getDataFolder()
        {
            return Config.getConfig().getDataFolder();
        }

        public RegistryKey getRegistry(string path, REG reg)
        {
            if (path == null)
            {
                switch (reg)
                {
                    case REG.HKCR:
                        return (Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry32));
                    case REG.HKCU:
                        return (Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32));
                    case REG.HKLM:
                        return (Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32));
                    default:
                        return (Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32));
                }
            } else {
                switch (reg)
                {
                    case REG.HKCR:
                        return (Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry32)).OpenSubKey(path, true);
                    case REG.HKCU:
                        return (Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32)).OpenSubKey(path, true);
                    case REG.HKLM:
                        return (Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)).OpenSubKey(path, true);
                    default:
                        return (Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)).OpenSubKey(path, true);
                }
            }
        }

       public enum REG
        {
            HKLM,
            HKCU,
            HKCR
        }
    }
}

﻿// This file is Part of CalDavSynchronizer (http://outlookcaldavsynchronizer.sourceforge.net/)
// Copyright (c) 2015 Gerhard Zehetbauer 
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using CalDavSynchronizer.Contracts;
using CalDavSynchronizer.Utilities;
using Microsoft.Win32;

namespace CalDavSynchronizer.DataAccess
{
  public class GeneralOptionsDataAccess : IGeneralOptionsDataAccess
  {
    private const string s_shouldCheckForNewerVersionsValueName = "CheckForNewerVersions";
    private const string s_storeAppDataInRoamingFolder = "StoreAppDataInRoamingFolder";
    private const string s_OptionsRegistryKey = @"Software\CalDavSynchronizer";


    public GeneralOptions LoadOptions ()
    {
      using (var key = OpenOptionsKey())
      {
        return new GeneralOptions()
               {
                   ShouldCheckForNewerVersions = (int) (key.GetValue (s_shouldCheckForNewerVersionsValueName) ?? 1) != 0,
                   StoreAppDataInRoamingFolder = (int) (key.GetValue (s_storeAppDataInRoamingFolder) ?? 0) != 0,
               };
      }
    }

    public void SaveOptions (GeneralOptions options)
    {
      using (var key = OpenOptionsKey())
      {
        key.SetValue (s_shouldCheckForNewerVersionsValueName, options.ShouldCheckForNewerVersions ? 1 : 0);
        key.SetValue (s_storeAppDataInRoamingFolder, options.StoreAppDataInRoamingFolder ? 1 : 0);
      }
    }

    public Version IgnoreUpdatesTilVersion
    {
      get
      {
        using (var key = OpenOptionsKey())
        {
          var versionString = (string) key.GetValue ("IgnoreUpdatesTilVersion");
          if (!string.IsNullOrEmpty (versionString))
            return new Version (versionString);
          else
            return null;
        }
      }
      set
      {
        using (var key = OpenOptionsKey())
        {
          if (value != null)
            key.SetValue ("IgnoreUpdatesTilVersion", value.ToString());
          else
            key.DeleteValue ("IgnoreUpdatesTilVersion");
        }
      }
    }

    private static RegistryKey OpenOptionsKey ()
    {
      var key = Registry.CurrentUser.OpenSubKey (s_OptionsRegistryKey, true);
      if (key == null)
        key = Registry.CurrentUser.CreateSubKey (s_OptionsRegistryKey);
      return key;
    }
  }
}
#region license

// Copyright (c) 2005 - 2008 Ahmar Tareen (ahmar.tareen@gmail.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Rhino.Commons
{
    ///<summary>
    /// Can be used to access the 64-bit registry regardless of the process
    /// the code is actually running in.
    ///</summary>
    public class Registry64
    {
        private const int KEY_WOW64_64KEY = 0x100;
        private const int KEY_WOW64_32KEY = 0x200;
        private const int READ_RIGHTS = 131097;
        private readonly IntPtr _hKey;


        ///<summary>
        /// Initializes a new instance of the <see cref="Registry64"/> class.
        ///</summary>
        ///<param name="_hKey">The base HKey</param>
        private Registry64(IntPtr _hKey)
        {
            this._hKey = _hKey;
        }


        ///<summary>
        /// Returns the value data for the specified value name under the
        /// given sub key.
        ///</summary>
        ///<param name="subKey">The sub key to look under.</param>
        ///<param name="valueName">The name fo the value to retrieve.</param>
        ///<returns>The value data as a string.</returns>
        public string GetValue(string subKey, string valueName)
        {
            IntPtr openKey = OpenSubKey(subKey);

            try
            {
                KeyValueInfo keyValueInfo = GetKeyValueInfo(openKey, valueName);
                return GetKeyValueData(openKey, keyValueInfo);
            }
            finally
            {
                RegCloseKey(openKey);
            }
        }


        private string GetKeyValueData(IntPtr openKey, KeyValueInfo keyValueName)
        {
            StringBuilder keyValue = new StringBuilder(((int)keyValueName.Length) - 1);
            int resultCode = RegQueryValueEx(openKey,
                                             keyValueName.Name,
                                             0,
                                             out keyValueName.Type,
                                             keyValue,
                                             ref keyValueName.Length);
            if (resultCode != 0) ThrowException(resultCode);
            return keyValue.ToString();
        }


        private KeyValueInfo GetKeyValueInfo(IntPtr openKey, string valueName)
        {
            uint keyType;
            uint keyValueLength = 0u;
            int resultCode = RegQueryValueEx(openKey, valueName, 0, out keyType, null, ref keyValueLength);
            if (resultCode != 0) ThrowException(resultCode);

            return new KeyValueInfo(keyType, keyValueLength, valueName);
        }


        private IntPtr OpenSubKey(string subKey)
        {
            IntPtr openKey;
            int resultCode = RegOpenKeyEx(_hKey, subKey, 0, KEY_WOW64_64KEY | READ_RIGHTS, out openKey);
            if (2 == resultCode)
                resultCode = RegOpenKeyEx(_hKey, subKey, 0, KEY_WOW64_32KEY | READ_RIGHTS, out openKey);
            if (resultCode != 0) ThrowException(resultCode);
            return openKey;
        }


        private void ThrowException(int errorCode)
        {
            switch (errorCode)
            {
                case 2:
                    throw new InvalidOperationException("Error 2: Key or value name not found.");
                case 3:
                    throw new InvalidOperationException("Error 3: Path not found.");
                case 5:
                    throw new InvalidOperationException("Error 5: Access is denied.");
                case 6:
                    throw new InvalidOperationException("Error 6: Invalid handle");
                case 9:
                    throw new InvalidOperationException("Error 9: Invalid block");
                case 12:
                    throw new InvalidOperationException("Error 12: Invalid Access");
                default:
                    throw new InvalidOperationException("Error " + errorCode +
                                                        ". Please refer to msdn documention on Winerror.h for further information.");
            }
        }



        private static readonly IntPtr HKEY_LOCAL_MACHINE = new IntPtr(-2147483646);


        ///<summary>
        /// Initializes a new instance of the <see cref="Registry64"/> class
        /// with HKEY_LOCAL_MACHINE set as the HKey.
        ///</summary>
        public static Registry64 LocalMachine
        {
            get { return new Registry64(HKEY_LOCAL_MACHINE); }
        }


        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int RegCloseKey(
            IntPtr hKey);


        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegOpenKeyEx")]
        private static extern int RegOpenKeyEx(
            IntPtr hKey,
            string subKey,
            uint options,
            int sam,
            out IntPtr phkResult);


        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW", SetLastError = true)]
        private static extern int RegQueryValueEx(
            IntPtr hKey,
            string lpValueName,
            int lpReserved,
            out uint lpType,
            StringBuilder lpData,
            ref uint lpcbData);


        private class KeyValueInfo
        {
            public uint Length;
            public string Name;
#pragma warning disable 219
            public uint Type;
#pragma warning restore 219


            public KeyValueInfo(uint type, uint length, string name)
            {
                Type = type;
                Length = length;
                Name = name;
            }
        }
    }
}

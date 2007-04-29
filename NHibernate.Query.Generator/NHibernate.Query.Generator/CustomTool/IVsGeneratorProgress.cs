#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
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


namespace CustomToolGenerator {

    using System;
    using System.Runtime.InteropServices;

    [
        ComImport, 
        Guid("BED89B98-6EC9-43CB-B0A8-41D6E2D6669D"), 
        InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)
    ]
    public interface IVsGeneratorProgress {
        //
        // Communicate errors
        // HRESULT GeneratorError([in] BOOL fWarning,                    
        //                        [in] DWORD dwLevel,
        //                        [in] BSTR bstrError,
        //                        [in] DWORD dwLine,
        //                        [in] DWORD dwColumn);
        //
        void GeneratorError(				bool fWarning,
              [MarshalAs(UnmanagedType.U4)] int dwLevel,
            [MarshalAs(UnmanagedType.BStr)] string bstrError,
              [MarshalAs(UnmanagedType.U4)] int dwLine,
              [MarshalAs(UnmanagedType.U4)] int dwColumn);

        //
        // Report progress to the caller.
        // HRESULT Progress([in] ULONG nComplete,        // Current position
        //                  [in] ULONG nTotal);          // Max value
        //
        void Progress(
            [MarshalAs(UnmanagedType.U4)] int nComplete, 
            [MarshalAs(UnmanagedType.U4)] int nTotal);
    }
}

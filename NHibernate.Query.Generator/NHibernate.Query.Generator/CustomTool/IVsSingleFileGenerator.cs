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
        Guid("3634494C-492F-4F91-8009-4541234E4E99"), 
        InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)
    ]
    public interface IVsSingleFileGenerator {
        //
        // Retrieve default properties for the generator
        // [propget]   HRESULT DefaultExtension([out,retval] BSTR* pbstrDefaultExtension);
        //
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetDefaultExtension();

        //
        // Generate the file
        // HRESULT Generate([in] LPCOLESTR wszInputFilePath,
        //					[in] BSTR bstrInputFileContents,
        //					[in] LPCOLESTR wszDefaultNamespace, 
        //					[out] BYTE**    rgbOutputFileContents,
        //					[out] ULONG*    pcbOutput,
        //					[in] IVsGeneratorProgress* pGenerateProgress);
        //
        void Generate(
            [MarshalAs(UnmanagedType.LPWStr)] string wszInputFilePath,
              [MarshalAs(UnmanagedType.BStr)] string bstrInputFileContents,
            [MarshalAs(UnmanagedType.LPWStr)] string wszDefaultNamespace, 
                                              out IntPtr rgbOutputFileContents,
                [MarshalAs(UnmanagedType.U4)] out int pcbOutput,
                                              IVsGeneratorProgress pGenerateProgress);
    }
}

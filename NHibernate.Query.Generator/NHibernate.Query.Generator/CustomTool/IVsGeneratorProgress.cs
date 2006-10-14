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

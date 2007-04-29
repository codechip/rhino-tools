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
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    
    /// <summary>
    ///     This wraps the IOleServiceProvider interface and provides an easy COM+ way to get at
    ///     services.
    /// </summary>
    public class ServiceProvider : IServiceProvider, IObjectWithSite {

        private static Guid IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");                    

        private IOleServiceProvider    serviceProvider;

        /// <summary>
        ///     Creates a new ServiceProvider object and uses the given interface to resolve
        ///     services.
        /// </summary>
        /// <param name='sp'>
        ///     The IOleServiceProvider interface to use.
        /// </param>
        public ServiceProvider(IOleServiceProvider sp) {
            serviceProvider = sp;
        }

        /// <summary>
        /// gives this class a chance to free its references.
        /// </summary>
        public virtual void Dispose() {
            if (serviceProvider != null) {
                serviceProvider = null;
            }
        }

        /// <summary>
        /// returns true if the given HRESULT is a failure HRESULT
        /// </summary>
        /// <param name="hr">HRESULT to test</param>
        /// <returns>true if the HRESULT is a failure, false if not.</returns>
        public static bool Failed(int hr) {
            return(hr < 0);
        }

        /// <summary>
        ///     Retrieves the requested service.
        /// </summary>
        /// <param name='serviceClass'>
        ///     The class of the service to retrieve.
        /// </param>
        /// <returns>
        ///     an instance of serviceClass or null if no
        ///     such service exists.
        /// </returns>
        public virtual object GetService(Type serviceClass) {

            if (serviceClass == null) {
                return null;
            }

            return GetService(serviceClass.GUID, serviceClass);
        }

        /// <summary>
        ///     Retrieves the requested service.
        /// </summary>
        /// <param name='guid'>
        ///     The GUID of the service to retrieve.
        /// </param>
        /// <returns>
        ///     an instance of the service or null if no
        ///     such service exists.
        /// </returns>
        public virtual object GetService(Guid guid) {
            return GetService(guid, null);
        }

        /// <summary>
        ///     Retrieves the requested service.  The guid must be specified; the class is only
        ///     used when debugging and it may be null.
        /// </summary>
        private object GetService(Guid guid, Type serviceClass) {

            // Valid, but wierd for caller to init us with a NULL sp
            //
            if (serviceProvider == null) {
                return null;
            }

            object service = null;

            // No valid guid on the passed in class, so there is no service for it.
            //
            if (guid.Equals(Guid.Empty)) {
                return null;
            }

            // We provide a couple of services of our own.
            //
            if (guid.Equals(typeof(IOleServiceProvider).GUID)) {
                return serviceProvider;
            }
            if (guid.Equals(typeof(IObjectWithSite).GUID)) {
                return (IObjectWithSite)this;
            }

            IntPtr pUnk;
            int hr = serviceProvider.QueryService(ref guid, ref IID_IUnknown, out pUnk);

            if (Succeeded(hr) && (pUnk != IntPtr.Zero)) {
                service = Marshal.GetObjectForIUnknown(pUnk);
                Marshal.Release(pUnk);
            }

            return service;
        }

        /// <summary>
        ///     Retrieves the current site object we're using to
        ///     resolve services.
        /// </summary>
        /// <param name='riid'>
        ///     Must be IServiceProvider.class.GUID
        /// </param>
        /// <param name='ppvSite'>
        ///     Outparam that will contain the site object.
        /// </param>
        /// <seealso cref='IObjectWithSite'/>
        void IObjectWithSite.GetSite(ref Guid riid, object[] ppvSite) {
            ppvSite[0] = GetService(riid);
        }

        /// <summary>
        ///     Sets the site object we will be using to resolve services.
        /// </summary>
        /// <param name='pUnkSite'>
        ///     The site we will use.  This site will only be
        ///     used if it also implements IOleServiceProvider.
        /// </param>
        /// <seealso cref='IObjectWithSite'/>
        void IObjectWithSite.SetSite(object pUnkSite) {
            if (pUnkSite is IOleServiceProvider) {
                serviceProvider = (IOleServiceProvider)pUnkSite;
            }
        }

        /// <summary>
        /// returns true if the given HRESULT is a success HRESULT
        /// </summary>
        /// <param name="hr">HRESULT to test</param>
        /// <returns>true if the HRESULT is a success, false if not.</returns>
        public static bool Succeeded(int hr) {
            return(hr >= 0);
        }
    }
}


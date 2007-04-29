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


using System.Threading;

namespace Rhino.Commons
{
    public sealed class WorkItem
    {
        private WaitCallback _callback;
        private object _state;
        private bool cancelledRun;
        private ExecutionContext _ctx;

        internal WorkItem(WaitCallback wc, object state, ExecutionContext ctx)
        {
            _callback = wc;
            _state = state;
            _ctx = ctx;
        }

        internal WaitCallback Callback
        {
            get { return _callback; }
        }

        public object State
        {
            get { return _state; }
        }

        public ExecutionContext Context
        {
            get { return _ctx; }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WorkItem"/> is canceled.
        /// This is valid if and only if the work item has not started running yet.
        /// If the WorkItem has started running, it is ignored
        /// </summary>
        /// <value><c>true</c> if cancelledRun; otherwise, <c>false</c>.</value>
        public bool Cancelled
        {
            get { return cancelledRun; }
            set { cancelledRun = value; }
        }
    }
}
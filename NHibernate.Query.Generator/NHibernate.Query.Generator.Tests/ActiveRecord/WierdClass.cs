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


using System;
using System.Collections.Generic;
using System.Text;

using Castle.ActiveRecord;
namespace NHibernate.Query.Generator.Tests.ActiveRecord
{
    [ActiveRecord]
    public class WeirdClass : ActiveRecordBase<WeirdClass>
    {
        WeirdKey key = new WeirdKey();
        Address address = new Address();

        [CompositeKey]
        public WeirdKey Key
        {
            get { return key; }
            set { key = value; }
        }

        [Nested]
        public Address Address
        {
            get { return address; }
            set { address = value; }
        }
    }

    public class Address
    {
        string street;
        Electronic electronic;

        [Property]
        public string Street
        {
            get { return street; }
            set { street = value; }
        }

        [Nested]
        public Electronic Electronic
        {
            get { return electronic; }
            set { electronic = value; }
        }
    }

    public class Electronic
    {
        string email;

        [Property]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
    }

    [Serializable]
    public class WeirdKey
    {
        string department;
        int level;

        [KeyProperty]
        public string Department
        {
            get { return department; }
            set { department = value; }
        }

        [KeyProperty]
        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [ActiveRecord("_istoric"), JoinedBase]
    public class Istoric : ActiveRecordBase<Istoric>
    {
        int id;
        private string name;

        [PrimaryKey]
        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        [Property]
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
    }


    [ActiveRecord("_mesaj")]
    public class MesajIst : Istoric
    {
        private string email;
        [Property]
        public virtual string Email
        {
            get { return email; }
            set { email = value; }
        }
    }
}

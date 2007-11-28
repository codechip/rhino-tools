using System;
using System.Collections.Generic;
using Castle.ActiveRecord;

namespace NHibernate.Query.Generator.Tests.ActiveRecord
{
	[ActiveRecord]
	public class Patient : ActiveRecordBase<Patient>
	{
		private long id;
		private IList<PatientRecord> patientRecords = new List<PatientRecord>();

		[PrimaryKey(Generator = PrimaryKeyType.HiLo)]
		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		[HasMany(typeof(PatientRecord), "PatientId", "PatientRecords", Cascade = ManyRelationCascadeEnum.All)]
		public virtual IList<PatientRecord> PatientRecords
		{
			get { return patientRecords; }
			set { patientRecords = value; }
		}

		public Patient()
		{
		}

		public Patient(IEnumerable<PatientRecord> patientRecords)
		{
			foreach (PatientRecord record in patientRecords)
			{
				this.patientRecords.Add(record);
			}
		}
	}

	[ActiveRecord]
	public class PatientRecord : ActiveRecordBase<PatientRecord>
	{
		private long id;
		private Name name;
		private DateTime? birthDate;

		[PrimaryKey]
		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		[Nested]
		public virtual Name Name
		{
			get { return name; }
			set { name = value; }
		}

		[Property]
		public virtual DateTime? BirthDate
		{
			get { return birthDate; }
			set { birthDate = value; }
		}

		public PatientRecord()
		{
		}

		public PatientRecord(Name name, DateTime? birthDate)
		{
			this.name = name;
			this.birthDate = birthDate;
		}
	}

	public class Name
	{
		private string firstName;
		private string lastName;

		[Property]
		public virtual string FirstName
		{
			get { return firstName; }
			set { firstName = value; }
		}

		[Property]
		public virtual string LastName
		{
			get { return lastName; }
			set { lastName = value; }
		}

		public Name()
		{
		}

		public Name(string firstName, string lastName)
		{
			this.firstName = firstName;
			this.lastName = lastName;
		}
	}
}

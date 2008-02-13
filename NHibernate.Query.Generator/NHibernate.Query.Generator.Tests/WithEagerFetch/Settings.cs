using Iesi.Collections.Generic;

namespace NHibernate.Query.Generator.Tests.WithEagerFetch
{
	public class Settings
	{
		private ISet<UserSetting> _userSettings = new HashedSet<UserSetting>();
		private ISet<GlobalSetting> _globalSettings = new HashedSet<GlobalSetting>();

		public virtual ISet<UserSetting> UserSettings
		{
			get { return _userSettings; }
			set { _userSettings = value; }
		}

		public virtual ISet<GlobalSetting> GlobalSettings
		{
			get { return _globalSettings; }
			set { _globalSettings = value; }
		}
	}

	public abstract class Setting : KeyedDomainObject<int>
	{
		private string _value;
		private string _name;
		private Application _application;

		public Setting()
		{
		}

		public Setting(Application application, string name, string value)
		{
			_name = name;
			_value = value;
			_application = application;
		}

		public abstract string SettingType { get; }

		public virtual Application Application
		{
			get { return _application; }
			set { _application = value; }
		}

		public virtual string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public virtual string Name
		{
			get { return _name; }
			set { _name = value; }
		}
	}

	public class UserSetting : Setting
	{
		public UserSetting()
		{}

		public UserSetting(Application application, string name, string value) : base(application, name, value)
		{
		}

		public override string SettingType
		{
			get { return "User"; }
		}
	}

	public class GlobalSetting : Setting
	{
		public GlobalSetting()
		{}

		public GlobalSetting(Application application, string name, string value) : base(application, name, value)
		{
		}

		public override string SettingType
		{
			get { return "Global"; }
		}
	}
}
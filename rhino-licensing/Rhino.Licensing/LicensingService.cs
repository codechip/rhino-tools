namespace Rhino.Licensing
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.ServiceModel;

	// because we use this service behavior,
	// we don't have to worry about multi threading issues.
	// it is not something that we expect to have to deal with huge load, anyway
	[ServiceBehavior(
		InstanceContextMode = InstanceContextMode.Single,
		ConcurrencyMode = ConcurrencyMode.Single)]
	public class LicensingService : ILicensingService
	{
		public static string SoftwarePublicKey { get; set; }
		public static string LicenseServerPrivateKey { get; set; }

		private readonly List<LicenseValidator> availableLicenses = new List<LicenseValidator>();
		private readonly Dictionary<Guid, KeyValuePair<DateTime, LicenseValidator>> leasedLicenses = new Dictionary<Guid, KeyValuePair<DateTime, LicenseValidator>>();

		public LicensingService()
		{
			if (SoftwarePublicKey == null)
				throw new InvalidOperationException("SoftwarePublicKey must be set before starting the service");

			if (LicenseServerPrivateKey == null)
				throw new InvalidOperationException("LicenseServerPrivateKey must be set before starting the service");

			var licensesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Licenses");
			if (Directory.Exists(licensesDirectory) == false)
			{
				try
				{
					Directory.CreateDirectory(licensesDirectory);
				}
				catch (Exception)
				{
					throw new DirectoryNotFoundException("Could not find licenses directory: " + licensesDirectory);
				}
			}
			foreach (var license in Directory.GetFiles(licensesDirectory, "*.xml"))
			{
				var set = new HashSet<string>();
				var validator = new LicenseValidator(SoftwarePublicKey, license);
				try
				{
					validator.AssertValidLicense();
					if (validator.LicenseType == LicenseType.Standard && 
						// this prevent a simple cheating of simply copying the same
						// license file several times
						set.Add(validator.Name))
						availableLicenses.Add(validator);
				}
				catch (Exception)
				{
					continue;
				}
			}
		}

		public string LeaseLicense(Guid id)
		{
			KeyValuePair<DateTime, LicenseValidator> value;
			if (leasedLicenses.TryGetValue(id, out value))
			{
				var licenseValidator = value.Value;
				return GenerateLicenseAndRenewLease(id, licenseValidator);
			}
			if (availableLicenses.Count > 0)
			{
				var availableLicense = availableLicenses[availableLicenses.Count-1];
				availableLicenses.RemoveAt(availableLicenses.Count-1);
				return GenerateLicenseAndRenewLease(id, availableLicense);
			}
			foreach (var kvp in leasedLicenses)
			{
				if ((DateTime.Now - kvp.Value.Key).TotalMinutes < 45)
					continue;
				leasedLicenses.Remove(kvp.Key);
				return GenerateLicenseAndRenewLease(id, kvp.Value.Value);
			}
			return null;
		}

		private string GenerateLicenseAndRenewLease(Guid id, LicenseValidator licenseValidator)
		{
			leasedLicenses[id] = new KeyValuePair<DateTime, LicenseValidator>(DateTime.Now.AddMinutes(30), licenseValidator);
			return GenerateLicense(id, licenseValidator);
		}

		private static string GenerateLicense(Guid id, LicenseValidator validator)
		{
			var generator = new LicenseGenerator(LicenseServerPrivateKey);
			return generator.Generate(validator.Name, id, DateTime.Now.AddMinutes(45), LicenseType.Floating);
		}
	}
}
import System.Security.Principal

[Module]
class AuthorizationExtension:

	[Extension]
	static IsManager[principal as IPrincipal] as bool:
		get:
			return principal.IsInRole("Managers")
if Principal.IsInRole("Administrators"):
	Allow("Administrators can always log in")
	return

if date.Now.Hour < 9 or date.Now.Hour > 17:
	Deny("Cannot log in outside of business hours, 09:00 - 17:00")
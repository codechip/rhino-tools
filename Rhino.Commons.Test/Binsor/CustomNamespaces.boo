
for type in AllTypes('Rhino.Commons.Test') \
	.Where({ t as System.Type | t.Name.EndsWith('Repository') }):
	component type < type.GetRepositoryInterface()
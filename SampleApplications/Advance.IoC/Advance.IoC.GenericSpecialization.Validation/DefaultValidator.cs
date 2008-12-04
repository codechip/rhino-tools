using System.Collections.Generic;

namespace Advance.IoC.GenericSpecialization.Validation
{
    public class DefaultValidator<T> : IValidator<T>
    {
        public string[] Validate(T instnace)
        {
            List<string> validationErrors = new List<string>();
            foreach (var property in typeof(T).GetProperties())
            {
                if(property.GetValue(instnace, null) == null)
                    validationErrors.Add(property.Name + " is null");
            }
            return validationErrors.ToArray();
        }
    }
}
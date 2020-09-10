namespace Moonpig.PostOffice.Api.Attributes
{
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    public class NonEmptyListAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            var list = value as List<object>;
            return list != null && list.Count > 0;
        }
    }
}
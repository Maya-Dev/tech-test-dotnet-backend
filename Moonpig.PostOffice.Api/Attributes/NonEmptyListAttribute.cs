namespace Moonpig.PostOffice.Api.Attributes
{
    using System.ComponentModel.DataAnnotations;
    using System.Collections;

    public class NonEmptyListAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            var list = value as IEnumerable;
            return list != null && list.GetEnumerator().MoveNext();
        }
    }
}
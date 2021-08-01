using System.ComponentModel.DataAnnotations;

namespace BikeThefts.Api.DTO
{
    public class RequiredGreaterThanZeroValidation : ValidationAttribute
    {
        /// <param name="value">The float value of the selection</param>
        /// <returns>True if value is greater than zero</returns>
        public override bool IsValid(object value)
        {
            // return true if value is a non-null number > 0, otherwise return false
            return value != null && double.TryParse(value.ToString(), out double number) && number > 0;
        }
    }
}

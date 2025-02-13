using System.ComponentModel.DataAnnotations;

namespace RunningClub.Misc;

public class FutureDateAttribute:ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not DateTime)
            return false;
        DateTime date = (DateTime)value;
        // return date.Date >= DateTime.Now.AddDays(1);
        return date >= DateTime.Now.Date.ToLocalTime();
    }
}
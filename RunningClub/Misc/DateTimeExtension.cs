namespace RunningClub.Misc;

public static class DateTimeExtension
{
    public static string ShowDateTime(this DateTime dateTime)
    {
        string month = "";
        switch (dateTime.Month)
        {
            case 1:
            {
                month = "Jan";
                break;
            }
            case 2:
            {
                month = "Feb";
                break;
            }
            case 3:
            {
                month = "Mar";
                break;
            }
            case 4:
            {
                month = "Apr";
                break;
            }
            case 5:
            {
                month = "May";
                break;
            }
            case 6:
            {
                month = "Jun";
                break;
            }
            case 7:
            {
                month = "Jul";
                break;
            }
            case 8:
            {
                month = "Aug";
                break;
            }
            case 9:
            {
                month = "Sep";
                break;
            }
            case 10:
            {
                month = "Oct";
                break;
            }
            case 11:
            {
                month = "Nov";
                break;
            }
            case 12:
            {
                month = "Dec";
                break;
            }
        }
        string time=dateTime.ToString("hh:mm tt");
        return $"{dateTime.DayOfWeek.ToString().Substring(0,3)}, {dateTime.Day} {month} {dateTime.Year} at {time}";
    }
}
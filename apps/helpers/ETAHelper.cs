namespace evacPlanMoni.apps.helpers
{
  public static class ETAHelper
  {
    public static string GetFormattedEta(double etaHours)
    {
      //https://stackoverflow.com/questions/1087699/decimal-hour-into-time
      // Convert the hours into a TimeSpan object
      var timeSpan = TimeSpan.FromHours(etaHours);

      // Format it into a readable string
      string formattedEta;
      if (timeSpan.TotalMinutes < 1)
      {
        // If it's less than a minute, show seconds
        formattedEta = $"{timeSpan.Seconds} seconds";
      }
      else if (timeSpan.TotalHours < 1)
      {
        // If it's less than an hour, show just minutes
        formattedEta = $"{(int)timeSpan.TotalMinutes} minutes";
      }
      else
      {
        // If it's over an hour, show hours and minutes
        formattedEta = $"{timeSpan.Hours} hours, {timeSpan.Minutes} minutes";
      }

      return formattedEta;
    }
  }

}
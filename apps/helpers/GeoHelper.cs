namespace evacPlanMoni.apps.helpers
{
  public static class GeoHelper
  {
    private const double EarthRadiusKm = 6371.0;

    //Code from https://rosettacode.org/wiki/Haversine_formula#C#
    public static double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
      var earthRadius = 6372.8; // In kilometers
      var dLat = DegreesToRadians(lat2 - lat1);
      var dLon = DegreesToRadians(lon2 - lon1);
      lat1 = DegreesToRadians(lat1);
      lat2 = DegreesToRadians(lat2);

      var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
      var c = 2 * Math.Asin(Math.Sqrt(a));
      return earthRadius * 2 * Math.Asin(Math.Sqrt(a));
    }

    private static double DegreesToRadians(double angle)
    {
      return Math.PI * angle / 180.0;
    }
  }
}
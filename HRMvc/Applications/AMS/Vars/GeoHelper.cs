using HRApiLibrary.Modules._12006O;

namespace HRMvc.Applications.AMS.Vars;

public static class GeoHelper
{
    private const double EarthRadiusMeters = 6_371_000;

    public static double DistanceMeters(double lat1, double lon1, double lat2, double lon2)
    {
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2))
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusMeters * c;
    }

    public static bool IsWithinAnySite(double lat, double lon, List<M12006_Site> sites)
    {
        return sites.Any(s =>
            DistanceMeters(lat, lon, (double)s.Latitude, (double)s.Longitude) <= s.RadiusMeters);
    }

    private static double ToRad(double deg) => deg * Math.PI / 180;
}

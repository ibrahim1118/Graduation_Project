using BLL.IRepository;
using DAL.Data;
using DAL.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Repository
{
    public class LocationRepository : GenricRepository<UserLocation>, ILocationRepository
    {
        private readonly AppDbContext context;

        public LocationRepository(AppDbContext context) : base(context) 
        {
            this.context = context;
        }
        

        public IEnumerable <UserLocation> GetNearestUsers(decimal latitude, decimal longtude)
        {
            var locations = context.UserLocations.Include(l=>l.AppUser).ToList(); // Fetch all locations into memory

            var originLat = 40.785091m;
            var originLon = -73.968285m;

            var nearbyLocations = locations
                .Select(location => new
                {
                    Location = location,
                    Distance = CalculateDistance(latitude, longtude, location.latitude, location.longitude)
                })
                .Where(result => result.Distance < 50) // Example: filter locations within 50 km
                .OrderBy(result => result.Distance)
                .Select(result => result.Location)
                .ToList();
            return nearbyLocations; 
        }

        private const decimal EarthRadiusKm = 6371.0m;

        public static decimal CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            // Convert latitude and longitude from degrees to radians
            decimal lat1Rad = DegreesToRadians(lat1);
            decimal lon1Rad = DegreesToRadians(lon1);
            decimal lat2Rad = DegreesToRadians(lat2);
            decimal lon2Rad = DegreesToRadians(lon2);

            // Haversine formula
            decimal dLat = lat2Rad - lat1Rad;
            decimal dLon = lon2Rad - lon1Rad;
            decimal a = (decimal)Math.Sin((double)(dLat / 2)) * (decimal)Math.Sin((double)(dLat / 2)) +
                        (decimal)Math.Cos((double)lat1Rad) * (decimal)Math.Cos((double)lat2Rad) *
                        (decimal)Math.Sin((double)(dLon / 2)) * (decimal)Math.Sin((double)(dLon / 2));
            decimal c = 2 * (decimal)Math.Atan2((double)Math.Sqrt((double)a), (double)Math.Sqrt((double)(1 - a)));

            return EarthRadiusKm * c; // Distance in kilometers
        }

        private static decimal DegreesToRadians(decimal degrees)
        {
            return degrees * (decimal)Math.PI / 180.0m;
        }

    }
}

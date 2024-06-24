using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.IRepository
{
    public interface ILocationRepository : IGenricRepository<UserLocation>
    {
        public IEnumerable<UserLocation> GetNearestUsers(decimal latitude, decimal longtude);
        public Task AddUserToken(string userid, string Token);
    
    }
}

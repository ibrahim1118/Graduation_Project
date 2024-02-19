using DAL.AuthEntity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface ITokenService
    {

        public Task<string> CreatTokeAysnc(AppUser user, UserManager<AppUser> userManager);
    }
}

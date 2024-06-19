﻿using DAL.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.AuthEntity
{
    public class AppUser : IdentityUser
    {
        public  string  FullName { get; set; }

        public string?  Image {  get; set; }
        public DateTime CreatedDate { get; set; }   

        public IEnumerable<UserLocation> userLocations { get; set; }
    }
}

﻿using Microsoft.AspNetCore.Identity;

namespace SistemaTurnos.Database.Model
{
    public class ApplicationUser : IdentityUser<int>
    {
        public bool FacebookLogin { get; set; }
    }
}

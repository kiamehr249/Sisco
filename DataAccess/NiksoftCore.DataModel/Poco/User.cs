﻿using Microsoft.AspNetCore.Identity;
using NiksoftCore.ViewModel;
using System.Collections.Generic;

namespace NiksoftCore.DataModel
{
    public class User : IdentityUser<int>
    {
        public AccountType AccountType { get; set; }
        public virtual ICollection<UserProfile> UserProfiles { get; set; }
    }
}

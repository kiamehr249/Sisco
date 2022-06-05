﻿using Microsoft.AspNetCore.Http;
using System;

namespace NiksoftCore.ViewModel
{
    public class UserProfileRequest
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string CompanyName { get; set; }
        public string NCode { get; set; }
        public string Mobile { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string BirthDate { get; set; }
        public int UserId { get; set; }
        public IFormFile AvatarFile { get; set; }
        public string Avatar { get; set; }
        public IFormFile IdCardFile { get; set; }
        public string IdCardImage { get; set; }
        public IFormFile NCardFile { get; set; }
        public string NCardImage { get; set; }
        public ProfileStatus Status { get; set; }
        public int ProvinceId { get; set; }
        public int CityId { get; set; }
        public int Gender { get; set; }
    }
}

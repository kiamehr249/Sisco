using Microsoft.AspNetCore.Http;

namespace NiksoftCore.ViewModel
{
    public class RestaurantAdminCreate
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProfileId { get; set; }
        public int RestaurantId { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NCode { get; set; }
        public string Address { get; set; }
        public int? Gender { get; set; }
        public IFormFile AvatarFile { get; set; }
        public string Avatar { get; set; }

        public bool AlreadyUser { get; set; }
        public int Error { get; set; }
    }
}

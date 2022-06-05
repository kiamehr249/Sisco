namespace NiksoftCore.ViewModel
{
    public class EmployeeRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProfileId { get; set; }
        public int RestaurantId { get; set; }
        public int BranchId { get; set; }
        public int EmployeeType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string NCode { get; set; }
        public string Address { get; set; }
        public int? Gender { get; set; }
        public string Avatar { get; set; }
    }
}

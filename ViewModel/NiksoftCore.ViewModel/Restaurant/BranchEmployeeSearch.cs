namespace NiksoftCore.ViewModel
{
    public class BranchEmployeeSearch : BaseRequest
    {
        public int BranchId { get; set; }
        public int EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NCode { get; set; }
        public string PhoneNumber { get; set; }
    }
}

using System.Collections.Generic;

namespace NiksoftCore.ViewModel
{
    public class BaseInfoRequest
    {
        public int Id { get; set; }
        public string KeyValue { get; set; }
        public string GroupValue { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public int? IntValue { get; set; }
        public double? DoubleValue { get; set; }
        public string StringValue { get; set; }
        public long? LongValue { get; set; }
        public bool BoolValue { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ViewObject { get; set; }
        public bool Enabled { get; set; }

        public List<BaseInfoViewModel> ViewModels { get; set; }
    }
}

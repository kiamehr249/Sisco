using System;

namespace NiksoftCore.DataModel
{
    public class BaseInfo
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
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ViewObject { get; set; }
        public int UserId { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual User User { get; set; }
    }
}

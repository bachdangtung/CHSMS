using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class SupplySettlementReport
    {
        public SupplySettlementReport()
        {
            SupplyConsumptionDetails = new HashSet<SupplyConsumptionDetail>();
        }

        public int SupplySettlementReportId { get; set; }
        public string? ServiceName { get; set; }
        public string? ServiceType { get; set; }
        public string? Unit { get; set; }
        public double? Quantity { get; set; }

        public virtual ICollection<SupplyConsumptionDetail> SupplyConsumptionDetails { get; set; }
    }
}

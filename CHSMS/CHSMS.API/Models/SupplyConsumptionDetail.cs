using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class SupplyConsumptionDetail
    {
        public int SupplyConsumptionDetailId { get; set; }
        public int? SupplySettlementReportId { get; set; }
        public string? SupplyName { get; set; }
        public string? UnitOfMeasure { get; set; }
        public double? Quantity { get; set; }
        public string? Note { get; set; }

        public virtual SupplySettlementReport? SupplySettlementReport { get; set; }
    }
}

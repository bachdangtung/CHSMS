using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class SupplyConsumptionReport
    {
        public int SupplyConsumptionReportId { get; set; }
        public int? MedicalSupplyId { get; set; }
        public string? UnitOfMeasure { get; set; }
        public DateTime? ReportDate { get; set; }
        public double? OpeningQuantityFree { get; set; }
        public double? OpeningQuantityCharged { get; set; }
        public double? OpeningQuantityInsured { get; set; }
        public double? ReceivedQuantityFree { get; set; }
        public double? ReceivedQuantityCharged { get; set; }
        public double? ReceivedQuantityInsured { get; set; }
        public double? ConsumedQuantityFree { get; set; }
        public double? ConsumedQuantityCharged { get; set; }
        public double? ConsumedQuantityInsured { get; set; }
        public double? ClosingQuantityFree { get; set; }
        public double? ClosingQuantityCharged { get; set; }
        public double? ClosingQuantityInsured { get; set; }
        public double? PlannedQuantityFree { get; set; }
        public double? PlannedQuantityCharged { get; set; }
        public double? PlannedQuantityInsured { get; set; }
        public double? ApprovedQuantityFree { get; set; }
        public double? ApprovedQuantityCharged { get; set; }
        public double? ApprovedQuantityInsured { get; set; }
        public string? Note { get; set; }

        public virtual MedicalSupply? MedicalSupply { get; set; }
    }
}

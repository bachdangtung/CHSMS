using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.Models;

namespace CHSMS.API.Repositories.Interfaces
{
    public interface IMedicalSupplyRepository
    {
        List<MedicalSupply> GetAllMedicalSupplies();
        List<MedicalSupplyInventory>? GetMedicalSupplyInventoryByMSID(int medicalSupplyId);
        double? GetMSQantityByID(int medicalSupplyId);
        double? GetActualMSQuantity(int medicalSupplyId, DateTime date);
        bool AddMedicalSupplyInventory(MedicalSupplyInventory medicalSupply);
        bool UpdateMedicalSupplyInventory(MedicalSupplyInventory medicalSupplyInventory);
        int ConsumeMedicalSupplyByMSID(ConsumpMSDTO consump);
        List<MedicalSupplyInventory> GetAvailableMedicalSupplyInventory(int medicalSupplyId);
        List<MedicalSupplyInventory> GetAllMedicalSupplyInventory(int msid);
        bool AddMedicalSupplyConsumption(MedicalSupplyConsumption medicalSupplyConsumption);
        Dictionary<MedicalSupply, double> GetAllMedicalSupplyConsumeReport(DateTime? from, DateTime? to);
        double MedicalSupplyConsumeReport(int msid, DateTime? from, DateTime? to);
        List<MedicalSupplyConsumption> GetAllMedicalSupplyConsumptionByDate(DateTime? from, DateTime? to);
        List<MedicalSupplyInventory> GetInputMedicalSupplyInventoryByDate(DateTime? from, DateTime? to);
        double? GetInputAmountOfMS(int MSID, DateTime? from, DateTime? to);
        MedicalSupply GetMedicalSupplyByID(int id);
        List<MedicalSupplyConsumption> MSConsumptionDetail(int id, DateTime? from, DateTime? to);
        double GetAddOnMSI(int id, DateTime? from, DateTime? to);
        List<MedicalSupplyConsumption> ConsumptionHistory(DateTime? from, DateTime? to);
        MedicalSupply? GetMedicalSupplyByMSIID(int id);
        MedicalSupplyConsumption? GetSupplyConsumptionByID(int id);
        MedicalSupplyInventory? GetMedicalSupplyInventoryById(int id);
        bool UpdateMedicalSupplyConsumption(MedicalSupplyConsumption medicalSupplyConsumption);
        double GetNumberOfExpiredMSI(int MSID, DateTime? from, DateTime? to);
        List<MedicalSupplyInventory> GetMedicalSupplyImportHistory(DateTime fromDate, DateTime toDate);
    }
}

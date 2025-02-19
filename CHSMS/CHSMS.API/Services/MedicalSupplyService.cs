using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.Repositories;
using CHSMS.API.Models;
namespace CHSMS.API.Services
{
    public class MedicalSupplyService
    {
        private readonly MedicalSupplyReposotory _medicalSupplyReposotory;
        public MedicalSupplyService(MedicalSupplyReposotory medicalSupplyReposotory)
        {
            _medicalSupplyReposotory = medicalSupplyReposotory;
        }
        public List<MedicalSupplyDTO> GetAllMedicalSupplies()
        {
            List<MedicalSupplyDTO> medicalSupplyDTOs = new List<MedicalSupplyDTO>();
            foreach (var medicalSupply in _medicalSupplyReposotory.GetAllMedicalSupplies())
            {
                var medicalSupplyDTO = new MedicalSupplyDTO
                {
                    MedicalSupplyId = medicalSupply.MedicalSupplyId,
                    Name = medicalSupply.Name,
                    SupplyType = medicalSupply.SupplyType,
                    UnitOfMeasure = medicalSupply.UnitOfMeasure,
                    SupplierId = medicalSupply.SupplierId,
                    Status = medicalSupply.Status,
                    ImportPrice = medicalSupply.ImportPrice,
                    SellingPrice = medicalSupply.SellingPrice,
                    BatchNumber = medicalSupply.BatchNumber,
                    BidNumber = medicalSupply.BidNumber,
                    Quantity = _medicalSupplyReposotory.GetSupplyQuantity(medicalSupply.MedicalSupplyId),
                    Supplier = medicalSupply.Supplier,
                    SupplyInventories = MedicalSupplyDetail(medicalSupply.MedicalSupplyId)
                };
                medicalSupplyDTOs.Add(medicalSupplyDTO);
            }
            return medicalSupplyDTOs;
        }
        public List<SupplyInventoryDTO> MedicalSupplyDetail(int medicalSupplyId)
        {
            List<SupplyInventoryDTO> supplyInventoryDTOs = new List<SupplyInventoryDTO>();
            List<SupplyInventory> supplyInventories = _medicalSupplyReposotory.MedicalSupplyDetail(medicalSupplyId);
            foreach (var supplyInventory in supplyInventories)
            {
                var supplyInventoryDTO = new SupplyInventoryDTO
                {
                    SupplyInventoryId = supplyInventory.SupplyInventoryId,
                    MedicalSupplyId = supplyInventory.MedicalSupplyId,
                    Quantity = supplyInventory.Quantity,
                    CertificateNumber = supplyInventory.CertificateNumber,
                    TransactionDate=supplyInventory.TransactionDate,
                    ExpirationDate = supplyInventory.ExpirationDate,
                    Note = supplyInventory.Note,
                    ReceiverId = supplyInventory.ReceiverId,    
                    TransactionType = supplyInventory.TransactionType,
                };
                supplyInventoryDTOs.Add(supplyInventoryDTO);
            }
            return supplyInventoryDTOs;
        }
    }
}

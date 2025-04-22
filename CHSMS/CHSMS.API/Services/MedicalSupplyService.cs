using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;

namespace CHSMS.API.Services
{
    public class MedicalSupplyService : IMedicalSupplyService
    {
        private readonly IMedicalSupplyRepository _medicalSupplyReposotory;
        public MedicalSupplyService(IMedicalSupplyRepository medicalSupplyReposotory)
        {
            _medicalSupplyReposotory = medicalSupplyReposotory;
        }

        //Get all medical supplies
        public List<MedicalSupplyDTO> GetAllMedicalSupplies()
        {
            List<MedicalSupplyDTO> medicalSupplyDTOs = new List<MedicalSupplyDTO>();
            foreach (var medicalSupply in _medicalSupplyReposotory.GetAllMedicalSupplies())
            {
                var medicalSupplyDTO = ConvertToMedicalsupplyDTO(medicalSupply);
                medicalSupplyDTO.Quantity = _medicalSupplyReposotory.GetMSQantityByID(medicalSupply.MedicalSupplyId);
                medicalSupplyDTOs.Add(medicalSupplyDTO);
            }
            return medicalSupplyDTOs;
        }

        //Get one medical supply by ID
        public List<MedicalSupplyInventoryDTO>? GetMedicalSupplyById(int medicalSupplyId)
        {
            var msi = _medicalSupplyReposotory.GetMedicalSupplyInventoryByMSID(medicalSupplyId);
            if (msi == null)
                return null;
            List<MedicalSupplyInventoryDTO> medicalSupplyInventoryDTOs = new List<MedicalSupplyInventoryDTO>();
            foreach (var item in msi)
            {
                var medicalSupplyInventoryDTO = ConvertToMedicalSupplyInventoryDTO(item);
                medicalSupplyInventoryDTOs.Add(medicalSupplyInventoryDTO);
            }
            return medicalSupplyInventoryDTOs;
        }

        //Get medical supply detail
        public List<MedicalSupplyInventoryDTO> MedicalSupplyInventoryByMedicalSupplyId(int medicalSupplyId)
        {
            List<MedicalSupplyInventoryDTO> supplyInventoryDTOs = new List<MedicalSupplyInventoryDTO>();
            List<MedicalSupplyInventory> supplyInventories = _medicalSupplyReposotory.GetAllMedicalSupplyInventory(medicalSupplyId);
            foreach (var supplyInventory in supplyInventories)
            {
                var supplyInventoryDTO = ConvertToMedicalSupplyInventoryDTO(supplyInventory);
                supplyInventoryDTOs.Add(supplyInventoryDTO);
            }
            return supplyInventoryDTOs;
        }

        public bool AddMedicalSupplyInventory(MedicalSupplyInventoryDTO medicalSupplyInventoryDTO)
        {
            var medicalSupply = new MedicalSupplyInventory
            {
                MedicalSupplyId = (int)medicalSupplyInventoryDTO.MedicalSupplyId,
                Quantity = medicalSupplyInventoryDTO.Quantity,
                CertificateNumber = medicalSupplyInventoryDTO.CertificateNumber,
                ManufactureDate = medicalSupplyInventoryDTO.ManufactureDate,
                TransactionDate = medicalSupplyInventoryDTO.TransactionDate,
                ExpiryDate = medicalSupplyInventoryDTO.ExpiryDate,
                Note = medicalSupplyInventoryDTO.Note,
                ReceiverId = medicalSupplyInventoryDTO.ReceiverId,
                TransactionType = medicalSupplyInventoryDTO.TransactionType,
                BatchNumber = medicalSupplyInventoryDTO.BatchNumber,
                ImportQuantity = medicalSupplyInventoryDTO.Quantity,
            };
            if (!_medicalSupplyReposotory.AddMedicalSupplyInventory(medicalSupply)) return false;
            return true;
        }

        public bool UpdateMedicalSupplyInventory(MedicalSupplyInventoryDTO medicalSupplyInventoryDTO)
        {
            var MedicalSupplyInventory = new MedicalSupplyInventory
            {
                SupplyInventoryId = medicalSupplyInventoryDTO.SupplyInventoryId,
                MedicalSupplyId = (int)medicalSupplyInventoryDTO.MedicalSupplyId,
                Quantity = medicalSupplyInventoryDTO.Quantity,
                CertificateNumber = medicalSupplyInventoryDTO.CertificateNumber,
                ManufactureDate = medicalSupplyInventoryDTO.ManufactureDate,
                TransactionDate = medicalSupplyInventoryDTO.TransactionDate,
                ExpiryDate = medicalSupplyInventoryDTO.ExpiryDate,
                ReceiverId = medicalSupplyInventoryDTO.ReceiverId,
                Note = medicalSupplyInventoryDTO.Note,
            };
            if (!_medicalSupplyReposotory.UpdateMedicalSupplyInventory(MedicalSupplyInventory)) return false;
            return true;
        }

        public int ConsumeMedicalSupply(ConsumpMSDTO consumpMSDTO)
        {
            return _medicalSupplyReposotory.ConsumeMedicalSupplyByMSID(consumpMSDTO);
        }

        public Dictionary<MedicalSupplyDTO, double> ConsumeReport(DateTime? from, DateTime? to)
        {
            MedicalSupplyDTO medicalSupplyDTO;
            Dictionary<MedicalSupplyDTO, double> result = new Dictionary<MedicalSupplyDTO, double>();
            var dict = _medicalSupplyReposotory.GetAllMedicalSupplyConsumeReport(from, to);
            foreach (var item in dict)
            {
                medicalSupplyDTO = new MedicalSupplyDTO();
                medicalSupplyDTO = ConvertToMedicalsupplyDTO(item.Key);
                medicalSupplyDTO.Quantity = _medicalSupplyReposotory.GetMSQantityByID(medicalSupplyDTO.MedicalSupplyId);
                result.Add(medicalSupplyDTO, item.Value);
            }
            return result;
        }

        //convert to MedicalSupplyDTO
        private MedicalSupplyDTO ConvertToMedicalsupplyDTO(MedicalSupply medicalSupply)
        {
            return new MedicalSupplyDTO
            {
                MedicalSupplyId = medicalSupply.MedicalSupplyId,
                MedicalSupplyName = medicalSupply.MedicalSupplyName,
                SupplyType = medicalSupply.SupplyType,
                UnitOfMeasure = medicalSupply.UnitOfMeasure,
                SupplierId = medicalSupply.SupplierId,
                Status = medicalSupply.Status,
                ImportPrice = medicalSupply.ImportPrice,
                SellingPrice = medicalSupply.SellingPrice,
                BidNumber = medicalSupply.BidNumber,

            };
        }

        //Convert to MedicalSupplyInventoryDTO
        private MedicalSupplyInventoryDTO ConvertToMedicalSupplyInventoryDTO(MedicalSupplyInventory medicalSupplyInventory)
        {
            return new MedicalSupplyInventoryDTO
            {
                SupplyInventoryId = medicalSupplyInventory.SupplyInventoryId,
                MedicalSupplyId = medicalSupplyInventory.MedicalSupplyId,
                Quantity = medicalSupplyInventory.Quantity,
                CertificateNumber = medicalSupplyInventory.CertificateNumber,
                ManufactureDate = medicalSupplyInventory.ManufactureDate,
                TransactionDate = medicalSupplyInventory.TransactionDate,
                ExpiryDate = medicalSupplyInventory.ExpiryDate,
                Note = medicalSupplyInventory.Note,
                ReceiverId = medicalSupplyInventory.ReceiverId,
                TransactionType = medicalSupplyInventory.TransactionType,
                BatchNumber = medicalSupplyInventory.BatchNumber,
            };
        }

        public List<MedicalSupplyDTO> GetAllActualMedicalSupplies(DateTime? date)
        {
            if (date == null)
            {
                return GetAllMedicalSupplies();
            }
            List<MedicalSupplyDTO> medicalSupplyDTOs = new List<MedicalSupplyDTO>();
            var medicalSupplies = _medicalSupplyReposotory.GetAllMedicalSupplies();
            foreach (var item in medicalSupplies)
            {
                var medicalSupplyDTO = ConvertToMedicalsupplyDTO(item);
                medicalSupplyDTO.Quantity = _medicalSupplyReposotory.GetActualMSQuantity(item.MedicalSupplyId, date.Value);
                medicalSupplyDTOs.Add(medicalSupplyDTO);
            }
            return medicalSupplyDTOs;
        }

        public List<MedicalSupplyConsumption> ConsumptionDetail(int id, DateTime? from, DateTime? to)
        {
            var result = _medicalSupplyReposotory.MSConsumptionDetail(id, from, to);
            return result;
        }

        public double GetAddOnMSI(int id, DateTime? from, DateTime? to)
        {
            return _medicalSupplyReposotory.GetAddOnMSI(id, from, to);
        }

        public List<MedicalSupplyConsumption> ConsumptionHistory(DateTime? from, DateTime? to)
        {
            return _medicalSupplyReposotory.ConsumptionHistory(from, to);
        }

        public MedicalSupply GetMedicalSupplyByMSIId(int id)
        {
            return _medicalSupplyReposotory.GetMedicalSupplyByMSIID(id);
        }

        public bool UpdateMedicalSupplyConsumption(ConsumpMSDTO medicalSupplyConsumption)
        {
            if (medicalSupplyConsumption == null)
            {
                return false;
            }
            var MSC = _medicalSupplyReposotory.GetSupplyConsumptionByID(medicalSupplyConsumption.ConsumpMSID.Value);
            if (MSC == null)
            {
                return false;
            }
            var medicalSupplyInventory = _medicalSupplyReposotory.GetMedicalSupplyInventoryById(medicalSupplyConsumption.MedicalSupplyInventoryId.Value);
            if (medicalSupplyInventory == null)
            {
                return false;
            }
            var numberUpdate = medicalSupplyConsumption.Quantity.Value - MSC.Amount.Value;
            medicalSupplyInventory.Quantity -= numberUpdate;
            if (medicalSupplyInventory.Quantity < 0)
            {
                return false;
            }
            var result1 = _medicalSupplyReposotory.UpdateMedicalSupplyInventory(medicalSupplyInventory);
            MSC.Amount = medicalSupplyConsumption.Quantity;
            MSC.Status = medicalSupplyConsumption.Status;
            MSC.Note = medicalSupplyConsumption.Note;
            var result = _medicalSupplyReposotory.UpdateMedicalSupplyConsumption(MSC);

            if (result1 && result)
            {
                return true;
            }
            return false;
        }

        public object GetExpiryMSI(int medicalSupplyId, DateTime? from, DateTime? to)
        {
            return _medicalSupplyReposotory.GetNumberOfExpiredMSI(medicalSupplyId, from, to);
        }

        public MedicalSupplyInventory GetMedicalSupplyInventoryById(int? medicalSupplyInventoryId)
        {
            return _medicalSupplyReposotory.GetMedicalSupplyInventoryById(medicalSupplyInventoryId.Value);
        }

        public List<MedicalSupplyInventory> GetMedicalSupplyImportHistory(DateTime fromDate, DateTime toDate)
        {
            if (fromDate == null || toDate == null)
            {
                return null;
            }
            if (fromDate > toDate || fromDate > DateTime.Now)
            {
                return null;
            }
            return _medicalSupplyReposotory.GetMedicalSupplyImportHistory(fromDate, toDate);
        }
    }
}

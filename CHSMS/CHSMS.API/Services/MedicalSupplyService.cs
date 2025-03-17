using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.Repositories;
using CHSMS.API.Models;
using System.Security.Cryptography.X509Certificates;

namespace CHSMS.API.Services
{
    public class MedicalSupplyService
    {
        private readonly MedicalSupplyReposotory _medicalSupplyReposotory;
        public MedicalSupplyService(MedicalSupplyReposotory medicalSupplyReposotory)
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
                medicalSupplyDTO.Quantity = _medicalSupplyReposotory.GetSupplyQuantity(medicalSupply.MedicalSupplyId);
                medicalSupplyDTOs.Add(medicalSupplyDTO);
            }
            return medicalSupplyDTOs;
        }

        //Get one medical supply by ID
        public MedicalSupplyDTO? GetMedicalSupply(int medicalSupplyId)
        {
            var medicalSupply = _medicalSupplyReposotory.GetMedicalSupply(medicalSupplyId);
            if (medicalSupply == null)
                return null;
            var medicalSupplyDTO = ConvertToMedicalsupplyDTO(medicalSupply);
            medicalSupplyDTO.Quantity = _medicalSupplyReposotory.GetSupplyQuantity(medicalSupply.MedicalSupplyId);
            return medicalSupplyDTO;
        }

        //Get medical supply detail
        public List<MedicalSupplyInventoryDTO> MedicalSupplyDetail(int medicalSupplyId)
        {
            List<MedicalSupplyInventoryDTO> supplyInventoryDTOs = new List<MedicalSupplyInventoryDTO>();
            List<MedicalSupplyInventory> supplyInventories = _medicalSupplyReposotory.MedicalSupplyDetail(medicalSupplyId);
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
                MedicalSupplyId = medicalSupplyInventoryDTO.MedicalSupplyId,
                Quantity = medicalSupplyInventoryDTO.Quantity,
                CertificateNumber = medicalSupplyInventoryDTO.CertificateNumber,
                ManufactureDate = medicalSupplyInventoryDTO.ManufactureDate,
                TransactionDate = medicalSupplyInventoryDTO.TransactionDate,
                ExpiryDate = medicalSupplyInventoryDTO.ExpiryDate,
                Note = medicalSupplyInventoryDTO.Note,
                ReceiverId = medicalSupplyInventoryDTO.ReceiverId,
                TransactionType = medicalSupplyInventoryDTO.TransactionType,
            };
            if (!_medicalSupplyReposotory.AddMedicalSupplyInventory(medicalSupply)) return false;
            return true;
        }

        public bool UpdateMedicalSupplyInventory(MedicalSupplyInventoryDTO medicalSupplyInventoryDTO)
        {
            var MedicalSupplyInventory = new MedicalSupplyInventory
            {
                SupplyInventoryId = medicalSupplyInventoryDTO.SupplyInventoryId,
                MedicalSupplyId = medicalSupplyInventoryDTO.MedicalSupplyId,
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

        public int ConsumeMedicalSupply(int id, double Quantity, bool BHYT, string? Note)
        {
            if (Quantity <= 0)
            {
                return -1;
            }
            if (_medicalSupplyReposotory.GetMedicalSupply(id) == null)
            {
                return -2;
            }
            if (_medicalSupplyReposotory.GetSupplyQuantity(id) < Quantity)
            {
                return -3;
            }
            return _medicalSupplyReposotory.ConsumeMedicalSupply(id, Quantity, BHYT, Note);
        }

        public Dictionary<MedicalSupplyDTO, double> ConsumeReport(DateTime? from, DateTime? to)
        {
            MedicalSupplyDTO medicalSupplyDTO;
            Dictionary<MedicalSupplyDTO, double> result = new Dictionary<MedicalSupplyDTO, double>();
            var dict = _medicalSupplyReposotory.MedicalSupplyConsumeReport(from, to);
            foreach (var item in dict)
            {
                medicalSupplyDTO = new MedicalSupplyDTO();
                medicalSupplyDTO = ConvertToMedicalsupplyDTO(item.Key);
                medicalSupplyDTO.Quantity = _medicalSupplyReposotory.GetSupplyQuantity(medicalSupplyDTO.MedicalSupplyId);
                result.Add(medicalSupplyDTO, item.Value);
            }
            return result;
        }

        //convert to MedicalSupplyDTO
        public MedicalSupplyDTO ConvertToMedicalsupplyDTO(MedicalSupply medicalSupply)
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
        public MedicalSupplyInventoryDTO ConvertToMedicalSupplyInventoryDTO(MedicalSupplyInventory medicalSupplyInventory)
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
                medicalSupplyDTO.Quantity = _medicalSupplyReposotory.GetActualSupplyQuantity(item.MedicalSupplyId, date.Value);
                medicalSupplyDTOs.Add(medicalSupplyDTO);
            }
            return medicalSupplyDTOs;
        }
    }
}

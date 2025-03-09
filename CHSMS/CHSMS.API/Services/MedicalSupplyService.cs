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
                var medicalSupplyDTO = new MedicalSupplyDTO
                {
                    MedicalSupplyId = medicalSupply.MedicalSupplyId,
                    MedicalSupplyName = medicalSupply.MedicalSupplyName,
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
                    MedicalSupplyInventories = MedicalSupplyDetail(medicalSupply.MedicalSupplyId)
                };
                medicalSupplyDTOs.Add(medicalSupplyDTO);
            }
            return medicalSupplyDTOs;
        }

        //Get one medical supply
        public MedicalSupplyDTO? GetMedicalSupply(int medicalSupplyId)
        {
            var medicalSupply = _medicalSupplyReposotory.GetMedicalSupply(medicalSupplyId);
            if (medicalSupply == null)
                return null;
            var medicalSupplyDTO = new MedicalSupplyDTO
            {
                MedicalSupplyId = medicalSupply.MedicalSupplyId,
                MedicalSupplyName = medicalSupply.MedicalSupplyName,
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
                MedicalSupplyInventories = MedicalSupplyDetail(medicalSupply.MedicalSupplyId)
            };
            return medicalSupplyDTO;
        }

        //Get medical supply detail
        public List<MedicalSupplyInventoryDTO> MedicalSupplyDetail(int medicalSupplyId)
        {
            List<MedicalSupplyInventoryDTO> supplyInventoryDTOs = new List<MedicalSupplyInventoryDTO>();
            List<MedicalSupplyInventory> supplyInventories = _medicalSupplyReposotory.MedicalSupplyDetail(medicalSupplyId);
            foreach (var supplyInventory in supplyInventories)
            {
                var supplyInventoryDTO = new MedicalSupplyInventoryDTO
                {
                    SupplyInventoryId = supplyInventory.SupplyInventoryId,
                    MedicalSupplyId = supplyInventory.MedicalSupplyId,
                    Quantity = supplyInventory.Quantity,
                    CertificateNumber = supplyInventory.CertificateNumber,
                    ManufactureDate = supplyInventory.ManufactureDate,
                    TransactionDate = supplyInventory.TransactionDate,
                    ExpiryDate = supplyInventory.ExpiryDate,
                    Note = supplyInventory.Note,
                    ReceiverId = supplyInventory.ReceiverId,
                    TransactionType = supplyInventory.TransactionType,
                };
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
    }
}

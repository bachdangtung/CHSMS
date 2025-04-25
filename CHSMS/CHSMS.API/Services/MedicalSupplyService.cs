using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.Repositories;
using CHSMS.API.Models;

namespace CHSMS.API.Services
{
    public class MedicalSupplyService
    {
        private readonly MedicalSupplyRepository _medicalSupplyReposotory;
        public MedicalSupplyService(MedicalSupplyRepository medicalSupplyReposotory)
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

        public bool AddMedicalSupplyInventory(List<MedicalSupplyInventoryDTO> medicalSupplyInventoryDTO)
        {
            if (medicalSupplyInventoryDTO == null || medicalSupplyInventoryDTO.Count == 0)
            {
                return false;
            }
            List<MedicalSupplyInventory> medicalSupplyInventories = new List<MedicalSupplyInventory>();
            foreach (var item in medicalSupplyInventoryDTO)
            {
                var medicalSupply = new MedicalSupplyInventory
                {
                    MedicalSupplyId = (int)item.MedicalSupplyId,
                    Quantity = item.Quantity,
                    CertificateNumber = item.CertificateNumber,
                    ManufactureDate = item.ManufactureDate,
                    TransactionDate = item.TransactionDate,
                    ExpiryDate = item.ExpiryDate,
                    Note = item.Note,
                    ReceiverId = item.ReceiverId,
                    TransactionType = item.TransactionType,
                    BatchNumber = item.BatchNumber,
                    ImportQuantity = item.Quantity,
                };
                medicalSupplyInventories.Add(medicalSupply);
            }
            if (!_medicalSupplyReposotory.AddMedicalSupplyInventory(medicalSupplyInventories)) return false;
            return true;
        }

        public bool UpdateMedicalSupplyInventory(MedicalSupplyInventoryDTO medicalSupplyInventoryDTO)
        {
            List<MedicalSupplyInventory> medicalSupplyInventories = new List<MedicalSupplyInventory>();
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
            medicalSupplyInventories.Add(MedicalSupplyInventory);
            if (!_medicalSupplyReposotory.UpdateMedicalSupplyInventory(medicalSupplyInventories)) return false;
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
            List<MedicalSupplyInventory> medicalSupplyInventories = new List<MedicalSupplyInventory>();
            medicalSupplyInventories.Add(medicalSupplyInventory);
            var result1 = _medicalSupplyReposotory.UpdateMedicalSupplyInventory(medicalSupplyInventories);
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

        public List<MedicalSupplyInventory>? GetMedicalSupplyImportHistory(DateTime fromDate, DateTime toDate)
        {
            if (fromDate > toDate || fromDate > DateTime.Now)
            {
                return null;
            }
            return _medicalSupplyReposotory.GetMedicalSupplyImportHistory(fromDate, toDate);
        }
        public List<MedicalSupplyInventoryStatistic>? GetAllMedicalSupplyInventoryStatistics()
        {
            return _medicalSupplyReposotory.GetAllMedicalSupplyInventoryStatistics();
        }

        public MedicalSupplyInventoryStatistic? GetMedicalSupplyInventoryStatisticsById(int medicalSupplyId)
        {
            return _medicalSupplyReposotory.GetMedicalSupplyInventoryStatisticById(medicalSupplyId);
        }

        public bool AddMedicalSupplyInventoryStatistic(List<MSIStatisticDTO> mSIStatisticDTOs)
        {
            var list = _medicalSupplyReposotory.GetAllMSISNotConfirm();
            List<MedicalSupplyInventoryStatistic> medicalSupplyInventoryStatistics = new List<MedicalSupplyInventoryStatistic>();
            if (mSIStatisticDTOs == null || mSIStatisticDTOs.Count == 0)
            {
                return false;
            }
            var adds = new List<MedicalSupplyInventoryStatistic>();
            foreach (var item in mSIStatisticDTOs)
            {
                var medicalSupplyInventoryStatistic = ConvertMedicalSupplyInventoryStatisticFromDTO(item);
                if (item.MsinventoryId == null || item.Quantity == null || item.ActualQuantity == null || item.StatisticPerson == null || item.StatisticDate == null)
                {
                    throw new Exception("Medical supply inventory statistic is not valid");
                }
                else
                if ((list.Count > 0) && (list.Any(x => x.MsinventoryId == medicalSupplyInventoryStatistic.MsinventoryId) == true))
                {
                    throw new Exception("Vật tư này đã tồn tại trong danh sách kiểm kê");
                }
                adds.Add(medicalSupplyInventoryStatistic);
            }
            return _medicalSupplyReposotory.AddMedicalSupplyInventoryStatistic(adds);
        }
        public bool UpdateMedicalSupplyInventoryStatistic(List<MSIStatisticDTO> mSIStatisticDTOs)
        {
            List<MedicalSupplyInventoryStatistic> medicalSupplyInventoryStatistics = new List<MedicalSupplyInventoryStatistic>();
            List<MedicalSupplyInventory> medicalSupplyInventories = new List<MedicalSupplyInventory>();
            if (mSIStatisticDTOs == null || mSIStatisticDTOs.Count == 0)
            {
                return false;
            }
            foreach (var item in mSIStatisticDTOs)
            {
                var medicalSupplyInventoryStatistic = ConvertMedicalSupplyInventoryStatisticFromDTO(item);
                var medicalSupplyInventory = _medicalSupplyReposotory.GetMedicalSupplyInventoryById(medicalSupplyInventoryStatistic.MsinventoryId);
                if (medicalSupplyInventory == null)
                {
                    throw new Exception("Vật tư không hợp lệ");
                }
                medicalSupplyInventoryStatistics.Add(medicalSupplyInventoryStatistic);
                if (medicalSupplyInventoryStatistic.IsUpdate)
                {
                    if (medicalSupplyInventoryStatistic.Quantity != medicalSupplyInventory.Quantity)
                    {
                        throw new Exception("Số lượng tồn không đồng nhất so với hệ thống");
                    }
                    medicalSupplyInventory.Quantity = medicalSupplyInventoryStatistic.ActualQuantity;
                    medicalSupplyInventories.Add(medicalSupplyInventory);
                }
            }


            if (medicalSupplyInventories.Count <= 0)
                return _medicalSupplyReposotory.UpdateMedicalSupplyInventoryStatistic(medicalSupplyInventoryStatistics);
            else
            {
                var result1 = _medicalSupplyReposotory.UpdateMedicalSupplyInventory(medicalSupplyInventories);
                var result2 = _medicalSupplyReposotory.UpdateMedicalSupplyInventoryStatistic(medicalSupplyInventoryStatistics);
                return result1 && result2;
            }
        }
        public List<MedicalSupplyInventoryStatistic>? GetMedicalSupplyInventoryStatisticsByStatisticDate(DateTime? from, DateTime? to)
        {
            if (!from.HasValue && !to.HasValue)
            {
                return _medicalSupplyReposotory.GetAllMedicalSupplyInventoryStatistics();
            }
            if (from > to || from > DateTime.Now)
            {
                return null;
            }
            var medicalSupplyInventoryStatistic = _medicalSupplyReposotory.GetMedicalSupplyInventoryStatisticsByStatisticDate(from.Value, to.Value);
            return medicalSupplyInventoryStatistic;
        }

        public bool DeleteMedicalSupplyInventoryStatistic(int medicalSupplyInventoryStatisticId)
        {
            var medicalSupplyInventoryStatistic = _medicalSupplyReposotory.GetMedicalSupplyInventoryStatisticById(medicalSupplyInventoryStatisticId);
            if (medicalSupplyInventoryStatistic == null)
            {
                return false;
            }
            return _medicalSupplyReposotory.DeleteMedicalSupplyInventoryStatistic(medicalSupplyInventoryStatistic);
        }

        public MedicalSupplyInventoryStatistic ConvertMedicalSupplyInventoryStatisticFromDTO(MSIStatisticDTO mSIStatisticDTO)
        {
            var obj = new MedicalSupplyInventoryStatistic
            {
                Msisid = mSIStatisticDTO.Msisid.Value,
                MsinventoryId = mSIStatisticDTO.MsinventoryId.Value,
                Quantity = mSIStatisticDTO.Quantity.Value,
                ActualQuantity = mSIStatisticDTO.ActualQuantity.Value,
                StatisticPerson = mSIStatisticDTO.StatisticPerson.Value,
                ConfirmPerson = mSIStatisticDTO.ConfirmPerson,
                StatisticDate = mSIStatisticDTO.StatisticDate.Value,
                ConfirmDate = mSIStatisticDTO.ConfirmDate,
                IsUpdate = mSIStatisticDTO.IsUpdate.Value || false,
                Note = mSIStatisticDTO.Note,
                UpdateDate = mSIStatisticDTO.UpdateDate,
            };
            return obj;
        }
    }
}

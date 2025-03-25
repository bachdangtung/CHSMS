namespace CHSMS.API.DTOs.Medicine
{
    public class MedicineSuggestionDTO
    {
        /// Tên thuốc hoặc vật tư y tế.
        public string MedicineName { get; set; }
        
        /// Hoạt chất chính có trong thuốc.
        public string ActiveIngredient { get; set; }

        /// Hàm lượng thuốc (ví dụ: 500mg, 10ml).
        public string Dosage { get; set; }

        /// Dạng bào chế của thuốc (viên nén, dung dịch, tiêm, v.v.).
        public string DosageForm { get; set; }

        /// Đơn giá của thuốc hoặc vật tư.
        public string UnitPrice { get; set; }

        /// Tuổi thọ hoặc thời hạn bảo quản của thuốc.
        public string ShelfLife { get; set; }
    }
}

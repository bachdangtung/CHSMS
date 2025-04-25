using CHSMS.API.DTOs.MedicalRecord;
using CHSMS.API.Models;

namespace CHSMS.API.Test.MedicalRecordHistoryServiceTest
{
    public static class TestHelper
    {
        public static List<MedicalRecordHistory> CreateDefaultMedicalRecordHistories()
        {
            var user1 = new User { UserId = 1, UserName = "Dr. Smith", Fullname = "John Smith" };
            var user2 = new User { UserId = 2, UserName = "Dr. Johnson", Fullname = "Sarah Johnson" };

            var medicalRecord1 = new MedicalRecord
            {
                MedicalRecordId = 101,
                PatientName = "John Doe",
                Gender = "Male",
                Dob = new DateTime(1980, 1, 1),
                HealthInsurance = "123456789",
                Address = "123 Main St",
                Job = "Engineer",
                EthnicGroup = "Caucasian"
            };

            var medicalRecord2 = new MedicalRecord
            {
                MedicalRecordId = 102,
                PatientName = "Jane Smith",
                Gender = "Female",
                Dob = new DateTime(1985, 5, 15),
                HealthInsurance = "987654321",
                Address = "456 Oak St",
                Job = "Teacher",
                EthnicGroup = "Asian"
            };

            return new List<MedicalRecordHistory>
            {
                new MedicalRecordHistory {
                    MedicalRecordHistoryId = 1,
                    MedicalRecordId = 101,
                    UserId = 1,
                    User = user1,
                    MedicalRecord = medicalRecord1,
                    Date = DateTime.Now.AddDays(-1),
                    DiagnoseConclusion = "Common cold",
                    TreatmentMethod = "Rest and fluids",
                    Symptom = "High fever",
                    BloodPressure = "120/80",
                    Pulse = 75,
                    RespiratoryRate = 16,
                    Temperature = 38.5,
                    Height = 178,
                    Weight = 75,
                    MedicalRecordHistoryCode = "MRH00001",
                    InsuranceExemption = 80,
                    PatientCategory = "Outpatient",
                    DiseaseProgress = "Mild",
                    DiseaseStage = "Early",
                    Icd = "J00",
                    MedicalOrder = "Rest for 3 days",
                    TreatmentBed = "None",
                    Note = "Patient should return if symptoms worsen"
                },
                new MedicalRecordHistory {
                    MedicalRecordHistoryId = 2,
                    MedicalRecordId = 102,
                    UserId = 2,
                    User = user2,
                    MedicalRecord = medicalRecord2,
                    Date = DateTime.Now.AddDays(-2),
                    DiagnoseConclusion = "Migraine",
                    TreatmentMethod = "Pain medication",
                    Symptom = "Severe headache",
                    BloodPressure = "110/70",
                    Pulse = 80,
                    RespiratoryRate = 14,
                    Temperature = 37.0,
                    Height = 165,
                    Weight = 60,
                    MedicalRecordHistoryCode = "MRH00002",
                    InsuranceExemption = 70,
                    PatientCategory = "Outpatient",
                    DiseaseProgress = "Moderate",
                    DiseaseStage = "Recurring",
                    Icd = "G43",
                    MedicalOrder = "Avoid bright lights",
                    TreatmentBed = "None",
                    Note = "Follow up in 2 weeks"
                }
            };
        }

        public static MedicalRecordHistoryDTO CreateDefaultMedicalRecordHistoryDTO()
        {
            return new MedicalRecordHistoryDTO
            {
                PatientId = 101,
                DiagnoseConclusion = "Common cold",
                TreatmentMethod = "Rest and fluids",
                Symptom = "High fever",
                RecordDate = DateTime.Now,
                BloodPressure = "120/80",
                Pulse = 75,
                RespiratoryRate = 16,
                Temperature = 38.5,
                Height = 178,
                Weight = 75,
                MedicalRecordHistoryCode = "MRH00001",
                InsuranceExemption = 80,
                PatientCategory = "Outpatient",
                DiseaseProgress = "Mild",
                DiseaseStage = "Early",
                ICD = "J00",
                MedicalOrder = "Rest for 3 days",
                TreatmentBed = "None",
                Note = "Patient should return if symptoms worsen"
            };
        }
    }
}

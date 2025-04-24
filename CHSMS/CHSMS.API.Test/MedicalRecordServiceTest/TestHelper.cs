using CHSMS.API.DTOs.MedicalRecord;
using CHSMS.API.Models;

namespace CHSMS.API.Test.MedicalRecordServiceTest
{
    public static class TestHelper
    {
        public static List<MedicalRecord> CreateTestMedicalRecords()
        {
            return new List<MedicalRecord>
            {
                new MedicalRecord
                {
                    MedicalRecordId = 1,
                    PatientName = "John Doe",
                    Gender = "Male",
                    Dob = new DateTime(1980, 1, 1),
                    EthnicGroup = "Caucasian",
                    EducationLevel = "Bachelor",
                    HealthInsurance = "Blue Cross",
                    Address = "123 Main St",
                    PhoneNumber = "555-123-4567",
                    Email = "john.doe@example.com",
                    Job = "Engineer",
                    Status = true,
                    Note = "Regular checkup"
                },
                new MedicalRecord
                {
                    MedicalRecordId = 2,
                    PatientName = "Jane Smith",
                    Gender = "Female",
                    Dob = new DateTime(1990, 5, 15),
                    EthnicGroup = "Asian",
                    EducationLevel = "Master",
                    HealthInsurance = "Aetna",
                    Address = "456 Oak Ave",
                    PhoneNumber = "555-987-6543",
                    Email = "jane.smith@example.com",
                    Job = "Teacher",
                    Status = true,
                    Note = "Allergy to penicillin"
                }
            };
        }
        public static MedicalRecord CreateTestMedicalRecord()
        {
            return new MedicalRecord
            {
                MedicalRecordId = 3,
                PatientName = "Robert Johnson",
                Gender = "Male",
                Dob = new DateTime(1975, 3, 20),
                EthnicGroup = "African American",
                EducationLevel = "PhD",
                HealthInsurance = "Medicare",
                Address = "789 Pine Blvd",
                PhoneNumber = "555-555-5555",
                Email = "robert.johnson@example.com",
                Job = "Professor",
                Status = true,
                Note = "Hypertension"
            };
        }
        public static MedicalRecordDTO CreateTestMedicalRecordDTO()
        {
            return new MedicalRecordDTO
            {
                MedicalRecordId = 0,
                PatientName = "New Patient",
                Gender = "Female",
                Dob = new DateTime(1995, 10, 25),
                EthnicGroup = "Hispanic",
                EducationLevel = "Bachelor",
                HealthInsurance = "Cigna",
                Address = "1010 Maple Dr",
                PhoneNumber = "555-444-3333",
                Email = "new.patient@example.com",
                Job = "Developer",
                Status = true,
                Note = "Initial consultation"
            };
        }
        public static void VerifyMedicalRecordMatchesDTO(MedicalRecord record, MedicalRecordDTO dto)
        {
            Assert.Equal(record.MedicalRecordId, dto.MedicalRecordId);
            Assert.Equal(record.PatientName, dto.PatientName);
            Assert.Equal(record.Gender, dto.Gender);
            Assert.Equal(record.Dob, dto.Dob);
            Assert.Equal(record.EthnicGroup, dto.EthnicGroup);
            Assert.Equal(record.EducationLevel, dto.EducationLevel);
            Assert.Equal(record.HealthInsurance, dto.HealthInsurance);
            Assert.Equal(record.Address, dto.Address);
            Assert.Equal(record.PhoneNumber, dto.PhoneNumber);
            Assert.Equal(record.Email, dto.Email);
            Assert.Equal(record.Job, dto.Job);
            Assert.Equal(record.Status, dto.Status);
            Assert.Equal(record.Note, dto.Note);
        }
    }
}

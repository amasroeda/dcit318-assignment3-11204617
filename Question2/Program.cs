using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareSystem
{
    public class Repository<T>
    {
        private readonly List<T> items = new();

        public void Add(T item) => items.Add(item);

        public List<T> GetAll() => new(items);

        public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);

        public bool Remove(Func<T, bool> predicate)
        {
            var item = items.FirstOrDefault(predicate);
            if (item != null)
            {
                items.Remove(item);
                return true;
            }
            return false;
        }
    }

    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString() => $"ID: {Id}, {Name}, Age: {Age}, Gender: {Gender}";
    }

    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString() => $"Prescription ID: {Id}, Med: {MedicationName}, Date: {DateIssued:d}";
    }

    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new();

        public void SeedData()
        {
            _patientRepo.Add(new Patient(1, "Alice Smith", 30, "Female"));
            _patientRepo.Add(new Patient(2, "Bob Johnson", 45, "Male"));
            _patientRepo.Add(new Patient(3, "Charlie Brown", 28, "Male"));

            _prescriptionRepo.Add(new Prescription(101, 1, "Amoxicillin", DateTime.Today.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(102, 1, "Ibuprofen", DateTime.Today.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(103, 2, "Paracetamol", DateTime.Today.AddDays(-3)));
            _prescriptionRepo.Add(new Prescription(104, 3, "Lisinopril", DateTime.Today));
            _prescriptionRepo.Add(new Prescription(105, 2, "Metformin", DateTime.Today.AddDays(-1)));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap = _prescriptionRepo.GetAll()
                .GroupBy(p => p.PatientId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("--- All Patients ---");
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine(patient);
            }
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.ContainsKey(patientId)
                ? _prescriptionMap[patientId]
                : new List<Prescription>();
        }

        public void PrintPrescriptionsForPatient(int id)
        {
            Console.WriteLine($"--- Prescriptions for Patient ID {id} ---");
            var prescriptions = GetPrescriptionsByPatientId(id);
            if (prescriptions.Count == 0)
            {
                Console.WriteLine("No prescriptions found.");
            }
            else
            {
                foreach (var prescription in prescriptions)
                {
                    Console.WriteLine(prescription);
                }
            }
        }

        public static void Main()
        {
            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();
            Console.WriteLine();
            app.PrintPrescriptionsForPatient(2); 
        }
    }
}

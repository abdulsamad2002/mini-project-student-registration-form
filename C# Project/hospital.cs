using System;
using System.Collections.Generic;

namespace HospitalManagementSystem
{
    public delegate decimal BillingStrategy(decimal baseCost);

    public class PatientEventArgs : EventArgs
    {
        public string PatientName { get; set; }
        public string PatientType { get; set; }
        public decimal BillAmount { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public abstract class Patient
    {
        public string PatientId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime AdmissionDate { get; set; }
        protected decimal BaseTreatmentCost { get; set; }

        public Patient(string id, string name, int age)
        {
            PatientId = id;
            Name = name;
            Age = age;
            AdmissionDate = DateTime.Now;
        }

        public abstract decimal CalculateTreatmentCost();
        public abstract string GetPatientType();

        public virtual void DisplayInfo()
        {
            Console.WriteLine("\n--- Patient Information ---");
            Console.WriteLine("ID: " + PatientId);
            Console.WriteLine("Name: " + Name);
            Console.WriteLine("Age: " + Age);
            Console.WriteLine("Type: " + GetPatientType());
            Console.WriteLine("Admission: " + AdmissionDate.ToString("yyyy-MM-dd HH:mm"));
        }
    }

    public class RegularPatient : Patient
    {
        public string Ailment { get; set; }

        public RegularPatient(string id, string name, int age, string ailment)
            : base(id, name, age)
        {
            Ailment = ailment;
            BaseTreatmentCost = 5000m;
        }

        public override decimal CalculateTreatmentCost()
        {
            return BaseTreatmentCost;
        }

        public override string GetPatientType()
        {
            return "Regular Patient";
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine("Ailment: " + Ailment);
        }
    }

    public class EmergencyPatient : Patient
    {
        public string EmergencyType { get; set; }
        public int SeverityLevel { get; set; }

        public EmergencyPatient(string id, string name, int age, string emergencyType, int severity)
            : base(id, name, age)
        {
            EmergencyType = emergencyType;
            SeverityLevel = severity;
            BaseTreatmentCost = 15000m;
        }

        public override decimal CalculateTreatmentCost()
        {
            return BaseTreatmentCost + (SeverityLevel * 2000m);
        }

        public override string GetPatientType()
        {
            return "Emergency Patient";
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine("Emergency Type: " + EmergencyType);
            Console.WriteLine("Severity Level: " + SeverityLevel + "/5");
        }
    }

    public class ICUPatient : Patient
    {
        public int DaysInICU { get; set; }
        public bool VentilatorRequired { get; set; }

        public ICUPatient(string id, string name, int age, int days, bool ventilator)
            : base(id, name, age)
        {
            DaysInICU = days;
            VentilatorRequired = ventilator;
            BaseTreatmentCost = 25000m;
        }

        public override decimal CalculateTreatmentCost()
        {
            decimal cost = BaseTreatmentCost * DaysInICU;
            if (VentilatorRequired)
                cost += 10000m * DaysInICU;
            return cost;
        }

        public override string GetPatientType()
        {
            return "ICU Patient";
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine("Days in ICU: " + DaysInICU);
            Console.WriteLine("Ventilator Required: " + (VentilatorRequired ? "Yes" : "No"));
        }
    }

    public static class BillingStrategies
    {
        public static decimal StandardBilling(decimal baseCost)
        {
            return baseCost;
        }

        public static decimal InsuranceBilling(decimal baseCost)
        {
            decimal discount = baseCost * 0.30m;
            Console.WriteLine("   Insurance Coverage: Rs. " + discount.ToString("N2"));
            return baseCost - discount;
        }

        public static decimal SeniorCitizenBilling(decimal baseCost)
        {
            decimal discount = baseCost * 0.20m;
            Console.WriteLine("   Senior Citizen Discount: Rs. " + discount.ToString("N2"));
            return baseCost - discount;
        }

        public static decimal GovernmentSchemeBilling(decimal baseCost)
        {
            decimal discount = baseCost * 0.50m;
            Console.WriteLine("   Government Subsidy: Rs. " + discount.ToString("N2"));
            return baseCost - discount;
        }
    }

    public class HospitalNotificationSystem
    {
        public event EventHandler<PatientEventArgs> PatientAdmitted;
        public event EventHandler<PatientEventArgs> BillGenerated;
        public event EventHandler<PatientEventArgs> EmergencyAlert;

        private List<string> subscribedDepartments = new List<string>
        {
            "Billing Department",
            "Nursing Station",
            "Pharmacy",
            "Laboratory"
        };

        public void OnPatientAdmitted(Patient patient)
        {
            PatientEventArgs args = new PatientEventArgs
            {
                PatientName = patient.Name,
                PatientType = patient.GetPatientType(),
                Timestamp = DateTime.Now
            };

            if (PatientAdmitted != null)
                PatientAdmitted(this, args);
        }

        public void OnBillGenerated(Patient patient, decimal billAmount)
        {
            PatientEventArgs args = new PatientEventArgs
            {
                PatientName = patient.Name,
                PatientType = patient.GetPatientType(),
                BillAmount = billAmount,
                Timestamp = DateTime.Now
            };

            if (BillGenerated != null)
                BillGenerated(this, args);
        }

        public void OnEmergencyAlert(Patient patient)
        {
            PatientEventArgs args = new PatientEventArgs
            {
                PatientName = patient.Name,
                PatientType = patient.GetPatientType(),
                Timestamp = DateTime.Now
            };

            if (EmergencyAlert != null)
                EmergencyAlert(this, args);
        }

        public void SubscribeDepartments()
        {
            PatientAdmitted += (sender, e) =>
            {
                Console.WriteLine("\n--- NOTIFICATION - Patient Admitted ---");
                Console.WriteLine("   Time: " + e.Timestamp.ToString("HH:mm:ss"));
                Console.WriteLine("   Patient: " + e.PatientName);
                Console.WriteLine("   Type: " + e.PatientType);
                foreach (var dept in subscribedDepartments)
                {
                    Console.WriteLine("   [OK] " + dept + " notified");
                }
            };

            BillGenerated += (sender, e) =>
            {
                Console.WriteLine("\n--- NOTIFICATION - Bill Generated ---");
                Console.WriteLine("   Time: " + e.Timestamp.ToString("HH:mm:ss"));
                Console.WriteLine("   Patient: " + e.PatientName);
                Console.WriteLine("   Amount: Rs. " + e.BillAmount.ToString("N2"));
                Console.WriteLine("   [OK] Billing Department notified");
                Console.WriteLine("   [OK] Accounts Department notified");
            };

            EmergencyAlert += (sender, e) =>
            {
                Console.WriteLine("\n*** EMERGENCY ALERT! ***");
                Console.WriteLine("   Time: " + e.Timestamp.ToString("HH:mm:ss"));
                Console.WriteLine("   Patient: " + e.PatientName);
                Console.WriteLine("   [OK] Emergency Team alerted");
                Console.WriteLine("   [OK] ICU prepared");
                Console.WriteLine("   [OK] Senior doctors notified");
            };
        }
    }

    public class BillingManager
    {
        private BillingStrategy billingStrategy;

        public void SetBillingStrategy(BillingStrategy strategy)
        {
            billingStrategy = strategy;
        }

        public decimal GenerateBill(Patient patient)
        {
            decimal treatmentCost = patient.CalculateTreatmentCost();
            Console.WriteLine("\n--- Bill Calculation ---");
            Console.WriteLine("Base Treatment Cost: Rs. " + treatmentCost.ToString("N2"));

            if (billingStrategy != null)
            {
                decimal finalAmount = billingStrategy(treatmentCost);
                Console.WriteLine("Final Bill Amount: Rs. " + finalAmount.ToString("N2"));
                return finalAmount;
            }

            Console.WriteLine("Final Bill Amount: Rs. " + treatmentCost.ToString("N2"));
            return treatmentCost;
        }
    }

    public class HospitalSystem
    {
        private List<Patient> patients = new List<Patient>();
        private HospitalNotificationSystem notificationSystem;
        private BillingManager billingManager;
        private int patientCounter = 1000;

        public HospitalSystem()
        {
            notificationSystem = new HospitalNotificationSystem();
            notificationSystem.SubscribeDepartments();
            billingManager = new BillingManager();
        }

        public void Run()
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("   HOSPITAL PATIENT MANAGEMENT SYSTEM     ");
            Console.WriteLine("===========================================");

            while (true)
            {
                Console.WriteLine("\n=== MAIN MENU ===");
                Console.WriteLine("1. Admit New Patient");
                Console.WriteLine("2. View All Patients");
                Console.WriteLine("3. Exit");
                Console.Write("Select option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AdmitPatient();
                        break;
                    case "2":
                        ViewAllPatients();
                        break;
                    case "3":
                        Console.WriteLine("\nThank you for using Hospital Management System!");
                        return;
                    default:
                        Console.WriteLine("Invalid option!");
                        break;
                }
            }
        }

        private void AdmitPatient()
        {
            Console.WriteLine("\n=== PATIENT ADMISSION ===");
            Console.Write("Enter Patient Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Age: ");
            int age = int.Parse(Console.ReadLine());

            Console.WriteLine("\nSelect Patient Type:");
            Console.WriteLine("1. Regular Patient");
            Console.WriteLine("2. Emergency Patient");
            Console.WriteLine("3. ICU Patient");
            Console.Write("Choice: ");
            string typeChoice = Console.ReadLine();

            Patient patient = null;
            string patientId = "P" + (++patientCounter).ToString();

            switch (typeChoice)
            {
                case "1":
                    Console.Write("Enter Ailment: ");
                    string ailment = Console.ReadLine();
                    patient = new RegularPatient(patientId, name, age, ailment);
                    break;

                case "2":
                    Console.Write("Enter Emergency Type: ");
                    string emergencyType = Console.ReadLine();
                    Console.Write("Enter Severity Level (1-5): ");
                    int severity = int.Parse(Console.ReadLine());
                    patient = new EmergencyPatient(patientId, name, age, emergencyType, severity);
                    notificationSystem.OnEmergencyAlert(patient);
                    break;

                case "3":
                    Console.Write("Enter Days in ICU: ");
                    int days = int.Parse(Console.ReadLine());
                    Console.Write("Ventilator Required? (y/n): ");
                    bool ventilator = Console.ReadLine().ToLower() == "y";
                    patient = new ICUPatient(patientId, name, age, days, ventilator);
                    break;

                default:
                    Console.WriteLine("Invalid patient type!");
                    return;
            }

            patients.Add(patient);
            patient.DisplayInfo();

            notificationSystem.OnPatientAdmitted(patient);

            ProcessBilling(patient);
        }

        private void ProcessBilling(Patient patient)
        {
            Console.WriteLine("\n=== BILLING STRATEGY ===");
            Console.WriteLine("1. Standard Billing");
            Console.WriteLine("2. Insurance Coverage");
            Console.WriteLine("3. Senior Citizen Discount");
            Console.WriteLine("4. Government Scheme");
            Console.Write("Select billing strategy: ");
            string billingChoice = Console.ReadLine();

            switch (billingChoice)
            {
                case "1":
                    billingManager.SetBillingStrategy(BillingStrategies.StandardBilling);
                    break;
                case "2":
                    billingManager.SetBillingStrategy(BillingStrategies.InsuranceBilling);
                    break;
                case "3":
                    billingManager.SetBillingStrategy(BillingStrategies.SeniorCitizenBilling);
                    break;
                case "4":
                    billingManager.SetBillingStrategy(BillingStrategies.GovernmentSchemeBilling);
                    break;
                default:
                    billingManager.SetBillingStrategy(BillingStrategies.StandardBilling);
                    break;
            }

            decimal finalBill = billingManager.GenerateBill(patient);

            notificationSystem.OnBillGenerated(patient, finalBill);

            Console.WriteLine("\n[OK] Patient admission complete!");
        }

        private void ViewAllPatients()
        {
            if (patients.Count == 0)
            {
                Console.WriteLine("\nNo patients admitted yet.");
                return;
            }

            Console.WriteLine("\n=== ALL PATIENTS ===");
            foreach (var patient in patients)
            {
                patient.DisplayInfo();
                Console.WriteLine(new string('-', 40));
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            HospitalSystem hospital = new HospitalSystem();
            hospital.Run();
        }
    }
}
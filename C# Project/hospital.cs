using System;
using System.Collections.Generic;

namespace HospitalManagementSystem
{
    public delegate decimal BillingStrategy(decimal amount);

    public class PatientEventArgs : EventArgs
    {
        public string PatientName;
        public string PatientType;
        public decimal Amount;
    }

    public abstract class Patient
    {
        public string Name;
        public int Age;
        public string PatientType;

        public Patient(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public abstract decimal CalculateTreatment();
    }

    public class RegularPatient : Patient
    {
        public RegularPatient(string name, int age) : base(name, age)
        {
            PatientType = "Regular";
        }

        public override decimal CalculateTreatment()
        {
            return 5000;
        }
    }

    public class EmergencyPatient : Patient
    {
        public EmergencyPatient(string name, int age) : base(name, age)
        {
            PatientType = "Emergency";
        }

        public override decimal CalculateTreatment()
        {
            return 15000;
        }
    }

    public class ICUPatient : Patient
    {
        public int Days;

        public ICUPatient(string name, int age, int days) : base(name, age)
        {
            PatientType = "ICU";
            Days = days;
        }

        public override decimal CalculateTreatment()
        {
            return 10000 * Days;
        }
    }

    public class Hospital
    {
        public event EventHandler<PatientEventArgs> PatientAdmitted;
        public event EventHandler<PatientEventArgs> BillGenerated;

        public void AdmitPatient(Patient patient)
        {
            Console.WriteLine("\nPatient Admitted: " + patient.Name);
            Console.WriteLine("Type: " + patient.PatientType);
            
            if (PatientAdmitted != null)
            {
                PatientEventArgs args = new PatientEventArgs();
                args.PatientName = patient.Name;
                args.PatientType = patient.PatientType;
                PatientAdmitted(this, args);
            }
        }

        public void GenerateBill(Patient patient, BillingStrategy strategy)
        {
            decimal treatment = patient.CalculateTreatment();
            Console.WriteLine("\nTreatment Cost: Rs." + treatment);
            
            decimal final = strategy(treatment);
            Console.WriteLine("Final Bill: Rs." + final);

            if (BillGenerated != null)
            {
                PatientEventArgs args = new PatientEventArgs();
                args.PatientName = patient.Name;
                args.Amount = final;
                BillGenerated(this, args);
            }
        }
    }

    public class BillingStrategies
    {
        public static decimal Standard(decimal amount)
        {
            return amount;
        }

        public static decimal Insurance(decimal amount)
        {
            decimal discount = amount * 0.3m;
            Console.WriteLine("Insurance Discount: Rs." + discount);
            return amount - discount;
        }

        public static decimal Government(decimal amount)
        {
            decimal discount = amount * 0.5m;
            Console.WriteLine("Government Subsidy: Rs." + discount);
            return amount - discount;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Hospital hospital = new Hospital();

            hospital.PatientAdmitted += (sender, e) =>
            {
                Console.WriteLine("\n[Notification] Billing Department: Patient " + e.PatientName + " admitted");
                Console.WriteLine("[Notification] Nursing Station: Prepare for " + e.PatientType + " patient");
            };

            hospital.BillGenerated += (sender, e) =>
            {
                Console.WriteLine("\n[Notification] Accounts: Bill of Rs." + e.Amount + " generated for " + e.PatientName);
            };

            while (true)
            {
                Console.WriteLine("\n--- Hospital Patient Management ---");
                Console.WriteLine("1. Admit Patient");
                Console.WriteLine("2. Exit");
                Console.Write("Choice: ");
                string choice = Console.ReadLine();

                if (choice == "2") break;

                if (choice == "1")
                {
                    Console.Write("\nEnter Name: ");
                    string name = Console.ReadLine();

                    Console.Write("Enter Age: ");
                    int age = int.Parse(Console.ReadLine());

                    Console.WriteLine("\nSelect Patient Type:");
                    Console.WriteLine("1. Regular");
                    Console.WriteLine("2. Emergency");
                    Console.WriteLine("3. ICU");
                    Console.Write("Choice: ");
                    string type = Console.ReadLine();

                    Patient patient = null;

                    if (type == "1")
                    {
                        patient = new RegularPatient(name, age);
                    }
                    else if (type == "2")
                    {
                        patient = new EmergencyPatient(name, age);
                    }
                    else if (type == "3")
                    {
                        Console.Write("Enter Days in ICU: ");
                        int days = int.Parse(Console.ReadLine());
                        patient = new ICUPatient(name, age, days);
                    }

                    if (patient != null)
                    {
                        hospital.AdmitPatient(patient);

                        Console.WriteLine("\nSelect Billing Strategy:");
                        Console.WriteLine("1. Standard");
                        Console.WriteLine("2. Insurance");
                        Console.WriteLine("3. Government");
                        Console.Write("Choice: ");
                        string billing = Console.ReadLine();

                        BillingStrategy strategy = BillingStrategies.Standard;

                        if (billing == "2")
                            strategy = BillingStrategies.Insurance;
                        else if (billing == "3")
                            strategy = BillingStrategies.Government;

                        hospital.GenerateBill(patient, strategy);
                    }
                }
            }

            Console.WriteLine("\nThank you!");
        }
    }
}
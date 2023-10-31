using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[Serializable]
class Employee
{
    public string PassportSeries { get; set; }
    public string PassportNumber { get; set; }
    public decimal Salary { get; set; }
    public List<CareerEntry> Career { get; set; }
    public List<Characteristic> Characteristics { get; set; }

    public Employee(string passportSeries, string passportNumber, decimal salary)
    {
        PassportSeries = passportSeries;
        PassportNumber = passportNumber;
        Salary = salary;
        Career = new List<CareerEntry>();
        Characteristics = new List<Characteristic>();
    }

    public void AddCharacteristic(string property, double rating)
    {
        Characteristics.Add(new Characteristic(property, rating));
    }
}

[Serializable]
class CareerEntry
{
    public DateTime AppointmentDate { get; set; }
    public string Position { get; set; }
    public string Department { get; set; }

    public CareerEntry(DateTime appointmentDate, string position, string department)
    {
        AppointmentDate = appointmentDate;
        Position = position;
        Department = department;
    }
}

[Serializable]
class Characteristic
{
    public string Property { get; set; }
    public double Rating { get; set; }

    public Characteristic(string property, double rating)
    {
        Property = property;
        Rating = rating;
    }
}

[Serializable]
class EmployeeContainer<T> : IEnumerable<T> where T : Employee
{
    private List<T> employees = new List<T>();

    public List<T> SortEmployeesByPassport()
    {
        List<T> sortedList = employees.OrderBy(e => e.PassportSeries + e.PassportNumber).ToList();
        return sortedList;
    }

    public List<T> SortEmployeesBySalary()
    {
        List<T> sortedList = employees.OrderBy(e => e.Salary).ToList();
        return sortedList;
    }

    public void AddEmployee(T employee)
    {
        employees.Add(employee);
    }

    public void Serialize(string fileName)
    {
        using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter =
                new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            formatter.Serialize(fileStream, this);
        }
    }

    public static EmployeeContainer<T> Deserialize(string fileName)
    {
        using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter =
                new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            return (EmployeeContainer<T>)formatter.Deserialize(fileStream);
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return employees.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

class Dispatcher
{
    static void Main(string[] args)
    {
        EmployeeContainer<Employee> container = new EmployeeContainer<Employee>();
        bool isAutoMode = args.Any(arg => arg.Equals("auto", StringComparison.OrdinalIgnoreCase));

        if (isAutoMode)
        {
            AutoAddAndSaveData(container);
        }
        else
        {
            while (true)
            {
                Console.WriteLine("Choose an action:");
                Console.WriteLine("1. Display employees");
                Console.WriteLine("2. Add employee");
                Console.WriteLine("3. Sort employees by passport");
                Console.WriteLine("4. Sort employees by salary");
                Console.WriteLine("5. Serialize or deserialize container");
                Console.WriteLine("6. Exit");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DisplayEmployees(container);
                        break;
                    case "2":
                        AddEmployee(container);
                        break;
                    case "3":
                        DisplaySortedEmployees(container.SortEmployeesByPassport());
                        break;
                    case "4":
                        DisplaySortedEmployees(container.SortEmployeesBySalary());
                        break;
                    case "5":
                        Console.Write("Enter the file name for serialization or deserialization: ");
                        string fileName = Console.ReadLine();
                        if (File.Exists(fileName))
                        {
                            container = EmployeeContainer<Employee>.Deserialize(fileName);
                            Console.WriteLine("Data loaded from file '" + fileName + "'.");
                        }
                        else
                        {
                            container.Serialize(fileName);
                            Console.WriteLine("Data saved to file '" + fileName + "'.");
                        }
                        break;
                    case "6":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }

    static void AutoAddAndSaveData(EmployeeContainer<Employee> container)
    {
        // Create a new employee
        Employee newEmployee = new Employee("XYZ", "98765", 60000.00M);

        // Add characteristics (just an example, you can add more characteristics)
        newEmployee.AddCharacteristic("Experience", 5.5);

        // Add the employee to the container
        container.AddEmployee(newEmployee);

        container.Serialize("test");

        Console.WriteLine("Data added and saved to 'test' file.");
    }

    static void AddEmployee(EmployeeContainer<Employee> container)
    {
        Console.Write("Enter passport series: ");
        string passportSeries = Console.ReadLine();

        Console.Write("Enter passport number: ");
        string passportNumber = Console.ReadLine();

        Console.Write("Enter salary: ");
        decimal salary = decimal.Parse(Console.ReadLine());

        Employee newEmployee = new Employee(passportSeries, passportNumber, salary);

        while (true)
        {
            Console.Write("Add a characteristic (Y/N): ");
            string choice = Console.ReadLine().Trim().ToLower();

            if (choice == "y")
            {
                Console.Write("Enter characteristic property: ");
                string property = Console.ReadLine();

                Console.Write("Enter characteristic rating: ");
                double rating = double.Parse(Console.ReadLine());

                newEmployee.AddCharacteristic(property, rating);
                break;
            }
            else if (choice == "n")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid choice. Please enter Y or N.");
            }
        }

        container.AddEmployee(newEmployee);
        Console.WriteLine("Employee added.");
    }

    static void DisplayEmployees(EmployeeContainer<Employee> container)
    {
        foreach (Employee employee in container)
        {
            Console.WriteLine("Passport: " + employee.PassportSeries + "-" + employee.PassportNumber + ", Salary: " + employee.Salary);
            foreach (Characteristic characteristic in employee.Characteristics)
            {
                Console.WriteLine("Characteristic: " + characteristic.Property + ", Rating: " + characteristic.Rating);
            }
        }
    }

    static void DisplaySortedEmployees(List<Employee> employees)
    {
        EmployeeContainer<Employee> sortedContainer = new EmployeeContainer<Employee>();
        foreach (Employee employee in employees)
        {
            sortedContainer.AddEmployee(employee);
        }
        DisplayEmployees(sortedContainer);
    }
}

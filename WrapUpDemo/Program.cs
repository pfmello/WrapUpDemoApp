using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapUpDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            List<PersonModel> people = new List<PersonModel>
            {
                new PersonModel {FirstName = "FuckIgor", LastName = "PoopChocalho", Email = "igoroco@gramacorps.com"},
                new PersonModel {FirstName = "Bigode", LastName = "Bafonso", Email = "bigolho@gramacorps.com"},
                new PersonModel {FirstName = "Danissoura", LastName = "Damasceno", Email = "danivareta@gramacorps.com"}
            };

            List<CarModel> cars = new List<CarModel>
            {
                new CarModel{Manufacturer = "Toyota", Model = "FuckCorolla" },
                new CarModel{Manufacturer = "Toyota", Model = "Highlander" },
                new CarModel{Manufacturer = "Ford", Model = "Mustang" }
            };

            DataAcess<PersonModel> peopleData = new DataAcess<PersonModel>();
            peopleData.BadEntryFound += PeopleData_BadEntryFound;
            peopleData.SaveToCSV(people, @"C:\Users\phili\Documents\C#\people.csv");

            DataAcess<CarModel> carData = new DataAcess<CarModel>();
            carData.BadEntryFound += CarData_BadEntryFound;
            carData.SaveToCSV(cars, @"C:\Users\phili\Documents\C#\cars.csv");


            Console.ReadLine();
        }

        private static void CarData_BadEntryFound(object sender, CarModel e)
        {
            Console.WriteLine($"Bad entry found: {e.Manufacturer} {e.Model}");
        }

        private static void PeopleData_BadEntryFound(object sender, PersonModel e)
        {
            Console.WriteLine($"Bad entry found: {e.FirstName} {e.LastName}");
        }
    }

    public class DataAcess<T> where T: new()
    {
        public event EventHandler<T> BadEntryFound;
        // Declarando que o T precisa de um construtor vazio !
        public void SaveToCSV(List<T> items, string filePath)
        {
            List<string> rows = new List<string>();
            T entry = new T();
            var cols = entry.GetType().GetProperties();

            string row = "";
            foreach (var col in cols)
            {
                row += $",{ col.Name }";
            }

            row = row.Substring(1); // FirstName, LastName, Email
            rows.Add(row);

            foreach (var item in items)
            {
                row = "";
                bool badWordDetected = false;

                foreach (var col in cols)
                {
                    string val = col.GetValue(item, null).ToString();
                    badWordDetected = BadWordDetector(val);

                    if(badWordDetected)
                    {
                        BadEntryFound.Invoke(this, item);
                        break;
                    }

                    row += $",{val}";
                }

                if (!badWordDetected)
                {
                    row = row.Substring(1);
                    rows.Add(row);
                }
            }

            File.WriteAllLines(filePath, rows);
        }

        private bool BadWordDetector(string word)
        {
            bool output = false;
            string lowerCaseTest = word.ToLower();

            if (lowerCaseTest.Contains("fuck") || lowerCaseTest.Contains("poop"))
            {
                output = true;
            }

            return output;
        }
    }
}

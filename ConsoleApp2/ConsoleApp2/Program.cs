using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TicketReservation
{
    // Базовый класс для автобусов
    public partial class Bus
    {
        public string Route { get; set; }
        public DateTime Date { get; set; }
        public int AvailableSeats { get; set; }

        public virtual void PrintInfo()
        {
            Console.WriteLine($"Маршрут: {Route}");
            Console.WriteLine($"Дата: {Date}");
            Console.WriteLine($"Свободные места: {AvailableSeats}");
        }

        public virtual void ReserveSeat()
        {
            AvailableSeats--;
        }
    }

    // Класс для хранения информации о пассажирах
    public class Passenger
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string PassportNumber { get; set; }
    }

    // Частичный класс для автобусов
    public partial class Bus
    {
        protected string DriverName { get; set; }
        protected string BusNumber { get; set; }
    }

    // Частичный класс для автобусов
    public partial class Bus
    {
        public const int MaxSeats = 50;
        public static int TotalBuses { get; set; }

        public static void PrintTotalBuses()
        {
            Console.WriteLine($"Total Buses: {TotalBuses}");
        }
    }

    // Методы расширения
    public static class StringExtensions
    {
        public static bool IsValidPhoneNumber(this string phoneNumber)
        {
            return true;
        }

        public static bool IsValidPassportNumber(this string passportNumber)
        {
            return true;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Чтение информации из xml-файла с использованием LINQ to XML
            XDocument doc = XDocument.Load("d://buses.xml");
            IEnumerable<Bus> buses = doc.Root.Elements("Bus")
                .Select(b => new Bus
                {
                    Route = b.Element("Route").Value,
                    Date = DateTime.Parse(b.Element("Date").Value),
                    AvailableSeats = int.Parse(b.Element("AvailableSeats").Value)
                });

            // Коллекция для хранения зарегистрированных пассажиров
            List<Passenger> passengers = new List<Passenger>();

            // Взаимодействие с пользователем
            Console.WriteLine("Добро пожаловать в Ticket Reservation System");
            Console.WriteLine("Доступные автобусы:");

            foreach (var bus in buses)
            {
                bus.PrintInfo();
            }

            Console.WriteLine();

            Console.Write("Выберите маршрут: ");
            string route = Console.ReadLine();

            Console.Write("Введите дату (yyyy-MM-dd): ");
            DateTime date = DateTime.Parse(Console.ReadLine());

            var selectedBus = buses.FirstOrDefault(b => b.Route == route && b.Date.Date == date.Date);

            if (selectedBus != null)
            {
                Console.WriteLine("Автобус найден:");
                selectedBus.PrintInfo();

                Console.WriteLine("Введите данные пассажира:");

                Console.Write("ФИО: ");
                string fullName = Console.ReadLine();

                Console.Write("Номер телефона: ");
                string phoneNumber = Console.ReadLine();

                Console.Write("Номер и серия паспорта: ");
                string passportNumber = Console.ReadLine();

                // Проверка валидности номера телефона и паспорта
                if (phoneNumber.IsValidPhoneNumber() && passportNumber.IsValidPassportNumber())
                {
                    Passenger passenger = new Passenger
                    {
                        FullName = fullName,
                        PhoneNumber = phoneNumber,
                        PassportNumber = passportNumber
                    };

                    passengers.Add(passenger);
                    selectedBus.ReserveSeat();

                    // Обновление xml-файла
                    doc.Root.Elements("Bus")
                        .Where(b => b.Element("Route").Value == selectedBus.Route && b.Element("Date").Value == selectedBus.Date.ToString("yyyy-MM-dd"))
                        .Select(b => b.Element("AvailableSeats").Value = selectedBus.AvailableSeats.ToString());

                    doc.Save("buses.xml");

                    Console.WriteLine("Билет успешно зарегистрирован");
                }
                else
{
                    Console.WriteLine("Неправильный номер телефона или паспорта.");
                }
            }
            else
            {
                Console.WriteLine("На выбранный маршрут и дату нет доступных автобусов.");
            }

            // Отмена регистрации
            Console.WriteLine("Хотите отменить регистрацию? (yes/no)");
            string cancelRegistration = Console.ReadLine();

            if (cancelRegistration.ToLower() == "yes")
            {
                Console.Write("Введите полное имя пассажира: ");
                string passengerName = Console.ReadLine();

                var passengerToRemove = passengers.FirstOrDefault(p => p.FullName == passengerName);

                if (passengerToRemove != null)
                {
                    passengers.Remove(passengerToRemove);
                    selectedBus.AvailableSeats++;

                    // Обновление xml-файла
                    // Обновление xml-файла
                    var busElement = doc.Root.Elements("Bus")
                        .FirstOrDefault(b => b.Element("Route").Value == selectedBus.Route && b.Element("Date").Value == selectedBus.Date.ToString("yyyy-MM-dd"));

                    if (busElement != null)
                    {
                        busElement.Element("AvailableSeats").SetValue(selectedBus.AvailableSeats.ToString());
                        doc.Save("buses.xml");
                    }
                }
                else
                {
                    Console.WriteLine("Пассажир не найден.");
                }
            }

            // Вывод информации о зарегистрированных пассажирах
            Console.WriteLine("Хотите просмотреть список зарегистрированных пассажиров? (yes/no)");
            string viewPassengers = Console.ReadLine();

            if (viewPassengers.ToLower() == "yes")
            {
                Console.WriteLine("Зарегистрированные пассажиры:");

                foreach (var passenger in passengers)
                {
                    Console.WriteLine($"ФИО: {passenger.FullName}");
                    Console.WriteLine($"Номер телефона: {passenger.PhoneNumber}");
                    Console.WriteLine($"Номер и серия паспорта: {passenger.PassportNumber}");
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Спасибо за использование Ticket Reservation System.");
        }
    }
}

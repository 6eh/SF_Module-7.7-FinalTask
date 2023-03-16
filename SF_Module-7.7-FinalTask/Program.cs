using System;

namespace SF_Module_7._7_FinalTask
{
    class Program
    {
        enum ProductType
        {
            Pizza,
            Sushi,
            Doner,
            Burger,
            Drink
        }

        enum DeliveryStatuses
        {
            Scheduled,     // Оформлено
            InTransit,    // В процессе
            Delivered,   // Доставлено
            Failed      // Доставка не удалась
        }

        enum WorksPosotiont
        {
            Director,
            Courer,
            Seller
        }

        class Product<TCode>
        {
            public TCode Code { get; set; }
            public ProductType Type { get; set; }
            public string Name { get; set; }
            public double Weight { get; set; }
            public string UnitType { get; set; }
            private decimal price;
            public decimal Price
            {
                get { return price; }
                set
                {
                    if (value > 0)
                        price = value;
                    else
                    {
                        price = 99999999;
                        Console.WriteLine("Price must be > 0");
                    }
                }
            }
            public Product(string name) => Name = name;
        }

        abstract class User
        {
            public Guid Id { get; } = Guid.NewGuid();
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public virtual string FullName()
            {
                return $"{FirstName} {LastName}";
            }
        }

        class Customer : User
        {
            private string phone;
            public string Phone
            {
                get { return phone; }
                set { phone = value; }
            }
            public DateTime Birthday { get; set; }
            public decimal BonusBalance { get; set; } = 0;
            public Customer(string firstName, string lastName)
            {
                FirstName = firstName;
                LastName = lastName;
            }
            public void AddBonus(decimal bonus) => BonusBalance += bonus;
        }

        class Enployee : User
        {
            public WorksPosotiont WorkPosition;
            public override string FullName()
            {
                return $"{FirstName} {LastName} ({WorkPosition})";
            }
        }

        public struct Address
        {
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public int PostalCode { get; set; }
            public int HomeNum { get; set; }
            public int ApartNum { get; set; }
            public Address(int postalCode, string state, string city, string street, int homeNum, int apartNum)
            {
                PostalCode = postalCode;
                State = state;
                City = city;
                Street = street;
                HomeNum = homeNum;
                ApartNum = apartNum;
            }
            public string FullAdress()
            {
                return $"{PostalCode}, {State}, {City}, {Street}, {HomeNum} - {ApartNum}";
            }
        }

        abstract class Delivery
        {
            public virtual string Type { get; } = string.Empty;
            public Address Address;
            public DeliveryStatuses Status;
        }

        class HomeDelivery : Delivery
        {
            public override string Type { get; } = "Home Delivery";
            public Enployee Courier;
            public DateTime TimeIntervalStart;
            public DateTime TimeIntervalFinish;
        }

        class PickPointDelivery : Delivery
        {
            public override string Type { get; } = "Pick Up";
            public string SecretCode;

        }

        class ShopDelivery : Delivery
        {
            public override string Type { get; } = "Delivery to the shop";
            public DateTime OpenDayTime;
            public DateTime CloseDateTime;
        }

        struct PaymentInfoCredit
        {
            public string Type
            {
                get { return "Credit"; }
            }
            public bool Paid { get; set; }              // Оплачено?
            public string CreditCompany { get; set; }   // Банк кредитор
            public byte CreditMounth { get; set; }      // Сколько кредитных месяцев
        }

        struct PaymentInfoOnline
        {
            public string Type
            {
                get { return "Online payment"; }
            }
            public bool Paid { get; set; }             // Оплачено?
            public string Service { get; set; }        // Сервис оплаты
        }

        class Order<TDelivery, TStruct, TCode> where TDelivery : Delivery
        {
            public int Number;

            public DateTime Date { get; set; } = DateTime.Now;
            public TDelivery Delivery;

            public TStruct PaymentInfo;

            public Product<TCode>[] Products;

            public Product<TCode> this[int index]
            {
                get => Products[index];
                set => Products[index] = value;
            }

            public Customer Customer;

            public string Description { get; set; }

            public decimal TotalOrderPrice()
            {
                decimal orderPrice = 0;
                foreach (var v in Products)
                {
                    orderPrice += v.Price;
                }
                return orderPrice;
            }

            public string OrderInfo()
            {
                string productsListStr = string.Empty;
                for (int i = 0; i < Products.Length; i++)
                    productsListStr += $"\n    {i + 1}. {Products[i].Code}: {Products[i].Type} - {Products[i].Name} ({Products[i].UnitType}: {Products[i].Weight}) = {Products[i].Price}";

                return $"Order # {Number} - {Date}\n\nCustomer: {Customer.FullName()} (Id: {Customer.Id})" +
                    $"\n\nDelivery information:" +
                    $"\n    Type: {Delivery.Type}\n    Adress: {Delivery.Address.FullAdress()}" +
                    $"\n\nProducts: {productsListStr}" +
                    $"\nTotal order price = {TotalOrderPrice()}\n\nDescription: {Description}";
            }
        }

        static void Main(string[] args)
        {
            NewOrder();
            Console.ReadKey();
        }

        static void NewOrder()
        {
            Customer customer = new Customer(firstName: "Sandra", lastName: "Horizon")
            {
                Phone = "12345678",
                BonusBalance = 50
            };

            Product<int> CheeseBurger = new Product<int>("Cheese Burger")
            {
                Code = 18800,
                Price = 80,
                Type = ProductType.Burger,
                Weight = 100,
                UnitType = "Weight"
            };

            Product<int> Cola = new Product<int>("Coca Cola Zero")
            {
                Code = 2089,
                Price = 20,
                Type = ProductType.Drink,
                Weight = 0.5,
                UnitType = "Volume"
            };

            Product<int>[] products = { CheeseBurger, Cola };

            HomeDelivery delivery = new HomeDelivery()
            {
                Address = new Address(198000, "S-Pb", "Saint-Petersburg", "Nevsky p.", homeNum: 1, apartNum: 8),
                Courier = new Enployee() { FirstName = "John", LastName = "Piterson", WorkPosition = WorksPosotiont.Courer },
                Status = DeliveryStatuses.Scheduled,
                TimeIntervalStart = DateTime.Parse("2023-01-01 16:00:00"),
                TimeIntervalFinish = DateTime.Parse("2023-01-01 19:00:00")
            };

            Order<Delivery, PaymentInfoOnline, int> order = new()
            {
                Number = 1,
                Delivery = delivery,
                PaymentInfo = new PaymentInfoOnline() { Paid = true, Service = "Webmoney" },
                Products = products,
                Customer = customer,
                Description = "Call 1 hours in advance"
            };

            // 5% от стоимости заказа начислить покупателю в виде бонусов
            var bonus = ((order.TotalOrderPrice() / 100) * 5);
            order.Customer.AddBonus(bonus);

            Console.WriteLine(order.OrderInfo());
            Console.WriteLine($"\nPayment information:" +
                $"\n    Paid: {order.PaymentInfo.Paid}" +
                $"\n    Payment type: {order.PaymentInfo.Type}" +
                $"\n    Payment service: {order.PaymentInfo.Service}");
            Console.WriteLine($"\nCustomer's bonus balance was add {bonus} bonus coins");
            Console.WriteLine($"Customer's bonus balance = {order.Customer.BonusBalance}");
            Console.WriteLine("-----------------");
        }
    }
}


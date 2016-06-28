using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Data;
namespace PartyWristbandAPI
{
    class Program
    {
        static long negInf = -999999999;
        static long pozInf = 999999999;
        static API ap = new API();
        static void Main(string[] args)
        {
            
            ap.somethingToPickUp += Ap_somethingToPickUp;
            string input;
            ap.endLogin += Ap_endLogin;
            ap.endLogout += Ap_endLogout;
            ap.endRegister += Ap_endRegister;
            ap.endNewTransaction += Ap_endNewTransaction;
            ap.endListTransactions += Ap_endListTransactions;
            ap.endListDrinks += Ap_endListDrinks;
            ap.endUserPayed += Ap_endUserPayed;
            ap.endListUsers += Ap_endListUsers;
            ap.ConnectionChangedState += Ap_ConnectionChangedState;
            do
            {
                Console.Write("(Select)>");
                input = Console.ReadLine();
                try {
                    switch (input)
                    {
                        case "login":
                            _Login();
                            break;
                        case "?":
                            Console.WriteLine("connected: " + ap.Connected +  "\nlogged in: " + ap.iok._prijavljen + "\nemail: " + ap.iok._mail );
                            break;
                        case "out":
                            _Logout();
                            break;
                        case "reg":
                            _Register();
                            break;
                        case "new":
                            _Transaction();
                            break;
                        case "AllTrans":
                            _AllTrans();
                            break;
                        case "UTrans":
                            _UTrans();
                            break;
                        case "Drinks":
                            _Drinks();
                            break;
                        case "ADrinks":
                            _ADrinks();
                            break;
                        case "UDrinks":
                            _UDrinks();
                            break;
                        case "T":
                            _T();
                            break;
                        case "Pay":
                            _Pay();
                            break;
                        case "q":
                            ap.Dispose();
                            break;
                        case "Users":
                            _Users();
                            break;
                        case "SerchU":
                            _SerchU();
                            break;
                        case "AllU":
                            _AllU();
                            break;
                    }
                }catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            } while (input != "q");
        }

        private static void Ap_ConnectionChangedState(object sender, bool e)
        {
            if (e)
                Console.WriteLine("Connected");
            else
                Console.WriteLine("Disconnected");
            Console.Write("(Select)>");
        }

        private static void _AllU()
        {
            ap.BeginListOfAllUsers(pozInf, negInf, ZajednickiPaket.UnPayedStatus.MattersNot, null, null, null, null, -3);
        }

        private static void _SerchU()
        {
            string wholeSomething = string.Empty;
            char c;
            while ('!' !=(c = Console.ReadKey().KeyChar))
            {
                if (c > 'a' && c < 'z')
                {
                    wholeSomething += c;
                }else if (c == '\b' && wholeSomething.Length !=0)
                {
                    wholeSomething = wholeSomething.Remove(wholeSomething.Length-1,1);
                }
                ap.BeginListOfAllUsers(pozInf, negInf, ZajednickiPaket.UnPayedStatus.MattersNot, null, null, null, wholeSomething, -3);

            }
        }

        private static void _Users()
        {
            short privLevel;
            string name, lastName, mail, something;
            long payMoreThen, payLessThen;

            Console.Write("(Users-mail)>");
            mail = Console.ReadLine();
            if (mail.ToLower() != "null")
            {
                ap.BeginListOfAllUsers(pozInf, negInf, ZajednickiPaket.UnPayedStatus.MattersNot, mail, null, null, null, -3);
                return;
            }

            Console.Write("(Users-something)>");
            something = Console.ReadLine();
            if (something.ToLower() != "null")
            {
                ap.BeginListOfAllUsers(pozInf, negInf, ZajednickiPaket.UnPayedStatus.MattersNot, null, null, null, something, -3);
                return;
            }

            Console.Write("(Users-Name)>");
            name = Console.ReadLine();
            if (name.ToLower() == "null")
                name = null;

            Console.Write("(Users-LastName)>");
            lastName = Console.ReadLine();
            if (lastName.ToLower() == "null")
                lastName = null;

            Console.Write("(Users-privLevel)>");
            something = Console.ReadLine();
            if (something.ToLower() == "null")
                privLevel = -3;
            else
                privLevel = Convert.ToInt16(something);

            Console.Write("(Users-SpendLessThen)>");
            something = Console.ReadLine();
            if (something.ToLower() == "null")
                payLessThen = negInf;
            else
                payLessThen = Convert.ToInt64(something);

            Console.Write("(Users-SpendMoreThen)>");
            something = Console.ReadLine();
            if (something.ToLower() == "null")
                payMoreThen = pozInf;
            else
                payMoreThen = Convert.ToInt64(something);
            ap.BeginListOfAllUsers(payMoreThen, payLessThen, ZajednickiPaket.UnPayedStatus.MattersNot, null, name, lastName, null, privLevel);

        }
        private static void Ap_endListUsers(object sender, API.ListUsers e)
        {

            int top = Console.CursorTop;
            int left = Console.CursorLeft;
            Console.CursorTop += 2;
            Console.CursorLeft = 0;
            Console.WriteLine(e.error);
            for (int i= 0; i < (e.users.Rows.Count + 13); i++)
            {
                Console.WriteLine(new string(' ',Console.BufferWidth-1));
            }
            Console.CursorTop = top + 2;
            Console.CursorLeft = 0;

            if (e.error == "OK")
                ap.PrintDataTable(e.users);
            Console.Write("(Select)>");
            Console.CursorTop = top;
            Console.CursorLeft = left;
        }


        private static void _Pay()
        {
            int amount;
            string userMail;
            Console.Write("(Pay-UserMail)>");
            userMail = Console.ReadLine();
            Console.Write("(Pay-Amount)>");
            amount = Convert.ToInt32(Console.ReadLine());
            ap.BeginUserPayed(false, amount, userMail);
        }
        private static void Ap_endUserPayed(object sender, API.UserPayed e)
        {
            Console.WriteLine("Pay" + e.error);
            if (e.error == "OK")
            {
                Console.WriteLine("Pay" + e.user);
                ap.PrintDataTable(e.listOfPayedStuf);
            }
            Console.Write("(Select)>");
        }

        private static void _T()
        {
            string id, userEmail, placeno;
            Console.Write("(Transakcije-ID[null = Matters not])>");
            id = Console.ReadLine();
            if (id != "null")
            {
                ap.BeginTransacition(id, null, ZajednickiPaket.Placeno.Nebitno);
            }
            id = null;
            Console.Write("(Transakcije-UsersEmail[null = Matters not])>");
            userEmail = Console.ReadLine();
            if (userEmail == "null")
                userEmail = null;
            Console.Write("(Transakcije-Placeno[y-Yes/n-No/null-Matters not])>");
            placeno = Console.ReadLine();
            switch (placeno)
            {
                case "y":
                    ap.BeginTransacition(null, userEmail, ZajednickiPaket.Placeno.Placeno);
                    break;
                case "n":
                    ap.BeginTransacition(null, userEmail, ZajednickiPaket.Placeno.Neplaceno);
                    break;
                default:
                    ap.BeginTransacition(null, userEmail, ZajednickiPaket.Placeno.Nebitno);
                    break;
            }
        }

        private static void _UDrinks()
        {
            ap.BeginViweNotAvalableDrinks();
        }
        private static void _ADrinks()
        {
            ap.BeginViewAvalableDrinks();
        }
        private static void _Drinks()
        {
            ap.BeginViewAllDrinks();
        }
        private static void Ap_endListDrinks(object sender, API.ListDrinks e)
        {
            Console.WriteLine("List Drinks" + e.error);
            try {
                ap.PrintDataTable(e.listOfDrinks);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.Write("(Select)>");
        }

        private static void _UTrans()
        {
            string mail;
            mail = Console.ReadLine();
            ap.BeginUsersTransactions(mail);
        }
        private static void _AllTrans()
        {
            ap.BeginAllTransactions();
        }
        private static void Ap_endListTransactions(object sender, API.ListTransactions e)
        {
            Console.WriteLine("List Transactions" + e.error);
            try {
                ap.PrintDataTable(e.ListOfTransactions);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.Write("(Select)>");
        }

        private static void _Register()
        {
            string ime, prezime, emaill, passs, oib, brojMoba, adresa;

            Console.Write("(register-ime)>");
            ime = Console.ReadLine();
            Console.Write("(register-prezime)>");
            prezime = Console.ReadLine();
            Console.Write("(register-email)>");
            emaill = Console.ReadLine();
            Console.Write("(register-pass)>");
            passs = Console.ReadLine();
            Console.Write("(register-oib)>");
            oib = Console.ReadLine();
            Console.Write("(register-brojMoba)>");
            brojMoba = Console.ReadLine();
            Console.Write("(register-adresa)>");
            adresa = Console.ReadLine();
            Console.Write("(register-slika)>");
            string putDoSlike = Console.ReadLine();
            byte[] slika;
            try
            {
                StreamReader sr = new StreamReader(putDoSlike);
                slika = Encoding.ASCII.GetBytes(sr.ReadToEnd());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                slika = null;
            }
            ap.BeginRegister(ime, prezime, emaill, passs, slika, oib, adresa, brojMoba);
        }
        private static void Ap_endRegister(object sender, API.Register e)
        {
            Console.WriteLine("Register " +  e.error);
            Console.Write("(Select)>");
        }

        private static void _Logout()
        {
            ap.BeginLogout();
        }
        private static void Ap_endLogout(object sender, API.Logout e)
        {
            Console.WriteLine("Logout " + e.error);
            Console.Write("(Select)>");
        }

        private static void _Login()
        {
            Console.Write("(login-email)>");
            string email = Console.ReadLine();

            Console.Write("(login-pass)>");
            string pass = Console.ReadLine();
            ap.BeginLogin(email, pass);
        }
        private static void Ap_endLogin(object sender, API.Login e)
        {
            Console.WriteLine("Login " + e.error);
            Console.Write("(Select)>");
        }

        private static void _Transaction()
        {
            string mail, drink;
            decimal size;
            int amount;
            Console.Write("(Transaction-buyersEmail)>");
            mail = Console.ReadLine();
            Console.Write("(Transaction-Drink)>");
            drink = Console.ReadLine();
            Console.Write("(Transaction-DrinkSize)>");
            size = Convert.ToDecimal(Console.ReadLine());
            Console.Write("(Transaction-Amount)>");
            amount = Convert.ToInt32(Console.ReadLine());

            ap.BeginNewTransaction(mail, drink, size, amount);
        }
        private static void Ap_endNewTransaction(object sender, API.Newtransaction e)
        {
            Console.WriteLine("New Transaction " + e.error);
            Console.Write("(Select)>");
        }

        private static void Ap_somethingToPickUp(object sender, API.NewMessageEventArgs e)
        {
            Console.WriteLine(e.obj + " " + e.error);
        }
    }
}

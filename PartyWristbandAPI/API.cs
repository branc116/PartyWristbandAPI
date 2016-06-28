using System;
using System.Net.Sockets;
using System.Net.Security;
using ZajednickiPaket;
using System.Security.Cryptography.X509Certificates;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PartyWristbandAPI
{
    public class API
    {
        InformacijeOKorisniku iOK;
        public InformacijeOKorisniku iok
        {
            get
            {
                return iOK;
            }
        }
        protected bool _connected;
        public bool Connected {
            get
            {
                return _connected;
            }
        }
        public bool pendingConnection;

        protected SslStream strm;
        protected string ip;
        protected int port;
        protected TcpClient me;
        protected Cripto cripto;
        protected byte[] BUFFER;
        protected List<byte> wholeMess;
        protected int wholeSize;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                //strm.Dispose(true);
                //me.Dispose(true);
            }
            // free native resources
        }
        public void Dispose()
        {
            
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Use for junk, more or less not usefull
        /// </summary>
        public event EventHandler<NewMessageEventArgs> somethingToPickUp;

        /// <summary>
        /// Use for 'BeginLogin'
        /// </summary>
        public event EventHandler<Login> endLogin;
        /// <summary>
        /// Use for 'BeginRegister'
        /// </summary>
        public event EventHandler<Register> endRegister;
        /// <summary>
        /// Use for 'BeginLogout'
        /// </summary>
        public event EventHandler<Logout> endLogout;
        /// <summary>
        /// Use for 'BeginNewTransaction'
        /// </summary>
        public event EventHandler<Newtransaction> endNewTransaction;
        /// <summary>
        /// Use for 'BeginAllTransactions', 'BeginUserTransactions'
        /// </summary>
        public event EventHandler<ListTransactions> endListTransactions;
        /// <summary>
        /// Uer for 'BeginUserPayed'
        /// </summary>
        public event EventHandler<UserPayed> endUserPayed;
        /// <summary>
        /// Use for 'BeginViewAllDrinks', 'BeginViewAvalableDrinks', 'BeginViewNotAvalableDrinks', 'BeginServhDrinks'
        /// </summary>
        public event EventHandler<ListDrinks> endListDrinks;
        /// <summary>
        /// Use for 'BeginAddnewDrink'
        /// </summary>
        public event EventHandler<AddNewDrink> endAddNewDrink;
        /// <summary>
        /// Use for 'BeginListOfAllUsers'
        /// </summary>
        public event EventHandler<ListUsers> endListUsers;
        /// <summary>
        /// Use for 'BeginAssigneWristbandToUser'
        /// </summary>
        public event EventHandler<AssigneWristbandToUser> endAssigneWristbandToUser;
        /// <summary>
        /// Use for 'UnAssigneWristbandFromUser'
        /// </summary>
        public event EventHandler<UnAssigneWristbandFromUser> endUnAssigneWristbandFromUser;
        /// <summary>
        /// Use for 'BeginWristbandChangeState'
        /// </summary>
        public event EventHandler<WristbandChangeState> endWristbandChangeState;
        /// <summary>
        /// Use for 'BeginAddNewWristband'
        /// </summary>
        public event EventHandler<AddNewWristband> endAddNewWristband;
        /// <summary>
        /// Use for 'BeginChangeUserPrivilageLevel'
        /// </summary>
        public event EventHandler<ChangeUserPrivilageLevel> endChangeUserPrivilageLevel;
        /// <summary>
        /// Use for 'BeginDownloadNewestVersion'
        /// </summary>
        public event EventHandler<DownloadNewestVersion> endDownloadNewestVersion;
        public event EventHandler<bool> ConnectionChangedState;
         
        public API()
        {
            iOK = new InformacijeOKorisniku();
            cripto = new Cripto();
            wholeMess = new List<byte>();
            wholeSize = 0;
            Task t = KeepConnecthing("127.0.0.1", 4999);
        }
        public API(string ip, int port)
        {
            Task t = KeepConnecthing(ip, port);
        }
        
        public class NewMessageEventArgs : EventArgs
        {
            public object obj;
            public string error;
            public NewMessageEventArgs(object Object, string strError)
            {
                obj = Object;
                error = strError;
            }
        }
        public class Login
        {
            public string error;
            public Login(string _error)
            {
                error = _error;
            }
        }
        public class Register {
            public string error;
            public Register(string _error)
            {
                error = _error;
            }
        }
        public class Logout
        {
            public string error;
            public Logout(string _error)
            {
                error = _error;
            }
        }
        public class Newtransaction
        {
            public string error;
            public Newtransaction(string _error)
            {
                error = _error;
            }
        }
        public class ListTransactions
        {
            public string error;
            public DataTable ListOfTransactions;

            public ListTransactions(string _error, DataTable _ListOfTransactions)
            {
                error = _error;
                ListOfTransactions = _ListOfTransactions;
            }
        }
        public class UserPayed
        {
            public string error;
            public DataTable listOfPayedStuf;
            public string user;
            DateTime timeOfPayment;
            public UserPayed(string _error, DataTable _listOfPayedStuf,string _user,DateTime _timeOfPayment)
            {
                error = _error;
                listOfPayedStuf = _listOfPayedStuf;
                user = _user;
                timeOfPayment = _timeOfPayment;
            }
        }
        public class ListDrinks
        {
            public string error;
            public DataTable listOfDrinks;

            public ListDrinks(string _error, DataTable _listOfDrinks)
            {
                error = _error;
                listOfDrinks = _listOfDrinks;
            }
        }
        public class AddNewDrink
        {
            public string error;
            public AddNewDrink(string _error)
            {
                error = _error;
            }
        }
        public class ListUsers
        {
            public string error;
            public DataTable users;
            public ListUsers(string _error, DataTable _users)
            {
                error = _error;
                users = _users;
            }
        }
        public class AssigneWristbandToUser
        {
            public string error;
            public AssigneWristbandToUser(string _error)
            {
                error = _error;
            }
        }
        public class UnAssigneWristbandFromUser
        {
            public string error;
            public UnAssigneWristbandFromUser(string _error)
            {
                error = _error;
            }
        }
        public class WristbandChangeState
        {
            public string error;
            public WristbandChangeState(string _error)
            {
                error = _error;
            }
        }
        public class AddNewWristband
        {
            public string error;
            public AddNewWristband(string _error)
            {
                error = _error;
            }
        }
        public class ChangeUserPrivilageLevel
        {
            public string error;
            public ChangeUserPrivilageLevel(string _error)
            {
                error = _error;
            }
        }
        public class DownloadNewestVersion
        {
            public string error;
            byte[] file; 
            public DownloadNewestVersion(string _error, byte[] _file)
            {
                error = _error;
                file = _file;
            }
        }
        
        public void BeginLogin(string email, string password)
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            Prijava p = new Prijava(email, password);
            MySend(p);
        }
        public void BeginRegister(string firstName, string secondName, string email,string password, byte[] profilePicture, string oib, string address, string phoneNum)
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            if (iOK._prijavljen)
                throw new Exception(Greske.LoggedInCanNotRegister.ToString());

            Registracija r = new Registracija(firstName, secondName, email, password, profilePicture, oib, phoneNum, address);
            MySend(r);
        }
        public void BeginLogout()
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            if (!iOK._prijavljen)
                throw new Exception(Greske.NotLoggedIn.ToString());

            Odjava od = new Odjava();
            od.KopirajInformacije(iOK);
            MySend(od);
        }
        public void BeginNewTransaction(string buyersEmail, string drink, decimal drinkSize, int amount)
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            if (!iOK._prijavljen)
                throw new Exception(Greske.NotLoggedIn.ToString());

            NovaTransakcija nT = new NovaTransakcija(buyersEmail, drink, drinkSize, amount);
            nT.KopirajInformacije(iOK);

            MySend(nT);
        }
        public void BeginAllTransactions()
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            if (!iOK._prijavljen)
                throw new Exception(Greske.NotLoggedIn.ToString());

            SveTransakcije at = new SveTransakcije();
            at.KopirajInformacije(iOK);
            MySend(at);
        }
        public void BeginUsersTransactions(string usersEmail)
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            if (!iOK._prijavljen)
                throw new Exception(Greske.NotLoggedIn.ToString());

            KorisnikoveTransakcije kt = new KorisnikoveTransakcije(usersEmail);
            kt.KopirajInformacije(iOK);
            MySend(kt);
        }
        public void BeginTransacition(string id, string userMail, Placeno p)
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            if (!iOK._prijavljen)
                throw new Exception(Greske.NotLoggedIn.ToString());
            Transakcije t = new Transakcije(id, userMail, p);
            t.KopirajInformacije(iOK);
            MySend(t);
        }
        public void BeginUserPayed(bool all, int amount, string userMail)
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            if (!iOK._prijavljen)
                throw new Exception(Greske.NotLoggedIn.ToString());

            UserPayedd uP = new UserPayedd(userMail, amount, all);
            uP.KopirajInformacije(iOK);
            MySend(uP);
        }
        public void BeginViewAllDrinks()
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            SvaDrinks sD = new SvaDrinks();
            sD.KopirajInformacije(iOK);
            MySend(sD);
        }
        public void BeginViewAvalableDrinks()
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            DostupnaDrinks dD = new DostupnaDrinks();
            dD.KopirajInformacije(iOK);
            MySend(dD);
        }
        public void BeginViweNotAvalableDrinks()
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            NedostupnaDrinks nD = new NedostupnaDrinks();
            nD.KopirajInformacije(iOK);
            MySend(nD);
        }
        public void BeginSerchDrinks(string find)
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            if (!iOK._prijavljen)
                throw new Exception(Greske.NotLoggedIn.ToString());

            PretragaDrinks pD = new PretragaDrinks(find);
            pD.KopirajInformacije(iOK);
            MySend(pD);
        }
        public void BeginAddNewDrink(string nameOfTheNewDrink, decimal sizeOfTheNewDrink, decimal priceOfTheNewDrink)
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            if (!iOK._prijavljen)
                throw new Exception(Greske.NotLoggedIn.ToString());

            NovoDrink nD = new NovoDrink(nameOfTheNewDrink, sizeOfTheNewDrink, priceOfTheNewDrink);
            nD.KopirajInformacije(iOK);
            MySend(nD);
        }
        public void BeginListOfAllUsers(long overallPayedMoreThen, long overallPayedLessThen, UnPayedStatus payedStatus, string usersMail, string name, string lastName, string something, short privLevel)
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            if (!iOK._prijavljen)
                throw new Exception(Greske.NotLoggedIn.ToString());

            PopisSvihKorisnika pSK = new PopisSvihKorisnika(overallPayedMoreThen,overallPayedLessThen, payedStatus, usersMail, name, lastName, something, privLevel);
            pSK.KopirajInformacije(iOK);
            MySend(pSK);
        }
        public void BeginAsigneWristbandToUser(string wristbandId, string userEmail)
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            if (!iOK._prijavljen)
                throw new Exception(Greske.NotLoggedIn.ToString());

            NarukvicaNovomKorisniku nNK = new NarukvicaNovomKorisniku(wristbandId, userEmail);
            nNK.KopirajInformacije(iOK);
            MySend(nNK);
        }
        public void BeginUnAsigneWristbandFromUser(string wristbandId)
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            if (!iOK._prijavljen)
                throw new Exception(Greske.NotLoggedIn.ToString());

            NarukvicaIzgublilaKorisnika nIK = new NarukvicaIzgublilaKorisnika(wristbandId);
            nIK.KopirajInformacije(iOK);
            MySend(nIK);
        }
        public void BeginWristbandChangeState(string wristbandId, bool funcional)
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            if (!iOK._prijavljen)
                throw new Exception(Greske.NotLoggedIn.ToString());

            NarukvicaPromjenilaStanje nPS = new NarukvicaPromjenilaStanje(wristbandId, funcional);
            nPS.KopirajInformacije(nPS);
            MySend(nPS);
        }
        public void BeginAddNewWristband(string wristbandId)
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            if (!iOK._prijavljen)
                throw new Exception(Greske.NotLoggedIn.ToString());

            NovaNarukvica nN = new NovaNarukvica(wristbandId);
            nN.KopirajInformacije(iOK);
            MySend(nN);
        }
        public void BeginChangeUsersPrivilageLevel(string userEmail, int newPrivilageLevel)
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            if (!iOK._prijavljen)
                throw new Exception(Greske.NotLoggedIn.ToString());

            PromjeniStupanjPrivilegija pSP = new PromjeniStupanjPrivilegija(userEmail, newPrivilageLevel);
            pSP.KopirajInformacije(iOK);
            MySend(pSP);
        }
        public void BeginDownloadNewestVersion(string curentVersion)
        {
            if (!Connected)
                throw new Exception(Greske.NotConnectedToServer.ToString());
            if (!iOK._prijavljen)
                throw new Exception(Greske.NotLoggedIn.ToString());

            PreuzmiNajnovijuVerziju pNV = new PreuzmiNajnovijuVerziju(curentVersion);
            pNV.KopirajInformacije(iOK);
            MySend(pNV);
        }
        public void PrintDataTable(DataTable dT)
        {
            int c = 0;
            int step = Console.BufferWidth / (dT.Columns.Count);
            Console.WriteLine();
            List<int> width = new List<int>( dT.Columns.Count);
            int left = Console.BufferWidth;
            int i = 0;
            foreach(DataColumn dc in dT.Columns)
            {
                string s = dc.ToString();
                if (s.Length < left)
                {
                    width.Add(s.Length +1);
                    left -= (s.Length + 1);
                }else
                {
                    width.Add(left);
                    left = 0;
                }
                i++;
            }
            
            foreach (DataRow dr in dT.Rows)
            {
                i = 0;
                foreach (object o in dr.ItemArray)
                {
                    int newL = Math.Max(o.ToString().Length + 1, width[i]);
                    left = left - (newL - width[i]) * Convert.ToInt16((0 < (left - newL + width[i])));
                    width[i] = width[i] + (newL - width[i]) * Convert.ToInt16((0 < (left - newL + width[i])));
                    i++;
                }
            }
            i = 0;
            foreach (DataColumn dc in dT.Columns)
            {
                Console.CursorLeft = c;
                Console.Write('|');
                Console.Write(dc);
                c += width[i];
                i++;
            }
            Console.WriteLine();
            int lastValid = 0;
            i = 0;
            for (int j = 0; j < Console.BufferWidth-1; j++)
            {
                if (width.Count>i && width[i] != j-lastValid)
                    Console.Write('-');
                else if (width.Count > i)
                {

                    Console.Write('|');
                    i++;
                    lastValid = j;
                }
            }
            Console.WriteLine();
            foreach (DataRow dR in dT.Rows)
            {
                c = 0;
                i = 0;
                foreach (object obj in dR.ItemArray)
                {
                    Console.CursorLeft = c;
                    Console.Write('|');
                    Console.Write(obj.ToString());
                    c += width[i];
                    i++;
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        public void PrintDataTable(object[][] obj)
        {
            DataTable dt = ObjectArrayToDataTable(obj);
            PrintDataTable(dt);
        }

        protected void _Login(Odgovor od)
        {
            if (od.vg == Odgovor.vrstaGreska.OK)
            {
                iOK.KopirajInformacije(od.Podaci());
            }
            EventHandler<Login> handel = endLogin;
            if (handel != null)
            {
                handel(null, new Login(od.vg.ToString()));
            }
            
        }
        protected void _Register(Odgovor od)
        {
            if (od.vg == Odgovor.vrstaGreska.OK)
            {
                iOK.KopirajInformacije(od.Podaci());
            }
            EventHandler<Register> handel = endRegister;
            if (handel != null)
            {
                handel(null, new Register(od.vg.ToString()));
            }
        }
        protected void _Logout(Odgovor od)
        {
            if (od.vg == Odgovor.vrstaGreska.OK)
            {
                iOK.KopirajInformacije(od.Podaci());
            }
            EventHandler<Logout> handel = endLogout;
            if (handel != null)
            {
                handel(null, new Logout(od.vg.ToString()));
            }
        }
        protected void _NewTransaction(Odgovor od)
        {
            if (od.vg == Odgovor.vrstaGreska.OK)
            {
                iOK.KopirajInformacije(od.Podaci());
            }
            EventHandler<Newtransaction> handel = endNewTransaction;
            if (handel != null)
            {
                handel(null, new Newtransaction(od.vg.ToString()));
            }
        }
        protected void _ListTransactions(Odgovor od)
        {
            object[][] objectArray = null;
            DataTable dt = new DataTable();
            
            if (od.vg == Odgovor.vrstaGreska.OK)
            {
                iOK.KopirajInformacije(od.Podaci());
                objectArray = od.data as object[][];
                dt = ObjectArrayToDataTable(objectArray);
            }

            EventHandler<ListTransactions> handel = endListTransactions;

            if (handel != null)
            {
                handel(null, new ListTransactions(od.vg.ToString(),dt));
            }
        }
        protected void _ListDrinks(Odgovor od)
        {
            object[][] objectArray = null;
            DataTable dt = new DataTable();

            if (od.vg == Odgovor.vrstaGreska.OK)
            {
                iOK.KopirajInformacije(od.Podaci());
                objectArray = od.data as object[][];
                dt = ObjectArrayToDataTable(objectArray);
            }

            EventHandler<ListDrinks> handel = endListDrinks;

            if (handel != null)
            {
                handel(null, new ListDrinks(od.vg.ToString(), dt));
            }
        }
        protected void _UserPayed(Odgovor od)
        {
            Bill objectArray = new Bill(null,0,DateTime.Now,null);
            DataTable dt = new DataTable();

            if (od.vg == Odgovor.vrstaGreska.OK)
            {
                iOK.KopirajInformacije(od.Podaci());
                objectArray = od.data as Bill;
                dt = ObjectArrayToDataTable(objectArray._payedItems);
            }

            EventHandler<UserPayed> handel = endUserPayed;

            if (handel != null)
            {
                handel(od.vg.ToString(), new UserPayed(od.vg.ToString(), dt, objectArray._userPayed, objectArray._dateOfPayment));
            }
        }
        protected void _AddNewDrink(Odgovor od)
        {
            if (od.vg == Odgovor.vrstaGreska.OK)
            {
                iOK.KopirajInformacije(od.Podaci());
            }
            EventHandler<AddNewDrink> handel = endAddNewDrink;
            if (handel != null)
            {
                handel(null, new AddNewDrink(od.vg.ToString()));
            }
        }
        protected void _ListUsers(Odgovor od)
        {
            object[][] objectArray = null;
            DataTable dt = new DataTable();

            if (od.vg == Odgovor.vrstaGreska.OK)
            {
                iOK.KopirajInformacije(od.Podaci());
                objectArray = od.data as object[][];
                dt = ObjectArrayToDataTable(objectArray);
            }

            EventHandler<ListUsers> handel = endListUsers;

            if (handel != null)
            {
                handel(null, new ListUsers(od.vg.ToString(), dt));
            }
        }
        protected void _AssigenWristbandToUser(Odgovor od)
        {
            if (od.vg == Odgovor.vrstaGreska.OK)
            {
                iOK.KopirajInformacije(od.Podaci());
            }
            EventHandler<AssigneWristbandToUser> handel = endAssigneWristbandToUser;
            if (handel != null)
            {
                handel(null, new AssigneWristbandToUser(od.vg.ToString()));
            }
        }
        protected void _UnAssigneWristbandFromUser(Odgovor od)
        {
            if (od.vg == Odgovor.vrstaGreska.OK)
            {
                iOK.KopirajInformacije(od.Podaci());
            }
            EventHandler<UnAssigneWristbandFromUser> handel = endUnAssigneWristbandFromUser;
            if (handel != null)
            {
                handel(null, new UnAssigneWristbandFromUser(od.vg.ToString()));
            }
        }
        protected void _WristbandChangeState(Odgovor od)
        {
            if (od.vg == Odgovor.vrstaGreska.OK)
            {
                iOK.KopirajInformacije(od.Podaci());
            }
            EventHandler<WristbandChangeState> handel = endWristbandChangeState;
            if (handel != null)
            {
                handel(null, new WristbandChangeState(od.vg.ToString()));
            }
        }
        protected void _NewWristband(Odgovor od)
        {
            if (od.vg == Odgovor.vrstaGreska.OK)
            {
                iOK.KopirajInformacije(od.Podaci());
            }
            EventHandler<AddNewWristband> handel = endAddNewWristband;
            if (handel != null)
            {
                handel(null, new AddNewWristband(od.vg.ToString()));
            }
        }
        protected void _ChangeUsersPrivilageLevel(Odgovor od)
        {
            if (od.vg == Odgovor.vrstaGreska.OK)
            {
                iOK.KopirajInformacije(od.Podaci());
            }
            EventHandler<ChangeUserPrivilageLevel> handel = endChangeUserPrivilageLevel;
            if (handel != null)
            {
                handel(null, new ChangeUserPrivilageLevel(od.vg.ToString()));
            }
        }
        protected void _DownloadNewestVersion(Odgovor od)
        {
            byte[] file = null;
            if (od.vg == Odgovor.vrstaGreska.OK)
            {
                iOK.KopirajInformacije(od.Podaci());
                file = od.data as byte[];
            }
            EventHandler<DownloadNewestVersion> handel = endDownloadNewestVersion;
            if (handel != null)
            {
                handel(null, new DownloadNewestVersion(od.vg.ToString(),file));
            }
        }

        private void Phrase(Odgovor od)
        {
            switch (od.poslaniZahtjev){
                case Odgovor.vrstePoslanihZahtjev.Prijava:
                    _Login(od);
                    break;
                case Odgovor.vrstePoslanihZahtjev.Registaracija:
                    _Register(od);
                    break;
                case Odgovor.vrstePoslanihZahtjev.Odjava:
                    _Logout(od);
                    break;
                case Odgovor.vrstePoslanihZahtjev.NovaTransakcija:
                    _NewTransaction(od);
                    break;
                case Odgovor.vrstePoslanihZahtjev.SveTransakcije:
                    _ListTransactions(od);
                    break;
                case Odgovor.vrstePoslanihZahtjev.KorisnikoveTransakcije:
                    _ListTransactions(od);
                    break;
                case Odgovor.vrstePoslanihZahtjev.UserPayed:
                    _UserPayed(od);
                    break;
                case Odgovor.vrstePoslanihZahtjev.NovoDrink:
                    _AddNewDrink(od);
                    break;
                case Odgovor.vrstePoslanihZahtjev.SvaDrinks:
                    _ListDrinks(od);
                    break;
                case Odgovor.vrstePoslanihZahtjev.NedostupnaDrinks:
                    _ListDrinks(od);
                    break;
                case Odgovor.vrstePoslanihZahtjev.DostupnaDrinks:
                    _ListDrinks(od);
                    break;
                case Odgovor.vrstePoslanihZahtjev.PretragaDrinks:
                    _ListDrinks(od);
                    break;
                case Odgovor.vrstePoslanihZahtjev.PopisSvihKorisnika:
                    _ListUsers(od);
                    break;
                case Odgovor.vrstePoslanihZahtjev.NarukvicaNovomKorisniku:
                    _AssigenWristbandToUser(od);
                    break;
                case Odgovor.vrstePoslanihZahtjev.NarukvicaIzgubilKorisnika:
                    _UnAssigneWristbandFromUser(od);
                    break;
                case Odgovor.vrstePoslanihZahtjev.NarukvicaPromjenilaStanje:
                    _WristbandChangeState(od);
                    break;
                case Odgovor.vrstePoslanihZahtjev.NovaNarukvica:
                    _NewWristband(od);
                    break;
                case Odgovor.vrstePoslanihZahtjev.PromjeniStupanjPrivilegija:
                    _ChangeUsersPrivilageLevel(od);
                    break;
                case Odgovor.vrstePoslanihZahtjev.PreuzmiNajnovijuVerziju:
                    _DownloadNewestVersion(od);
                    break;
                default:
                    throw new Exception("!implemented " + od.poslaniZahtjev);
            }
        }

        public enum Greske
        {
            NotConnectedToServer,
            NotLoggedIn,
            LoggedInCanNotRegister
        }

        public void TryConnect(string ip, int port)
        {
            me = new TcpClient(AddressFamily.InterNetwork);
            this.ip = ip;
            pendingConnection = true;
            me.BeginConnect(ip, port, OnBeginConnect, me);

        }

        protected async Task KeepConnecthing(string ip, int port)
        {
            while (true)
            {
                if (!Connected && !pendingConnection)
                    TryConnect(ip, port);
                await Task.Delay(500);
            }
        }
        protected void OnDC()
        {
            if (_connected)
                ConnectionChangedState?.Invoke(null, false);
            pendingConnection = false;
            _connected = false;
            wholeMess = new List<byte>();
            wholeSize = 0;

        }
        protected void CallPickUP(object obj, string error)
        {
            EventHandler<NewMessageEventArgs> handel = somethingToPickUp;
            if (handel != null)
            {
                handel(null, new NewMessageEventArgs(obj, error));
            }
        }
        protected async void MySend(object obj)
        {
            byte[] buff = await cripto.Encode(obj);
            strm.BeginWrite(buff, 0, buff.Length, OnSendComplete, strm);
        }
        protected DataTable ObjectArrayToDataTable(object[][] obj)
        {
            DataTable dt = new DataTable();
            dt.BeginLoadData();
            
            for(int i=0;i<obj.Length;i++)
            {
                if (i == 0)
                {
                    foreach (object o in obj[0])
                    {
                        dt.Columns.Add(o.ToString());
                    }
                }
                else {

                    try
                    {
                        dt.Rows.Add(obj[i]);
                    }
                    catch (Exception ex) { Console.WriteLine(ex); }
                }
                
            }
            dt.EndLoadData();
            return dt;
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None || true)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }
        private void OnBeginConnect(IAsyncResult ar)
        {
            try
            {
                TcpClient tc = ar.AsyncState as TcpClient;

                tc.EndConnect(ar);
                strm = new SslStream(tc.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                try
                {
                    strm.BeginAuthenticateAsClient(ip, OnAuthenticate, strm);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    OnDC();

                }
            }
            catch {
                OnDC();
            }
        }
        private void OnAuthenticate(IAsyncResult ar)
        {
            SslStream at = ar.AsyncState as SslStream;

            try
            {
                at.EndAuthenticateAsClient(ar);
                _connected = true;
                pendingConnection = false;
                ConnectionChangedState?.Invoke(null, true);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            try
            {
                BUFFER = new byte[2 << 10];
                at.BeginRead(BUFFER, 0, 2 << 10, OnRecive, at);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                OnDC();

            }
        }
        private void OnSendComplete(IAsyncResult ar)
        {
            SslStream snd = ar.AsyncState as SslStream;
            try
            {
                snd.EndWrite(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private void OnRecive(IAsyncResult ar)
        {

            SslStream secureSocket = ar.AsyncState as SslStream;
            try
            {
                
                Odgovor odg;
                int lenght = secureSocket.EndRead(ar);
            
                wholeMess.InsertRange(wholeMess.Count, BUFFER);
                wholeSize += lenght;
                wholeMess.RemoveRange(wholeSize, wholeMess.Count - wholeSize);
                try
                {
                    odg = cripto.Decode(wholeMess) as Odgovor;
                    wholeMess = new List<byte>();
                    wholeSize = 0;
                    Phrase(odg);    
                }
                catch { }

                
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                OnDC();
            }
            try
            {
                secureSocket.BeginRead(BUFFER, 0, 2 << 10, OnRecive, secureSocket);
            }catch(Exception ex)
            {
                OnDC();
            } 
        }
    }
}

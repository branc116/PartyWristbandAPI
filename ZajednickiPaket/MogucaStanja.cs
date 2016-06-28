using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Security;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
namespace ZajednickiPaket
{
    [Serializable]
    public enum Dostupnost
    {
        Dostupno,
        NeDostupno,
        Nebitno
    }
    [Serializable]
    public enum Placeno
    {
        Placeno,
        Neplaceno,
        Nebitno
    }
    public enum UnPayedStatus
    {
        AllPayd,
        SomePayed,
        NothingPayed,
        MattersNot
    }


    public enum VrstaKorisnika
    {
        anonimniKorisnik,
        kupac,
        konobar,
        gazda,
        admin
    }
    public class Cripto
    {
        private DataContractJsonSerializer dcj;
        private BinaryFormatter bf;

        public Cripto()
        {
            Type a1 = typeof(Prijava);
            Type a2 = typeof(DostupnaDrinks);
            dcj = new DataContractJsonSerializer(typeof(Prijava), new Type[3] { a1,a2, typeof(string)});
            bf = new BinaryFormatter();
        }
        public async Task<byte[]> Encode(object file)
        {


            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, file);
            byte[] b = ms.GetBuffer();
            
            //Console.WriteLine(Encoding.ASCII.GetChars(buff.ToArray()));
            List<byte> buff = b.ToList();
            buff.AddRange(Encoding.ASCII.GetBytes("<EOF>"));
            return buff.ToArray();
        }
        public object Decode(byte[] buff)
        {
            List<byte> buf = buff.ToList();
            //Console.WriteLine("size = " + buf.Count + "\n" + Encoding.ASCII.GetString(buff));
            if (buf.Count < 5 || Encoding.ASCII.GetChars(buf.GetRange(buf.Count - 5, 5).ToArray()).Intersect("<EOF>".ToCharArray()).Count<char>() == 0)
            {
                throw new Exception("not eof");
            }

            MemoryStream ms = new MemoryStream(buff);

            //return dcj.ReadObject(ms);
            return bf.Deserialize(ms);
        }
        public object Decode(List<byte> buff)
        {
            return Decode(buff.ToArray());
        }
    }
    [Serializable]
    public class InformacijeOKorisniku
    {
        public string _sesionId, _mail;
        public bool _prijavljen;
        public VrstaKorisnika vK;
        public void PopuniInformacije(
            string sesionId,
            string mail,
            bool prijavljen,
            VrstaKorisnika vK
            )
        {
            _sesionId = sesionId;
            _mail = mail;
            _prijavljen = prijavljen;
            this.vK = vK;
        }
        public void KopirajInformacije(
            InformacijeOKorisniku iok
            )
        {
            this._sesionId = iok._sesionId;
            this._mail = iok._mail;
            this._prijavljen = iok._prijavljen;
            this.vK = iok.vK;
        }
        public InformacijeOKorisniku Podaci()
        {
            return this;
        }
       
    }
    [Serializable]
    public class Prijava : InformacijeOKorisniku
    {

        public string lozinka;

        public Prijava(string mail, string password)
        {
            _mail = mail;
            _sesionId = lozinka = password;
        }
    }
    [Serializable]
    public class Registracija : InformacijeOKorisniku
    {
        public string _ime, _prezime, _pass, _oib, _brojMoba, _adresa;
        public byte[] _slika;
        public Registracija(
            string ime,
            string prezime,
            string mail,
            string pass,
            byte[] slika,
            string oib,
            string brojMoba,
            string adresa
            )
        {
            _ime = ime;
            _prezime = prezime;
            _sesionId = _pass   = pass;
            _mail = mail;
            _slika = slika;
            _oib = oib;
            _brojMoba = brojMoba;
            _adresa = adresa;
        }
    }
    [Serializable]
    public class Odjava : InformacijeOKorisniku
    {
    }
    [Serializable]
    public class NovaTransakcija : InformacijeOKorisniku
    {
        public string _emailKupca, _drink;
        public decimal _kolicina;
        public int _n;
        public NovaTransakcija(
            string emailKupca,
            string drink,
            decimal kolicina,
            int n
            )
        {
            _emailKupca = emailKupca;
            _drink = drink;
            _kolicina = kolicina;
            _n = n;
        }
    }
    [Serializable]
    public class KrivoUnesenaTransakcija : InformacijeOKorisniku
    {
        string _iDTransakcije;
        public KrivoUnesenaTransakcija(
            string idTransakcije
            )
        {
            _iDTransakcije = idTransakcije;

        }
    }
    [Serializable]
    public class SveTransakcije : InformacijeOKorisniku
    {

    }
    [Serializable]
    public class KorisnikoveTransakcije : InformacijeOKorisniku
    {
        public string _emailKupca;
        public KorisnikoveTransakcije(

            string emailKupca

            )
        {
            _emailKupca = emailKupca;
        }
    }
    [Serializable]
    public class Transakcije : InformacijeOKorisniku
    {
        public string _id, _usersMail;
        public Placeno _placeno;
        public Transakcije(string id)
        {
            _id = id;
            _usersMail = null;
            _placeno = Placeno.Nebitno;
        }
        public Transakcije(string usersMail, Placeno placeno)
        {
            _placeno = placeno;
            _usersMail = usersMail;
            _id = null;
        }
        public Transakcije(Placeno placeno)
        {
            _placeno = placeno;
            _usersMail = null;
            _id = null;
        }
        public Transakcije()
        {
            _placeno = Placeno.Nebitno;
            _usersMail = null;
            _id = null;
        }
        public Transakcije(string id, string userMail, Placeno p)
        {
            _id = id;
            _usersMail = userMail;
            _placeno = p;
        }
    }
    [Serializable]
    public class UserPayedd : InformacijeOKorisniku
    {
        public int _amount;
        public string _usersMail;
        public bool _all;
        public UserPayedd(string usersMail)
        {
            _amount = 0;
            _usersMail = usersMail;
            _all = true;
        }
        public UserPayedd(string userMail, int amount)
        {
            _amount = amount;
            _usersMail = userMail;
            _all = false;
        }
        public UserPayedd(string userMail, int amount, bool all)
        {
            _amount = amount;
            _all = all;
            _usersMail = userMail; 
        }
    }

    [Serializable]
    public class SvaDrinks : InformacijeOKorisniku
    {

    }
    [Serializable]
    public class DostupnaDrinks : InformacijeOKorisniku
    {
        public DostupnaDrinks() { }
    }
    [Serializable]
    public class NedostupnaDrinks : InformacijeOKorisniku
    {
        public NedostupnaDrinks() { }
    }
    [Serializable]
    public class PretragaDrinks : InformacijeOKorisniku
    {
        public string _atributPretrage;
        public PretragaDrinks(
            string atributPretrage
            )
        {
            _atributPretrage = atributPretrage;
        }
    }
    [Serializable]
    public class NovoDrink :InformacijeOKorisniku
    {
        string ime;
        decimal kolicina;
        decimal cijena;
        public NovoDrink
            (
                string _ime,
                decimal _size,
                decimal _cijena
            )
        {
            ime = _ime;
            kolicina = _size;
            cijena = _cijena;
        }
    }
    [Serializable]
    public class Drinks : InformacijeOKorisniku
    {
        string id, name;
        Dostupnost dostupnost;

        public Drinks(string Name, Dostupnost d) {
            id = null;
            name = Name;
            dostupnost = d;
        }
        
        public Drinks(Dostupnost d)
        {
            dostupnost = d;
            id = null;
            name = null;
        }
        public Drinks(string Id)
        {
            name = null;
            id = Id;
            dostupnost = Dostupnost.Nebitno;
        }
    }

    [Serializable]
    public class PopisSvihKorisnika : InformacijeOKorisniku
    {
        public long _overallPayedMoreThen, _overallPayedLessThen;
        public UnPayedStatus _UnPayed;
        public string _usersMail, _name, _lastName, _something;
        public short _privLevel;
        public PopisSvihKorisnika(long overallPayedMoreThen, long overallPayedLessThen, UnPayedStatus UnPayed, string usersMail, string name, string lastName, string something, short privLevel)
        {
            _overallPayedLessThen = overallPayedLessThen;
            _overallPayedMoreThen = overallPayedMoreThen;
            _UnPayed = UnPayed;
            _usersMail = usersMail;
            _name = name;
            _lastName = lastName;
            _something = something;
            _privLevel = privLevel; 
        }
        public PopisSvihKorisnika()
        {
            _overallPayedLessThen = -999999999;
            _overallPayedMoreThen = 999999999;
            _UnPayed = UnPayedStatus.MattersNot;
            _usersMail = null;
            _name = null;
            _lastName = null;
            _something = null;
            _privLevel = -3;
        }
    }
    [Serializable]
    public class NarukvicaNovomKorisniku : InformacijeOKorisniku
    {
        public string _idNarukvice, _mailKorisnika;
        public NarukvicaNovomKorisniku(
            string idNarukvice,
            string mailKorisnika
            )
        {
            _idNarukvice = idNarukvice;
            _mailKorisnika = mailKorisnika;

        }
    }
    [Serializable]
    public class NarukvicaIzgublilaKorisnika : InformacijeOKorisniku
    {
        public string _idNarukvice;
        public NarukvicaIzgublilaKorisnika(
            string idNarukvice
            )
        {
            _idNarukvice = idNarukvice;
        }
    }
    [Serializable]
    public class NarukvicaPromjenilaStanje : InformacijeOKorisniku
    {
        public string _idNarukvice;
        public bool _novoStanje;
        public NarukvicaPromjenilaStanje(
            string idNarukvice,
            bool novoStanje
            )
        {
            _idNarukvice = idNarukvice;
            _novoStanje = novoStanje;
        }
    }
    [Serializable]
    public class NovaNarukvica : InformacijeOKorisniku
    {
        public string _idNarukvice;
        public NovaNarukvica(
            string idNarukvice
            )
        {
            _idNarukvice = idNarukvice;
        }
    }
    [Serializable]
    public class PromjeniStupanjPrivilegija : InformacijeOKorisniku
    {
        string _mailKorisnika;
        public int _noviStupanjPrivilegija;
        public PromjeniStupanjPrivilegija(
                string mailKorisnika,
                int noviStupanjPrivilegija

            )
        {
            _mailKorisnika = mailKorisnika;
            _noviStupanjPrivilegija = noviStupanjPrivilegija;
        }
    }
    [Serializable]
    public class PreuzmiNajnovijuVerziju : InformacijeOKorisniku
    {
        public string _trenutnaVerzija;
        public PreuzmiNajnovijuVerziju(
                string trenutnaVerzija
            )
        {
            _trenutnaVerzija = trenutnaVerzija;
        }

    }
    [Serializable]
    public class Odgovor : InformacijeOKorisniku
    {
        public enum vrstePoslanihZahtjev
        {
            Prijava,
            Registaracija,
            Odjava,
            NovaTransakcija,
            KrivoUnesenaTransakcija,
            SveTransakcije,
            KorisnikoveTransakcije,
            UserPayed,
            SvaDrinks,
            NovoDrink,
            DostupnaDrinks,
            NedostupnaDrinks,
            PretragaDrinks,
            PopisSvihKorisnika,
            NarukvicaNovomKorisniku,
            NarukvicaIzgubilKorisnika,
            NarukvicaPromjenilaStanje,
            NovaNarukvica,
            PromjeniStupanjPrivilegija,
            PreuzmiNajnovijuVerziju,
            NistGreska
        }
        public enum vrstaGreska
        {
            OK,
            BadPassword,
            BadEmail,
            LowLevel,
            BadAmount,
            RegisterEmailInUse,
            BadDrink,
            BadLogin,
            BadOib,
            Default
        }

        public vrstePoslanihZahtjev poslaniZahtjev;
        public string greske;
        public vrstaGreska vg;
        public object data;
        public Type T;



        public Odgovor(
            Type ty,
            object obj,
            string gr,
            string sesId,
            vrstePoslanihZahtjev zahtjev
            )
        {
            data = obj;
            greske = gr;
            T = ty;
            _sesionId = sesId;
            poslaniZahtjev = zahtjev;
        }
        public Odgovor(
            object obj,
            string greska,
            vrstePoslanihZahtjev zahtjev)
        {
            T = obj.GetType();
            data = obj;
            poslaniZahtjev = zahtjev;
            greske = greska;
        }
        public Odgovor(
            object obj,
            string greska
            )
        {
            T = obj.GetType();
            data = obj;
            greske = greska;
        }
        public Odgovor(
            object obj,
            vrstaGreska _vg
            )
        {
            data = obj;

            T = obj.GetType();

            vg = _vg;
        }
        
    
      

    }
    [Serializable]
    public class Bill
    {
        public object[][] _payedItems;
        public long _amountPayed;
        public DateTime _dateOfPayment;
        public string _userPayed;
        public Bill(object[][] payedItems, long amount, DateTime dateOfPayment, string userPayed)
        {
            _payedItems = payedItems;
            _amountPayed = amount;
            _dateOfPayment = dateOfPayment;
            _userPayed = userPayed;
        }
        public Bill(Bill b)
        {
            _payedItems = b._payedItems;
            _amountPayed = b._amountPayed;
            _dateOfPayment = b._dateOfPayment;
            _userPayed = b._userPayed;
        }
    }
}

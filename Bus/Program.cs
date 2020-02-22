using System;
using System.Linq;
using System.IO;
using System.Globalization;

namespace Bus // Aplikacija za evidenciju i prodaju autobusnih karata - BUS
{
    /*      Aplikacija preko Glavnog izbornika nudi mogucnost biranje opcija: 1) Prodaja karata; 2) Evidencija prodanih karata; 3) Unos/izmjena karata
     * 1)   Korisnik preko izbornika Prodaja ima opciju biranja prodaje autobusnih karata po gradskim zonama (1-4 zone), ispis racuna te izvjestaje u tekstualne datoteke  
     * 2)   Evidencija prodanih karata omogucava prikaz statistike prodanih karata sa sortiranjem i filtriranjem te prikaz prodaje za odredjeni vremenski interval                                                                 
     * 3)   Korisnik preko izbornika Unos/izmjena moze unositi, izmjenjivati, brisati i pregledavati karte putem raznih izbornika te spremati podatke u tekstualne datoteke */

    public struct Karta
    {
        public string zona;
        public string vrsta;
        public double cijena;

        public Karta(string _zona, string _vrsta, double _cijena)
        {
            this.zona = _zona;
            this.vrsta = _vrsta;
            this.cijena = _cijena;
        }
    }

    public struct ProdanaKarta
    {
        public string zona;
        public string vrsta;
        public double cijenaKarte;
        public int kolicina;
        public double ukupnaCijena;
        public int id;
        public DateTime datum;
        public string vrijeme;

        public ProdanaKarta(string _zona, string _vrsta, double _cijena, int _broj, double _racun, int _id, DateTime _datum, string _vrijeme)
        {
            this.zona = _zona;
            this.vrsta = _vrsta;
            this.cijenaKarte = _cijena;
            this.kolicina = _broj;
            this.ukupnaCijena = _racun;
            this.id = _id;
            this.datum = _datum;
            this.vrijeme = _vrijeme;
        }
    }

    public struct NizKarata // Autobusne karte rasporedjene po gradskim zonama (1-4)
    {
        public Karta[] prvaZona;
        public Karta[] drugaZona;
        public Karta[] trecaZona;
        public Karta[] cetvrtaZona;
    }


    class Program
    {
        static NizKarata niz;
        static ProdanaKarta[] nizProdaja;

        public static void Izbornik() // Glavni izbornik
        {
            int unos;

            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n             GLAVNI IZBORNIK\n---------------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("1 - Prodaja autobusnih karata\n" +
                                  "2 - Pregled prodaje autobusnih karata\n" +
                                  "3 - Unos/izmjena autobusnih karata\n\n" +
                                  "0 - Izlaz");
                Datum();
                Console.ForegroundColor = ConsoleColor.White;
            } while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out unos) || unos < 0 || unos > 3);

            Console.Clear();

            switch (unos)
            {
                case 1:
                    Prodaja_Izbornik();
                    break;
                case 2:
                    Evidencija();
                    break;
                case 3:
                    Unos_izmjena();
                    break;
                case 0:
                    Izlaz();
                    break;
            }
        }

        public static void Prodaja_Izbornik() // Izbornik za prodaju karata
        {
            UcitajProdaju();

            int unos;

            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nIZBORNIK PRODAJE AUTOBUSNIH KARATA PO ZONAMA\n--------------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" Za koju gradsku zonu zelite prodati karte:\n--------------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("1 - Zona \"1\"\n" +
                                  "2 - Zona \"2\"\n" +
                                  "3 - Zona \"3\"\n" +
                                  "4 - Zona \"4\"\n" +
                                  "\n5 - Natrag na Glavni izbornik\n" +
                                  "\n0 - Izlaz");
                Datum();
                Console.ForegroundColor = ConsoleColor.White;
            } while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out unos) || unos > 5 || unos < 0);

            UcitajKarte();

            switch (unos)
            {
                case 1:
                    Prodaja_Karata("1");
                    break;
                case 2:
                    Prodaja_Karata("2");
                    break;
                case 3:
                    Prodaja_Karata("3");
                    break;
                case 4:
                    Prodaja_Karata("4");
                    break;
                case 5:
                    Izbornik();
                    break;
                case 0:
                    Izlaz();
                    break;
            }
        }

        public static void IspisZone(Karta[] zona) // Ispis karata po zoni
        {

            for (int i = 0; i < zona.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(i + 1 + " - " + zona[i].vrsta + ": " + zona[i].cijena + " kn");
            }
        }

        public static void PregledKarataPoZonama() // Ispis karata po svim zonama
        {
            int unos;

            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nPREGLED SVIH VRSTA KARATA PO ZONAMA\n-----------------------------------");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\n  1. zona\n===========");
                Console.ForegroundColor = ConsoleColor.Gray;
                IspisZone(niz.prvaZona);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\n  2. zona\n=========");
                Console.ForegroundColor = ConsoleColor.Yellow;
                IspisZone(niz.drugaZona);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\n  3. zona\n===========");
                Console.ForegroundColor = ConsoleColor.Yellow;
                IspisZone(niz.trecaZona);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\n  4. zona\n===========");
                Console.ForegroundColor = ConsoleColor.Yellow;
                IspisZone(niz.cetvrtaZona);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n\n-----------------------------\n" +
                                  "\n1 - Natrag na Unos/izmjena" +
                                  "\n2 - Natrag na Glavni izbornik\n" +
                                  "\n0 - Izlaz");
                Datum();
                Console.ForegroundColor = ConsoleColor.White;
            } while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out unos) || unos < 0 || unos > 2);

            if (unos == 1)
            {
                Unos_izmjena();
            }
            else if (unos == 2)
            {
                Izbornik();
            }
            else
            {
                Izlaz();
            }
        }

        public static void Prodaja_Karata(string zona) // Izbornik za prodaju karata >> Ispis u teksutalne datoteke "racun.txt" i "prodaneKarte.txt"
        {
            int unos, brojKarata;
            double racun;

            Karta[] trenutnaZona = new Karta[0];

            switch (zona)
            {
                case "1":
                    trenutnaZona = niz.prvaZona;
                    break;
                case "2":
                    trenutnaZona = niz.drugaZona;
                    break;
                case "3":
                    trenutnaZona = niz.trecaZona;
                    break;
                case "4":
                    trenutnaZona = niz.cetvrtaZona;
                    break;
            }

        prodajniIzbornik:
            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n       IZBORNIK PRODAJNIH VRSTA KARATA\n---------------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("   Unesite vrstu karte koju zelite prodati:\n---------------------------------------------\n");
                IspisZone(trenutnaZona);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n9 - Natrag na Glavni izbornik\n" +
                                  "\n0 - Izlaz");
                Datum();
                Console.ForegroundColor = ConsoleColor.White;
            } while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out unos));

            if (unos < trenutnaZona.Length + 1 && unos >= 1)
            {
            prodajaKolicina:
                do
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n                PRODAJA KARATA \n------------------------------------------------\n");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Unesite broj karata koji zelite prodati: (1-100) \n------------------------------------------------\n");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\n\n\n\n\n\n\n0 - Natrag na glavni izbornik");
                    Datum();
                    Console.ForegroundColor = ConsoleColor.White;
                } while (!int.TryParse(Console.ReadLine(), out brojKarata) || brojKarata < 0 || brojKarata > 100);

                if (brojKarata == 0)
                {
                    Izbornik();
                }
                else if (brojKarata < 0 || brojKarata > 100)
                {
                    goto prodajaKolicina;
                }
                else
                {
                    racun = brojKarata * trenutnaZona[unos - 1].cijena;

                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n           NAPLATA KARTE\n-------------------------------------\n");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(" Ukupan iznos za naplatiti: {0},00 kn", racun);
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("\n-------------------------------------\n        Broj racuna: " + (nizProdaja.Length + 1) + "/2020" +
                        "\n\n           PRODANA KARTA\n-------------------------------------\n\n" + "Zona: " + trenutnaZona[unos - 1].zona + "\nVrsta karte: " +
                        trenutnaZona[unos - 1].vrsta + "\nCijena karte: " + trenutnaZona[unos - 1].cijena + ",00 kn\nKolicina: " + brojKarata + "\nNacin placanja: Gotovina");

                    using (StreamWriter sw = new StreamWriter("racun.txt"))
                    {
                        sw.WriteLine("  BUS d.o.o. - Poslovnica Aspira\n Domovinskog rata 65, 21000 Split\n----------------------------------" +
                            "\nDatum racuna: " + DateTime.Now.AddMinutes(00).ToString("dd.MM.yyyy. HH:mm:ss") + "\nBroj racuna: " + (nizProdaja.Length + 1) + "/2020" +
                            "\n\n     POTVRDA O CIJENI KARTE\n----------------------------------\nVrsta karte:               Zona: " + trenutnaZona[unos - 1].zona +
                            "\n----------------------------------\n" + trenutnaZona[unos - 1].vrsta + "\nCijena karte: " + trenutnaZona[unos - 1].cijena + ",00 KN\nKolicina: " +
                            brojKarata + " kom\n\nUKUPNO ZA NAPLATU: " + racun + ",00 KN\nNacin placanja: Gotovina");
                    }
                    Datum();

                    using (StreamWriter sw = File.AppendText("prodaneKarte.txt"))
                    {
                        sw.WriteLine(trenutnaZona[unos - 1].zona + "-" + trenutnaZona[unos - 1].vrsta + "-" + trenutnaZona[unos - 1].cijena + "-" + brojKarata + "-"
                       + racun + "-" + (nizProdaja.Length + 1) + "-" + DateTime.Now.AddMinutes(00).ToString("MM.dd.yyyy-HH:mm:ss"));
                    }
                    Console.ReadKey();
                    Izbornik();
                }
            }
            else if (unos == 9)
            {
                Izbornik();
            }
            else if (unos == 0)
            {
                Izlaz();
            }
            else goto prodajniIzbornik;
        }

        public static void UcitajKarte() // Ucitava podatke io kartama iz teksutalne datoteke "karte.txt"
        {
            NizKarata nizz = new NizKarata();
            Karta[] NizKarata;

            if (File.Exists("karte.txt"))
            {
                int brojLinija = File.ReadLines("karte.txt").Count();
                NizKarata = new Karta[brojLinija];
                int brojac = 0;
                using (StreamReader sr = File.OpenText("karte.txt"))
                {
                    string linija = "";
                    string[] temp;
                    while ((linija = sr.ReadLine()) != null)
                    {
                        temp = linija.Split('-');
                        NizKarata[brojac].zona = temp[0];
                        NizKarata[brojac].vrsta = temp[1];
                        NizKarata[brojac].cijena = float.Parse(temp[2]);
                        brojac++;
                    }
                }

                for (int i = 0; i <= 4; i++)
                {
                    brojac = 0;
                    for (int j = 0; j < NizKarata.Length; j++)
                    {
                        if (NizKarata[j].zona == i.ToString())
                        {
                            brojac++;
                        }
                    }
                    switch (i)
                    {
                        case 1:
                            nizz.prvaZona = new Karta[brojac];
                            break;
                        case 2:
                            nizz.drugaZona = new Karta[brojac];
                            break;
                        case 3:
                            nizz.trecaZona = new Karta[brojac];
                            break;
                        case 4:
                            nizz.cetvrtaZona = new Karta[brojac];
                            break;
                    }
                }

                int[] nizBrojaca = Enumerable.Repeat(0, 4).ToArray();

                for (int i = 0; i < NizKarata.Length; i++)
                {
                    switch (int.Parse(NizKarata[i].zona))
                    {
                        case 1:
                            nizz.prvaZona[nizBrojaca[0]] = NizKarata[i];
                            nizBrojaca[0]++;
                            break;
                        case 2:
                            nizz.drugaZona[nizBrojaca[1]] = NizKarata[i];
                            nizBrojaca[1]++;
                            break;
                        case 3:
                            nizz.trecaZona[nizBrojaca[2]] = NizKarata[i];
                            nizBrojaca[2]++;
                            break;
                        case 4:
                            nizz.cetvrtaZona[nizBrojaca[3]] = NizKarata[i];
                            nizBrojaca[3]++;
                            break;
                    }
                }
                niz = nizz;
            }
            else
            {
                using (StreamWriter sw = File.AppendText("karte.txt"))
                {
                    sw.WriteLine("1-Pojedinacna karta za jedno putovanje-11\n2-Pojedinacna karta za jedno putovanje-13\n3-Pojedinacna karta za jedno putovanje-17\n4-Pojedinacna karta za jedno putovanje-21\n" +
                                 "1-Pojedinacna karta za dva putovanja-18\n2-Pojedinacna karta za dva putovanja-22\n3-Pojedinacna karta za dva putovanja-27\n4-Pojedinacna karta za dva putovanja-33\n" +
                                 "1-Karta za djecu (od 6 do 10 godina)-5\n2-Karta za djecu (od 6 do 12 godina)-5\n3-Karta za djecu (od 6 do 12 godina)-5\n4-Karta za djecu (od 6 do 12 godina)-5\n" +
                                 "1-Mjesecna pokazna karta za gradjane-290\n2-Mjesecna pokazna karta za gradjane-380\n3-Mjesecna pokazna karta za gradjane-460\n4-Mjesecna pokazna karta za gradjane-570\n" +
                                 "1-Mjesecna pokazna karta za ucenike i studente-130\n2-Mjesecna pokazna karta za ucenike i studente-190\n3-Mjesecna pokazna karta za ucenike i studente-230\n4-Mjesecna pokazna karta za ucenike i studente-265\n" +
                                 "1-Mjesecna pokazna karta za umirovljenike i nezaposlene-143\n2-Mjesecna pokazna karta za umirovljenike i nezaposlene-200\n3-Mjesecna pokazna karta za umirovljenike i nezaposlene-240\n4-Mjesecna pokazna karta za umirovljenike i nezaposlene-280");
                }
            }
        }

        public static void UcitajProdaju() // Ucitava podatke o prodanim kartama iz tekstualne datoteke prodaneKarte.txt
        {
            ProdanaKarta[] prodajaKarata = new ProdanaKarta[0];

            if (File.Exists("prodaneKarte.txt"))
            {
                int brojLinija = File.ReadLines("prodaneKarte.txt").Count();
                prodajaKarata = new ProdanaKarta[brojLinija];
                using (StreamReader sr = File.OpenText("prodaneKarte.txt"))
                {
                    string linija = "";
                    string[] temp;
                    int brojac = 0;

                    while ((linija = sr.ReadLine()) != null)
                    {
                        temp = linija.Split('-');
                        prodajaKarata[brojac].zona = temp[0];
                        prodajaKarata[brojac].vrsta = temp[1];
                        prodajaKarata[brojac].cijenaKarte = double.Parse(temp[2]);
                        prodajaKarata[brojac].kolicina = int.Parse(temp[3]);
                        prodajaKarata[brojac].ukupnaCijena = double.Parse(temp[4]);
                        prodajaKarata[brojac].id = int.Parse(temp[5]);
                        prodajaKarata[brojac].datum = DateTime.ParseExact(temp[6], "MM.dd.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        prodajaKarata[brojac].vrijeme = temp[7];
                        brojac++;
                    }
                }
                nizProdaja = prodajaKarata;
            }
            else
            {
                using (StreamWriter sw = File.AppendText("prodaneKarte.txt"))
                {

                }
            }
        }

        public static void SortiranjeProdanihKarata(string type, int brojac) // Sortiranje podataka o prodaji karata
        {
            switch (type)
            {
                case "zona":
                    if (brojac % 2 == 0)
                    {
                        Array.Sort(nizProdaja, (x, y) => String.Compare(x.zona, y.zona));
                    }
                    else
                    {
                        Array.Sort(nizProdaja, (x, y) => String.Compare(y.zona, x.zona));
                    }
                    break;
                case "vrsta":
                    if (brojac % 2 == 0)
                    {
                        Array.Sort(nizProdaja, (x, y) => String.Compare(x.vrsta, y.vrsta));
                    }
                    else
                    {
                        Array.Sort(nizProdaja, (x, y) => String.Compare(y.vrsta, x.vrsta));
                    }
                    break;
                case "cijena":
                    if (brojac % 2 == 0)
                    {
                        nizProdaja = nizProdaja.OrderBy(item => item.cijenaKarte).ToArray();
                    }
                    else
                    {
                        nizProdaja = nizProdaja.OrderByDescending(item => item.cijenaKarte).ToArray();
                    }
                    break;
                case "id":
                    if (brojac % 2 == 0)
                    {
                        nizProdaja = nizProdaja.OrderBy(item => item.id).ToArray();
                    }
                    else
                    {
                        nizProdaja = nizProdaja.OrderByDescending(item => item.id).ToArray();
                    }
                    break;
                case "vrijeme":
                    if (brojac % 2 == 0)
                    {
                        Array.Sort(nizProdaja, (x, y) => String.Compare(x.vrijeme, y.vrijeme));
                    }
                    else
                    {
                        Array.Sort(nizProdaja, (x, y) => String.Compare(y.vrijeme, x.vrijeme));
                    }
                    break;
                case "datum":
                    if (brojac % 2 == 0)
                    {
                        Array.Sort(nizProdaja, (x, y) => DateTime.Compare(x.datum, y.datum));
                    }
                    else
                    {
                        Array.Sort(nizProdaja, (x, y) => DateTime.Compare(y.datum, x.datum));
                    }
                    break;
                case "ukupnaCijena":
                    if (brojac % 2 == 0)
                    {
                        nizProdaja = nizProdaja.OrderBy(item => item.ukupnaCijena).ToArray();
                    }
                    else
                    {
                        nizProdaja = nizProdaja.OrderByDescending(item => item.ukupnaCijena).ToArray();
                    }
                    break;
            }
        }

        public static void FiltriranjeProdanihKarata(string type) // Filtriranje podataka o prodaji karata
        {
            switch (type)
            {
                case "vrsta":
                    for (int i = 0; i < nizProdaja.Length; i++)
                    {
                        Console.WriteLine(i + 1 + ". | Zona: " + nizProdaja[i].zona + " | Vrsta: " + nizProdaja[i].vrsta);
                    }
                    break;
                case "kolicina":
                    for (int i = 0; i < nizProdaja.Length; i++)
                    {
                        Console.WriteLine(i + 1 + ". | Zona: " + nizProdaja[i].zona + " | Vrsta: " + nizProdaja[i].vrsta + " | Kolicina: " + nizProdaja[i].kolicina + " kom");
                    }
                    break;
                case "cijena":
                    for (int i = 0; i < nizProdaja.Length; i++)
                    {
                        Console.WriteLine(i + 1 + ". | Zona: " + nizProdaja[i].zona + " | Vrsta: " + nizProdaja[i].vrsta + " | Cijena: " + nizProdaja[i].cijenaKarte + " kn");
                    }
                    break;
                case "id":
                    for (int i = 0; i < nizProdaja.Length; i++)
                    {
                        Console.WriteLine(i + 1 + " | Zona: " + nizProdaja[i].zona + " | Vrsta: " + nizProdaja[i].vrsta + " | Broj racuna: " + nizProdaja[i].id);
                    }
                    break;
                case "vrijeme":
                    for (int i = 0; i < nizProdaja.Length; i++)
                    {
                        Console.WriteLine(i + 1 + ". | Zona: " + nizProdaja[i].zona + " | Vrsta: " + nizProdaja[i].vrsta + " | Vrijeme: " + nizProdaja[i].vrijeme);
                    }
                    break;
                case "datum":
                    for (int i = 0; i < nizProdaja.Length; i++)
                    {
                        Console.WriteLine(i + 1 + ". | Zona: " + nizProdaja[i].zona + " | Vrsta: " + nizProdaja[i].vrsta + " | Datum: " + nizProdaja[i].datum.ToString("dd.MM.yyyy"));
                    }
                    break;
                case "ukupnaCijena":
                    for (int i = 0; i < nizProdaja.Length; i++)
                    {
                        Console.WriteLine(i + 1 + ". | Zona: " + nizProdaja[i].zona + " | Vrsta: " + nizProdaja[i].vrsta + " | Iznos racuna: " + nizProdaja[i].ukupnaCijena + " kn");
                    }
                    break;
            }
        }
        public static void PregledProdanihPoDatumu() // Pregled podataka o prodaji prema odredjenom vremenskom razdoblju
        {
            string pocetniDatumString, zavrsniDatumString;
            DateTime pocetniDatum, zavrsniDatum;

            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n            PRETRAGA PO DATUMU\n--------------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" Unesite pocetni datum: (dan.mjesec.godina)");
                Datum();
                Console.ForegroundColor = ConsoleColor.White;
                pocetniDatumString = Console.ReadLine();
            } while (!DateTime.TryParseExact(pocetniDatumString, "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out pocetniDatum)); Datum();

            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n             PRETRAGA PO DATUMU\n--------------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" Unesite zavrsni datum: (dan.mjesec.godina)");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Datum();
                Console.ForegroundColor = ConsoleColor.White;
                zavrsniDatumString = Console.ReadLine();
            } while (!DateTime.TryParseExact(zavrsniDatumString, "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out zavrsniDatum));

            int brojac = 0;
            int unos;

            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n             PRETRAGA PO DATUMU\n--------------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" Karte prodane u vremenskom razdoblju od " + pocetniDatum.ToString("d.M.yyyy") + ". godine do " + zavrsniDatum.ToString("d.M.yyyy") + ". godine:" +
                    "\n---------------------------------------------------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.Gray;

                for (int i = 0; i < nizProdaja.Length; i++)
                {
                    if (nizProdaja[i].datum > pocetniDatum && nizProdaja[i].datum < zavrsniDatum)
                    {
                        Console.WriteLine(brojac + 1 + ". | Broj racuna: " + nizProdaja[i].id + " | Zona: " + nizProdaja[i].zona + " | Vrsta: " + nizProdaja[i].vrsta + " | Cijena: " +
                                    nizProdaja[i].cijenaKarte + " kn" + " | Kolicina: " + nizProdaja[i].kolicina + " kom" + " | Iznos racuna: " +
                                    nizProdaja[i].ukupnaCijena + " kn | Datum: " + nizProdaja[i].datum.ToString("dd.MM.yyyy") + " | Vrijeme: " + nizProdaja[i].vrijeme);
                        brojac++;
                    }
                }
                if (brojac == 0)
                {
                    Console.WriteLine("\n\n Nema prodanih karata u odabranom vremenskom razdoblju!");
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n---------------------------------------------\n");
                Console.WriteLine("1 - Natrag na Evidencija prodaje\n\n" +
                                  "2 - Natrag na Glavni izbornik\n\n" +
                                  "0 - Izlaz");
                Datum();
                Console.ForegroundColor = ConsoleColor.White;
            } while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out unos) || unos < 0 || unos > 2);

            switch (unos)
            {
                case 1:
                    Evidencija();
                    break;
                case 2:
                    Izbornik();
                    break;
                case 0:
                    Izlaz();
                    break;
            }
        }

        public static void Evidencija() // Izbornik evidencije prodaje karata sa sortiranjem podataka
        {
            int unos, unos2;
            UcitajProdaju();
            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n IZBORNIK EVIDENCIJE PRODAJE AUTOBUSNIH KARATA\n------------------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("1 - Evidencija racuna prodanih karata sa sortiranjem\n" +
                                  "2 - Evidencija racuna prodanih karata sa filtriranjem\n" +
                                  "3 - Pregled racuna po vremenskim intervalima\n\n" +
                                  "4 - Natrag na Glavni izbornik\n\n" +
                                  "0 - Izlaz");
                Datum();
                Console.ForegroundColor = ConsoleColor.White;
            } while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out unos) || unos < 0 || unos > 4);

            int brojac = 0;

            switch (unos)
            {
                case 1:
                    do
                    {
                        do
                        {
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("\n EVIDENCIJA RACUNA SVIH PRODANIH AUTOBUSNIH KARATA\n---------------------------------------------------\n");
                            Console.ForegroundColor = ConsoleColor.Gray;

                            for (int i = 0; i < nizProdaja.Length; i++)
                            {
                                Console.WriteLine(i + 1 + ". | Broj racuna: " + nizProdaja[i].id + " | Zona: " + nizProdaja[i].zona + " | Vrsta: " + nizProdaja[i].vrsta + " | Cijena: " +
                                nizProdaja[i].cijenaKarte + " kn | Kolicina: " + nizProdaja[i].kolicina + " kom" + " | Iznos racuna: " +
                                nizProdaja[i].ukupnaCijena + " kn | Datum: " + nizProdaja[i].datum.ToString("dd.MM.yyyy") + " | Vrijeme: " + nizProdaja[i].vrijeme);
                            }

                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine("\n SORTIRAJ EVIDENCIJU RACUNA:\n-----------------------------");
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("\n1 - Po broju racuna" +
                                              "\n2 - Po zonama" +
                                              "\n3 - Po vrsti karata" +
                                              "\n4 - Po cijeni karte" +
                                              "\n5 - Po iznosu racuna" +
                                              "\n6 - Po datumu " +
                                              "\n7 - Po vremenu" +
                                              "\n\n8 - Natrag na Evidencija prodaje" +
                                              "\n9 - Natrag na Glavni izbornik\n" +
                                              "\n0 - Izlaz");
                            Datum();
                            Console.ForegroundColor = ConsoleColor.White;
                        } while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out unos2) || unos2 < 0 || unos2 > 9);

                        switch (unos2)
                        {
                            case 1:
                                SortiranjeProdanihKarata("id", brojac++);
                                break;
                            case 2:
                                SortiranjeProdanihKarata("zona", brojac++);
                                break;
                            case 3:
                                SortiranjeProdanihKarata("vrsta", brojac++);
                                break;
                            case 4:
                                SortiranjeProdanihKarata("cijena", brojac++);
                                break;
                            case 5:
                                SortiranjeProdanihKarata("ukupnaCijena", brojac++);
                                break;
                            case 6:
                                SortiranjeProdanihKarata("datum", brojac++);
                                break;
                            case 7:
                                SortiranjeProdanihKarata("vrijeme", brojac++);
                                break;
                        }
                    } while (unos2 != 0 && unos2 != 8 && unos2 != 9);

                    switch (unos2)
                    {
                        case 0:
                            Izlaz();
                            break;
                        case 8:
                            Evidencija();
                            break;
                        case 9:
                            Izbornik();
                            break;
                    }
                    break;
                case 2:
                    FiltriranjeEvidencija();
                    break;
                case 3:
                    PregledProdanihPoDatumu();
                    break;
                case 4:
                    Izbornik();
                    break;
                case 0:
                    Izlaz();
                    break;
            }
        }

        public static void FiltriranjeEvidencija() // Evidencija prodaje karata sa filtiranjem podataka 
        {
            int unos;
            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nEVIDENCIJA RACUNA SVIH PRODANIH AUTOBUSNIH KARATA\n-------------------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.Gray;

                for (int i = 0; i < nizProdaja.Length; i++)
                {
                    Console.WriteLine(i + 1 + ". | Broj racuna: " + nizProdaja[i].id + " | Zona: " + nizProdaja[i].zona + " | Vrsta: " + nizProdaja[i].vrsta + " | Cijena: " +
                    nizProdaja[i].cijenaKarte + " kn | Kolicina: " + nizProdaja[i].kolicina + " kom" + " | Iznos racuna: " +
                    nizProdaja[i].ukupnaCijena + " kn | Datum: " + nizProdaja[i].datum.ToString("dd.MM.yyyy") + " | Vrijeme: " + nizProdaja[i].vrijeme);
                }

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\n FILTRIRAJ EVIDENCIJU RACUNA:\n------------------------------");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n1 - Po vrsti karte" +
                                  "\n2 - Po broju racuna" +
                                  "\n3 - Po cijeni karte" +
                                  "\n4 - Po kolicini prodanih karata" +
                                  "\n5 - Po iznosu racuna" +
                                  "\n6 - Po datumu " +
                                  "\n7 - Po vremenu" +
                                  "\n\n8 - Natrag na Evidencija prodaje" +
                                  "\n9 - Natrag na Glavni izbornik\n" +
                                  "\n0 - Izlaz");
                Datum();
                Console.ForegroundColor = ConsoleColor.White;
            } while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out unos) || unos < 0 || unos > 9);

            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nEVIDENCIJA RACUNA SVIH PRODANIH AUTOBUSNIH KARATA\n-------------------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.Gray;

                switch (unos)
                {
                    case 1:
                        FiltriranjeProdanihKarata("vrsta");
                        break;
                    case 2:
                        FiltriranjeProdanihKarata("id");
                        break;
                    case 3:
                        FiltriranjeProdanihKarata("cijena");
                        break;
                    case 4:
                        FiltriranjeProdanihKarata("kolicina");
                        break;
                    case 5:
                        FiltriranjeProdanihKarata("ukupnaCijena");
                        break;
                    case 6:
                        FiltriranjeProdanihKarata("datum");
                        break;
                    case 7:
                        FiltriranjeProdanihKarata("vrijeme");
                        break;
                    case 8:
                        Evidencija();
                        break;
                    case 9:
                        Izbornik();
                        break;
                    case 0:
                        goto kraj;
                }

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\n FILTRIRAJ EVIDENCIJU RACUNA:\n------------------------------");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n1 - Po vrsti karte" +
                                    "\n2 - Po broju racuna" +
                                    "\n3 - Po cijeni karte" +
                                    "\n4 - Po kolicini prodanih karata" +
                                    "\n5 - Po iznosu racuna" +
                                    "\n6 - Po datumu " +
                                    "\n7 - Po vremenu" +
                                    "\n\n8 - Natrag na Evidencija prodaje" +
                                    "\n9 - Natrag na Glavni izbornik\n" +
                                    "\n0 - Izlaz");
                Datum();
                Console.ForegroundColor = ConsoleColor.White;
            } while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out unos) || unos != 0);
        kraj: Izlaz();
        }

        public static void Unos_izmjena() // Izbornik za unos novih te izmjenu i brisanje postojecih karata
        {
            UcitajKarte();

            int unos;

            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nIZBORNIK ZA UNOS I IZMJENU VRSTA KARATA\n---------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("1 - Pregled\n" +
                                  "2 - Unos\n" +
                                  "3 - Izmjena\n" +
                                  "4 - Brisanje\n\n" +
                                  "5 - Natrag na Glavni izbornik\n\n" +
                                  "0 - Izlaz");
                Datum();
                Console.ForegroundColor = ConsoleColor.White;
            } while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out unos) || unos < 0 || unos > 5);

            Console.Clear();

            switch (unos)
            {
                case 1:
                    PregledKarataPoZonama();
                    break;
                case 2:
                    UnosKarte();
                    break;
                case 3:
                    IzmjenaKarte();
                    break;
                case 4:
                    BrisanjeKarte();
                    break;
                case 5:
                    Izbornik();
                    break;
                case 0:
                    Izlaz();
                    break;
            }
        }

        public static void UnosKarte() // Izbornik za unos nove karte i spremanje karte u tekstualnu datoteku "karte.txt"
        {
            Karta karta1 = new Karta();

            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n          UNOS NOVE KARTE\n------------------------------------------------");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Unesite zonu u koju zelite spremiti kartu: (1-4)\n------------------------------------------------");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n\n\n\n\n\n\n0 - Natrag na Glavni izbornik ");
                Datum();
                Console.ForegroundColor = ConsoleColor.White;
                karta1.zona = Console.ReadKey().KeyChar.ToString();
            } while (karta1.zona != "1" && karta1.zona != "2" && karta1.zona != "3" && karta1.zona != "4" && karta1.zona != "0");

            if (karta1.zona == "0")
            {
                Izbornik();
            }

            do
            {
                Console.Clear(); Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n        UNOS NOVE KARTE: \n----------------------------");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Unesite vrstu (naziv) karte:\n----------------------------");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n\n\n\n\n\n\n0 - Natrag na Glavni izbornik ");
                Datum();
                Console.ForegroundColor = ConsoleColor.White;
                karta1.vrsta = Console.ReadLine();
            } while (karta1.vrsta == "");

            if (karta1.zona == "0")
            {
                Izbornik();
            }

            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n               UNOS NOVE KARTE      :\n--------------------------------------");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Unesite iznos za cijenu karte: (0-1000)\n--------------------------------------");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n\n\n\n\n\n");
                Datum();
                Console.ForegroundColor = ConsoleColor.White;
            } while (!double.TryParse(Console.ReadLine(), out karta1.cijena) || karta1.cijena < 0 || karta1.cijena > 1000);


            using (StreamWriter sw = File.AppendText("karte.txt"))
            {
                sw.WriteLine(karta1.zona + "-" + karta1.vrsta + "-" + karta1.cijena);
            }
            UcitajKarte();
            Izbornik();

        }

        public static void IzmjenaKarte() // Izbornik za izmjenu postojece karte koja se nalazi u teksutalnoj datoteci "karte.txt"
        {
            int unosIzmjene, izmjenaZone;
            string izmjenaNaziva;
            double izmjenaCijene;

            Karta[] nizzKarata = new Karta[0];

            if (File.Exists("karte.txt"))
            {
                int brojLinija = File.ReadLines("karte.txt").Count();
                nizzKarata = new Karta[brojLinija];

                using (StreamReader sr = File.OpenText("karte.txt"))
                {
                    string linija = "";
                    string[] temp;
                    int brojac = 0;
                    while ((linija = sr.ReadLine()) != null)
                    {
                        temp = linija.Split('-');
                        nizzKarata[brojac].zona = temp[0];
                        nizzKarata[brojac].vrsta = temp[1];
                        nizzKarata[brojac].cijena = double.Parse(temp[2]);
                        brojac++;
                    }
                }
            }

            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n                  PREGELD SVIH KARATA\n-----------------------------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("      Unesite broj karte koji zelite promijeniti:\n-----------------------------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.Cyan;

                for (int i = 0; i < nizzKarata.Length; i++)
                {
                    Console.WriteLine(i + 1 + ". Zona: " + nizzKarata[i].zona + " | Vrsta: " + nizzKarata[i].vrsta + " | Cijena: " + nizzKarata[i].cijena + " kn");
                }

                Console.WriteLine("\n\n\n0 - Natrag na Glavni izbornik");
                Datum();
                Console.ForegroundColor = ConsoleColor.White;
            } while (!int.TryParse(Console.ReadLine(), out unosIzmjene) || unosIzmjene < 0 || unosIzmjene > nizzKarata.Length);

            if (unosIzmjene == 0)
            {
                Izbornik();
            }
            else
            {
                do
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n                IZMJENA KARTE\n--------------------------------------------------\n");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Unesite zonu u koju zelite spremiti izmjenu: (1-4)\n--------------------------------------------------\n");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\n1 - \"1\" Zona\n2 - \"2\" Zona\n3 - \"3\" Zona\n4 - \"4\" Zona\n\n\n\n0 - Natrag na Glavni izbornik");
                    Datum();
                    Console.ForegroundColor = ConsoleColor.White;
                } while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out izmjenaZone) || izmjenaZone < 0 || izmjenaZone > 4);

                if (izmjenaZone == 0)
                {
                    Izbornik();
                }

                do
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n         IZMJENA KARTE\n----------------------------------\n");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Unesite novi naziv (vrstu) karte:\n----------------------------------\n\n\n\n0 - Natrag na Glavni izbornik");
                    Datum();
                    Console.ForegroundColor = ConsoleColor.White;
                    izmjenaNaziva = Console.ReadLine();
                } while (izmjenaNaziva == "");

                if (izmjenaNaziva == "0")
                {
                    Izbornik();
                }

                do
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("           IZMJENA KARTE\n-----------------------------------\n");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Unesite novu cijenu karte: (0-1000)\n-----------------------------------\n\n\n\n");
                    Datum();
                    Console.ForegroundColor = ConsoleColor.White;
                } while (!double.TryParse(Console.ReadLine(), out izmjenaCijene) || izmjenaCijene < 0 || izmjenaCijene > 1000);

                string[] izmjenjenaLinija = File.ReadAllLines("karte.txt");
                izmjenjenaLinija[unosIzmjene - 1] = izmjenaZone.ToString() + "-" + izmjenaNaziva.ToString() + "-" + izmjenaCijene.ToString();
                File.WriteAllLines("karte.txt", izmjenjenaLinija);

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("            IZMJENA KARTE\n------------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Izmjenjena karta je pohranjena u datoteku!\n");
                Datum();
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey();
                UcitajKarte();
                Izbornik();
            }
        }

        public static void BrisanjeKarte() // Brisanje karte iz tekstualne datoteke "karte.txt"
        {
            int unosBrisanje;
            Karta[] nizzKarata = new Karta[0];

            if (File.Exists("karte.txt"))
            {
                int brojLinija = File.ReadLines("karte.txt").Count();
                nizzKarata = new Karta[brojLinija];

                using (StreamReader sr = File.OpenText("karte.txt"))
                {
                    string linija = "";
                    string[] temp;
                    int brojac = 0;
                    while ((linija = sr.ReadLine()) != null)
                    {
                        temp = linija.Split('-');
                        nizzKarata[brojac].zona = temp[0];
                        nizzKarata[brojac].vrsta = temp[1];
                        nizzKarata[brojac].cijena = double.Parse(temp[2]);
                        brojac++;
                    }
                }
            }

            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n                  PREGELD SVIH KARATA\n----------------------------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("      Unesite broj karte koji zelite izbrisati:\n----------------------------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.Cyan;

                for (int i = 0; i < nizzKarata.Length; i++)
                {
                    Console.WriteLine(i + 1 + ". Zona: " + nizzKarata[i].zona + " | Vrsta: " + nizzKarata[i].vrsta + " | Cijena: " + nizzKarata[i].cijena + " kn");
                }

                Console.WriteLine("\n0 - Natrag na Glavni izbornik");
                Datum();
                Console.ForegroundColor = ConsoleColor.White;
            } while (!int.TryParse(Console.ReadLine(), out unosBrisanje) || unosBrisanje < 0 || unosBrisanje > nizzKarata.Length);

            if (unosBrisanje == 0)
            {
                Izbornik();
            }
            else
            {
                string tempBrisanje = Path.GetTempFileName();

                var ostajuLinije = File.ReadAllLines("karte.txt").Where(l => l != nizzKarata[unosBrisanje - 1].zona + "-" +
                nizzKarata[unosBrisanje - 1].vrsta + "-" + nizzKarata[unosBrisanje - 1].cijena);

                File.WriteAllLines(tempBrisanje, ostajuLinije);
                File.Delete("karte.txt");
                File.Move(tempBrisanje, "karte.txt");

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n               BRISANJE KARTE\n--------------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n\n\n\n     Izbrisali ste kartu iz datoteke!\n");
                Datum();
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey();
                Izbornik();
            }
        }

        public static void Datum() // Ispis datuma
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n\n\n\n---------------------------------------------\n" + DateTime.Now.AddMinutes(00).ToString("dd.MM.yyyy.                         HH:mm:ss"));
            Console.WriteLine("\n---------------------------------------------\nCopyright 2020 Bus d.o.o. - Poslovnica Aspira\n\n\n");
        }

        public static void Izlaz() // Izlaz iz aplikacije
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n             IZLAZ IZ APLIKACIJE\n--------------------------------------------\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\n\n\n       Hvala na koristenju aplikacije!\n\n\n");
            Datum();
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            UcitajKarte();
            UcitajProdaju();
            Izbornik();
        }
    }
}


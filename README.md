# microservices-C-example
example of connection of 6 small microservices via queue - Full description in Polish


Założenie Service Bus	2
Założenie Kolejek	4
Założenie Application insights	5
Założenie repozytorium git  na azure devops	6
Założenie bazy Danych	7
connection String	7
Połączenie w Microsoft SQL management Studio	7
Stworzenie odpowiednich tabel dla serwisow	7
Stworzenie Mikroserwisów	9
"applicationUrl":	10
SerwisConsole	10
User Interface	10
Obsluga Konsoli	11
Controller konsoli (głownie do odebrania informacji po przetworzeniu ich przez mikroserwisy)	12
Prośba o liste badan z serwis db	12
Wysłanie Informacji do SerwisOrg	13
SerwisOrg	14
Zdefiniowanie ConnectionStrings	14
Zdefiniowanie obiektów kolejek	15
I następnie wysyłamy tą samą wiadomość do każdej kolejki	16
SerwisEwus	18
Deserializacja	18
serwisDbDat	18
Dane do bazy danych i kolejki	18
ServiceBusManager	19
ConnectionStrings	19
Odebranie informacji	19
Zapis Do bazy danych	21
Wysyłanie to następnej kolejki	24
SerwisDB	24
Connection Strings	24
DbDat	25
ServiceBusManager	26
Controller	29
Send to giveBack	30
Serwis CT	31
Connection Strings	31
DbDat	31
Dane do bazy danych i kolejki	32
Kontekst	32
Zdefiniowanie Kontrollera	33
ServiceBusManager	33
CheckContraindicationsCT	38
Serwis MRI	38
Connection Strings	38
DbDat	39
Dane do bazy danych i kolejki	39
Kontekst	40
Zdefiniowanie kontrollera	40
ServiceManager	41
CheckContraindications MRI	45
Serwis give back	46
Connection Strings	46
Dane do bazy danych i wyslania spowrotem do konsoli	46
Obiekty do deserializacji kolejki	47
Kontekst	48
ServiceBusManager	48
Rozszerzenie bazy danych	49
Następnie definiujemy odpowiednie konteksty bazodanowe	50
Controllery dodatkowe	51
Testowanie	57
L - list	57
E - error	57
CT bez przeciwwskazań	58
CT i przeciwwskazania	59
MRI i przeciwwskazania	59



Założenie Service Bus
Najpierw wyszukujemy 

Add

Basic tier - najniższe koszty
Create
Założenie Kolejek

Klikamy + queue
W przypadku każdej kolejki dodajemy Shared Access policies i kopiujemy connection String

Zakładamy kilka kolejek bedacymi odpowiedzialnymi za komunikacje 
SerwisOrg - informacje wysyłane przez serwisORG i odbierane przez  serwisDb - potem serwis DB zapisuje dane w bazie danych
Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=XkieCakVC8Na7MVgLKov+GuMHXyQl8MdgNpUo9MG3vc=;EntityPath=serwisorg
serwisEwus - wysylane przez serwis ORg i odbierane przez serwis Ewus
Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=ii2y1gc1Q+zQjVqfpNbKZKjq1l3LVYi+wXtAa8Fp8jw=;EntityPath=serwisewus
serwisCTver3 - wysyłane przez serwisOrg i odbierane przez serwisCTver3
Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=SFIsryaDyiIkQp32u6pwAXl3izZVds+WjdPF4Jt1nBg=;EntityPath=serwisctver3

serwisMRI - wysyłane przez serwis org i odbierane przez serwisMRI
Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=pYo+Cwak/KwBNF5UdAgkyXtmBqoyi+ERvU0X9W2S2y0=;EntityPath=serwismri
serwisGiveBack - wysyłane przez serwisMRI, serwisCT, serwisEwus, serwis dd i odbierane przez serwisGiveBack
Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=jZsunxSwVlOfVLPSDR9fSLcEA/2j1F3TtVOVqJix/Zs=;EntityPath=serwisgiveback

Założenie Application insights


InstrumentationKey=754be992-604d-48c3-825e-090997e6bae1;IngestionEndpoint=https://westeurope-1.in.applicationinsights.azure.com/

Nastepnie dodanie do kazdego serwisu w appsettings 
  "ApplicationInsights": {
    "InstrumentationKey": "754be992-604d-48c3-825e-090997e6bae1"
  },


Założenie repozytorium git  na azure devops 

Zrobiono to za pomoca opcji sync w solution explorer 

ProjektCtiMRI - Repos (azure.com)




Założenie bazy Danych

connection String 
Server=tcp:dp102miazkiewicz.database.windows.net,1433;Initial Catalog=CTiMRI;Persist Security Info=False;User ID=k.miazkiewicz;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;

Połączenie w Microsoft SQL management Studio
login 
hasło

Stworzenie odpowiednich tabel dla serwisow
Zamiast boolean jest BIT

serwisDb (zapisywane dane przysyłane za pośrednictwem Serwis Org z klienckiej aplikacji konsolowej) 

CREATE TABLE serwisDbDat (
 ID_column INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
 Pesel int ,
 studyType varchar(50) ,
 imie varchar(50),
 nazwisko varchar(50),
 RodzajBadania varchar(50),
isFerroMagnetic BIT ,
isNiewydolNerek BIT ,
isNadczynnoscTarczycy BIT 

);
serwisEwus - zapisywane dane jak imie nazwisko i pesel pacjenta obecna data i czy jest uprawnionym do robienia badań z ramienia NFZ
CREATE TABLE serwisEwusDat  (
 ID_column INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
 Pesel int ,
 imie varchar(50),
 nazwisko varchar(50),
isOkinNFZ int 

);


serwisCTver3 -  zapisywane dane jak imie nazwisko i pesel pacjenta obecna data i czy nie ma przeciwskazan do CT

CREATE TABLE serwisCTDat  (
 ID_column INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
 Pesel int ,
 imie varchar(50),
 nazwisko varchar(50),
isOktoCT BIT 
);

serwisMRI   - zapisywane dane jak imie nazwisko i pesel pacjenta obecna data i czy nie ma przeciwskazan do MRI

CREATE TABLE serwisMRIDat  (
 ID_column INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
 Pesel int ,
 imie varchar(50),
 nazwisko varchar(50),
isOktoMRI BIT 
);

serwisGiveBack - 
CREATE TABLE serwisGiveBackDat  (
 ID_column INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
 Pesel int ,
 imie varchar(50),
 nazwisko varchar(50),
 studyType varchar(50) ,
noContraIndications BIT ,
isOktoNFZ BIT ,
isSavedToPax BIT 
);


Stworzenie Mikroserwisów
Z powodu trudności w kompatybilności paczek nugetowych, stworzylismy Template serwisu z już gotowymi paczkami i stworzonymi niektórymi klasami - jak do obsługi kolejki  czy  z podstawowa konfiguracja do application insights .
Project-> export template 

Nowe projekty na bazie templatów stworzono rozwiazanie o struktórze jak poniżej


 "applicationUrl":
Pamietamy też by w ustawieniach startowych poszczególne mikroserwiry miały inny     "applicationUrl":


SerwisConsole
Serwis zapewniajacy interfejs tekstowy
User Interface


        public async Task mainConsoleFunction()
        {

            switch (Console.ReadLine())
            {
                case "l":
                    Console.WriteLine("processing...");
                  var  str = await listStudies();
                    Console.WriteLine(str);

                    break;
                case "ct":

                    commonForStudies("CT");
                    break;
                case "mri":

                    commonForStudies("MRI");

                    break;
                case "e":
                    Console.WriteLine("wywolywanie bledu ...");
                    putToGetError();
                    break;
            }
        }



Wpisania nowego badania CT - komenda ct
Wpisania nowego badania MRI - komenda mri






Obsluga Konsoli
        //obsluguje wydanie polecen do konsoli i tworzy podstawowy obiekt ktory nastepnie bedzie wyslany
        public async Task commonForStudies(string studyType)
        {
            Console.WriteLine("imie");
            var imie = Convert.ToString(Console.ReadLine());
            Console.WriteLine("nazwisko");
            var nazwisko = Convert.ToString(Console.ReadLine());
            Console.WriteLine("pesel");
            var pesel = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("czy pacjent ma w ciele elementy metaliczne  jesli tak wpisz t jesli nie wpisz n");
            var isFerroMagnetic = Convert.ToString(Console.ReadLine()) == "t";
            Console.WriteLine("czy zdiagnozowano u pacjenta niewydolnosc nerek");
            var isNiewydolNerek = Convert.ToString(Console.ReadLine()) == "t";
            Console.WriteLine("czy zdiagnozowano u pacjenta nadczynność tarczycy");
            var isNadczynnoscTarczycy = Convert.ToString(Console.ReadLine()) == "t";

            newStudySend(pesel, studyType, imie, nazwisko, isFerroMagnetic, isNiewydolNerek, isNadczynnoscTarczycy);

        }




Controller konsoli (głownie do odebrania informacji po przetworzeniu ich przez mikroserwisy)

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class ConsoleController : ControllerBase
    {
        private readonly DpDataContext _context;
        private readonly ILogger _logger;
        public ConsoleController(DpDataContext context,  ILogger logger) {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public IActionResult Add(serwisGiveBack studyData)
        {
            //zapisujemy i sprawdzamy czy pesel jest w bazie danych zeby nie wyswietlac kilka razy tego samego
            if (!_context.localTable.ToList().Select(it => it.pesel).Contains(studyData.Pesel))
            {
                Console.WriteLine(" receiveeed ");
                Console.WriteLine("rodzaj badania " + studyData.studyType);
                Console.WriteLine("imie " + studyData.imie);
                Console.WriteLine("nazwisko " + studyData.nazwisko);
                Console.WriteLine("pesel " + studyData.Pesel);
                Console.WriteLine("pacjent w Ewus " + studyData.isOktoNFZ);
                Console.WriteLine("badanie zapisane " + studyData.isSavedToPax);
                Console.WriteLine("brak przeciwskazan do badania " + studyData.noContraIndications);
            }
            else {
                _context.localTable.Add(new localDat { pesel = studyData.Pesel});
            }
            
            return Ok("");
        }

    }



Prośba o liste badan z serwis db
        // wysyla odpowiedni obiekt do kolejki w celu uzyskania informacji o liscie pacjentow
        public async Task<String> listStudies()
        {
            string token = await GetToken();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);
              var list = await client.GetAsync("https://localhost:5008/api/serwisDb");
            var str = await list.Content.ReadAsStringAsync();
            return str;



        }


Funkcja do wywoływania 


Wysłanie Informacji do SerwisOrg 
newStudySend() ma za zadanie wysłania za pomocą protokołu HTTP (post) informacji do SerwisOrg informacji na temat nowego badania, wszystko z zachowaniem autentykacji do czego służy funkcja getToken() zwracająca token uprawniający do wysłania HTTP post

    // wysyla za pośrednictwem protokolu HTTP
        public async Task newStudySend(int pesel, string studyType, string imie, string nazwisko, bool isFerroMagnetic, bool isNiewydolNerek, bool isNadczynnoscTarczycy)
        {

            string token = await GetToken();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer "+ token);
         

           // var app = PublicClientApplicationBuilder.Create("fce95216-40e5-4a34-b041-f287e46532be")
            //        .WithAuthority("https://login.microsoftonline.com/146ab906-a33d-47df-ae47-fb16c039ef96/v2.0/")
           //         .WithDefaultRedirectUri()
            //        .Build();

         //   var result =   await   app.AcquireTokenInteractive(new[] { "api://fce95216-40e5-4a34-b041-f287e46532be/.default" }).ExecuteAsync();

            serwisDbDat message = new serwisDbDat()
            {
                pesel = pesel,
                studyType = studyType,
                imie = imie,
                nazwisko = nazwisko,
                isFerroMagnetic = isFerroMagnetic,
                isNiewydolNerek = isNiewydolNerek,
                isNadczynnoscTarczycy = isNadczynnoscTarczycy
            };

            string studyJson = System.Text.Json.JsonSerializer.Serialize(message);
            client.PostAsync("https://localhost:5001/api/SerwisOrg", new StringContent(studyJson,Encoding.UTF8, "application/json"));
        

            Console.WriteLine("wyslane");
        }







SerwisOrg
Tutaj będziemy odbierac informacje nadane za pośrednictwem protokołu Http (z autentykacją) od aplikacji konsolowej i wysyłać do serii kolejek 

Zdefiniowanie ConnectionStrings
Zdefiniowanie Connection Strings w celu umożliwienia wysyłania informacji do danych kolejek


  "ConnectionStrings": {
    "SerwisOrgConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=XkieCakVC8Na7MVgLKov+GuMHXyQl8MdgNpUo9MG3vc=",
    "SerwisEwusConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=ii2y1gc1Q+zQjVqfpNbKZKjq1l3LVYi+wXtAa8Fp8jw=",
    "serwisCTver3ConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=SFIsryaDyiIkQp32u6pwAXl3izZVds+WjdPF4Jt1nBg=",
    "serwisMRIconnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=pYo+Cwak/KwBNF5UdAgkyXtmBqoyi+ERvU0X9W2S2y0="
  }


Zdefiniowanie obiektów kolejek
W klasie ServiceBusManager w obrębie ServiceOrg mikroserwisu  i ich inicjalizacja w konstruktorze

       
 private readonly QueueClient _queueClientSerwisOrg;
        private readonly QueueClient _queueClientSerwisEwus;
        private readonly QueueClient _queueClientserwisCTver3;
        private readonly QueueClient _queueClientserwisMRI;

        // sprawdzic czy informacja jest w kolejce wyklad 2 1:09:37
        public ServiceBusManager(IConfiguration configuration) {
            _queueClientSerwisOrg = new QueueClient(configuration.GetConnectionString("SerwisOrgConnectionString"), "serwisorg");
            _queueClientSerwisEwus = new QueueClient(configuration.GetConnectionString("SerwisEwusConnectionString"), "serwisewus");
            _queueClientserwisCTver3 = new QueueClient(configuration.GetConnectionString("serwisCTver3ConnectionString"), "serwisctver3");
            _queueClientserwisMRI = new QueueClient(configuration.GetConnectionString("serwisMRIconnectionString"), "serwismri");
        }




I następnie wysyłamy tą samą wiadomość do każdej kolejki

        public async Task SendMessage(serwisDbDat payload)
        {
            string data = JsonConvert.SerializeObject(payload);
            Message message = new Message(Encoding.UTF8.GetBytes(data));
           await  _queueClientSerwisOrg.SendAsync(message);
            await _queueClientSerwisEwus.SendAsync(message);
             // musimy kontrolowac gdzie to wysyłamy w zależności od rodzaju badania
            var rodzajBad = payload.studyType;
            if (rodzajBad== "CT") {
                await _queueClientserwisCTver3.SendAsync(message);
            }
            if (rodzajBad == "MRI")
            {
                await _queueClientserwisMRI.SendAsync(message);
            }
            }

        }


Await w przypadku każdego może nie ma sensu z punktu widzenia wydajności ale w przypadku takich prostych zadań też nie jest zbyt szkodliwe

W serviceBusExplorerze (Azure) widzimy ze wiadomość doszła


SerwisEwus
Deserializacja
serwisDbDat
Do serwisEwus a także CT , MRI i db wklejamy definicje  obiektu który będzie wysyłany z Serwis Org tak aby można przeprowadzić deserializację

    public class serwisDbDat
    {
        public int pesel { get; set; }
        public string studyType { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public bool isFerroMagnetic { get; set; }// prawda jesli sa ferromagnetyczne elementy metalowe w ciele
        public bool isNiewydolNerek { get; set; } // prawda jesli pacjent ma niewydolnosc nerek
        public bool isNadczynnoscTarczycy { get; set; }/// prawda jesli pacjent ma nadczynnosc tarczycy

    }

Dane do bazy danych i kolejki
Definiujemy również klasę gromadząca informacje która będzie wykorzystana do zapisu do bazy danych jak i do przesłania informacji do kolejki give it back

   public class serwisEwusDat
    {
        [Key]
        public int ID_column { get; set; }

        public int Pesel { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public bool isOkInNFZ { get; set; }


    }


ServiceBusManager
ConnectionStrings
  "ConnectionStrings": {
    "SerwisOrgConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=XkieCakVC8Na7MVgLKov+GuMHXyQl8MdgNpUo9MG3vc=",
    "SerwisEwusConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=ii2y1gc1Q+zQjVqfpNbKZKjq1l3LVYi+wXtAa8Fp8jw=",
    "serwisCTver3ConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=SFIsryaDyiIkQp32u6pwAXl3izZVds+WjdPF4Jt1nBg=",
    "serwisMRIconnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=pYo+Cwak/KwBNF5UdAgkyXtmBqoyi+ERvU0X9W2S2y0=",
    "serwisGiveBackConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=pYo+Cwak/KwBNF5UdAgkyXtmBqoyi+ERvU0X9W2S2y0=",
    "DefaultConnection": "Server=tcp:dp102miazkiewicz.database.windows.net,1433;Initial Catalog=CTiMRI;Persist Security Info=False;User ID=k.miazkiewicz;Password=Warszawa1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30"

  }


Odebranie informacji
Musimy odebrać informację nadesłaną do SerisEwuś kolejki aby to zrobić musimy dokonać rejestracji

 najpierw  rejestrujemy sie dokolejki i definiujemy która funkcja będzie wywołana gdy informacja dojdzie


     // najpierw  rejestrujemy sie dokolejki i definiujemy która funkcja będzie wywołana gdy informacja dojdzie
        public async Task  Register()
        {
            var options = new MessageHandlerOptions((e) => Task.CompletedTask)
            {
                AutoComplete = false
            };
             _queueClientSerwisEwus.RegisterMessageHandler( processMessage, options);
        }



Tworzymy definicje obiektu który będzie później przesłany dalej i zapisany do bazy danych

        private string imie1;
        private string nazwisko1;
        private bool isEwusOk;

        public serwisEwusDat(int pesel, string imie1, string nazwisko1, bool isEwusOk)
        {
            Pesel = pesel;
            this.imie1 = imie1;
            this.nazwisko1 = nazwisko1;
            this.isEwusOk = isEwusOk;
        }

        public int Pesel { get; set; }
        public int imie { get; set; }
        public int nazwisko { get; set; }
        public bool isOkinNFZ { get; set; }


Definiujemy kontekst dla bazy danych

    public class DpDataContext : DbContext
    {
        public DpDataContext(DbContextOptions options) : base(options) { 
        }


        public DbSet<serwisEwusDat > serwisEwusDat  { get; set; }

    }


Funkcje register która za chwile pokażemy wywołujemy w kontrolerze serwisu ewus przekazujac konteks bazy danych w celu umożliwienia zapisu przychodzących informacji


    public class serwisEwusController : ControllerBase
    {
        private readonly DpDataContext _context;
        private readonly ServiceBusManager _sender;
        private readonly ILogger _logger;
        public serwisEwusController(DpDataContext context, ServiceBusManager sender, ILogger logger) {
            _logger = logger;
            _sender = sender;
            _context = context;

            sender.register(context);
        }




tu definiujemy co zrobimy z wiadomością gdy zostanie ona odebrana z kolejki

        //tu definiujemy co zrobimy z wiadomością gdy zostanie ona 
        public async Task processMessage(Message message, CancellationToken token)
        {
            
            // deserializacja JSON w obiekt C# serwisDbDat
            var payload = JsonConvert.DeserializeObject<serwisDbDat>(Encoding.UTF8.GetString(message.Body));
            // tu wywołamy fukcje sprawdzajaca czy pacjent jest w Ewus
            var dummyCheck = new DummyEwusCheck();
            var isEwusOk =await dummyCheck.checkEwus(payload);
            //stworzenie obiektu który z jednej strony będzie zapisywany do bazy danych a z drugiej wysyłany do kolejnej kolejki giveItBackService
            var dat = new serwisEwusDat ( payload.pesel, payload.imie, payload.nazwisko, isEwusOk);
              //zapis do bazy danych informacji czy pacjent jest w Ewus
            saveToDb(dat);
            // wysylamy do kolejnej kolejki w celu dalszego przetworzenia informacji
            sendToGiveBack(dat);

        }



Zapis Do bazy danych

Aby móc zapisać cokolwiek do bazy danych potrzebujemy dostępu do DbDataContext ze względu na lazy initialization kontrollera i technologię dependency injection narzuconą przez framework jest to nieco skomplikowane


Tworzymy nową klasę DbControll z jedną metodą


    public class DbControll
    {
        private DpDataContext context;

        public DbControll(DpDataContext context)
        {
            this.context = context;
        }

        /**
     zapisujemy informacje do bazy danych
     */
        public async Task saveToDb(serwisEwusDat dat)
        {
            context.Add(dat);
        }
    }
}



Z powodów błędów z dependency injection zapis dzieje się za pośrednictwem HTTP put

    private async Task sendToBeSaved(serwisEwusDat dat) {

            string token = await GetToken();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);


                     string studyJson = System.Text.Json.JsonSerializer.Serialize(dat);
            client.PostAsync("https://localhost:5003/api/serwisEwus", new StringContent(studyJson, Encoding.UTF8, "application/json"));

        }

Oczywiście z autentykacja

       private async Task<string> GetToken()
        {
            using var client = new HttpClient();

            DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
            {
                Address = "https://login.microsoftonline.com/146ab906-a33d-47df-ae47-fb16c039ef96/v2.0/",
                Policy =
          {
          ValidateEndpoints = false
          }
            });

            if (disco.IsError)
                throw new InvalidOperationException(
                $"No discovery document. Details: {disco.Error}");

            var tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "fce95216-40e5-4a34-b041-f287e46532be",
                ClientSecret = "XWGsyzt9uM-Ia2SA8WE7~gvUJ~4og-U_1p",
                Scope = "api://fce95216-40e5-4a34-b041-f287e46532be/.default"
            };

            var token = await client.RequestClientCredentialsTokenAsync(tokenRequest);

            if (token.IsError)
                throw new InvalidOperationException($"Couldn't gather token. Details: {token.Error}");

            return token.AccessToken;
        }





Wysyłanie to następnej kolejki

       /**
         wysylamy do kolejki GiveBack 
         */
        public async Task sendToGiveBack(serwisEwusDat dat) {
            string data = JsonConvert.SerializeObject(dat);
            Message message = new Message(Encoding.UTF8.GetBytes(data));
            message.Label = "serwisEwus"; // ustawiamy odpowiedni Label  by później móc odpowiednio zdeserializować ten obiekt w GiveBack
          await   _queueClientserisGiveBack.SendAsync(message);
        }



I w controllerze

        [HttpPost]
        public IActionResult Add(serwisEwusDat studyData)
        {
            _dbContr.saveToDb(studyData);

            return Ok("");
        }



SerwisDB
Generalnie służy jedynie do zapisu danych przesyłanych z serwis Org zakładamy że jest to jakaś szczególna baza danych np. PAX z plikami Dicom; ponadto przesyła informacje do give back informacje że dokonano zapisu

Connection Strings

Dodajemy connection strings do appsettings

  "ConnectionStrings": {
    "SerwisOrgConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=XkieCakVC8Na7MVgLKov+GuMHXyQl8MdgNpUo9MG3vc=",
    "SerwisEwusConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=ii2y1gc1Q+zQjVqfpNbKZKjq1l3LVYi+wXtAa8Fp8jw=",
    "serwisCTver3ConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=SFIsryaDyiIkQp32u6pwAXl3izZVds+WjdPF4Jt1nBg=",
    "serwisMRIconnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=pYo+Cwak/KwBNF5UdAgkyXtmBqoyi+ERvU0X9W2S2y0=",
    "serwisGiveBackConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=jZsunxSwVlOfVLPSDR9fSLcEA/2j1F3TtVOVqJix/Zs=",
    "DefaultConnection": "Server=tcp:dp102miazkiewicz.database.windows.net,1433;Initial Catalog=CTiMRI;Persist Security Info=False;User ID=k.miazkiewicz;Password=Warszawa1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

  }


DbDat
Dodajemy klase skopiowana z serwis org by umożliwić poprawna deserializacje informacji nadesłanej z serwis org
    public class serwisDbDat
    {
        [Key]
        public int ID_column  { get; set; }
        public int pesel { get; set; }
        public string studyType { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public bool isFerroMagnetic { get; set; }// prawda jesli sa ferromagnetyczne elementy metalowe w ciele
        public bool isNiewydolNerek { get; set; } // prawda jesli pacjent ma niewydolnosc nerek
        public bool isNadczynnoscTarczycy { get; set; }/// prawda jesli pacjent ma nadczynnosc tarczycy

    }




ServiceBusManager

Jedynie różnica że rejestruje się do kolejki Serwis org i że jedyna akcja wykonywaną jest zapis do bazy danych
    public class ServiceBusManager
    {

        private readonly QueueClient _queueClientSerwisOrg;
        private Model.DpDataContext _context;
    
        private DpDataContext context;
        private static HttpClient client = new HttpClient();

        public IConfiguration Configuration { get; }

        public ServiceBusManager( IConfiguration configuration, DpDataContext context)
        {
            _queueClientSerwisOrg = new QueueClient(configuration.GetConnectionString("SerwisOrgConnectionString"), "serwisorg");
            _context = context;
             Configuration = configuration;
        }



        // najpierw  rejestrujemy sie dokolejki i definiujemy która funkcja będzie wywołana gdy informacja dojdzie
        public async Task Register()
        {



            Console.WriteLine("RRRRRRRRRRRRRRRRRRRRRRRRRR egistered");

            var options = new MessageHandlerOptions((e) => Task.CompletedTask)
            {
                //trzeba rowniez pamietac zeby zamknac wiadomosc by znikneła z kolejki
                AutoComplete = true
            };
            _queueClientSerwisOrg.RegisterMessageHandler(processMessage, options);
        }
        //tu definiujemy co zrobimy z wiadomością gdy zostanie ona odebrana z kolejki

        public async Task processMessage(Message message, CancellationToken token)

        {
            Console.WriteLine("information processsing ");
            var payload = JsonConvert.DeserializeObject<serwisDbDat>(Encoding.UTF8.GetString(message.Body));
            sendToBeSaved(payload);


        }



        private async Task sendToBeSaved(serwisDbDat dat)
        {

            string token = await GetToken();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);


            string studyJson = System.Text.Json.JsonSerializer.Serialize(dat);
            client.PostAsync("https://localhost:5008/api/serwisDb", new StringContent(studyJson, Encoding.UTF8, "application/json"));

        }












        private async Task<string> GetToken()
        {
            using var client = new HttpClient();

            DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
            {
                Address = "https://login.microsoftonline.com/146ab906-a33d-47df-ae47-fb16c039ef96/v2.0/",
                Policy =
          {
          ValidateEndpoints = false
          }
            });

            if (disco.IsError)
                throw new InvalidOperationException(
                $"No discovery document. Details: {disco.Error}");

            var tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "fce95216-40e5-4a34-b041-f287e46532be",
                ClientSecret = "XWGsyzt9uM-Ia2SA8WE7~gvUJ~4og-U_1p",
                Scope = "api://fce95216-40e5-4a34-b041-f287e46532be/.default"
            };

            var token = await client.RequestClientCredentialsTokenAsync(tokenRequest);

            if (token.IsError)
                throw new InvalidOperationException($"Couldn't gather token. Details: {token.Error}");

            return token.AccessToken;
        }





    }



Controller
Niemal identyczna kopia serwis ewus

    public class serwisDbController : ControllerBase
    {
        private readonly DpDataContext _context;
        private readonly ServiceBusManager _sender;
        private readonly ILogger _logger;

        public serwisDbController(IConfiguration configuration,DpDataContext context,  ILogger logger) {
            _logger = logger;
            _sender = new ServiceBusManager(configuration,context);
            _sender.Register();
            _context = context;

        }

        public IActionResult serviceBusAction()
        {
         
            return Ok("");
        }
        [HttpPost]
        public IActionResult Add(serwisDbDat studyData)
        {
            _context.serwisDbDat.Add(studyData);
            _context.SaveChanges();
            return Ok("");
        }

        [HttpGet]
        public IActionResult getting()
        {



            return Ok("");
        }
    }




Send to giveBack
Zakładamy że zapis sie wykonal i w przypadku bledu nie doszło by do wysłania z powodu tego że w metodzie wykonującej zapis dodaliożmy await  poniżej funkcja wysyłająca wiadomość do kolejki give it back


        /**
  wysylamy do kolejki GiveBack 
  */
        public async Task sendToGiveBack(serwisDbDat payload)
        {

            var dat = new serwisDbDatToSend()
            {
                Pesel = payload.pesel,

                imie = payload.imie,

                nazwisko = payload.nazwisko,

                isSaved = true

            };

            string data = JsonConvert.SerializeObject(dat);

            Message message = new Message(Encoding.UTF8.GetBytes(data));

            message.Label = "serwisDbDatToSend"; // ustawiamy odpowiedni Label  by później móc odpowiednio zdeserializować ten obiekt w GiveBack




            await _queueClientserisGiveBack.SendAsync(message);

        }





Serwis CT
Connection Strings

Dodajemy connection strings do appsettings

  "ConnectionStrings": {
    "SerwisOrgConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=XkieCakVC8Na7MVgLKov+GuMHXyQl8MdgNpUo9MG3vc=",
    "SerwisEwusConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=ii2y1gc1Q+zQjVqfpNbKZKjq1l3LVYi+wXtAa8Fp8jw=",
    "serwisCTver3ConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=SFIsryaDyiIkQp32u6pwAXl3izZVds+WjdPF4Jt1nBg=",
    "serwisMRIconnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=pYo+Cwak/KwBNF5UdAgkyXtmBqoyi+ERvU0X9W2S2y0=",
    "serwisGiveBackConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=jZsunxSwVlOfVLPSDR9fSLcEA/2j1F3TtVOVqJix/Zs=",
    "DefaultConnection": "Server=tcp:dp102miazkiewicz.database.windows.net,1433;Initial Catalog=CTiMRI;Persist Security Info=False;User ID=k.miazkiewicz;Password=Warszawa1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

  }

DbDat
Dodajemy klase skopiowana z serwis org by umożliwić poprawna deserializacje informacji nadesłanej z serwis org
    public class serwisDbDat
    {
        [Key]
        public int ID_column  { get; set; }
        public int pesel { get; set; }
        public string studyType { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public bool isFerroMagnetic { get; set; }// prawda jesli sa ferromagnetyczne elementy metalowe w ciele
        public bool isNiewydolNerek { get; set; } // prawda jesli pacjent ma niewydolnosc nerek
        public bool isNadczynnoscTarczycy { get; set; }/// prawda jesli pacjent ma nadczynnosc tarczycy

    }


Dane do bazy danych i kolejki
Definiujemy również klasę gromadząca informacje która będzie wykorzystana do zapisu do bazy danych jak i do przesłania informacji do kolejki give it back

   public class serwisCTDat 
    {
        [Key]
        public int ID_column { get; set; }

        public int Pesel { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public bool isOktoCT { get; set; }


    }



Kontekst
Definiujemy kontekst bazy danych 

    public class DpDataContext : DbContext
    {
        public DpDataContext(DbContextOptions options) : base(options) { 
        }


        public DbSet<serwisCTDat> serwisCTDat { get; set; }

    }
}




Zdefiniowanie Kontrollera
    public class serwisDbController : ControllerBase
    {
        private readonly DpDataContext _context;
        private readonly ServiceBusManager _sender;
        private readonly ILogger _logger;

        public serwisDbController(IConfiguration configuration,DpDataContext context,  ILogger logger) {
            _logger = logger;
            _sender = new ServiceBusManager(configuration,context);
            _sender.Register();
            _context = context;

        }

        [HttpPost]
        public IActionResult Add(serwisCTDat studyData)
        {
            _context.serwisCTDat.Add(studyData);
            _context.SaveChanges();
            return Ok("");
        }

        [HttpGet]
        public IActionResult getting()
        {



            return Ok("");
        }
    }

}


ServiceBusManager



   public class ServiceBusManager
    {

        private readonly QueueClient _queueClientSerwisCt;
        private Model.DpDataContext _context;
        private readonly QueueClient _queueClientserisGiveBack;


        private DpDataContext context;
        private static HttpClient client = new HttpClient();
        private CheckContraindicationsCT checkContra = new  CheckContraindicationsCT();


        public IConfiguration Configuration { get; }

        public ServiceBusManager( IConfiguration configuration, DpDataContext context)
        {
            _queueClientserisGiveBack = new QueueClient(configuration.GetConnectionString("serwisGiveBackConnectionString"), "serwisgiveback");
            _queueClientSerwisCt = new QueueClient(configuration.GetConnectionString("SerwisOrgConnectionString"), "serwisctver3");
            _context = context;
             Configuration = configuration;
        }



        // najpierw  rejestrujemy sie dokolejki i definiujemy która funkcja będzie wywołana gdy informacja dojdzie
        public async Task Register()
        {



            Console.WriteLine("RRRRRRRRRRRRRRRRRRRRRRRRRR egistered");

            var options = new MessageHandlerOptions((e) => Task.CompletedTask)
            {
                //trzeba rowniez pamietac zeby zamknac wiadomosc by znikneła z kolejki
                AutoComplete = true
            };
            _queueClientSerwisCt.RegisterMessageHandler(processMessage, options);
        }
        //tu definiujemy co zrobimy z wiadomością gdy zostanie ona odebrana z kolejki

        public async Task processMessage(Message message, CancellationToken token)

        {
            Console.WriteLine("information processsing ");
            var payload = JsonConvert.DeserializeObject<serwisDbDat>(Encoding.UTF8.GetString(message.Body));

            var isContraindication = checkContra.checkContraindication(payload);

            var newDat = new serwisCTDat()
            {
                Pesel = payload.pesel,

                imie = payload.imie,

                nazwisko = payload.nazwisko,

                isOktoCT = isContraindication

            };

            await sendToBeSaved(newDat);
            // wysylamy do kolejnej kolejki w celu dalszego przetworzenia informacji

            await sendToGiveBack(newDat);


        }



        private async Task sendToBeSaved(serwisCTDat dat)
        {

            string token = await GetToken();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);


            string studyJson = System.Text.Json.JsonSerializer.Serialize(dat);
            client.PostAsync("https://localhost:5004/api/serwisCT", new StringContent(studyJson, Encoding.UTF8, "application/json"));

        }



        /**
  wysylamy do kolejki GiveBack 
  */
        public async Task sendToGiveBack(serwisCTDat newDat)
        {

        

            string data = JsonConvert.SerializeObject(newDat);

            Message message = new Message(Encoding.UTF8.GetBytes(data));

            message.Label = "serwisCTDat"; // ustawiamy odpowiedni Label  by później móc odpowiednio zdeserializować ten obiekt w GiveBack




            await _queueClientserisGiveBack.SendAsync(message);

        }









        private async Task<string> GetToken()
        {
            using var client = new HttpClient();

            DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
            {
                Address = "https://login.microsoftonline.com/146ab906-a33d-47df-ae47-fb16c039ef96/v2.0/",
                Policy =
          {
          ValidateEndpoints = false
          }
            });

            if (disco.IsError)
                throw new InvalidOperationException(
                $"No discovery document. Details: {disco.Error}");

            var tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "fce95216-40e5-4a34-b041-f287e46532be",
                ClientSecret = "XWGsyzt9uM-Ia2SA8WE7~gvUJ~4og-U_1p",
                Scope = "api://fce95216-40e5-4a34-b041-f287e46532be/.default"
            };

            var token = await client.RequestClientCredentialsTokenAsync(tokenRequest);

            if (token.IsError)
                throw new InvalidOperationException($"Couldn't gather token. Details: {token.Error}");

            return token.AccessToken;
        }





    }










CheckContraindicationsCT
Definiujemy nową klasę która na podstawie informacji przekazanych z serwis org  definiuje czy są przeciwwskazania do przeprowadzania tomografii komputerowej z kontrastem ( są to nadczynność tarczycy lub niewydolność nerek)


{
    public class CheckContraindicationsCT
    {
        public bool checkContraindication(DP.Patients.Model.serwisDbDat payload) {

            return !(payload.isNadczynnoscTarczycy || payload.isNiewydolNerek);

        }

    }
}



Serwis MRI
Connection Strings

Dodajemy connection strings do appsettings

  "ConnectionStrings": {
    "SerwisOrgConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=XkieCakVC8Na7MVgLKov+GuMHXyQl8MdgNpUo9MG3vc=",
    "SerwisEwusConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=ii2y1gc1Q+zQjVqfpNbKZKjq1l3LVYi+wXtAa8Fp8jw=",
    "serwisCTver3ConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=SFIsryaDyiIkQp32u6pwAXl3izZVds+WjdPF4Jt1nBg=",
    "serwisMRIconnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=pYo+Cwak/KwBNF5UdAgkyXtmBqoyi+ERvU0X9W2S2y0=",
    "serwisGiveBackConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=jZsunxSwVlOfVLPSDR9fSLcEA/2j1F3TtVOVqJix/Zs=",
    "DefaultConnection": "Server=tcp:dp102miazkiewicz.database.windows.net,1433;Initial Catalog=CTiMRI;Persist Security Info=False;User ID=k.miazkiewicz;Password=Warszawa1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

  }

DbDat
Dodajemy klase skopiowana z serwis org by umożliwić poprawna deserializacje informacji nadesłanej z serwis org
    public class serwisDbDat
    {
        [Key]
        public int ID_column  { get; set; }
        public int pesel { get; set; }
        public string studyType { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public bool isFerroMagnetic { get; set; }// prawda jesli sa ferromagnetyczne elementy metalowe w ciele
        public bool isNiewydolNerek { get; set; } // prawda jesli pacjent ma niewydolnosc nerek
        public bool isNadczynnoscTarczycy { get; set; }/// prawda jesli pacjent ma nadczynnosc tarczycy

    }

Dane do bazy danych i kolejki
Definiujemy również klasę gromadząca informacje która będzie wykorzystana do zapisu do bazy danych jak i do przesłania informacji do kolejki give it back

   public class serwisMRIDat 
    {
        [Key]
        public int ID_column { get; set; }

        public int Pesel { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public bool isOktoMRI { get; set; }


    }

Kontekst
Definiujemy kontekst bazy danych 

    public class DpDataContext : DbContext
    {
        public DpDataContext(DbContextOptions options) : base(options) { 
        }


        public DbSet<serwisMRIDat> serwisMRIDat{ get; set; }

    }
}




Zdefiniowanie kontrollera
   public class serwisDbController : ControllerBase
    {
        private readonly DpDataContext _context;
        private readonly ServiceBusManager _sender;
        private readonly ILogger _logger;

        public serwisDbController(IConfiguration configuration,DpDataContext context,  ILogger logger) {
            _logger = logger;
            _sender = new ServiceBusManager(configuration,context);
            _sender.Register();
            _context = context;

        }

     
        [HttpPost]
        public IActionResult Add(serwisMRIDat studyData)
        {
            _context.serwisMRIDat.Add(studyData);
            _context.SaveChanges();
            return Ok("");
        }

        [HttpGet]
        public IActionResult getting()
        {



            return Ok("");
        }
    }


ServiceManager

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using DP.Patients.Model;
using System.Net.Http;
using System.Threading;
using System.Net.Http.Headers;

using Ctver3.Model;
using Ctver3.Services;
using IdentityModel.Client;

namespace DP.Patients.NewFolder1
{
    public class ServiceBusManager
    {

        private readonly QueueClient _queueClientSerwisMRI;
        private Model.DpDataContext _context;
        private readonly QueueClient _queueClientserisGiveBack;
        private checkContraindicationsMRI checkContra = new checkContraindicationsMRI();


        private DpDataContext context;
        private static HttpClient client = new HttpClient();

        public IConfiguration Configuration { get; }

        public ServiceBusManager( IConfiguration configuration, DpDataContext context)
        {
            _queueClientserisGiveBack = new QueueClient(configuration.GetConnectionString("serwisGiveBackConnectionString"), "serwisgiveback");
            _queueClientSerwisMRI = new QueueClient(configuration.GetConnectionString("SerwisOrgConnectionString"), "serwismri");
            _context = context;
             Configuration = configuration;
        }



        // najpierw  rejestrujemy sie dokolejki i definiujemy która funkcja będzie wywołana gdy informacja dojdzie
        public async Task Register()
        {



            Console.WriteLine("RRRRRRRRRRRRRRRRRRRRRRRRRR egistered");

            var options = new MessageHandlerOptions((e) => Task.CompletedTask)
            {
                //trzeba rowniez pamietac zeby zamknac wiadomosc by znikneła z kolejki
                AutoComplete = true
            };
            _queueClientSerwisMRI.RegisterMessageHandler(processMessage, options);
        }
        //tu definiujemy co zrobimy z wiadomością gdy zostanie ona odebrana z kolejki

        public async Task processMessage(Message message, CancellationToken token)

        {
            Console.WriteLine("information processsing ");
            var payload = JsonConvert.DeserializeObject<serwisDbDat>(Encoding.UTF8.GetString(message.Body));

            var isContraindication = checkContra.checkContraindication(payload);

            var newDat = new serwisMRIDat()
            {
                Pesel = payload.pesel,

                imie = payload.imie,

                nazwisko = payload.nazwisko,

                isOktoMRI = isContraindication

            };


            await sendToBeSaved(newDat);
            // wysylamy do kolejnej kolejki w celu dalszego przetworzenia informacji

            await sendToGiveBack(newDat);


        }



        private async Task sendToBeSaved(serwisMRIDat dat)
        {

            string token = await GetToken();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);


            string studyJson = System.Text.Json.JsonSerializer.Serialize(dat);
            client.PostAsync("https://localhost:5007/api/serwisMRI", new StringContent(studyJson, Encoding.UTF8, "application/json"));

        }



        /**
  wysylamy do kolejki GiveBack 
  */
        public async Task sendToGiveBack(serwisMRIDat payload)
        {

            string data = JsonConvert.SerializeObject(payload);

            Message message = new Message(Encoding.UTF8.GetBytes(data));

            message.Label = "serwisMRIDat"; // ustawiamy odpowiedni Label  by później móc odpowiednio zdeserializować ten obiekt w GiveBack

            await _queueClientserisGiveBack.SendAsync(message);

        }









        private async Task<string> GetToken()
        {
            using var client = new HttpClient();

            DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
            {
                Address = "https://login.microsoftonline.com/146ab906-a33d-47df-ae47-fb16c039ef96/v2.0/",
                Policy =
          {
          ValidateEndpoints = false
          }
            });

            if (disco.IsError)
                throw new InvalidOperationException(
                $"No discovery document. Details: {disco.Error}");

            var tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "fce95216-40e5-4a34-b041-f287e46532be",
                ClientSecret = "XWGsyzt9uM-Ia2SA8WE7~gvUJ~4og-U_1p",
                Scope = "api://fce95216-40e5-4a34-b041-f287e46532be/.default"
            };

            var token = await client.RequestClientCredentialsTokenAsync(tokenRequest);

            if (token.IsError)
                throw new InvalidOperationException($"Couldn't gather token. Details: {token.Error}");

            return token.AccessToken;
        }





    }






CheckContraindications MRI
Definiujemy nową klasę która na podstawie informacji przekazanych z serwis org  definiuje czy są przeciwwskazania do przeprowadzania rezonansu magnetycznego 

    public class checkContraindicationsMRI
    {
        public bool checkContraindication(DP.Patients.Model.serwisDbDat payload)
        { return !(payload.isFerroMagnetic); }

    }


Serwis give back
Connection Strings

Dodajemy connection strings do appsettings

  "ConnectionStrings": {
    "SerwisOrgConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=XkieCakVC8Na7MVgLKov+GuMHXyQl8MdgNpUo9MG3vc=",
    "SerwisEwusConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=ii2y1gc1Q+zQjVqfpNbKZKjq1l3LVYi+wXtAa8Fp8jw=",
    "serwisCTver3ConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=SFIsryaDyiIkQp32u6pwAXl3izZVds+WjdPF4Jt1nBg=",
    "serwisMRIconnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=pYo+Cwak/KwBNF5UdAgkyXtmBqoyi+ERvU0X9W2S2y0=",
    "serwisGiveBackConnectionString": "Endpoint=sb://ctimrib.servicebus.windows.net/;SharedAccessKeyName=main;SharedAccessKey=jZsunxSwVlOfVLPSDR9fSLcEA/2j1F3TtVOVqJix/Zs=",
    "DefaultConnection": "Server=tcp:dp102miazkiewicz.database.windows.net,1433;Initial Catalog=CTiMRI;Persist Security Info=False;User ID=k.miazkiewicz;Password=Warszawa1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

  }


Dane do bazy danych i wyslania spowrotem do konsoli
Definiujemy również klasę gromadząca informacje która będzie wykorzystana do zapisu do bazy danych jak i do przesłania informacji spowrotem do konsoli 

    public class DataBaseObj
    {
        [Key]
        public int ID_column { get; set; }
        public int Pesel { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public string studyType { get; set; }
        public bool isOktoNFZ { get; set; }
        public bool isSavedToPax { get; set; }


    }




Obiekty do deserializacji kolejki
    public class serwisCTDat
    {
        [Key]
        public int ID_column { get; set; }

        public int Pesel { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public bool isOktoCT { get; set; }


    }

    public class serwisDbDatToSend
    {
        [Key]
        public int ID_column { get; set; }

        public int Pesel { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public bool isSaved { get; set; }


    }

        [Key]
        public int ID_column { get; set; }

        public int Pesel { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public bool isOkInNFZ { get; set; }


    }


    public class serwisMRIDat
    {
        [Key]
        public int ID_column { get; set; }

        public int Pesel { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public bool isOktoMRI { get; set; }


    }





Kontekst
Definiujemy kontekst bazy danych 

    public class DpDataContext : DbContext
    {
        public DpDataContext(DbContextOptions options) : base(options) { 
        }


        public DbSet<DataBaseObj> TableName { get; set; }

    }





ServiceBusManager

Musimy tu
1) odebrać informacje z kolejki serwisGive back 
Zdeserializować odpowiednio w zależności od “label”
Zapisać dane do bazy danych i sprawdzić w nim czy już mamy wszystkie informacje potrzebne do dalszego przetwarzania (informacja z serwisu db, serwisu ct lub mri oraz z serwisu ewus)
Operowac na repozytorium musimy tak by pamietac o resynchronizacji (informacje z kolejki przychodzą asynchronicznie - ale dzięki zapisowi informacji do bazy danych za resynchronizacje w praktyce będzie odpowiadał silnik bazodanowy)

Najpierw definiujemy słownik w którym zgromadzimy nadchodzące informacje
        // gromadzi informacje na temat  danych wiadomości po peselu
        private Dictionary<int,List<Message>>  dictOfMessages = new Dictionary<int, List<Message>>();


Rozszerzenie bazy danych
Tworzymy tabele z których będzie korzystał serwis Org w celu zapisu i sprawdzania obecności informacji przypisanych do danego peselu.



CREATE TABLE localTable(
 ID_column INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
 Pesel int ,
);

CREATE TABLE serwisDbDatForSerwisOrg (
 ID_column INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
 Pesel int ,
 studyType varchar(50) ,
 imie varchar(50),
 nazwisko varchar(50),
isSaved BIT 

);


serwisEwus - zapisywane dane jak imie nazwisko i pesel pacjenta obecna data i czy jest uprawnionym do robienia badań z ramienia NFZ
CREATE TABLE serwisEwusDatForSerwisOrg  (
 ID_column INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
 Pesel int ,
 imie varchar(50),
 nazwisko varchar(50),
isOkinNFZ BIT 

);


serwisCTver3 -  zapisywane dane jak imie nazwisko i pesel pacjenta obecna data i czy nie ma przeciwskazan do CT

CREATE TABLE serwisCTDatForSerwisOrg  (
 ID_column INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
 Pesel int ,
 imie varchar(50),
 nazwisko varchar(50),
isOktoCT BIT 
);

serwisMRI   - zapisywane dane jak imie nazwisko i pesel pacjenta obecna data i czy nie ma przeciwskazan do MRI

CREATE TABLE serwisMRIDatForSerwisOrg  (
 ID_column INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
 Pesel int ,
 imie varchar(50),
 nazwisko varchar(50),
isOktoMRI BIT 
);

Następnie definiujemy odpowiednie konteksty bazodanowe


 public class DpDataContextserwisMRIDat : DbContext
    {
        public DpDataContextserwisMRIDat(DbContextOptions options) : base(options)
        {
        }


        public DbSet<serwisMRIDat> serwisMRIDatForSerwisOrg { get; set; }

    }


public class DpDataContextserwisEwusDat : DbContext
{
    public DpDataContextserwisEwusDat(DbContextOptions options) : base(options)
    {
    }


    public DbSet<serwisEwusDat> serwisEwusDatForSerwisOrg { get; set; }

}



    public class DpDataContextserwisDbDatToSend : DbContext
{
    public DpDataContextserwisDbDatToSend(DbContextOptions options) : base(options)
    {
    }


    public DbSet<serwisDbDatToSend> serwisDbDatForSerwisOrg { get; set; }

}



    public class DpDataContextserwisCTDat : DbContext
{
    public DpDataContextserwisCTDat(DbContextOptions options) : base(options)
    {
    }


    public DbSet<serwisCTDat> serwisCTDatForSerwisOrg { get; set; }

}






Controllery dodatkowe

Definiujemy dodatkowe controllery w celu obsługi tabel każdy ma funkcje dodawania informacji jak i poszukiwania rekordu związanego z danym peselem

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class serwisOrgControllerMRIDAt : ControllerBase
    {
        private readonly DpDataContextserwisMRIDat _context;
        private readonly ServiceBusManager _sender;
        private readonly ILogger _logger;

        public serwisOrgControllerMRIDAt(IConfiguration configuration, DpDataContextserwisMRIDat context,  ILogger logger) {
            _logger = logger;
            _sender.Register();
            _context = context;

        }

     
        [HttpPost]
        public IActionResult Add(serwisMRIDat studyData)
        {
            _context.serwisMRIDatForSerwisOrg.Add(studyData);
            _context.SaveChanges();
            return Ok("");
        }

        [HttpGet]
        public IActionResult getting( int pesel)
        {
            return Ok(_context.serwisMRIDatForSerwisOrg.SingleOrDefault(obj => obj.Pesel == pesel));
        }
    }

}

 



{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class serwisOrgserwisEwusDatController : ControllerBase
    {
        private readonly DpDataContextserwisEwusDat _context;
        private readonly ILogger _logger;

        public serwisOrgserwisEwusDatController(IConfiguration configuration, DpDataContextserwisEwusDat context,  ILogger logger) {
            _logger = logger;
            _context = context;

        }

     
        [HttpPost]
        public IActionResult Add(serwisEwusDat studyData)
        {
            _context.serwisEwusDatForSerwisOrg.Add(studyData);
            _context.SaveChanges();
            return Ok("");
        }

        [HttpGet]
        public IActionResult getting( int pesel)
        {
            return Ok(_context.serwisEwusDatForSerwisOrg.SingleOrDefault(obj => obj.Pesel == pesel));
        }
    }

}




    public class serwisOrgCTDAtController : ControllerBase
    {
        private readonly DpDataContextserwisCTDat _context;
        private readonly ILogger _logger;

        public serwisOrgCTDAtController(IConfiguration configuration, DpDataContextserwisCTDat context,  ILogger logger) {
            _logger = logger;
            _context = context;

        }

     
        [HttpPost]
        public IActionResult Add(serwisCTDat studyData)
        {
            _context.serwisCTDatForSerwisOrg.Add(studyData);
            _context.SaveChanges();
            return Ok("");
        }

        [HttpGet]
        public IActionResult getting( int pesel)
        {
            return Ok(_context.serwisCTDatForSerwisOrg.SingleOrDefault(obj => obj.Pesel == pesel));
        }
    }

}
 


Nastepnie definiujemy wyspecjalizowany kontroller który będzie służył jedynie usunięciu rekordów z tabel związanych z danym numerem pesel

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class deleteByPeselController : ControllerBase
    {
        private readonly DpDataContextserwisMRIDat _contextMRI;
        private readonly DpDataContextserwisEwusDat _contextEwus;
        private readonly DpDataContextserwisDbDatToSend _contextDbPax;
        private readonly DpDataContextserwisCTDat _contextCT;
        private readonly ILogger _logger;

        public deleteByPeselController(DpDataContextserwisMRIDat contextMRI, DpDataContextserwisEwusDat contextEwus, DpDataContextserwisDbDatToSend contextDbPax, DpDataContextserwisCTDat contextCT, ILogger logger)
        {
            _contextMRI = contextMRI;
            _contextEwus = contextEwus;
            _contextDbPax = contextDbPax;
            _contextCT = contextCT;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult getting( int pesel)
        {
           var mri =  _contextMRI.serwisMRIDatForSerwisOrg.SingleOrDefault(obj => obj.Pesel == pesel);
          var ewus =   _contextEwus.serwisEwusDatForSerwisOrg.SingleOrDefault(obj => obj.Pesel == pesel);
           var pax =  _contextDbPax.serwisDbDatForSerwisOrg.SingleOrDefault(obj => obj.Pesel == pesel);
           var ct =  _contextCT.serwisCTDatForSerwisOrg.SingleOrDefault(obj => obj.Pesel == pesel);

            _contextMRI.serwisMRIDatForSerwisOrg.Remove(mri)  ;
            _contextEwus.serwisEwusDatForSerwisOrg.Remove(ewus)  ;
            _contextDbPax.serwisDbDatForSerwisOrg.Remove(pax)   ;
            _contextCT.serwisCTDatForSerwisOrg.Remove(ct)   ;
            return Ok();
        }
    }

}

 



Rejestrujemy contexty bazodanowe  w startup

   public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers( options=> { options.RespectBrowserAcceptHeader = true; }).AddXmlDataContractSerializerFormatters();

            services.AddDbContext<DpDataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<DpDataContextserwisMRIDat>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<DpDataContextserwisEwusDat>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<DpDataContextserwisDbDatToSend>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<DpDataContextserwisCTDat>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
          

            






Wreszcie wywołujemy i definiujemy metody mające przekazać dalej zebrane informacje 
   private async Task sendToBeSaved(serwisGiveBack dat)
        {

            string token = await GetToken();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);


            string studyJson = System.Text.Json.JsonSerializer.Serialize(dat);
            client.PostAsync("https://localhost:5006/api/serwisOrg", new StringContent(studyJson, Encoding.UTF8, "application/json"));

        }

        private async Task sendToConsole(serwisGiveBack dat) {
            string studyJson = System.Text.Json.JsonSerializer.Serialize(dat);

            client.PostAsync("https://localhost:5002/api/Console", new StringContent(studyJson, Encoding.UTF8, "application/json"));
        }



            if (mapToLabels(dictOfMessages[pesel]).Count > 2) {
                var messegeTosend = prepareOutputMessage(pesel);
                sendToBeSaved(messegeTosend);
                sendToConsole(messegeTosend);
 }
           



Pełny kod serwis bus controller




using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using DP.Patients.Model;
using System.Net.Http;
using System.Threading;
using System.Net.Http.Headers;
using serwisDb.Model;
using IdentityModel.Client;
using Ctver3.Model;

namespace DP.Patients.NewFolder1
{
    public class ServiceBusManager
    {

        private Model.DpDataContext _context;
        private readonly QueueClient _queueClientserisGiveBack;
        private int counter = 0;

        private DpDataContext context;
        private static HttpClient client = new HttpClient();
        // gromadzi informacje na temat  danych wiadomości po peselu
      //  private Dictionary<int,List<Message>>  dictOfMessages = new Dictionary<int, List<Message>>();

        public IConfiguration Configuration { get; }

        public ServiceBusManager( IConfiguration configuration, DpDataContext context)
        {
            _queueClientserisGiveBack = new QueueClient(configuration.GetConnectionString("serwisGiveBackConnectionString"), "serwisgiveback");
            _context = context;
             Configuration = configuration;
        }



        // najpierw  rejestrujemy sie dokolejki i definiujemy która funkcja będzie wywołana gdy informacja dojdzie
        public async Task Register()
        {
            Console.WriteLine("RRRRRRRRRRRRRRRRRRRRRRRRRR egistered");

            var options = new MessageHandlerOptions((e) => Task.CompletedTask)
            {
                //trzeba rowniez pamietac zeby zamknac wiadomosc by znikneła z kolejki
                AutoComplete = true,
                MaxConcurrentCalls = 1

            };
            _queueClientserisGiveBack.RegisterMessageHandler(processMessage, options);
        }
        // pobieramy ze wszystkich tabel zapisujacych messege'e  wiadomosci zwiazane z danym peselem  i zwracamy ilosc informacji które sa znalezione w bazie danych (nie null)
        public async Task<int> getInfosOnBasisOfPesel(int pesel) {

            string token = await GetToken();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);

            Console.WriteLine("mmm234m " + await client.GetAsync("https://localhost:5006/api/serwisOrgMRIDAt?pesel=" + pesel).Result.Content.ReadAsStringAsync()) ;
            Console.WriteLine("cc234cc " + await client.GetAsync("https://localhost:5006/api/serwisOrgCTDAt?pesel=" + pesel).Result.Content.ReadAsStringAsync()) ;
            Console.WriteLine("p243ppp " + await client.GetAsync("https://localhost:5006/api/serwisOrgserwisDbDatToSend?pesel=" + pesel).Result.Content.ReadAsStringAsync()) ;
            Console.WriteLine("ee24ee " + await client.GetAsync("https://localhost:5006/api/serwisOrgserwisEwusDat?pesel=" + pesel).Result.Content.ReadAsStringAsync()) ;
            var mri = await client.GetAsync("https://localhost:5006/api/serwisOrgMRIDAt?pesel=" + pesel).Result.Content.ReadAsStringAsync();
            var ct = await client.GetAsync("https://localhost:5006/api/serwisOrgCTDAt?pesel=" + pesel).Result.Content.ReadAsStringAsync();
            var pax = await client.GetAsync("https://localhost:5006/api/serwisOrgserwisDbDat?pesel=" + pesel).Result.Content.ReadAsStringAsync();
            var ewus = await client.GetAsync("https://localhost:5006/api/serwisOrgserwisEwusDat?pesel=" + pesel).Result.Content.ReadAsStringAsync();
            var nullCounter = 0;
            if (mri.Contains(pesel.ToString())) { nullCounter++;
                Console.WriteLine("mmmmmmriOK");
            };
            if (ct.Contains(pesel.ToString())) { nullCounter++;
                Console.WriteLine("ctOK");

            };
            if (pax.Contains(pesel.ToString())) { nullCounter++;

                Console.WriteLine("paxOK");

            };
            if (ewus.Contains(pesel.ToString())) { nullCounter++;
                Console.WriteLine("ewusOK");

            };
              
            return 4-nullCounter;
        }
        public async Task addMessegeToproperTable(Message message)
        {
            var label = message.Label;
            string token = await GetToken();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);



            if (label == "serwisMRIDat")
            {
                var payload = JsonConvert.DeserializeObject<serwisMRIDat>(Encoding.UTF8.GetString(message.Body));
                string studyJson = System.Text.Json.JsonSerializer.Serialize(payload);
                client.PostAsync("https://localhost:5006/api/serwisOrgMRIDAt", new StringContent(studyJson, Encoding.UTF8, "application/json"));

            }
            if (label == "serwisEwus")
            {
                var payload = JsonConvert.DeserializeObject<serwisEwusDat>(Encoding.UTF8.GetString(message.Body));
                string studyJson = System.Text.Json.JsonSerializer.Serialize(payload);
                client.PostAsync("https://localhost:5006/api/serwisOrgserwisEwusDat", new StringContent(studyJson, Encoding.UTF8, "application/json"));

            }
            if (label == "serwisDbDatToSend")
            {
                var payload = JsonConvert.DeserializeObject<serwisDbDatToSend>(Encoding.UTF8.GetString(message.Body));
                string studyJson = System.Text.Json.JsonSerializer.Serialize(payload);
                client.PostAsync("https://localhost:5006/api/serwisOrgserwisDbDatToSend", new StringContent(studyJson, Encoding.UTF8, "application/json"));


            }
            if (label == "serwisCTDat")
            {
                var payload = JsonConvert.DeserializeObject<serwisCTDat>(Encoding.UTF8.GetString(message.Body));
                string studyJson = System.Text.Json.JsonSerializer.Serialize(payload);
                client.PostAsync("https://localhost:5006/api/serwisOrgCTDAt", new StringContent(studyJson, Encoding.UTF8, "application/json"));


            }


        }


        //tu definiujemy co zrobimy z wiadomością gdy zostanie ona odebrana z kolejki

        public async Task processMessage(Message message, CancellationToken token)

        {
            Console.Write("  message.Label  " + message.Label);
            counter += 1;
            Console.Write(" counter " + counter);
            var pesel = getPeselFromMessage(message);
            Console.Write("  pesel " + pesel);




            // sprawdzmy czy mamy już wystarczającą ilość inoformacji (3)

           await addMessegeToproperTable(message);

           var infoAmount =  await getInfosOnBasisOfPesel(pesel);
            Console.WriteLine("infoAmounttttttttttttttttttttttttttttttttttt  "+ infoAmount);
                        //po dodaniu informacji do kolejki sprawdzamy czy już mamy przynajmniej 3 labels
            if (infoAmount>2) {

                Console.WriteLine("sending ");
                            var messegeTosend = await prepareOutputMessage(pesel);
                           await sendToBeSaved(messegeTosend);
                           await sendToConsole(messegeTosend);

                await client.GetAsync("https://localhost:5006/api/deleteByPesel?pesel=" + pesel);
            }
             
                               }




        /**
         przygotowujemy tu wiadomość do zapisania i do wysłania za pomocą protokołu Http do konsoli
         */


        // informacje przypisane do danego peselu

         private async Task<serwisGiveBack> prepareOutputMessage(int pesel) {
              int Pesel  = pesel;
             string imie="";
             string nazwisko="";
             string studyType="";
             bool isOktoNFZ=false;
             bool isSavedToPax= false;
             bool noContraIndications = false;


            var mri = await client.GetAsync("https://localhost:5006/api/serwisOrgMRIDAt?pesel=" + pesel).Result.Content.ReadAsStringAsync();
            var ct = await client.GetAsync("https://localhost:5006/api/serwisOrgCTDAt?pesel=" + pesel).Result.Content.ReadAsStringAsync();
            var pax = await client.GetAsync("https://localhost:5006/api/serwisOrgserwisDbDatToSend?pesel=" + pesel).Result.Content.ReadAsStringAsync();
            var ewus = await client.GetAsync("https://localhost:5006/api/serwisOrgserwisEwusDat?pesel=" + pesel).Result.Content.ReadAsStringAsync();

            Console.WriteLine("aaaaaaaaaaaaaaaaaaazxczxaaaaaaaaaaa" + ewus);
            Console.WriteLine("aaaaaaaaaaaaaaaaazxczxcaaaaaaaaaaaaa" + ct);
            Console.WriteLine("aaaaaaaaaaaaaaazczxcaaaaaaaaaaaaaaa" + pax);
            Console.WriteLine("aaaaaaaaaaazxczxcaaaaaaaaaaaaaaaaaaa" + mri);
   

            if (mri.Contains(pesel.ToString()))
            {
                try
                {
                    var payload = JsonConvert.DeserializeObject<serwisMRIDat>(mri);
                    studyType = "MRI";
                    noContraIndications = payload.isOktoMRI;
                }
                catch (Exception ex) { 
                
                }
                }
                    if (ewus.Contains(pesel.ToString()))
            {   var payload = JsonConvert.DeserializeObject<serwisEwusDat>(ewus);
                        isOktoNFZ = payload.isOkInNFZ;
                    }
                    if (pax.Contains(pesel.ToString()))
            {
                        var payload = JsonConvert.DeserializeObject<serwisDbDatToSend>(pax);
                        imie = payload.imie;
                        nazwisko = payload.nazwisko;
                        isSavedToPax = payload.isSaved;

                    }
            if (ct.Contains(pesel.ToString()))
            {
                try { 
                        var payload = JsonConvert.DeserializeObject<serwisCTDat>(ct);
                studyType = "CT";
                noContraIndications = payload.isOktoCT;
            } 
            catch (Exception ex) { 
            }
                    



                }
                return new serwisGiveBack() {
                    Pesel = Pesel,
                    imie = imie,
                    nazwisko = nazwisko,
                    studyType = studyType,
                    isOktoNFZ = isOktoNFZ,
                    isSavedToPax = isSavedToPax,
                    noContraIndications = noContraIndications

                };
            }
    



        /**
         pobiera informacje o peselu z Messege
         */
        private int getPeselFromMessage (Message message) {
            var label = message.Label;

            if (label == "serwisMRIDat")
            { var payload = JsonConvert.DeserializeObject<serwisMRIDat>(Encoding.UTF8.GetString(message.Body));   return payload.Pesel;

            }
            if (label == "serwisEwus")
            {  var payload = JsonConvert.DeserializeObject<serwisEwusDat>(Encoding.UTF8.GetString(message.Body));   return payload.Pesel;
            }
            if (label == "serwisDbDatToSend")
            {    var payload = JsonConvert.DeserializeObject<serwisDbDatToSend>(Encoding.UTF8.GetString(message.Body));  return payload.Pesel;
                            }
            if (label == "serwisCTDat")
            {
                var payload = JsonConvert.DeserializeObject<serwisCTDat>(Encoding.UTF8.GetString(message.Body));
                return payload.Pesel;

            }
            else return 0;

        }




        private async Task sendToBeSaved(serwisGiveBack dat)
        {

            string token = await GetToken();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);


            string studyJson = System.Text.Json.JsonSerializer.Serialize(dat);
            client.PostAsync("https://localhost:5006/api/serwisOrg", new StringContent(studyJson, Encoding.UTF8, "application/json"));

        }

        private async Task sendToConsole(serwisGiveBack dat) {
            string token = await GetToken();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);


            string studyJson = System.Text.Json.JsonSerializer.Serialize(dat);
            Console.WriteLine("seending to console  ");
            client.PostAsync("https://localhost:5002/api/Console", new StringContent(studyJson, Encoding.UTF8, "application/json"));
        }



        private async Task<string> GetToken()
        {
            using var client = new HttpClient();

            DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
            {
                Address = "https://login.microsoftonline.com/146ab906-a33d-47df-ae47-fb16c039ef96/v2.0/",
                Policy =
          {
          ValidateEndpoints = false
          }
            });

            if (disco.IsError)
                throw new InvalidOperationException(
                $"No discovery document. Details: {disco.Error}");

            var tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "fce95216-40e5-4a34-b041-f287e46532be",
                ClientSecret = "XWGsyzt9uM-Ia2SA8WE7~gvUJ~4og-U_1p",
                Scope = "api://fce95216-40e5-4a34-b041-f287e46532be/.default"
            };

            var token = await client.RequestClientCredentialsTokenAsync(tokenRequest);

            if (token.IsError)
                throw new InvalidOperationException($"Couldn't gather token. Details: {token.Error}");

            return token.AccessToken;
        }





    }










}








Testowanie

L - list

Próba wyswietleia listy badan (pobierane za pomoca protokolu http z serwis db) - komenda l



E - error







Powyzej widoczne celowo bledne wywolanie get


CT bez przeciwwskazań
Dodajemy do systemu badanie CT spodziewany się braku przeciwwskazań do badania


CT i przeciwwskazania



MRI i przeciwwskazania







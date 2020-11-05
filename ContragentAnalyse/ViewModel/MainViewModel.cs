using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ContragentAnalyse.Extension;
using ContragentAnalyse.Model.Entities;
using ContragentAnalyse.Model.Implementation;
using ContragentAnalyse.Model.Interfaces;
using ContragentAnalyse.ViewModel.Commands;
using NPOI;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using NPOI.XWPF.UserModel;
using OfficeOpenXml;
using System.Windows.Controls;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace ContragentAnalyse.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Реализация Singleton
        private static MainViewModel instance;
        public static MainViewModel GetInstance(IDataProvider provider, IEquationProvider eqProvider)
        {
            instance ??= new MainViewModel(provider, eqProvider);
            return instance;
        }
        #endregion

        #region ViewModel методы
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region dataProviders
        readonly IDataProvider _dbProvider;
        private readonly IEquationProvider eqProvider;
        #endregion

        #region Текущие значения
        private ObservableCollection<Client> _foundClients = new ObservableCollection<Client>();
        private ObservableCollection<Criteria> _riskCriteria = new ObservableCollection<Criteria>();
        private ObservableCollection<Country> _country = new ObservableCollection<Country>();
        private ObservableCollection<Contracts> _contracts = new ObservableCollection<Contracts>();
        public ObservableCollection<Client> FoundClients { get => _foundClients; set => _foundClients = value; }
        public ObservableCollection<Criteria> RiskCriteriasList { get => _riskCriteria; set => _riskCriteria = value; }
        public ObservableCollection<Country> CountryList { get => _country; set => _country = value; }

        public ObservableCollection<Contracts> ContractsList { get => _contracts; set => _contracts = value; }
        private ObservableCollection<ContactType> _contactTypes = new ObservableCollection<ContactType>();
        public ObservableCollection<ContactType> ContactTypes
        {
            get => _contactTypes;
            set => _contactTypes = value;
        }
        private ObservableCollection<Criteria> selectedCriterias = new ObservableCollection<Criteria>();
        public ObservableCollection<Criteria> SelectedCriterias => selectedCriterias;
        private ObservableCollection<Contracts> selectedContracts = new ObservableCollection<Contracts>();
        public ObservableCollection<Contracts> SelectedContracts => selectedContracts;
        public delegate void OnSelectedClientChanged();
        public event OnSelectedClientChanged SelectedClientChanged;
        private Client _selectedClient;
        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                SelectedClientChanged?.Invoke();



                if (!IsAnyClientSelected && SelectedClient != null)
                {
                    IsAnyClientSelected = true;
                }
                else
                {
                    IsAnyClientSelected = false;
                }
            }
        }

        public string CurrentClientContracts //добавляем договора в один список, чтобы вывести на экран
        {
            get
            {
                if (SelectedClient != null && SelectedClient.ClientToContracts != null)
                {
                    List<Contracts> contracts = new List<Contracts>();

                    foreach (ClientToContracts pair in SelectedClient.ClientToContracts)
                    {
                        contracts.Add(pair.Contracts);
                    }
                    return string.Join(", ", contracts.Select(i => i.Name));
                }
                else
                {
                    return string.Empty;
                }
            }
        }


        public List<ScoringHistoryGrouped> CurrentClientHistory //история критериев
        {
            get
            {
                if (SelectedClient == null)
                    return null;
                return _dbProvider.GetClientHistory(SelectedClient).ToList();
            }
        }


        public string CurrentClientCurrency //добавляем валюту в один список, чтобы вывести на экран
        {
            get
            {
                if (SelectedClient != null && SelectedClient.ClientToCurrency != null)
                {
                    List<Currency> currency = new List<Currency>();
                    foreach (ClientToCurrency listcurrency in SelectedClient.ClientToCurrency)
                    {
                        currency.Add(listcurrency.Currency);
                    }
                    return string.Join(", ", currency.Select(i => i.Name));
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public BanksProductHistory _bankProductHistory = new BanksProductHistory();
        public BanksProductHistory BankProductHistory
        {
            get => _bankProductHistory;
            set => _bankProductHistory = value;
        }

        public Employees _currentEmployee = new Employees();
        public Employees CurrentEmployee
        {
            get => _currentEmployee;
            set => _currentEmployee = value;
        }

        public ObservableCollection<PrescoringScoringHistory> _selectedHistory = new ObservableCollection<PrescoringScoringHistory>();
        public ObservableCollection<PrescoringScoringHistory> SelectedHistory
        {
            get => _selectedHistory;
            set => _selectedHistory = value;
        }

        private bool _isAnyClientSelected = false;
        public bool IsAnyClientSelected
        {
            get
            {
                return _isAnyClientSelected;
            }
            set
            {
                _isAnyClientSelected = value;
                RaisePropertyChanged(nameof(IsAnyClientSelected));
            }
        }

        //Счета в валюте

        public DateTime? NextScoringDate
        {
            get
            {
                if (SelectedClient != null)
                {
                    return SelectedClient.NextScoringDate;
                }
                else
                {
                    return null;
                }
            }
        } // ctrl + K + F

        private MainViewModel(IDataProvider provider, IEquationProvider eqProvider)
        {
            _dbProvider = provider;
            this.eqProvider = eqProvider;
            Initialize();
            InitializeCommands();
            InitializeData();

        }

        private void Initialize()
        {
            //Если что то нужно подписать на изменения SelectedClient то допиши это сюда
            SelectedClientChanged += () =>
            {
                RaisePropertyChanged(nameof(SelectedClient));
                RaisePropertyChanged(nameof(SelectedHistory));
                RaisePropertyChanged(nameof(NextScoringDate));
                RaisePropertyChanged(nameof(CurrentClientContracts));
                RaisePropertyChanged(nameof(CurrentClientCurrency));
                RaisePropertyChanged(nameof(CurrentClientHistory));
                RaisePropertyChanged(nameof(BankProductHistory));
            };
        }

        /// <summary>
        /// Инициализирует все поля при старте программы
        /// </summary>
        private void InitializeData()
        {
            RiskCriteriasList = new ObservableCollection<Criteria>(_dbProvider.GetCriterias());
            _dbProvider.GetContactTypes().ToList().ForEach(ContactTypes.Add); //???
            RaisePropertyChanged(nameof(RiskCriteriasList));
            CurrentEmployee = _dbProvider.GetCurrentEmployee();
            CountryList = new ObservableCollection<Country>(_dbProvider.GetCountrys());
            RaisePropertyChanged(nameof(CountryList));
        }

        private void InitializeCommands()
        {
            //TODO подставить реализацию команд
            SearchCommand = new MyCommand<string>(SearchMethod);
            AddClientCommand = new MyCommand<string>(AddClientMethod);
            SaveCommand = new MyCommand(SaveAncetaMethod); //сохранение анкеты
            CalculateCommand = new MyCommand<string>(CalculateMethod); //кнопка посчитать
            EstimationRiskCommand = new MyCommand(EstimationRiskMethod); //оценка риска Почему ты используешь команду с параметром, если параметр не нужен?
            SaveRiskRecordCommand = new MyCommand(SaveRiskRecordMethod); //сохранить критерии и уровень риска
            ExportWordCommand = new MyCommand(ExportWordMethod);
            ExportExcelCommand = new MyCommand(ExportExcelCommandMethod);
            SaveChangesCommand = new MyCommand(CommitMethod);
            StoreSelection = new MyCommand<object>(StoreSelectionMethod);
            ContractBankProducts = new MyCommand(ContractBankProductsMethod);
        }

        #endregion
        #region Commands
        public MyCommand<string> SearchCommand { get; set; }
        public MyCommand<string> AddClientCommand { get; set; }
        public MyCommand EditCommand { get; set; }
        public MyCommand SaveCommand { get; set; }
        public MyCommand ExportWordCommand { get; set; }
        public MyCommand<string> CalculateCommand { get; set; }
        public MyCommand EstimationRiskCommand { get; set; }
        public MyCommand  SaveRiskRecordCommand { get; set; }
        public MyCommand ExportExcelCommand { get; set; }
        public MyCommand SaveChangesCommand { get; set; }
        public MyCommand<object> StoreSelection { get; set; }
        public MyCommand ContractBankProducts { get; set; }
    #endregion

    public readonly string TemplateFileName = @"C:\Projects\CounterpartyMonitoring\ContragentAnalyse\Anceta.docx";

        private void ExportWordMethod() // SAVE WORD
        {
            //Ты скопировала этот текст из интернета, а мне предлагаешь поправить.?)) Статьи читай а не видео смотри)
            // Самые основы ООП гласят что один класс отвечает за что то одно. Equation отдельно, БД отдельно, а ворд почему то в куче
            // MainViewModel не должен ничего знать о ворде. Нужно сделать интерфейс и создать имплементацию
            // Microsoft.Interop.Office не совместима с твоим проектом. Найди другую библиотеку. Вводишь Ворд и смотришь предложенные.
            // Ищи бесплатные, которые не отключатся через месяц
            var DateAct = SelectedClient.ActualizationDate;
            var Country = SelectedClient.Country;
            var Contract = SelectedClient.ClientToContracts; // ! не id
            var Currency = SelectedClient.ClientToCurrency;//список валют ! не id
            var RestrictedAccounts = SelectedClient.RestrictedAccounts;
            var COP = SelectedClient.CardOP;
            var AdditionalBIN = SelectedClient.AdditionalBIN;
            var NextScoringDate = this.NextScoringDate;
            var LevelRisk = SelectedClient.Level;
            var BankProducts = SelectedClient.BankProduct;
            var Criteria = SelectedClient.ClientToCriteria;//! не id

            try
            {

                //POITextExtractor worddoc = new POITextExtractor(TemplateFileName) ;
                Application wordApp = new Application();
                //  Document wordDoc = wordApp.Documents.Open(TemplateFileName);

                /*
                ReplaceWordStub("{DateAct}", Convert.ToString(DateAct),wordDocument);
                ReplaceWordStub("{Country}", Convert.ToString(Country), wordDocument);
                ReplaceWordStub("{Contract}", Convert.ToString(Contract), wordDocument);
                ReplaceWordStub("{Currency}", Convert.ToString(Currency), wordDocument);
                ReplaceWordStub("{RestrictedAccounts}", Convert.ToString(RestrictedAccounts), wordDocument);
                ReplaceWordStub("{COP}", Convert.ToString(COP), wordDocument);
                ReplaceWordStub("{AdditionalBIN}", AdditionalBIN, wordDocument);
                ReplaceWordStub("{NextScoringDate}", Convert.ToString(NextScoringDate), wordDocument);
                ReplaceWordStub("{LevelRisk}", LevelRisk, wordDocument);
                ReplaceWordStub("{BankProducts}", BankProducts, wordDocument);
                ReplaceWordStub("{Criteria}", Convert.ToString(Criteria), wordDocument);
              //empl  ReplaceWordStub("{DateAct}", DateAct, wordDocument);
                wordDocument.SaveAs(@"C:\result.docx");
                wordaplication.Visible = true;
                */
            }
            catch { MessageBox.Show("Не выгружается документ"); }
        }
        /* private void ReplaceWordStub(string stubToReplace,string text, Microsoft.Office.Interop.Word.Document worddocument)
         {
             var range = worddocument.Content;
             range.Find.ClearFormatting();
             range.Find.Execute(FindText: stubToReplace, ReplaceWith: text);
         }*/
        
        
        private void SaveAncetaMethod()
        {
            CommitMethod();
        }

        private void SaveRiskRecordMethod() // сохранение истории
        {
            if (SelectedClient.ClientToCriteria == null)
            {
                SelectedClient.ClientToCriteria = new List<ClientToCriteria>();
            }
            SelectedClient.ClientToCriteria.Clear();
            foreach (Criteria criteria in SelectedCriterias)
            {
                SelectedClient.ClientToCriteria.Add(new ClientToCriteria
                {
                    Client = SelectedClient,
                    Criteria = criteria,
                    DateAdd = DateTime.Now
                });
            }
            foreach (Criteria criteria1 in SelectedCriterias)
            {
                SelectedClient.PrescoringScoringHistory.Add(new PrescoringScoringHistory
                {
                    Client = SelectedClient,
                    Employee_Id = CurrentEmployee.Id,
                    DatePresScor = DateTime.Now,
                    Criteria = criteria1,
                    ClosedClient = closedClients    
                });
            }
            CommitMethod();
        }
        public string notbankproduct = "";
        public void ContractBankProductsMethod() //несоответствие банковских продуктов
        {

            if (SelectedClient != null && SelectedClient.ClientToContracts != null)
            {
                notbankproduct = "";
                string contr = "";
                string listcotracts = "";
                List<Contracts> contract = new List<Contracts>();
                foreach (ClientToContracts contrlist in SelectedClient.ClientToContracts)
                {
                    contract.Add(contrlist.Contracts);
                }
                listcotracts = string.Join(",", contract.Select(i => i.Name));
                for (int i = 0; i <= listcotracts.Length - 1; i++)
                {
                    //Сначала выполняется условие слева, потом справа
                    //Условие слева вызывает ошибку - выход за пределы массива
                    //у тебя массив на 309 элементов, соответственно индекс последнего - 308
                    // Для того что бы данная ошибка не появлялась - не обращайся к элементу которого не существует
                    if ((listcotracts[i] != ','))
                    {
                        contr += listcotracts[i];
                    }
                    else if (listcotracts[i] == ',')
                    {
                        if (SelectedClient.BankProduct.IndexOf(contr) <= -1 && (notbankproduct.IndexOf(contr) <= -1))  //если не попадает в список банковских продуктов >-был
                        {

                            notbankproduct += contr + ", ";
                        }
                        contr = "";
                    }
                }
                BankProductHistory.Name = notbankproduct; // это не SelectedHistory--должен быть объект с историей банковских продуктов
            }
        }
 
        private void CalculateMethod(string BINStr)
        {
            if (!string.IsNullOrWhiteSpace(BINStr))
            {
                Criteria[] criteriaslist = _dbProvider.GetCriterialist(BINStr); //найти критерии для клиента по бину  
            }
            else
            {
                MessageBox.Show("Поле ввода не должно быть пустым!");
            }
        }
        public bool closedClients=false;
        private void EstimationRiskMethod()
        {
            closedClients = true;
            if (SelectedClient.Level.IndexOf("Низкий") > -1)
            {
                SelectedClient.NextScoringDate = DateTime.Now.AddYears(2);
                RaisePropertyChanged(nameof(NextScoringDate));
            }
            else
            {
                SelectedClient.NextScoringDate = DateTime.Now.AddYears(1);
                RaisePropertyChanged(nameof(NextScoringDate));
            }
            CommitMethod();
        }

        private void StoreSelectionMethod(object SelectedItems) //метод отвечающий за выбранные критерии добовляет их в SelectedCriterias
        {
            if (SelectedItems is IEnumerable collection)
            {
                SelectedCriterias.Clear();
                foreach (object obj in collection)
                {
                    SelectedCriterias.Add(obj as Criteria);
                }
            }
        }

        private void AddClientMethod(string BINStr)
        {
            if (string.IsNullOrWhiteSpace(BINStr))
            {
                MessageBox.Show("Поле ввода не должно быть пустым!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (_dbProvider.IsAnyClientExist(BINStr))
            {
                MessageBoxResult result = MessageBox.Show("Указанный Клиент существует в БД. Перезаписать?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if(result == MessageBoxResult.Yes)
                {
                    Client returnClient = eqProvider.GetClient(BINStr.ToUpper());
                    /*
                     * returnclient = HTTP://123.ru
                     * SelectedClient = HTTP://456.com
                     * SelectedClient = HTTP://123.ru
                     * HTTP://123.ru = Client198
                     * HTTP://456.com = Client19
                     * SelectedClient.Name = "abc"; - Так можно
                     * */

                    SelectedClient.FullName = returnClient.FullName;
                    SelectedClient.ShortName = returnClient.ShortName;
                    //SelectedClient.ActualizationDate = returnClient.ActualizationDate;
                    SelectedClient.AdditionalBIN = returnClient.AdditionalBIN;
                    SelectedClient.BecomeClientDate = returnClient.BecomeClientDate;
                    SelectedClient.CardOP = returnClient.CardOP;
                    SelectedClient.ClientManager = returnClient.ClientManager;
                    SelectedClient.ClientToContracts = returnClient.ClientToContracts;                 
                    SelectedClient.ClientToCurrency = returnClient.ClientToCurrency;
                    SelectedClient.Country = returnClient.Country;
                    SelectedClient.CurrencyLicence = returnClient.CurrencyLicence;
                    SelectedClient.Employees = returnClient.Employees;
                    SelectedClient.EnglName = returnClient.EnglName;
                    SelectedClient.INN = returnClient.INN;
                    SelectedClient.LicenceEstDate = returnClient.LicenceEstDate;
                    SelectedClient.LicenceNumber = returnClient.LicenceNumber;
                    SelectedClient.Mnemonic = returnClient.Mnemonic;
                    SelectedClient.Name = returnClient.Name;
                    SelectedClient.OGRN = returnClient.OGRN;
                    SelectedClient.OGRN_Date = returnClient.OGRN_Date;
                    SelectedClient.RegDate_RP = returnClient.RegDate_RP;
                    SelectedClient.RegistrationRegion = returnClient.RegistrationRegion;
                    SelectedClient.RegStruct_Name = returnClient.RegStruct_Name;
                    SelectedClient.RestrictedAccount = returnClient.RestrictedAccount;
                    SelectedClient.RKC_BIK = returnClient.RKC_BIK;
                    CommitMethod();
                }
                return;
            }
           // List<String> binlist = new List<String>() { "Y01309", "Y01044", "Y01440", "Y01987", "Y01970", "Y04194", "Y03207", "Y03888", "Y03758", "Y01213", "Y03907", "Y02255", "Y01294", "Y04526", "Y01368", "Y01012", "Y03183", "Y00023", "Y00254", "Y01996", "Y00942", "Y00735", "Y03917", "Y01891", "Y01786", "Y01223", "Y00461", "Y00521", "Y04295", "Y00823", "Y03609", "Y00129", "Y03847", "Y03826", "Y03980", "Y03241", "Y00879", "Y00742", "Y00405", "Y00280", "Y00003", "Y00514", "Y04119", "Y01224", "Y02612", "Y03972", "Y01541", "Y00293", "Y01396", "Y00518", "Y00676", "Y01137", "Y01598", "Y00533", "Y01545", "Y00140", "Y03143", "Y01094", "Y01321", "Y00692", "Y01025", "Y01208", "Y01553", "Y01271", "Y01067", "Y01457", "Y04237", "Y01453", "Y00391", "Y03961", "Y03967", "Y00959", "Y04710", "Y00336", "Y00290", "Y03643", "Y00613", "Y00032", "Y02296", "Y01492", "Y01496", "Y05179", "Y01792", "Y04345", "Y03314", "Y01602", "Y03842", "Y03337", "Y04030", "Y00529", "Y03172", "Y04790", "Y03698", "Y00305", "Y00149", "Y04190", "Y00588", "Y00544", "Y02284", "Y01026", "Y01246", "Y01093", "Y00806", "Y04025", "Y01071", "Y00939", "Y01504", "Y01555", "Y03102", "Y02737", "Y01140", "Y03708", "Y04007", "Y01824", "Y01210", "Y00487", "Y04919", "Y01543", "Y05010", "Y04000", "Y02743", "Y00884", "Y01322", "Y02484", "Y00125", "Y01383", "Y00303", "Y01859", "Y00331", "Y03561", "Y03858", "Y04358", "Y02685", "Y01042", "Y01927", "Y00105", "Y00455", "Y01829", "Y05312", "Y00849", "Y00773", "Y04357", "Y00844", "Y00362", "Y01193", "Y01186", "Y02232", "Y00900", "Y01127", "Y01178", "Y04096", "Y03723", "Y00813", "Y01204", "Y05521", "Y00961", "Y04060", "Y00135", "Y01535", "Y00754", "Y03882", "Y01091", "Y04574", "Y03916", "Y04230", "Y01056", "Y01171", "Y01526", "Y00847", "Y00633", "Y03720", "Y01506", "Y01509", "Y00339", "Y03665", "Y00890", "Y01381", "Y01330", "Y00661", "Y01916", "Y01715", "Y01101", "Y01141", "Y02125", "Y01710", "Y03757", "Y02263", "Y05238", "Y02102", "Y04083", "Y00827", "Y01703", "Y00217", "Y01273", "Y04634", "Y04094", "Y00372", "Y01856", "Y01489", "Y00147", "Y00967", "Y04192", "Y00008", "Y00924", "Y00703", "Y01821", "Y02976", "Y01190", "Y02399", "Y03301", "Y04990", "Y00987", "Y01725", "Y03959", "Y00342", "Y00720", "Y03146", "Y01581", "Y01609", "Y01203", "Y05113", "Y00655", "Y01687", "Y01187", "Y03844", "Y00652", "Y01004", "Y00433", "Y00723", "Y01244", "Y01183", "Y01326", "Y01382", "Y03938", "Y00765", "Y00181", "Y04590", "Y00668", "Y05177", "Y01170", "Y01253", "Y03624", "Y00196", "Y01559", "Y02307", "Y01107", "Y02030", "Y02121", "Y03775", "Y01887", "Y00687", "Y03319", "Y00663", "Y01389", "Y03661", "Y03933", "Y01380", "Y00466", "Y02679", "Y02715", "Y01881", "Y01771", "Y00355", "Y04080", "Y00298", "Y03684", "Y00436", "Y02950", "Y00683", "Y00616", "Y05318", "Y05328", "Y03629", "Y02776", "Y01839", "Y00060", "Y01169", "Y04155", "Y00868", "Y05320", "Y02029", "Y00998", "Y01779", "Y03475", "Y00872", "Y00596", "Y04619", "Y03075", "Y03843", "Y00612", "Y00255", "Y01053", "Y02706", "Y00186", "Y01308", "Y02052", "Y01262", "Y03181", "Y00816", "Y00414", "Y03836", "Y02581", "Y03560", "Y00452", "Y00334", "Y02920", "Y04523", "Y04744", "Y03772", "Y01464", "Y00344", "Y00802", "Y01463", "Y02395", "Y00895", "Y01765", "Y03941", "Y03769", "Y00445", "Y04053", "Y02427", "Y04118", "Y00260", "Y00979", "Y00406", "Y00526", "Y00658", "Y03340", "Y00282", "Y02310", "Y01323", "Y02999", "Y00372", "Y02650", "Y01343", "Y01087", "Y01033", "Y03954", "Y00599", "Y01370", "Y01995", "Y00369", "Y02390", "Y05920", "Y00594", "Y03370", "Y04308", "Y00997", "Y00341", "Y02215", "Y01226", "Y00536", "Y01692", "Y03205", "Y05748", "Y00272", "Y04833", "Y00996", "Y02501", "Y00583", "Y01450", "Y00330", "Y04117", "Y01743", "Y02325", "Y01035", "Y00686", "Y00411", "Y03786", "Y01488", "Y01078", "Y02073", "Y00801", "Y03909", "Y00749", "Y00573", "Y02446", "Y00725", "Y01456", "Y01379", "Y01529", "Y01491", "Y01926", "Y01459", "Y01326", "Y01062", "Y04322", "Y06054" };
           // foreach (string bin in binlist)
           // {
                Client newClient = eqProvider.GetClient(BINStr.ToUpper()); //найти клиента в EQ //BINStr
                if (newClient != null)
                {
                    _dbProvider.AddClient(newClient); //добавить в БД
                    SelectedClient = newClient;
                }
          //  }
            MessageBox.Show("Клиент добавлен!");
        }

        private void SearchMethod(string searchStr)
        {
            if (!string.IsNullOrWhiteSpace(searchStr))
            {
                List<Client> clients = _dbProvider.GetClients(searchStr).ToList();

                //TODO продумать предупреждение о большом количестве результатов поиска при поиске строки '"' или 'А'
                Func<IEnumerable<Client>> GetClientsFunc = GetSearchFunction(searchStr); //Func - какой то метод, который обязуется вернуть IEnumerable<Client>
                FoundClients.Clear();
                FoundClients.AddRange(GetClientsFunc.Invoke()); // .Invoke - вызывает срабатывание метода
            }                                               //FoundClients.AddRange - Extension Method или метод расширения. Реализация тут ContragentAnalyse.Extension
            else
            {
                MessageBox.Show("Поле ввода не должно быть пустым!");
            }
        }

        private Func<IEnumerable<Client>> GetSearchFunction(string searchString)
        {

            //TODO обсудить с каких символов начинается ПИН клиента (Банка или юрлица)
            char[] BinFirstLetters = { 'U', 'Y' };
            if (string.IsNullOrWhiteSpace(searchString)) return null;
            if (searchString.Length == 6 && BinFirstLetters.Any(i => i == searchString.ToUpper()[0]))
                return () => _dbProvider.GetClients(searchString);
            else
                return () => _dbProvider.GetClientsByName(searchString);
        }

        private void CommitMethod()
        {
            //TODO это работает не так
            _dbProvider.Commit();
        }

        //Excel_Risk
        private void ExportExcelCommandMethod()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using (ExcelPackage excel = new ExcelPackage())
            {
                ExcelWorksheet sheet = excel.Workbook.Worksheets.Add("Sheet1");
              //  sheet.Cells[1, 1].Value = "TestExcel";
                sheet.Cells[1, 1].Value = "Дата";
                sheet.Cells[1, 2].Value = "Вид оценки";
                sheet.Cells[1, 3].Value = "Уровень оценки";
                //linq запрос
                //Работает с коллекциями
                List<PrescoringScoringHistory> history = new List<PrescoringScoringHistory>();
                // IEnumerable
                // 1 1 1 1 1 1 1 1
                //     
                // 1 1
                // 2
                //Нужно узнать на сколько объектов запускать цикл?
                string levelRisk = SelectedClient.Level;
                int HistoryRecordsCount = SelectedHistory.Where(i => i.Client_Id == SelectedClient.Id).Count();//
                int currentRow = 2;
                int currentCol = 1;
                foreach(PrescoringScoringHistory record in SelectedHistory)
                {
                    sheet.Cells[currentRow, 1].Value = record.DatePresScor;
                   /* sheet.Cells[currentRow, 2].Value = record.Criteria.Name;
                    sheet.Cells[currentRow++, 3].Value = "";*/
                }
                //string readedValue = sheet.Cells[1, 1].Text;
                excel.SaveAs(new System.IO.FileInfo("NewExcel.xlsx"));
            }
            MessageBox.Show("Файл скачен!");
        }
    }
}

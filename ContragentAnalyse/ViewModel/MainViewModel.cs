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
using WordAdapterLib;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using NPOI.XWPF.UserModel;
using System.Windows.Controls;

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
        /*    //ViewModel
          //  public ICollectionView BusinessCollection { get; set; }
            public List<RiskCriteriasList> SelectedObject { get; set; }

            //Codebehind
            private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                MainViewModel DataContext = null;
                var viewmodel = (MainViewModel)DataContext;
                viewmodel.SelectedItems = selectoritems.SelectedItems
                    .Cast<RiskCriteriasList>()
                    .ToList();
            }*/
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

        private Client _selectedClient;
        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                RaisePropertyChanged(nameof(SelectedClient));
                RaisePropertyChanged(nameof(NextScoringDate));

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
        //История PrescoringScoring
        private PrescoringScoringHistory _selectedScoringHistory;
        public PrescoringScoringHistory SelectedScoringHistory
        {
            get => _selectedScoringHistory;
            set
            {
                _selectedScoringHistory = value;
                RaisePropertyChanged(nameof(SelectedScoringHistory));
            }
        }

        public string CurrentClientContracts //добавляем договора в один список, чтобы вывести на экран
        {
            get
            {
                if(SelectedClient != null)
                {
                    List<Contracts> contracts = new List<Contracts>();
                    foreach(ClientToContracts pair in SelectedClient.ClientToContracts)
                    {
                        contracts.Add(pair.Contracts);
                    }
                    return string.Join(", ", contracts);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string CurrentClientCurrency //добавляем валюту в один список, чтобы вывести на экран
        {
            get
            {
                if (SelectedClient != null)
                {
                    List<Currency> currency = new List<Currency>();
                    foreach (ClientToCurrency listcurrency in SelectedClient.ClientToCurrency)
                    {
                        currency.Add(listcurrency.Currency);
                    }
                    return string.Join(", ", currency);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public Employees _currentEmployee = new Employees();
        public Employees CurrentEmployee
        {
            get => _currentEmployee;
            set => _currentEmployee = value;
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
            InitializeCommands();
            InitializeData();

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
            ExportExcelCommand = new MyCommand(() => MessageBox.Show($"Exel"));
            SaveChangesCommand = new MyCommand(CommitMethod);
            StoreSelection = new MyCommand<object>(StoreSelectionMethod);
        }


        public string history;
        #endregion
        #region Commands
        public MyCommand<string> SearchCommand { get; set; }
        public MyCommand<string> AddClientCommand { get; set; }
        public MyCommand EditCommand { get; set; }
        public MyCommand SaveCommand { get; set; }
        public MyCommand ExportWordCommand { get; set; }
        public MyCommand<string> CalculateCommand { get; set; }
        public MyCommand EstimationRiskCommand { get; set; }
        public MyCommand SaveRiskRecordCommand { get; set; }
        public MyCommand ExportExcelCommand { get; set; }
        public MyCommand SaveChangesCommand { get; set; }
        public MyCommand<object> StoreSelection { get; set; }
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
            var NextScoringDate= this.NextScoringDate;
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
        private void EstimationRiskMethod()
        {
            if (SelectedClient.Level.IndexOf("Низкий") >-1)
              {
                  SelectedClient.NextScoringDate= DateTime.Now.AddYears(2);
                  RaisePropertyChanged(nameof(NextScoringDate));
              }
              else
              {
                  SelectedClient.NextScoringDate = DateTime.Now.AddYears(1);
                 RaisePropertyChanged(nameof(NextScoringDate));
              }
            CommitMethod();    
        }
        //private void AddRequestsMethod()
        //{
        //    //проверка на null
        //    if (SelectedClient.Requests == null) //??? нет ссылки на объект "Ссылка на объект не установлена на экземпляр объекта"    Контрагентанализ.модель представления.MainViewModel.SelectedClient.get возвращает null.
        //    {
        //        SelectedClient.Requests = new ObservableCollection<Request>();
        //    }
        //    SelectedClient.Requests.Add(new Request
        //    {
        //        SendDate = DateTime.Now
        //    });
        //    RaisePropertyChanged(nameof(SelectedClient.Requests));
        //}

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
                    Criteria = criteria
                });
            }
            //Добавить новое поле в PrescoringScoringHistory - метод;
            SelectedScoringHistory.Employee_Id = CurrentEmployee.Id; 
             SelectedScoringHistory.DatePresScor = DateTime.Now;
           SelectedScoringHistory.Client_Id = SelectedClient.Id;
            RaisePropertyChanged(nameof(SelectedScoringHistory));
            /*history = Convert.ToString(DateTime.Now) + " " + CurrentEmployee.Name + " "+SelectedClient.NextScoringDate; //не верно
            RaisePropertyChanged(nameof(history));*/
            CommitMethod();  
        }

        private void CalculateMethod(string BINStr)
        {
            if (!string.IsNullOrWhiteSpace(BINStr))
            {
                Criteria[] criteriaslist = _dbProvider.GetCriterialist(BINStr); //найти критерии для клиента по бину
                //_dbProvider.AddCriteriaList(criteriaslist); //Посчитать сумму баллов критериев 
            }
            else
            {
                MessageBox.Show("Поле ввода не должно быть пустым!");
            }
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
            if (!string.IsNullOrWhiteSpace(BINStr.ToUpper()))
            {
                Client newClient = eqProvider.GetClient(BINStr.ToUpper()); //найти клиента в EQ
                if (newClient != null)
                {
                    _dbProvider.AddClient(newClient); //добавить в БД
                    SelectedClient = newClient;
                }
            }
            else
            {
                MessageBox.Show("Поле ввода не должно быть пустым!");
            }
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

        //
    }
}

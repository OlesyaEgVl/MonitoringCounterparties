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
        private ObservableCollection<Contracts> _contracts = new ObservableCollection<Contracts>();
        public ObservableCollection<Client> FoundClients { get => _foundClients; set => _foundClients = value; }
        public ObservableCollection<Criteria> RiskCriteriasList { get => _riskCriteria; set => _riskCriteria = value; }
        public ObservableCollection<Contracts> ContractsList { get => _contracts; set => _contracts = value; }
        private ObservableCollection<ContactType> _contactTypes = new ObservableCollection<ContactType>();
        public ObservableCollection<ContactType> ContactTypes
        {
            get => _contactTypes;
            set => _contactTypes = value;
        }

        private ObservableCollection<Criteria> selectedCriterias = new ObservableCollection<Criteria>();
        public ObservableCollection<Criteria> SelectedCriterias => selectedCriterias;
        private Client _selectedClient;
        //private DateTime _nextScoringDate;
        public Employees _currentEmployee;
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
        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                RaisePropertyChanged(nameof(SelectedClient));

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
     
        public DateTime? nextScoringDate
        {
            get
            { 
                if (SelectedClient != null)
                    {
                    
                    return (DateTime)SelectedClient.NextScoringDate;
                    
                }
                    else
                    {
                        return null;
                    }
             }
        }
        
        private MainViewModel(IDataProvider provider, IEquationProvider eqProvider)
        {
            _dbProvider = provider;
            this.eqProvider = eqProvider;
            InitializeCommands();
            InitializeData();

        }
         public Employees LoadData
          {
              get => _currentEmployee;
              set
              {
                  _currentEmployee = value;
                  RaisePropertyChanged(nameof(LoadData));
                  if (SelectedClient.Employees.CodeName == Environment.UserName)
                  {
                      _currentEmployee = SelectedClient.Employees;
                  } 
              }
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

        

        private void InitializeData()
        {
            RiskCriteriasList = new ObservableCollection<Criteria>(_dbProvider.GetCriterias());
            _dbProvider.GetContactTypes().ToList().ForEach(ContactTypes.Add); //???
            RaisePropertyChanged(nameof(RiskCriteriasList));

          //  ContractsList = new ObservableCollection<Contracts>(_dbProvider.GetContracts());
          //  RaisePropertyChanged(nameof(ContractsList));
            //???
           
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
            var NextScoringDate= nextScoringDate;
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
                  RaisePropertyChanged(nameof(nextScoringDate));
              }
              else
              {
                  SelectedClient.NextScoringDate = DateTime.Now.AddYears(1);
                 RaisePropertyChanged(nameof(nextScoringDate));
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
            history = Convert.ToString(DateTime.Now) + " " +LoadData.Name+ " "/*+SelectedClient.NextScoringDate*/;
            RaisePropertyChanged(nameof(history));

            CommitMethod();
           
            /*SelectedCriterias.Clear(); 
            SelectedClient.ClientToCriteria.Clear();*/
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

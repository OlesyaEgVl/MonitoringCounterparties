using ContragentAnalyse.Extension;
using ContragentAnalyse.Model.Entities;
using ContragentAnalyse.Model.Interfaces;
using ContragentAnalyse.ViewModel.Commands;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using static ContragentAnalyse.Model.Implementation.EquationProvider;
using LicenseContext = OfficeOpenXml.LicenseContext;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ContragentAnalyse.Reports;
using System.Diagnostics;

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
        private readonly IDataProvider _dbProvider;
        private readonly IEquationProvider eqProvider;
        #endregion

        #region Текущие значения
        bool isSelectionActive = false;
        private Scoring scoring;
        public Scoring SelectedScoring
        {
            get => scoring;
            set
            {
                scoring = value;
                RaisePropertyChanged(nameof(SelectedScoring));
            }
        }
        private Client _selectedClient;
        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                RaisePropertyChanged(nameof(SelectedClient));
            }
        }
        public Employee CurrentEmployee { get; set; } = new Employee();
        public ObservableCollection<Contracts> SelectedContracts { get; } = new ObservableCollection<Contracts>();
        public ObservableCollection<Criteria> SelectedCriterias { get; set; } = new ObservableCollection<Criteria>();
        #endregion

        #region DataCollections
        public ObservableCollection<Client> ClientsFound { get; set; } = new ObservableCollection<Client>();
        public ObservableCollection<Criteria> AllCriterias { get; set; } = new ObservableCollection<Criteria>();
        public ObservableCollection<Country> AllCountries { get; set; } = new ObservableCollection<Country>();
        public ObservableCollection<Contracts> ContractsList { get; set; } = new ObservableCollection<Contracts>();
        public ObservableCollection<ContactType> ContactTypes { get; set; } = new ObservableCollection<ContactType>();

        #endregion

        #region ViewModelProperties
        
        #endregion

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
            
        }

        private void SelectedCriterias_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (SelectedScoring == null)
            {
                return;
            }
            if (e.OldItems != null)
            {
                IEnumerable<Criteria> itemsToDelete = e.OldItems.Cast<Criteria>();
                foreach (Criteria cr in itemsToDelete)
                {
                    SelectedScoring.Criterias.Remove(SelectedScoring.Criterias.Single(i => i.CriteriaId == cr.Id));
                }
            }

            if (e.NewItems != null)
            {
                List<Criteria> itemsToAdd = new List<Criteria>(e.NewItems.Cast<Criteria>());
                foreach (Criteria cr in itemsToAdd)
                {
                    SelectedScoring.Criterias.Add(new ScoringToCriteria { Criteria = cr, Scoring = SelectedScoring, CriteriaId=cr.Id, ScoringId=SelectedScoring.Id });
                }
            }
            
            
        }

        /// <summary>
        /// Инициализирует все поля при старте программы
        /// </summary>
        private void InitializeData()
        {
            AllCriterias = new ObservableCollection<Criteria>(_dbProvider.GetCriterias());
            _dbProvider.GetContactTypes().ToList().ForEach(ContactTypes.Add); //???
            RaisePropertyChanged(nameof(AllCriterias));
            CurrentEmployee = _dbProvider.GetCurrentEmployee();
            AllCountries = new ObservableCollection<Country>(_dbProvider.GetCountries());
            RaisePropertyChanged(nameof(AllCountries));
        }

        #region Commands
        public MyCommand<string> SearchCommand { get; set; }
        public MyCommand<Scoring> SelectHistoryRecord { get; set; }
        public MyCommand<string> AddClientCommand { get; set; }
        public MyCommand EditCommand { get; set; }
        public MyCommand SaveCommand { get; set; }
        public MyCommand ExportWordCommand { get; set; }
        public MyCommand CloseScoringCommand { get; set; }
        public MyCommand SaveRiskRecordCommand { get; set; }
        public MyCommand<string> ExportExcelCommand { get; set; }
        public MyCommand SaveChangesCommand { get; set; }
        public MyCommand<object> StoreSelection { get; set; }
        public MyCommand CalculateRiskLevel { get; set; }
        public MyCommand<string> RefreshClientInfoFromEq { get; set; }
        public MyCommand UnloadingExcelCommand { get; set; }
        public MyCommand AddScoringCommand { get; set; }
        #endregion

        private void InitializeCommands()
        {
            SearchCommand = new MyCommand<string>(SearchMethod);
            AddClientCommand = new MyCommand<string>(AddClientMethod);
            SaveCommand = new MyCommand(SaveAncetaMethod); //сохранение анкеты
            CloseScoringCommand = new MyCommand(CloseScoringAction); //оценка риска Почему ты используешь команду с параметром, если параметр не нужен?
            SaveRiskRecordCommand = new MyCommand(SaveRiskRecordMethod); //сохранить критерии и уровень риска
            ExportWordCommand = new MyCommand(ExportWordMethod);
            ExportExcelCommand = new MyCommand<string>(ExportExcelCommandMethod);
            SaveChangesCommand = new MyCommand(CommitMethod);
            StoreSelection = new MyCommand<object>(StoreSelectionMethod);
            CalculateRiskLevel = new MyCommand(CalculateRiskLevelAction);
            RefreshClientInfoFromEq = new MyCommand<string>(RefreshClientInfoFromEquationAction);
            UnloadingExcelCommand = new MyCommand(UnloadingExcelMethod);
            SelectHistoryRecord = new MyCommand<Scoring>(SelectHistoryRecordMethod);
            AddScoringCommand = new MyCommand(AddScoringMethod);
        }

        private void UnloadingExcelMethod()
        {
            ExcelReports excel = new ExcelReports();
            excel.UnloadingExcelMethod(_dbProvider.GetClients());
        }

        private void ExportExcelCommandMethod(string obj)
        {
            ExcelReports excel = new ExcelReports();
            excel.ExportExcelCommandMethod(SelectedClient);
        }

        private void ExportWordMethod()
        {
            Wordreports word = new Wordreports();
            word.ExportWordMethod(SelectedClient, CurrentEmployee);
        }

        private void AddScoringMethod()
        {
            Scoring newScoring = new Scoring
            {
                Client = SelectedClient,
                Employee = CurrentEmployee,
                IsClosed = false
            };
            _dbProvider.AddScoring(newScoring);
        }

        private void SelectHistoryRecordMethod(Scoring record)
        {
            isSelectionActive = true;
            SelectedCriterias.Clear();
            foreach(Criteria criteria in record.Criterias.Select(i => i.Criteria))
            {
                SelectedCriterias.Add(criteria);
            }
            isSelectionActive = false;
        }

        private void SaveAncetaMethod()
        {
            _dbProvider.Commit();
        }

        private void SaveRiskRecordMethod()
        {
            _dbProvider.SaveScoring(SelectedScoring);
        }

        /// <summary>
        /// Пересчитываем уровень риска из файлов excel + выбранные критерии
        /// </summary>
        public void CalculateRiskLevelAction() //несоответствие банковских продуктов
        {
            ExcelReports excel = new ExcelReports();
            excel.Calculate(SelectedClient, SelectedScoring);
            MessageBox.Show("Уровень риска посчитан", "Операция завершена успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Закрываем скоринг для редактирования
        /// </summary>
        private void CloseScoringAction()
        {
            SelectedScoring.IsClosed = true;
            if(SelectedClient.Scorings.Last().Criterias.Select(i=>i.Criteria.Weight).Sum() <= 3.4)
            {
                SelectedClient.NextScoringDate = DateTime.Now.AddYears(2);
            }
            else
            {
                SelectedClient.NextScoringDate = DateTime.Now.AddYears(1);
            }
            _dbProvider.Commit();
        }

        /// <summary>
        /// Метод перезаписывает выбранные критерии в коллекцию SelectedCriterias
        /// </summary>
        /// <param name="SelectedItems"></param>
        private void StoreSelectionMethod(object SelectedItems)
        {
            if (SelectedItems is IList collection && !isSelectionActive)
            {
                List<Criteria> criteriasToDelete = SelectedScoring.Criterias.Select(i=>i.Criteria).Where(i => !collection.Cast<Criteria>().Any(c => c.Id == i.Id)).ToList();
                List<Criteria> criteriasToAdd = collection.Cast<Criteria>().Where(i => !SelectedScoring.Criterias.Select(i=>i.Criteria).Any(c => c.Id == i.Id)).ToList();
                
                foreach (Criteria criteria in criteriasToDelete)
                {
                    SelectedCriterias.Remove(criteria);
                    SelectedScoring.Criterias.Remove(SelectedScoring.Criterias.Single(i => i.CriteriaId == criteria.Id));
                }
                foreach(Criteria criteria in criteriasToAdd)
                {
                    SelectedCriterias.Add(criteria);
                    SelectedScoring.Criterias.Add(new ScoringToCriteria { Criteria = criteria, Scoring = SelectedScoring, CriteriaId = criteria.Id, ScoringId = SelectedScoring.Id });
                }
            }
        }

        /// <summary>
        /// Обновить поля клиента из equation
        /// </summary>
        /// <param name="clientBIN"></param>
        private void RefreshClientInfoFromEquationAction(string clientBIN)
        {
            if (_dbProvider.IsAnyClientExist(clientBIN))
            {
                Client clientFromEquation;
                try
                {
                    clientFromEquation = eqProvider.GetClient(clientBIN);
                }
                catch
                {
                    MessageBox.Show("Отсутствует соединение с Equation. Проверьте соединение и повторите попытку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                SelectedClient.FullName = clientFromEquation.FullName;
                SelectedClient.ShortName = clientFromEquation.ShortName;
                if(clientFromEquation.Actualization.Count > 0)
                {
                    SelectedClient.Actualization.Add(clientFromEquation.Actualization[0]);
                }
                SelectedClient.AdditionalBIN = clientFromEquation.AdditionalBIN;
                SelectedClient.BecomeClientDate = clientFromEquation.BecomeClientDate;
                SelectedClient.CardOP = clientFromEquation.CardOP;
                SelectedClient.SEB = clientFromEquation.SEB;
                SelectedClient.ClientManager = clientFromEquation.ClientManager;
                SelectedClient.ClientToContracts = clientFromEquation.ClientToContracts;
                SelectedClient.ClientToCurrency = clientFromEquation.ClientToCurrency;
                SelectedClient.Country = clientFromEquation.Country;
                SelectedClient.CurrencyLicence = clientFromEquation.CurrencyLicence;
                SelectedClient.Employees = clientFromEquation.Employees;
                SelectedClient.EnglName = clientFromEquation.EnglName;
                SelectedClient.INN = clientFromEquation.INN;
                SelectedClient.LicenceEstDate = clientFromEquation.LicenceEstDate;
                SelectedClient.LicenceNumber = clientFromEquation.LicenceNumber;
                SelectedClient.Mnemonic = clientFromEquation.Mnemonic;
                SelectedClient.Name = clientFromEquation.Name;
                SelectedClient.OGRN = clientFromEquation.OGRN;
                SelectedClient.OGRN_Date = clientFromEquation.OGRN_Date;
                SelectedClient.RegDate_RP = clientFromEquation.RegDate_RP;
                SelectedClient.RegistrationRegion = clientFromEquation.RegistrationRegion;
                SelectedClient.RegStruct_Name = clientFromEquation.RegStruct_Name;
                SelectedClient.RestrictedAccount = clientFromEquation.RestrictedAccount;
                SelectedClient.RKC_BIK = clientFromEquation.RKC_BIK;
                _dbProvider.Commit();
            }
            MessageBox.Show("Данные клиента обновлены", "Операция выполнена успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Добавление клиента
        /// </summary>
        /// <param name="clientBin"></param>
        private void AddClientMethod(string clientBin)
        {
            if (string.IsNullOrWhiteSpace(clientBin))
            {
                MessageBox.Show("Поле ввода не должно быть пустым!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (_dbProvider.IsAnyClientExist(clientBin))
            {
                MessageBox.Show("Данный клиент уже существует!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
                     
            Client newClient = eqProvider.GetClient(clientBin);
            if (newClient != null)
            {
                _dbProvider.AddClient(newClient);
                SelectedClient = newClient;
            }
            MessageBox.Show("Клиент добавлен!", "Операция завершена успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Поиск клиента
        /// </summary>
        /// <param name="searchStr"></param>
        private void SearchMethod(string searchStr)
        {
            if (string.IsNullOrWhiteSpace(searchStr))
            {
                MessageBox.Show("Поле ввода не должно быть пустым!");
                return;
            }
            Func<IEnumerable<Client>> GetClientsFunc = GetSearchFunction(searchStr);
            ClientsFound.Clear();
            ClientsFound.AddRange(GetClientsFunc.Invoke());
        }

        /// <summary>
        /// Функция выбора алгоритма для поиска клиента
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        private Func<IEnumerable<Client>> GetSearchFunction(string searchString)
        {
            char[] BinFirstLetters = { 'U', 'Y' };
            if (searchString.Length == 6 && BinFirstLetters.Any(i => i == searchString.ToUpper()[0]))
                return () => _dbProvider.GetClients(searchString);
            else
                return () => _dbProvider.GetClientsByName(searchString);
        }

        private void CommitMethod()
        {
            _dbProvider.Commit();
        }
      
        
    }
}

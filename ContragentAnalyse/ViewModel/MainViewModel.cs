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

using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;

namespace ContragentAnalyse.ViewModel
{
    public class MainViewModel :INotifyPropertyChanged
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
        public ObservableCollection<Client> FoundClients { get => _foundClients; set => _foundClients = value; }
        public ObservableCollection<Criteria> RiskCriteriasList { get => _riskCriteria; set => _riskCriteria = value; }


        private ObservableCollection<Criteria> selectedCriterias = new ObservableCollection<Criteria>();
        public ObservableCollection<Criteria> SelectedCriterias => selectedCriterias;
       
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
        #endregion

        List<string>DictionaryCountry = new List<string>() { "Австралия", "Австрия", "Азербайджан", "Армения"};
        
        // код, заполняющий словарь

        // возвращает массив строк из словаря, соответствующих паттерну
        List<string> GetItems(string pattern)
        {
            List<string> ret = new List<string>();
            foreach (string s in DictionaryCountry)
                if (s.StartsWith(pattern))
                    ret.Add(s);

            return ret;
        }
        
        #region Commands
        public MyCommand<string> SearchCommand { get; set; }
        public MyCommand <string> AddClientCommand { get; set; }
        public MyCommand EditCommand { get; set; }
        public MyCommand SaveCommand { get; set; }
        public MyCommand ExportWordCommand { get; set; }
        public MyCommand<string> CalculateCommand { get; set; }
        public MyCommand SaveRiskRecordCommand { get; set; }
        public MyCommand ExportExcelCommand { get; set; }
        public MyCommand SaveChangesCommand { get; set; }
        public MyCommand<object> StoreSelection { get; set; }
        #endregion

        private MainViewModel(IDataProvider provider, IEquationProvider eqProvider)
        {
            _dbProvider = provider;
            this.eqProvider = eqProvider;
            InitializeCommands();
            InitializeData();
            //eqProvider.reader(); /* Метод выбран, просто, чтобы работал код. Сейчас ничего нужного не выполняет*/
        }

        private void InitializeData()
        {
            RiskCriteriasList = new ObservableCollection<Criteria>(_dbProvider.GetCriterias());
            RaisePropertyChanged(nameof(RiskCriteriasList));
        }

        private void InitializeCommands()
        {
            //TODO подставить реализацию команд
            SearchCommand = new MyCommand<string>(SearchMethod);
            AddClientCommand = new MyCommand<string>(AddClientMethod);
            EditCommand = new MyCommand(() => MessageBox.Show($"Редактировать"));
            SaveCommand = new MyCommand(SaveAncetaMethod);
            CalculateCommand = new MyCommand<string>(CalculateMethod);
            SaveRiskRecordCommand = new MyCommand(SaveRiskRecordMethod); ;//сохранить критерии и уровень риска
            ExportWordCommand = new MyCommand(()=> MessageBox.Show($"Скачать Word"));
            ExportExcelCommand = new MyCommand(()=> MessageBox.Show($"Exel"));
            SaveChangesCommand = new MyCommand(CommitMethod);
            StoreSelection = new MyCommand<object>(StoreSelectionMethod);
        }

        private void SaveAncetaMethod()
        {
            throw new NotImplementedException();
        }

       

        private void SaveRiskRecordMethod()
        {
            
        }

        private void CalculateMethod(string BINStr)
        {
            if (!string.IsNullOrWhiteSpace(BINStr))
            {
                Criteria [] criteriaslist = _dbProvider.GetCriterialist(BINStr); //найти критерии для клиента по бину
                _dbProvider.AddCriteriaList(criteriaslist); //Посчитать сумму баллов критериев
                
            }
            else
            {
                MessageBox.Show("Поле ввода не должно быть пустым!");
            }
        }

        private void StoreSelectionMethod(object SelectedItems)
        {
            if(SelectedItems is IEnumerable collection)
            {
                SelectedCriterias.Clear();
                foreach(object obj in collection)
                {
                    SelectedCriterias.Add(obj as Criteria);
                }

            }
        }

        private void AddClientMethod(string BINStr)
        {
            if (!string.IsNullOrWhiteSpace(BINStr))
            {
                Client newClient = eqProvider.GetClient(BINStr); //найти клиента в EQ
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

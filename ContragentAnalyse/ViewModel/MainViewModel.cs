using System;
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
        public static MainViewModel GetInstance(IDataProvider provider)
        {
            instance ??= new MainViewModel(provider);
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
        #endregion

        #region Текущие значения
        private ObservableCollection<Client> _foundClients = new ObservableCollection<Client>();
        private ObservableCollection<Criteria> _riskCriteria = new ObservableCollection<Criteria>();
        public ObservableCollection<Client> FoundClients { get => _foundClients; set => _foundClients = value; }
        public ObservableCollection<Criteria> RiskCriteriasList { get => _riskCriteria; set => _riskCriteria = value; }

        //подсказки в поле поиска Риски
        public Stack<string> subjectFromDB;
        private ObservableCollection<string> _subjectHints = new ObservableCollection<string>();
        public ObservableCollection<string> SubjectHints { get =>_subjectHints; set => _subjectHints = value;}

        //private Criteria _selectedRisk;
        public Criteria SelectedRisk { get; set; }
       /* {
            get => _selectedRisk;
            set
            {
                _selectedRisk = value;
                RaisePropertyChanged(nameof(SelectedRisk));
            }
        }
       */
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

        #region Commands
        public MyCommand<string> SearchCommand { get; set; }
        public MyCommand AddClientCommand { get; set; }
        public MyCommand EditCommand { get; set; }
        public MyCommand SaveCommand { get; set; }
        public MyCommand ExportWordCommand { get; set; }
        public MyCommand CalculateCommand { get; set; }
        public MyCommand SaveRiskRecordCommand { get; set; }
        public MyCommand ExportExcelCommand { get; set; }
        public MyCommand SaveChangesCommand { get; set; }
        #endregion

        private MainViewModel(IDataProvider provider)
        {
            _dbProvider = provider;
            InitializeCommands();
            InitializeData();
        }

        private void InitializeData()
        {

        }

        private void InitializeCommands()
        {
            //TODO подставить реализацию команд
            SearchCommand = new MyCommand<string>(SearchMethod);
            AddClientCommand = new MyCommand(()=> MessageBox.Show($"Добавить нового клиента"));
            EditCommand = new MyCommand(() => MessageBox.Show($"Редактировать"));
            SaveCommand = new MyCommand(()=>MessageBox.Show($"Сохранить изменения"));
            CalculateCommand = new MyCommand(()=> MessageBox.Show($"Посчитать"));
            SaveRiskRecordCommand = new MyCommand(()=> MessageBox.Show($"Сохранить историю"));
            ExportWordCommand = new MyCommand(()=> MessageBox.Show($"Скачать Word"));
            ExportExcelCommand = new MyCommand(()=> MessageBox.Show($"Exel"));
            SaveChangesCommand = new MyCommand(CommitMethod);
        }


        private void SearchMethod(string searchStr)
        {
            //TODO продумать предупреждение о большом количестве результатов поиска при поиске строки '"' или 'А'
            Func<IEnumerable<Client>> GetClientsFunc = GetSearchFunction(searchStr); //Func - какой то метод, который обязуется вернуть IEnumerable<Client>
            FoundClients.Clear();
            FoundClients.AddRange(GetClientsFunc?.Invoke()); // .Invoke - вызывает срабатывание метода
            //FoundClients.AddRange - Extension Method или метод расширения. Реализация тут ContragentAnalyse.Extension
        }

        private Func<IEnumerable<Client>> GetSearchFunction(string searchString)
        {
            //TODO обсудить с каких символов начинается ПИН клиента (Банка или юрлица)
            char[] BinFirstLetters = { 'U', 'Y' };
            if (string.IsNullOrWhiteSpace(searchString)) return null;
            if(searchString.Length == 6 && BinFirstLetters.Any(i=>i == searchString.ToUpper()[0]))
                return () => _dbProvider.GetClients(searchString);
            else
                return () => _dbProvider.GetClientsByName(searchString);
        }

        private void CommitMethod()
        {
            //TODO это работает не так
            _dbProvider.Commit();
        }
    }
}

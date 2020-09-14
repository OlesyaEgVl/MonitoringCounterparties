using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using ContragentAnalyse.Model.Entities;
using ContragentAnalyse.Model.Implementation;
using ContragentAnalyse.Model.Interfaces;
using ContragentAnalyse.ViewModel.Commands;

namespace ContragentAnalyse.ViewModel
{
    public class MainViewModel :INotifyPropertyChanged
    {
        #region Реализация Singleton
        private static MainViewModel instance;
        public static MainViewModel GetInstance()
        {
            instance ??= new MainViewModel();
            return instance;
        }
        #endregion

        #region dataProviders
        IDataProvider _dbProvider = new EFDataProvider();
        
        #endregion

        #region Текущие значения
        public string SearchBIN { get; set; }
        public string SearchName { get; set; }
        private ObservableCollection<Client> _foundClients = new ObservableCollection<Client>();
        public ObservableCollection<Client> FoundClients { get => _foundClients; set => _foundClients = value; }
        private Client _selectedClient;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Client SelectedClient
        {
            get
            {
                return _selectedClient;
            }
            set
            {
                _selectedClient = value;
                if (_selectedClient != null)
                {
                    CurrentBank = _selectedClient.Bank;
                    OnPropertyChanged(nameof(CurrentBank));
                    OnPropertyChanged(nameof(SelectedClient));
                }
            }
        }

        public Bank CurrentBank { get; set; }
        public DateTime DataAct { get; set; }
        public DateTime? DateReceivedKit { get; set; }
        public DateTime? DateRequests { get; set; }
        public string ClientManagers { get; set; }
        public string Comments { get; set; }
        public DateTime? DateNext { get; set; }
        public string Criterias { get; set; }
        public string Contract { get; set; }
        public string AccountNumbers { get; set; }
        public bool? CardKOP { get; set; }
        public string Contact { get; set; }
        
        #endregion

        #region Commands
        public MyCommand SearchCommand { get; set; }
        public MyCommand AddNewClient { get; set; }
        public MyCommand Editing { get; set; }
        public MyCommand SaveChanges { get; set; }
        public MyCommand Word { get; set; }
        public MyCommand Count { get; set; }
        public MyCommand UnloadHistory { get; set; }
        public MyCommand UnloadExcel { get; set; }
        public MyCommand CommitChanges { get; set; }
        #endregion

        #region CurrentData
        public string TestString { get; set; }
        #endregion

        private MainViewModel()
        {
            InitializeCommands();
            //InitializeData();
        }

        /* private void InitializeData()
         {

         }*/
        

        private void InitializeCommands()
        {
            SearchCommand = new MyCommand(SearchMethod);
            //TODO подставить реализацию команд
            AddNewClient = new MyCommand(()=> MessageBox.Show($"Добавить нового клиента"));
            Editing = new MyCommand(() => MessageBox.Show($"Редактировать"));
            SaveChanges = new MyCommand(()=>MessageBox.Show($"Сохранить изменения"));
            Word = new MyCommand(()=> MessageBox.Show($"Скачать Word"));
            Count = new MyCommand(()=> MessageBox.Show($"Посчитать"));
            UnloadHistory = new MyCommand(()=> MessageBox.Show($"Сохранить историю"));
            UnloadExcel = new MyCommand(()=> MessageBox.Show($"Exel"));
            CommitChanges = new MyCommand(CommitMethod);
        }

        private void SearchMethod()
        {
            if (!string.IsNullOrWhiteSpace(SearchBIN)) 
            {
                FoundClients.Clear();
                foreach(Client client in _dbProvider.GetClients(SearchBIN))
                {
                    FoundClients.Add(client);
                }
            }
            else
            if(!string.IsNullOrWhiteSpace(SearchName))
            {
                FoundClients.Clear();
                List<Client> banks = _dbProvider.GetClientsName(SearchName).ToList();
                foreach (Client client in banks)
                {
                    FoundClients.Add(client);
                }
              
            }
            else
            {
                MessageBox.Show("Поле БИН/Наименование не может быть пустым!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            DataAct = _foundClients[0].Bank.Actualizations.OrderBy(i => i.DateActEKS).FirstOrDefault().DateActEKS;
            OnPropertyChanged(nameof(DataAct));
            DateNext = _foundClients[0].Bank.PrescoringScoring.OrderBy(i => i.DateNextScoring).FirstOrDefault().DateNextScoring;
            OnPropertyChanged(nameof(DateNext));
            ClientManagers = _foundClients[0].Bank.Client.OrderBy(i => i.ClientManager).FirstOrDefault().ClientManager;
            OnPropertyChanged(nameof(ClientManagers));
            CardKOP = _foundClients[0].Bank.Client.OrderBy(i => i.CardOP).FirstOrDefault().CardOP;
            OnPropertyChanged(nameof(CardKOP));
            //Criterias = _foundClients[0].Bank.PrescoringScoring.OrderBy(i => i.CriteriaToScoring.).FirstOrDefault().CriteriaToScoring;
            // OnPropertyChanged(nameof(Criterias));// Мб тут по другому? Критериев же много
            Contract  = _foundClients[0].Bank.Contracts.OrderBy(i => i.Name).FirstOrDefault().Name;
            AccountNumbers = _foundClients[0].Bank.RestrictedAccounts.OrderBy(i => i.AccountNumber).FirstOrDefault().AccountNumber;
            Contact = _foundClients[0].Bank.Contacts.OrderBy(i => i.ContactFIO).FirstOrDefault().ContactFIO;
            Contact += " "+_foundClients[0].Bank.Contacts.OrderBy(i => i.Value).FirstOrDefault().Value;
            OnPropertyChanged(nameof(Contract));
            OnPropertyChanged(nameof(AccountNumbers));
            OnPropertyChanged(nameof(Contact));
            
        }

        private void CommitMethod()
        {
            _dbProvider.Commit();
        }
      
    }
}

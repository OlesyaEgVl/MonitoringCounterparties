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
                CurrentBank = _selectedClient.Bank;
                OnPropertyChanged(nameof(CurrentBank));
                OnPropertyChanged(nameof(SelectedClient));
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
        public SearchCommand SearchCommand { get; set; }
        public AddNewClient AddNewClient { get; set; }

        public Editing Editing { get; set; }
        public SaveChanges SaveChanges { get; set; }
        public Word Word { get; set; }
        public Count Count { get; set; }
        public UnloadHistory UnloadHistory { get; set; }
        public UnloadExcel UnloadExcel { get; set; }
        public CommitChanges CommitChanges { get; set; }
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
            SearchCommand = new SearchCommand(SearchMethod);
            AddNewClient = new AddNewClient();
            Editing = new Editing();
            SaveChanges = new SaveChanges();
            Word = new Word();
            Count = new Count();
            UnloadHistory = new UnloadHistory();
            UnloadExcel = new UnloadExcel();
            CommitChanges = new CommitChanges(CommitMethod);
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
            {
                MessageBox.Show("Поле БИН не может быть пустым!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

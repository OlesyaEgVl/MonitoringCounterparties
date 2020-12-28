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


namespace ContragentAnalyse.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private const string ExcelPassword = "789456";
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
        public string SelectedCriteriasLevel
        {
            get
            {
                double riskLevel = 0;

                //string RiskLevelName = "Не определено";
                if (SelectedCriterias.Count == 0)
                {
                    string RiskLevelName = "Не определено";
                    return  RiskLevelName;
                }
                else
                {
                    riskLevel = SelectedCriterias.Select(i => i.Weight).Sum();
                    
                    string RiskLevelName = "";
                    switch (riskLevel)
                    {
                        case double n when n > 13.1:
                            RiskLevelName = $"{riskLevel.ToString("N1")} - Критичный";
                            break;
                        case double n when n >= 5.5 && n <= 13.1:
                            RiskLevelName = $"{riskLevel.ToString("N1")} - Высокий";
                            break;
                        case double n when n >= 3.4 && n < 5.5:
                            RiskLevelName = $"{riskLevel.ToString("N1")} - Средний";
                            break;
                        case double n when n < 3.4:
                            RiskLevelName = $"{riskLevel.ToString("N1")} - Низкий";
                            break;
                        default:
                            RiskLevelName = "Не определено";
                            break;
                    }
                    return RiskLevelName;
                }
            }
        }
        private ScoringHistoryGrouped _selectedHistoryRecord = new ScoringHistoryGrouped();
        public ScoringHistoryGrouped SelectedHistoryRecord 
        {
            get
            {
                return _selectedHistoryRecord;
            }
            set
            {
                _selectedHistoryRecord = value;
               // RaisePropertyChanged(nameof(IsCurrentHistoryRecordClosed));
            }
        }
        public bool IsCurrentHistoryRecordClosed
        {
            get
            {
                if (SelectedHistoryRecord == null)
                    return false;
                return !SelectedHistoryRecord.ClosedClient;
            }
        }
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
                if (SelectedClient != null)
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
                RaisePropertyChanged(nameof(IsCurrentHistoryRecordClosed));                             
                RaisePropertyChanged(nameof(BankProductHistory));
                SelectedCriterias.Clear();
                NostroUnusualOperationsString = string.Empty;
                NostroUnusualOperationsNOSTRO = string.Empty;
                //NostroUnusualOperationsLORO = string.Empty;
                BankProductHistory.Name = string.Empty;
                BankProductHistory.Id = 0;
                RaisePropertyChanged(nameof(NostroUnusualOperationsString));
                RaisePropertyChanged(nameof(SelectedHistoryRecord));
                RaisePropertyChanged(nameof(NostroUnusualOperationsNOSTRO));
                RaisePropertyChanged(nameof(NostroUnusualOperationsLORO));
                RaisePropertyChanged(nameof(BankProductHistory.Name));
                RaisePropertyChanged(nameof(BankProductHistory.Id));
               // RaisePropertyChanged(nameof(SelectedCriterias));

                //мб не тут
                //RaisePropertyChanged(nameof(SelectedCriteriasLevel));
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
           // SelectedHistory = new ObservableCollection<PrescoringScoringHistory>(_dbProvider.GetScoringHistory());
        }
        #region Commands
        public MyCommand<string> SearchCommand { get; set; }
        public MyCommand<ScoringHistoryGrouped> SelectHistoryRecord { get; set; }
        public MyCommand<string> AddClientCommand { get; set; }
        public MyCommand EditCommand { get; set; }
        public MyCommand SaveCommand { get; set; }
        public MyCommand ExportWordCommand { get; set; }
        public MyCommand<string> CalculateCommand { get; set; }
        public MyCommand EstimationRiskCommand { get; set; }
        public MyCommand SaveRiskRecordCommand { get; set; }
        public MyCommand<string> ExportExcelCommand { get; set; }
        public MyCommand SaveChangesCommand { get; set; }
        public MyCommand<object> StoreSelection { get; set; }
        public MyCommand ContractBankProducts { get; set; }
        public MyCommand<string> UpdateCommand { get; set; }
        public MyCommand UnloadingExcelCommand { get; set; }
        public MyCommand AddScoringCommand { get; set; }
        #endregion
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
            ExportExcelCommand = new MyCommand<string>(ExportExcelCommandMethod);
            SaveChangesCommand = new MyCommand(CommitMethod);
            StoreSelection = new MyCommand<object>(StoreSelectionMethod);
            ContractBankProducts = new MyCommand(ContractBankProductsMethod);
            UpdateCommand = new MyCommand<string>(UpdateMethod);
            UnloadingExcelCommand = new MyCommand(UnloadingExcelMethod);
            SelectHistoryRecord = new MyCommand<ScoringHistoryGrouped>(SelectHistoryRecordMethod);
            AddScoringCommand = new MyCommand(AddScoringMethod);
        }
        public string SelectedClientLatestNostro
        {
            get
            {
                if (SelectedClient == null)
                {
                    return string.Empty;
                }
                return SelectedClient.PrescoringScoringHistory.Last().NOSTRO;
            }
        }
        public string SelectedClientLatestLORO
        {
            get
            {
                if (SelectedClient == null)
                {
                    return string.Empty;
                }
                return SelectedClient.PrescoringScoringHistory.Last().LORO;
            }
        }
        private void AddScoringMethod()
        {
            List<PrescoringScoringHistory> records = new List<PrescoringScoringHistory>();

            foreach (Criteria criteria in SelectedCriterias)
            {
                records.Add(new PrescoringScoringHistory
                {
                    // здесь нужно инициализировать переменные
                    Client = SelectedClient,
                    Employee_Id = CurrentEmployee.Id,
                    DatePresScor = DateTime.Now,
                    Criteria = criteria,
                    ClosedClient = closedClients,
                    Comment = CurrentHistoryComment,
                    NostroLevel = NostroUnusualOperationsString,
                    NOSTRO = NostroUnusualOperationsNOSTRO,
                    LORO = NostroUnusualOperationsLORO
                });
            }
            _dbProvider.AddScoring(records);
        }

        private void SelectHistoryRecordMethod(ScoringHistoryGrouped record)
        {
            //Здесь ты можешь вызвать команду пересчета уровня риска
            //здесь тебе нужно обработать всё всё всё
            // Выбранные критерии = record...
            IsSelectionLocked = true;
            RaisePropertyChanged(nameof(IsCurrentHistoryRecordClosed));
            SelectedCriterias.Clear();
            SelectedCriterias.AddRange(record.HistoryRecords.Select(i => i.Criteria));
            NostroUnusualOperationsString = record.NostroLevel;
            RaisePropertyChanged(nameof(NostroUnusualOperationsString));
            CurrentHistoryComment = record.CurrentHistoryComment;
            RaisePropertyChanged(nameof(CurrentHistoryComment));
            IsSelectionLocked = false;
        }
        #endregion


        const string filepath = @"C:\Projects\CounterpartyMonitoring\ContragentAnalyse\bin\Debug\netcoreapp3.0\Anceta.docx";
        const string filepathCopy = @"C:\Projects\CounterpartyMonitoring\ContragentAnalyse\bin\Debug\netcoreapp3.0\AncetaCopy.docx";

        private void ExportWordMethod() // SAVE WORD
        {
            FileInfo fileInfo = new FileInfo(filepath);
            fileInfo = fileInfo.CopyTo(filepathCopy, true); //поменять путь к файлу
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open($"{fileInfo}", true))
            {
                string docTextFullName = string.Empty; // ""
                string docTextBIN = string.Empty;
                string docTextDateAct = string.Empty;
                string docTextCountry = string.Empty;
                string docTextContract = string.Empty;
                string docTextCurrency = string.Empty;
                string docTextRestrictedAccounts = string.Empty;
                string docTextCOP = string.Empty;
                string docTextAdditionalBIN = string.Empty;
                string docTextDateNextScoring = string.Empty;
                string docTextLevelRisk = string.Empty;
                string docTextCriteria = string.Empty;
                string docTextEmployee = string.Empty;
                string allText;
                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    allText = sr.ReadToEnd();
                }

                bookmarks = wordDoc.MainDocumentPart.RootElement.Descendants<BookmarkStart>().ToList();
                bm = new Dictionary<string, BookmarkStart>();
                foreach (BookmarkStart bms in bookmarks)
                {
                    bm[bms.Name] = bms;
                }
                SetBookmarkText("FullName", SelectedClient.FullName);
                SetBookmarkText("BIN", SelectedClient.BIN);
                SetBookmarkText("DateAct", SelectedClient.ActualizationDate.ToString());
                SetBookmarkText("Country", SelectedClient.RegistrationRegion);
                SetBookmarkText("Contract", string.Join(',', SelectedClient.ClientToContracts.Select(i => i.Contracts).Select(i => i.Name)));
                SetBookmarkText("Currency", string.Join(',', SelectedClient.ClientToCurrency.Select(i => i.Currency).Select(i => i.Name)));
                SetBookmarkText("RestrictedAccounts", string.Join(',', SelectedClient.RestrictedAccounts.Select(i => i.AccountNumber)));
                SetBookmarkText("COP", SelectedClient.CardOP.ToString());
                SetBookmarkText("AdditionalBIN", SelectedClient.AdditionalBIN);
                SetBookmarkText("DateNextScoring", SelectedClient.NextScoringDate.ToString());
                SetBookmarkText("LevelRisk", SelectedClient.Level);
                SetBookmarkText("Criteria", string.Join(',', SelectedClient.ClientToCriteria.Select(i => $"{i.Criteria.Name} {i.Criteria.Weight}")));
                SetBookmarkText("Employee", CurrentEmployee.Name.ToString());
            }
            MessageBox.Show("Файл скачен!");
        }
        List<BookmarkStart> bookmarks;
        Dictionary<string, BookmarkStart> bm;

        private void SetBookmarkText(string BookmarkName, string Text)
        {
            if (bm == null || bm.Count < 1)
            {
                throw new Exception("Закладки не указаны");
            }
            Run bookmarkText = bm[BookmarkName].NextSibling<Run>();
            if (bookmarkText != null)
            {
                bookmarkText.GetFirstChild<Text>().Text = Text;
            }
        }

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
                    ClosedClient = closedClients,
                    Comment = CurrentHistoryComment,
                    NostroLevel = NostroUnusualOperationsString,
                    NOSTRO=NostroUnusualOperationsNOSTRO,
                    LORO=NostroUnusualOperationsLORO
                });
            }
            RaisePropertyChanged(nameof(SelectedHistoryRecord));
            CommitMethod();
            CurrentHistoryComment = string.Empty;
            SelectedCriterias.Clear();
            RaisePropertyChanged(nameof(SelectedCriterias));
        }

        public string notbankproduct = "";
        public string NostroUnusualOperationsString { get; set; }
        private string _currentHistoryComment = string.Empty;
        public string CurrentHistoryComment
        {
            get
            {
                return _currentHistoryComment;
            }
            set
            {
                _currentHistoryComment = value;
                RaisePropertyChanged(nameof(CurrentHistoryComment));
            }
        }

        public bool IsSelectionLocked { get; private set; }
        public string NostroUnusualOperationsNOSTRO  { get; set; }
        public string NostroUnusualOperationsLORO { get; set; }
        public float NostroUnusualOperationsLOROcount = 0;
        public void ContractBankProductsMethod() //несоответствие банковских продуктов
        {
            float NostroUnusualOperations = 0;//балл по необычным
            //Ностро и необычные операции
            int kolsheets = 0;
            const string excel_input = @"\\moscow\hdfs\WORK\Middle Office\International Compliance\Operations and Investments\Investigations\Investigation Status_june17_2.xlsx";
            const string excel_input2 = @"\\moscow\hdfs\WORK\Middle Office\International Compliance\SANCTIONS\NOSTRO\Off-line запросы\Off-Line запросы.xlsx";
            const string excel_input2Copy = @"C:\Users\U_M166J\Desktop\book2.xlsx";
            const string excel_input3 = @"\\moscow\itfs\Запросы Комплаенс-Банка\000 Мониторинг ЛОРО\Запросы в лоро-Банки.xlsx";
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //НОСТРО 1
            using (ExcelPackage excel = new ExcelPackage(new System.IO.FileInfo(excel_input)))
            {
                ExcelWorkbook XlWB = excel.Workbook;
                foreach (ExcelWorksheet sheets in XlWB.Worksheets)
                {
                    kolsheets++;
                }
                for (int j = kolsheets; j >= kolsheets - 11; j--)
                {
                    ExcelWorksheet sheet = excel.Workbook.Worksheets[j - 1];
                    for (int i = 1; i < sheet.Dimension.Rows; i++)
                    {
                        if ((sheet.Cells[i, 6].Text == SelectedClient.BIN) && (sheet.Cells[i, 24].Text != "") && (sheet.Cells[i, 25].Text == ""))
                        {
                            NostroUnusualOperations += 0.5f;
                        }

                    }
                }
            }
            kolsheets = 0;
            // НОСТРО 2
            //if (File.GetCreationTime(excel_input2).Hour >= 1 && File.GetCreationTime(excel_input2).Date == DateTime.Now.Date)
            //  {
            FileInfo fileInfo = new FileInfo(excel_input2);
            fileInfo = fileInfo.CopyTo(excel_input2Copy, true);
            using (ExcelPackage excel = new ExcelPackage(new FileInfo(excel_input2Copy), ExcelPassword))
            {
                int numsheet = 0;
                ExcelWorkbook XlWB = excel.Workbook;
                foreach (ExcelWorksheet sheets in XlWB.Worksheets)
                {
                    kolsheets++;
                    if (sheets.Name.IndexOf("по наст время") > -1)
                    {
                        numsheet = kolsheets;
                    }
                }
                ExcelWorksheet sheet = excel.Workbook.Worksheets[numsheet - 1];
                for (int i = 1; i < sheet.Dimension.Rows; i++)// поменять на 1
                {
                    if ((sheet.Cells[i,9].Text == SelectedClient.BIN) && sheet.Cells[i, 25].Text != "" && (sheet.Cells[i, 26].Text == ""))// в некоторых ячейках есть запятая - это заполненным считается или нет?
                    {
                        NostroUnusualOperations += 0.5f;
                    }
                }
            }
            NostroUnusualOperationsNOSTRO = NostroUnusualOperations +"";
           // RaisePropertyChanged(nameof(NostroUnusualOperationsNOSTRO));
            // fileInfo.Delete();
            kolsheets = 0;
            //NostroUnusualOperationsLORO = 0;
            //ЛОРО
            using (ExcelPackage excel = new ExcelPackage(new System.IO.FileInfo(excel_input3)))
            {
                int todaydateyear = DateTime.Now.Year;
                int dateDay = DateTime.Now.Day;
                int dateMonth = DateTime.Now.Month;
                int dateYear = DateTime.Now.Year - 1;
                DateTime date = new DateTime(dateYear, dateMonth, dateDay, 0, 0, 0); //не уверена день месяц/месяц день
                DateTime todaydate = new DateTime(todaydateyear, dateMonth, dateDay, 0, 0, 0);
                System.TimeSpan dateTo = todaydate.Subtract(date);
                DateTime newdate = todaydate.Subtract(dateTo);
                int numsheet = 0;

                ExcelWorkbook XlWB = excel.Workbook;
                foreach (ExcelWorksheet sheets in XlWB.Worksheets) //не попадает
                {
                    kolsheets++;
                    if (sheets.Name.IndexOf("Сводный") > -1)
                    {
                        numsheet = kolsheets-1;
                    }
                }
                ExcelWorksheet sheet = excel.Workbook.Worksheets[numsheet];
                for (int i = 1; i < sheet.Dimension.Rows; i++)
                {
                    if((sheet.Cells[i, 4].Text == SelectedClient.BIN) && (sheet.Cells[i, 16].Text == "Необычные") && (sheet.Cells[i, 13].Text == "") && (Convert.ToDateTime(sheet.Cells[i, 14].Text) >= Convert.ToDateTime(newdate.Date.ToString("d"))))
                    {
                        NostroUnusualOperations += 1.5f;
                        NostroUnusualOperationsLORO += 1.5f;
                    }
                    else
                    if ((sheet.Cells[i, 4].Text == SelectedClient.BIN) && (sheet.Cells[i, 16].Text == "Необычные") && (sheet.Cells[i, 13].Text != "нет") && (Convert.ToDateTime(sheet.Cells[i, 13].Text) >= Convert.ToDateTime(newdate.Date.ToString("d"))))
                    {
                        NostroUnusualOperations += 1.5f;
                        NostroUnusualOperationsLOROcount += 1.5f;
                    }
                }
            }
             NostroUnusualOperationsLORO = NostroUnusualOperationsLOROcount+"";
        //  RaisePropertyChanged(nameof(NostroUnusualOperationsLORO));

        double itogNostro = 0;
            itogNostro = SelectedCriterias.Select(i => i.Weight).Sum() + NostroUnusualOperations;
            string BankProductName = string.Empty;
            switch (itogNostro)
            {
                case double n when n > 13.1:
                    NostroUnusualOperationsString = $"{itogNostro.ToString("N1")} - Критичный";
                    BankProductName = "Сотрудничество приостановлено/запрещено";
                    break;
                case double n when n >= 5.6 && n <= 13.1:
                    NostroUnusualOperationsString = $"{itogNostro.ToString("N1")} - Высокий";
                    BankProductName = "Кор.счета рублевые;Кор.счета валютные + Счет с ограничениями V ;Межбанк ; Синдицированное кредитование ;ALFA-FOREX и/или RISDA и/или ISDA и/или RISDA FI и/или RISDA онлайн и/или CSA онлайн;ALFA-FOREX и/или RISDA и/или ISDA и/или RISDA FI и/или RISDA онлайн и/или CSA онлайн и/или ISMA и/или Соглашения по ценным бумагам;ISMA и/или Соглашения по ценным бумагам;Драгметаллы;Организация секьюритизаций;Объединение банкоматных сетей;Кор.счета рублевые + Пластиковые карты и/или Договор по операциям ПК и/или Процессинг и/или Договор НПС (нац.платеж.сист.);";
                    break;
                case double n when n >= 3.5 && n <= 5.5:
                    NostroUnusualOperationsString = $"{itogNostro.ToString("N1")} - Средний";
                    BankProductName = "Кор.счета рублевые;Кор.счета валютные + Счет с ограничениями V ;Межбанк ; Синдицированное кредитование ;ALFA-FOREX и/или RISDA и/или ISDA и/или RISDA FI и/или RISDA онлайн и/или CSA онлайн;ALFA-FOREX и/или RISDA и/или ISDA и/или RISDA FI и/или RISDA онлайн и/или CSA онлайн и/или ISMA и/или Соглашения по ценным бумагам;ISMA и/или Соглашения по ценным бумагам;Драгметаллы;Организация секьюритизаций;Объединение банкоматных сетей;Кор.счета рублевые + Пластиковые карты и/или Договор по операциям ПК и/или Процессинг и/или Договор НПС (нац.платеж.сист.); Зарплатные проекты;Электр.банк.гарантия и/или Непокрыт.аккредитив.Бенефициар;Банкнотные сделки;Собственные векселя;Договор на инкассацию; ";
                    break;
                case double n when n <= 3.4:
                    NostroUnusualOperationsString = $"{itogNostro.ToString("N1")} - Низкий";
                    BankProductName = "Кор.счета рублевые;Кор.счета валютные + Счет с ограничениями V ;Межбанк ; Синдицированное кредитование ;ALFA-FOREX и/или RISDA и/или ISDA и/или RISDA FI и/или RISDA онлайн и/или CSA онлайн;ALFA-FOREX и/или RISDA и/или ISDA и/или RISDA FI и/или RISDA онлайн и/или CSA онлайн и/или ISMA и/или Соглашения по ценным бумагам;ISMA и/или Соглашения по ценным бумагам;Драгметаллы;Организация секьюритизаций;Объединение банкоматных сетей;Кор.счета рублевые + Пластиковые карты и/или Договор по операциям ПК и/или Процессинг и/или Договор НПС (нац.платеж.сист.); Зарплатные проекты;Электр.банк.гарантия и/или Непокрыт.аккредитив.Бенефициар;Банкнотные сделки;Собственные векселя;Договор на инкассацию; Кор.счета валютные;Кор.счета валютные + Пластиковые карты и/или Договор по операциям ПК и/или Процессинг и/или Договор НПС (нац.платеж.сист.);Брокерское обслуживание;Депозитарное обслуживание;";
                    break;
                default:
                    NostroUnusualOperationsString = "Не определено";
                    BankProductName = "Не определено";
                    break;
            }

            //Банковские продукты
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
                    if ((listcotracts[i] != ','))
                    {
                        contr += listcotracts[i];
                    }
                    else if (listcotracts[i] == ',')
                    { 
                       // if (contr[contr.Length]==' ') {contr= }
                        if (BankProductName.IndexOf(contr) <= -1 && (notbankproduct.IndexOf(contr) <= -1))  //если не попадает в список банковских продуктов >-был
                        {
                            notbankproduct += contr + ", ";
                        }
                        contr = "";
                    }
                }
                BankProductHistory.Name = notbankproduct;
                RaisePropertyChanged(nameof(BankProductHistory));
                RaisePropertyChanged(nameof(NostroUnusualOperationsString));
                SelectedHistoryRecord.NOSTRO = NostroUnusualOperationsNOSTRO;
                RaisePropertyChanged(nameof(SelectedHistoryRecord.NOSTRO));
                SelectedHistoryRecord.LORO = NostroUnusualOperationsLORO;
                RaisePropertyChanged(nameof(SelectedHistoryRecord.LORO));
                RaisePropertyChanged(nameof(NostroUnusualOperationsLORO));
                RaisePropertyChanged(nameof(SelectedHistoryRecord));

               MessageBox.Show("Посчитано!");
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
        public bool closedClients = false;
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
            closedClients = true;
            RaisePropertyChanged(nameof(closedClients));
            CommitMethod();
        }

        private void StoreSelectionMethod(object SelectedItems) //метод отвечающий за выбранные критерии добовляет их в SelectedCriterias
        {
            SelectedCriterias.Clear();
            if (SelectedItems is IEnumerable collection && !IsSelectionLocked)
            {
                foreach (object obj in collection)
                {
                    SelectedCriterias.Add(obj as Criteria);
                }
            }
            RaisePropertyChanged(nameof(SelectedCriteriasLevel));
            
        }

        private void UpdateMethod(string BINStr) //кнопка обновленя клиента
        {
                if (_dbProvider.IsAnyClientExist(BINStr))
                {
                    Client returnClient = null;
                    try
                    {
                        returnClient = eqProvider.GetClient(BINStr.ToUpper());
                    }
                    catch (NotConnectedException e)
                    {
                        MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    SelectedClient.FullName = returnClient.FullName;
                    SelectedClient.ShortName = returnClient.ShortName;
                    if (SelectedClient.Actualization == null)
                    {
                        SelectedClient.Actualization = new List<Actualization>();
                    }
                    SelectedClient.Actualization.Add(returnClient.Actualization[0]);
                    SelectedClient.AdditionalBIN = returnClient.AdditionalBIN;
                    SelectedClient.BecomeClientDate = returnClient.BecomeClientDate;
                    SelectedClient.CardOP = returnClient.CardOP;
                    SelectedClient.SEB = returnClient.SEB;
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
            MessageBox.Show("Клиент перезаписан!");
            return;
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
                MessageBox.Show("Данный клиент уже существует!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
           /* 
            List<String> binlist1 = new List<String>() { "Y00003", "Y00008", "Y00010", "Y00023", "Y00032", "Y00060", "Y00105", "Y00125", "Y00129", "Y00135", "Y00140", "Y00147", "Y00149", "Y00181", "Y00186", "Y00196", "Y00217", "Y00254", "Y00255", "Y00260", "Y00272", "Y00280", "Y00282", "Y00290", "Y00293", "Y00298", "Y00303", "Y00305", "Y00330", "Y00331", "Y00334", "Y00336", "Y00339", "Y00341", "Y00342", "Y00344", "Y00355", "Y00362", "Y00369", "Y00372", "Y00372", "Y04094", "Y00391", "Y00405", "Y00406", "Y00411", "Y00414", "Y00433", "Y00436", "Y00445", "Y00452", "Y00455", "Y00461", "Y00466", "Y00487", "Y00514", "Y00518", "Y00521", "Y00526", "Y00529", "Y00533", "Y00536", "Y00544", "Y00573", "Y00583", "Y00588", "Y00594", "Y00596", "Y00599", "Y00612", "Y00613", "Y00616", "Y00633", "Y00652", "Y00655", "Y00658", "Y00661", "Y00663", "Y00668", "Y00676", "Y00683", "Y00686", "Y00687", "Y00692", "Y00703", "Y00720", "Y00723", "Y00725", "Y00735", "Y00742", "Y00749", "Y00754", "Y00765", "Y00773", "Y00801", "Y00802", "Y00806", "Y00813", "Y00816", "Y00823", "Y00827", "Y00844", "Y00847", "Y00849", "Y00868", "Y00872", "Y00879", "Y00884", "Y00890", "Y00895", "Y00900", "Y00924", "Y00939", "Y00942", "Y00959", "Y00961", "Y00967", "Y00979", "Y00987", "Y00996", "Y00997", "Y00998", "Y01004", "Y01012", "Y01025", "Y01026", "Y01033", "Y01035", "Y01042", "Y01044", "Y01053", "Y01056", "Y01062", "Y01067", "Y01071", "Y01078", "Y01087", "Y01091", "Y01093", "Y01094", "Y01101", "Y01107", "Y01127", "Y01137", "Y01140", "Y01141", "Y01169", "Y01170", "Y01171", "Y01178", "Y01183", "Y01326", "Y01186", "Y01187", "Y01190", "Y01193", "Y01203", "Y01204", "Y01208", "Y01210", "Y01213", "Y01223", "Y01224", "Y01226", "Y01244", "Y01246", "Y01253", "Y01262", "Y01271", "Y01273", "Y01294", "Y01308", "Y01309", "Y01321", "Y01322", "Y01323", "Y01326", "Y01330", "Y01343", "Y01368", "Y01370", "Y01379", "Y01380", "Y01381", "Y01382", "Y01383", "Y01389", "Y01396", "Y01440", "Y01450", "Y01453", "Y01456", "Y01457", "Y01459", "Y01463", "Y01464", "Y01488", "Y01489", "Y01491", "Y01492", "Y01496", "Y01504", "Y01506", "Y01509", "Y01526", "Y01529", "Y01535", "Y01541", "Y01543", "Y01545", "Y01553", "Y01555", "Y01559", "Y01581", "Y01598", "Y01602", "Y01609", "Y01687", "Y01692", "Y01703", "Y01710", "Y01715", "Y01725", "Y01743", "Y01765", "Y01771", "Y01779", "Y01786", "Y01792", "Y01821", "Y01824", "Y01829", "Y01839", "Y01856", "Y01859", "Y01881", "Y01887", "Y01891", "Y01916", "Y01926", "Y01927", "Y01970", "Y01987", "Y01995", "Y01996", "Y02029", "Y02030", "Y02052", "Y02073", "Y02102", "Y02121", "Y02125", "Y02215", "Y02232", "Y02255", "Y02263", "Y02284", "Y02296", "Y02307", "Y02310", "Y02325", "Y02390", "Y02395", "Y02399", "Y02427", "Y02446", "Y02484", "Y02501", "Y02581", "Y02612", "Y02650", "Y02679", "Y02685", "Y02706", "Y02715", "Y02737", "Y02743", "Y02776", "Y02920", "Y02950", "Y02976", "Y02999", "Y03075", "Y03102", "Y03143", "Y03146", "Y03172", "Y03181", "Y03183", "Y03205", "Y03207", "Y03241", "Y03301", "Y03314", "Y03319", "Y03337", "Y03340", "Y03370", "Y03475", "Y03560", "Y03561", "Y03609", "Y03624", "Y03629", "Y03643", "Y03661", "Y03665", "Y03684", "Y03698", "Y03708", "Y03720", "Y03723", "Y03757", "Y03758", "Y03769", "Y03772", "Y03775", "Y03786", "Y03826", "Y03836", "Y03842", "Y03843", "Y03844", "Y03847", "Y03858", "Y03882", "Y03888", "Y03907", "Y03909", "Y03916", "Y03917", "Y03933", "Y03938", "Y03941", "Y03954", "Y03959", "Y03961", "Y03967", "Y03972", "Y03980", "Y04000", "Y04007", "Y04025", "Y04030", "Y04053", "Y04060", "Y04080", "Y04083", "Y04096", "Y04117", "Y04118", "Y04119", "Y04155", "Y04190", "Y04192", "Y04194", "Y04230", "Y04237", "Y04295", "Y04308", "Y04322", "Y04345", "Y04357", "Y04358", "Y04423", "Y04523", "Y04526", "Y04574", "Y04590", "Y04619", "Y04634", "Y04710", "Y04744", "Y04790", "Y04833", "Y04919", "Y04990", "Y05010", "Y05113", "Y05177", "Y05179", "Y05238", "Y05312", "Y05318", "Y05320", "Y05328", "Y05521", "Y05748", "Y05920", "Y06054", "Y06143" };
          
                foreach (string bin in binlist)
            {*/            
            Client newClient = eqProvider.GetClient(BINStr.ToUpper()); //найти клиента в EQ //BINStr
            if (newClient != null)
            {
                _dbProvider.AddClient(newClient); //добавить в БД
                SelectedClient = newClient;
            }
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
        private void ExportExcelCommandMethod(string BINStr)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string riskExcel = $"NewHistoryRiskExcel_{DateTime.Now:dd-MM-yyyy-mmss}.xlsx";
            using (ExcelPackage excel = new ExcelPackage())
            {
                ExcelWorksheet sheet = excel.Workbook.Worksheets.Add("Sheet1");
                int row = 1;
                int coll = 1;
                sheet.Cells[row, coll++].Value = "Дата";
                sheet.Cells[row, coll++].Value = "Вид оценки";
                sheet.Cells[row, coll++].Value = "Уровень оценки";
                sheet.Cells[row, coll++].Value = "Комментарий";
                //linq запрос
                //Работает с коллекциями
                List<ScoringHistoryGrouped> history = _dbProvider.GetClientHistory(SelectedClient).ToList();
                //history.Add();
                //
                // IEnumerable
                // 1 1 1 1 1 1 1 1
                //     
                // 1 1
                // 2
                
                //Нужно узнать на сколько объектов запускать цикл?
                string levelRisk = SelectedClient.Level;
                int HistoryRecordsCount = SelectedHistory.Where(i => i.Client_Id == SelectedClient.Id).Count();
                row = 2;
                coll = 1;
                sheet.Column(1).Style.Numberformat.Format = "dd.MM.yyyy";
                foreach (ScoringHistoryGrouped record in history/*SelectedHistory*/)
                {
                    sheet.Cells[row, coll++].Value = record.HistoryRecords.Select(rec => rec.DatePresScor).First();
                    sheet.Cells[row, coll++].Value = string.Join(", ", record.HistoryRecords.Select(rec=>rec.Criteria.Name));
                    sheet.Cells[row, coll++].Value = record.Level;
                    sheet.Cells[row, coll++].Value = string.Join(", ", record.CurrentHistoryComment);
                    coll = 1;
                    row++;
                }
                //string readedValue = sheet.Cells[1, 1].Text;
                
                excel.SaveAs(new System.IO.FileInfo(riskExcel));
            }
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = new System.Diagnostics.ProcessStartInfo(riskExcel);
            proc.StartInfo.UseShellExecute = true;
            proc.Start();
        }

        // 3я страница - Выгрузка
        private void UnloadingExcelMethod()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string unloadingExcel = $"NewUnloadingExcel_{DateTime.Now:dd-MM-yyyy-mmss}.xlsx";

            using (ExcelPackage excel = new ExcelPackage())
            {
                int row = 1;
                int col = 1;
                ExcelWorksheet sheet = excel.Workbook.Worksheets.Add("Sheet1");
                sheet.Cells[row, col++].Value = "БИН";
                sheet.Cells[row, col++].Value = "Полное наименование";
                sheet.Cells[row, col++].Value = "Статус клиента";
                sheet.Cells[row, col++].Value = "Страна регистрации";
                sheet.Cells[row, col++].Value = "Дата актуализации";
                sheet.Cells[row, col++].Value = "Другие БИНы Клиента";
                sheet.Cells[row, col++].Value = "Дата пересмотра";
                sheet.Cells[row, col++].Value = "Действующие договоры";
                sheet.Cells[row, col++].Value = "Счета в валюте";
                sheet.Cells[row, col++].Value = "Наличие КОП";
                sheet.Cells[row, col++].Value = "Счет с ограничениями";
                sheet.Cells[row, col++].Value = "Комплаенс-менеджер";
                sheet.Cells[row, col++].Value = "Клиент менеджер";
                sheet.Cells[row, col++].Value = "Выявленные критерии риска";
                sheet.Cells[row, col++].Value = "Уровень риска с учетом ЛОРО/НОСТРО";
                sheet.Cells[row, col++].Value = "Запросы: Дата направления";
                sheet.Cells[row, col++].Value = "Запросы: Дата получения";
                sheet.Cells[row, col++].Value = "Запросы: Комментарий";
                sheet.Cells[row, col++].Value = "Контакты";
                row = 2;
                col = 1;
                IEnumerable<Client> AllClients = _dbProvider.GetClients();
                sheet.Column(5).Style.Numberformat.Format = "dd.MM.yyyy";
                sheet.Column(7).Style.Numberformat.Format = "dd.MM.yyyy";
                sheet.Column(16).Style.Numberformat.Format = "dd.MM.yyyy";
                sheet.Column(17).Style.Numberformat.Format = "dd.MM.yyyy";
                foreach(Client client in AllClients)
                {
                    sheet.Cells[row, col++].Value = client.BIN;
                    sheet.Cells[row, col++].Value = client.FullName;
                    sheet.Cells[row, col++].Value = client.ActualizationStatus;
                    sheet.Cells[row, col++].Value = client.Country;
                    sheet.Cells[row, col++].Value = client.ActualizationDate;
                    sheet.Cells[row, col++].Value = client.AdditionalBIN;
                    sheet.Cells[row, col++].Value = client.NextScoringDate;
                    sheet.Cells[row, col++].Value = string.Join(',', client.ClientToContracts.Select(i=>i.Contracts).Select(i=>i.Name));
                    sheet.Cells[row, col++].Value = string.Join(',', client.ClientToCurrency.Select(i=>i.Currency).Select(i=>i.Name));
                    sheet.Cells[row, col++].Value = client.CardOP;
                    sheet.Cells[row, col++].Value = string.Join(',', client.RestrictedAccounts.Select(i=>i.AccountNumber));
                    sheet.Cells[row, col++].Value = client.ClientManagerNew;
                    sheet.Cells[row, col++].Value = client.ClientManager;
                    sheet.Cells[row, col++].Value = string.Join(',', client.ClientToCriteria.Select(i=>$"{i.Criteria.Name} {i.Criteria.Weight}"));
                    sheet.Cells[row, col++].Value = client.PrescoringScoringHistory.LastOrDefault()?.NostroLevel;
                    sheet.Cells[row, col++].Value = client.Requests.Select(i=>i.SendDate);                   
                    sheet.Cells[row, col++].Value = client.Requests.Select(i=>i.RecieveDate);
                    sheet.Cells[row, col++].Value = client.Requests.Select(i=>i.Comment);
                    sheet.Cells[row, col++].Value = string.Join(',', client.Contacts.Select(i=>$" {i.ContactFIO} {i.Value}"));
                    row++;
                    col = 1;
                }

                excel.SaveAs(new System.IO.FileInfo(unloadingExcel));
            }
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = new System.Diagnostics.ProcessStartInfo(unloadingExcel);
            proc.StartInfo.UseShellExecute = true;
            proc.Start();
        }
    }
}

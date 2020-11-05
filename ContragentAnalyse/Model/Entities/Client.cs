using ContragentAnalyse.Model.Entities.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using LicenseContext = OfficeOpenXml.LicenseContext;
using OfficeOpenXml;

namespace ContragentAnalyse.Model.Entities
{
   public class Client : NamedEntity
    {
        public string Mnemonic { get; set; }
        public string ClientManager { get; set; }
        public bool? CardOP { get; set; }
        public bool? RestrictedAccount { get; set; }
        public int Client_type_Id { get; set; }
        public DateTime? BecomeClientDate { get; set; }
        public int ResponsibleUnit_Id { get; set; }
        public int CoordinatingEmployee_Id { get; set; }
        public string BIN { get; set; }
        public string AdditionalBIN { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string EnglName { get; set; }
        
        public string LicenceNumber { get; set; }
        public DateTime? LicenceEstDate { get; set; }
        public string RKC_BIK { get; set; }
        public string INN { get; set; }
        public string OGRN { get; set; }
        public DateTime? OGRN_Date { get; set; }
        public string RegName_RP { get; set; }
        public DateTime? RegDate_RP { get; set; }
        public string RegStruct_Name { get; set; }
        public bool? CurrencyLicence { get; set; }
        public string RegistrationRegion { get; set; }
        public string AddressPrime { get; set; }
        public DateTime? NextScoringDate { get; set; }
        public int? Country_Id { get; set; }
        public int? ClientToCurrency_Id { get; set; }
        public string ClientManagerNew { get; set; }
        public string Level
        {
            get
            {
                float riskLevel = 0;
                int kolsheets = 0;
                const string excel_input = @"C:\Users\U_M166J\source\repos\ConsoleApp1\ConsoleApp1\bin\Debug\netcoreapp3.1\input.xlsx";
                const string excel_input2 = @"C:\Users\U_M166J\source\repos\ConsoleApp1\ConsoleApp1\bin\Debug\netcoreapp3.1\input2.xlsx";
                const string excel_input3 = @"C:\Users\U_M166J\source\repos\ConsoleApp1\ConsoleApp1\bin\Debug\netcoreapp3.1\input3.xlsx";
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                //НОСТРО 1
                using (ExcelPackage excel = new ExcelPackage(new System.IO.FileInfo(excel_input)))
                {
                    ExcelWorkbook XlWB = excel.Workbook;
                    foreach (ExcelWorksheet sheets in XlWB.Worksheets)
                    {
                        kolsheets++;
                    }
                    for (int j = kolsheets; j >= kolsheets - 12; j--)
                    {
                        ExcelWorksheet sheet = excel.Workbook.Worksheets[j];
                        for (int i = 1; i < sheet.Dimension.Rows; i++)
                        {
                            if ((sheet.Cells[i, 6].Text == BIN) && (sheet.Cells[i, 24].Text != null) && (sheet.Cells[i, 25].Text == null))
                            {
                                riskLevel += 0.5f;
                            }

                        }
                    }
                }
                kolsheets=0;
                // НОСТРО 2
                using (ExcelPackage excel = new ExcelPackage(new System.IO.FileInfo(excel_input2), "789456"))
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
                    ExcelWorksheet sheet = excel.Workbook.Worksheets[numsheet];
                    for (int i = 1; i < sheet.Dimension.Rows; i++)
                    {
                        if ((sheet.Cells[i, 6].Text == BIN) && (sheet.Cells[i, 24].Text != null) && (sheet.Cells[i, 25].Text == null))
                        {
                            riskLevel += 0.5f;
                        }
                    }
                }
                kolsheets = 0;
                //ЛОРО
                using (ExcelPackage excel = new ExcelPackage(new System.IO.FileInfo(excel_input3)))
                {
                    int todaydateyear = DateTime.Now.Year;
                    int dateDay = DateTime.Now.Day;
                    int dateMonth = DateTime.Now.Month;
                    int dateYear= DateTime.Now.Year-1;
                    DateTime date = new DateTime(dateYear, dateMonth, dateDay, 0, 0, 0); //не уверена день месяц/месяц день
                    DateTime todaydate = new DateTime(todaydateyear, dateMonth, dateDay, 0, 0, 0);
                    System.TimeSpan dateTo= todaydate.Subtract(date);
                    DateTime newdate = todaydate.Subtract(dateTo);
                    int numsheet = 0;
                    ExcelWorkbook XlWB = excel.Workbook;
                    foreach (ExcelWorksheet sheets in XlWB.Worksheets)
                    {
                        kolsheets++;
                        if (sheets.Name.IndexOf("Сводный") > -1)
                        {
                            numsheet = kolsheets;

                        }
                    }
                    ExcelWorksheet sheet = excel.Workbook.Worksheets[numsheet];
                    for (int i = 1; i < sheet.Dimension.Rows; i++)
                    {
                        if ((sheet.Cells[i, 4].Text == BIN)&&(Convert.ToDateTime(sheet.Cells[i, 13].Text) >= Convert.ToDateTime(newdate.Date.ToString("d"))) && (sheet.Cells[i, 21].Text != null) && (sheet.Cells[i, 25].Text == null)&&(sheet.Cells[i, 13].Text!="нет") && (sheet.Cells[i, 13].Text != null))
                        {
                            riskLevel += 0.5f;
                        }
                    }
                }
                string RiskLevelName = "Не определено";
                if (ClientToCriteria == null)
                {
                    RiskLevelName = "Не определено";
                    return RiskLevelName;
                }
                else
                {
                    riskLevel = ClientToCriteria.Select(i => i.Criteria.Weight).Sum(); // ругается, что бывает нулевое значение
                    RiskLevelName = string.Empty;
                    switch (riskLevel)
                    {
                        case float n when n > 13.1:
                            RiskLevelName = $"{riskLevel} - Критичный";
                            break;
                        case float n when n >= 5.6 && n <= 13.1:
                            RiskLevelName = $"{riskLevel} - Высокий";
                            break;
                        case float n when n >= 3.5 && n <= 5.5:
                            RiskLevelName = $"{riskLevel} - Средний";
                            break;
                        case float n when n <= 3.4:
                            RiskLevelName = $"{riskLevel} - Низкий";
                            break;
                        default:
                            RiskLevelName = "Не определено";
                            break;
                    }
                    return RiskLevelName;
                }
            }
        }
               
        public string BankProduct
        {
            get
            {
                float riskLevel = ClientToCriteria.Select(i => i.Criteria.Weight).Sum();
                string BankProductName = string.Empty;
                switch (riskLevel)
                {
                    case float n when n > 13.1:
                        BankProductName = "Сотрудничество приостановлено/запрещено";
                        break;
                    case float n when n >= 5.6 && n <= 13.1:
                        BankProductName = "Кор.счета рублевые ;Кор.счета валютные + Счет с ограничениями V ;Межбанк ; Синдицированное кредитование ;ALFA-FOREX и/или RISDA и/или ISDA и/или RISDA FI и/или RISDA онлайн и/или CSA онлайн;ALFA-FOREX и/или RISDA и/или ISDA и/или RISDA FI и/или RISDA онлайн и/или CSA онлайн и/или ISMA и/или Соглашения по ценным бумагам;ISMA и/или Соглашения по ценным бумагам;Драгметаллы;Организация секьюритизаций;Объединение банкоматных сетей;Кор.счета рублевые + Пластиковые карты и/или Договор по операциям ПК и/или Процессинг и/или Договор НПС (нац.платеж.сист.);";
                        break;
                    case float n when n >= 3.5 && n <= 5.5:
                        BankProductName = "Зарплатные проекты;Электр.банк.гарантия и/или Непокрыт.аккредитив.Бенефициар;Банкнотные сделки;Собственные векселя;Договор на инкассацию; ";
                        break;
                    case float n when n <= 3.4 && n>0:
                        BankProductName = "Кор.счета валютные;Кор.счета валютные + Пластиковые карты и/или Договор по операциям ПК и/или Процессинг и/или Договор НПС (нац.платеж.сист.);Брокерское обслуживание;Депозитарное обслуживание;";
                        break;
                    default:
                        BankProductName = "Не определено";
                        break;
                }
                return BankProductName;
               
            }
        }

        public DateTime? ActualizationDate
        {
            get
            {
                if(Actualization != null && Actualization.Count > 0)
                {
                    return Actualization?.Max(i => i.DateActEKS);
                }
                else
                {
                    return null;
                }
            }
        }
        public string ActualizationStatus
        {
            get
            {
                if (Actualization != null && Actualization.Count > 0)
                {
                    return Actualization?.Max(i => i.Status);
                }
                else
                {
                    return null;
                }
            }
        }
        [ForeignKey(nameof(ResponsibleUnit_Id))]
        public virtual ResponsibleUnit ResponsibleUnit { get; set; }
        [ForeignKey(nameof(Client_type_Id))]
        public virtual TypeClient TypeClient { get; set; }
        [ForeignKey(nameof(CoordinatingEmployee_Id))]
        public virtual Employees Employees { get; set; }
        [ForeignKey(nameof(Country_Id))]
        public virtual Country Country { get; set; }
        public List<ClientToCurrency> ClientToCurrency { get; set; }

        private ObservableCollection<Request> requests = new ObservableCollection<Request>();
        public ObservableCollection<Request> Requests
        {
            get => requests;
            set => requests = value;
        }
        public List<Actualization> Actualization { get; set; }
        public List<PrescoringScoring> PrescoringScoring { get; set; }
        public List<PrescoringScoringHistory> PrescoringScoringHistory { get; set; }
        public List<StopFactors> StopFactors { get; set; }
        public List<RestrictedAccounts> RestrictedAccounts { get; set; }
        public List<Contacts> Contacts { get; set; }
        public List<ClientToCriteria> ClientToCriteria { get; set; }
        public List<ClientToContracts> ClientToContracts { get; set; }

    }
}

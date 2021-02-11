using ContragentAnalyse.Model.Entities;
using ContragentAnalyse.Model.Implementation;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ContragentAnalyse.Reports
{

    public class ExcelReports
    {
        private const string INVESTIGATION_STATUS_FILENAME = @"\\moscow\hdfs\WORK\Middle Office\International Compliance\Operations and Investments\Investigations\Investigation Status_june17_2.xlsx";
        private const string OFFLINE_REQUESTS_FILENAME = @"\\moscow\hdfs\WORK\Middle Office\International Compliance\SANCTIONS\NOSTRO\Off-line запросы\Off-Line запросы.xlsx";
        private const string OFFLINE_REQUESTS_PSWRD = "789456";
        private const string LORO_BANKS_REQUEST_FILENAME = @"\\moscow\itfs\Запросы Комплаенс-Банка\000 Мониторинг ЛОРО\Запросы в лоро-Банки.xlsx";
        private const string OFFLINE_REQUESTS_COPY_FILEPATH = @"\Temp";
        private const string OFFLINE_REQUESTS_COPY_FILENAME = @"\Temp\offline_loro.xlsx";

        public ExcelReports()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            Directory.CreateDirectory(OFFLINE_REQUESTS_COPY_FILEPATH);
        }

        public void Calculate(Client client, Scoring scoring)
        {
            float riskLevel = 0;
            
            //НОСТРО 1
            using (ExcelPackage excel = new ExcelPackage(new FileInfo(INVESTIGATION_STATUS_FILENAME)))
            {
                ExcelWorkbook XlWB = excel.Workbook;
                foreach(ExcelWorksheet sheet in XlWB.Worksheets)
                {
                    for (int i = 1; i < sheet.Dimension.Rows; i++)
                    {
                        if (sheet.Cells[i, 6].Text.ToLower().Equals(client.BIN.ToLower()) && 
                            string.IsNullOrWhiteSpace(sheet.Cells[i, 24].Text) && 
                            string.IsNullOrWhiteSpace(sheet.Cells[i, 25].Text))
                        {
                            riskLevel += 0.5f;
                        }
                    }
                }
            }

            // НОСТРО 2
            FileInfo fileInfo = new FileInfo(OFFLINE_REQUESTS_FILENAME);
            fileInfo = fileInfo.CopyTo(OFFLINE_REQUESTS_COPY_FILENAME, true) ;
            using (ExcelPackage excel = new ExcelPackage(new FileInfo(OFFLINE_REQUESTS_COPY_FILENAME), OFFLINE_REQUESTS_PSWRD))
            {
                ExcelWorkbook XlWB = excel.Workbook;
                ExcelWorksheet sheet = excel.Workbook.Worksheets.Single(i=>i.Name.Contains("по наст время"));
                for (int i = 1; i < sheet.Dimension.Rows; i++)
                {
                    if (sheet.Cells[i, 9].Text.ToLower().Equals(client.BIN.ToLower()) && 
                        string.IsNullOrWhiteSpace(sheet.Cells[i, 25].Text) && 
                        string.IsNullOrWhiteSpace(sheet.Cells[i, 26].Text))// в некоторых ячейках есть запятая
                    {
                        riskLevel += 0.5f;
                    }
                }
            }
            
            using (ExcelPackage excel = new ExcelPackage(new FileInfo(LORO_BANKS_REQUEST_FILENAME)))
            {
                DateTime yearBeforeNow = new DateTime(DateTime.Now.Year - 1, DateTime.Now.Month, DateTime.Now.Day);

                ExcelWorkbook XlWB = excel.Workbook;
                ExcelWorksheet sheet = excel.Workbook.Worksheets.Single(i=>i.Name.Contains("Сводный"));
                for (int i = 1; i < sheet.Dimension.Rows; i++)
                {
                    if (sheet.Cells[i, 4].Text.ToLower().Equals(client.BIN.ToLower()) && 
                        sheet.Cells[i, 16].Text.ToLower().Equals("необычные") && 
                        string.IsNullOrWhiteSpace(sheet.Cells[i, 13].Text) && 
                        Convert.ToDateTime(sheet.Cells[i, 14].Text) >= yearBeforeNow)
                    {
                        riskLevel += 1.5f;
                    }
                    else if (sheet.Cells[i, 4].Text.ToLower().Equals(client.BIN.ToLower()) && 
                        sheet.Cells[i, 16].Text.ToLower().Equals("необычные") && 
                        !sheet.Cells[i, 13].Text.ToLower().Equals("нет") && 
                        Convert.ToDateTime(sheet.Cells[i, 13].Text) >= yearBeforeNow)
                    {
                        riskLevel += 1.5f;
                    }
                }
            }
            double NostroRiskLevelTotal = scoring.Criterias.Select(i => i.Criteria.Weight).Sum() + riskLevel;
            scoring.NostroRiskLevel = NostroRiskLevelTotal;
        }

        public void ExportExcelCommandMethod(Client client)
        {
            string reportFilePath = $"NewHistoryRiskExcel_{DateTime.Now:dd-MM-yyyy-mmss}.xlsx";
            using (ExcelPackage excel = new ExcelPackage())
            {
                ExcelWorksheet sheet = excel.Workbook.Worksheets.Add("Sheet1");
                int row = 1;
                int coll = 1;
                sheet.Cells[row, coll++].Value = "Дата";
                sheet.Cells[row, coll++].Value = "Вид оценки";
                sheet.Cells[row, coll++].Value = "Уровень риска";
                sheet.Cells[row, coll++].Value = "Комментарий";
                ObservableCollection<Scoring> history = client.Scorings;

                //Нужно узнать на сколько объектов запускать цикл?
                row = 2;
                coll = 1;
                sheet.Column(1).Style.Numberformat.Format = "dd.MM.yyyy";
                foreach (Scoring record in history)
                {
                    sheet.Cells[row, coll++].Value = record.CreatedAt;
                    sheet.Cells[row, coll++].Value = string.Join(", ", record.Criterias.Select(rec => rec.Criteria.Name));
                    sheet.Cells[row, coll++].Value = record.NostroRiskLevel + record.LoroRiskLevel;
                    sheet.Cells[row, coll++].Value = string.Join(", ", record.Comment);
                    coll = 1;
                    row++;
                }
                excel.SaveAs(new FileInfo(reportFilePath));
            }
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = new System.Diagnostics.ProcessStartInfo(reportFilePath);
            proc.StartInfo.UseShellExecute = true;
            proc.Start();
        }

        public void UnloadingExcelMethod(IEnumerable<Client> clients)
        {
            string reportFilePath = $"NewUnloadingExcel_{DateTime.Now:dd-MM-yyyy-mmss}.xlsx";

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
                sheet.Column(5).Style.Numberformat.Format = "dd.MM.yyyy";
                sheet.Column(7).Style.Numberformat.Format = "dd.MM.yyyy";
                sheet.Column(16).Style.Numberformat.Format = "dd.MM.yyyy";
                sheet.Column(17).Style.Numberformat.Format = "dd.MM.yyyy";
                foreach (Client client in clients)
                {
                    sheet.Cells[row, col++].Value = client.BIN;
                    sheet.Cells[row, col++].Value = client.FullName;
                    sheet.Cells[row, col++].Value = client.ActualizationStatus;
                    sheet.Cells[row, col++].Value = client.Country;
                    sheet.Cells[row, col++].Value = client.ActualizationDate;
                    sheet.Cells[row, col++].Value = client.AdditionalBIN;
                    sheet.Cells[row, col++].Value = client.NextScoringDate;
                    sheet.Cells[row, col++].Value = string.Join(',', client.ClientToContracts.Select(i => i.Contracts).Select(i => i.Name));
                    sheet.Cells[row, col++].Value = string.Join(',', client.ClientToCurrency.Select(i => i.Currency).Select(i => i.Name));
                    sheet.Cells[row, col++].Value = client.CardOP;
                    sheet.Cells[row, col++].Value = string.Join(',', client.RestrictedAccounts.Select(i => i.AccountNumber));
                    sheet.Cells[row, col++].Value = client.ClientManagerNew;
                    sheet.Cells[row, col++].Value = client.ClientManager;
                    if(client.Scorings.Count!=0 && client.Scorings.Last().Criterias.Count == 0)
                    {
                        sheet.Cells[row, col++].Value = string.Join(',', client.Scorings.Last().Criterias.Select(i => $"{i.Criteria.Name} {i.Criteria.Weight}"));
                    }
                    else
                    {
                        sheet.Cells[row, col++].Value = "";
                    }
                    sheet.Cells[row, col++].Value = client.Scorings.LastOrDefault()?.NostroRiskLevel;
                    sheet.Cells[row, col++].Value = client.Requests.Select(i => i.SendDate);
                    sheet.Cells[row, col++].Value = client.Requests.Select(i => i.RecieveDate);
                    sheet.Cells[row, col++].Value = client.Requests.Select(i => i.Comment);
                    sheet.Cells[row, col++].Value = string.Join(',', client.Contacts.Select(i => $" {i.ContactFIO} {i.Value}"));
                    row++;
                    col = 1;
                }

                excel.SaveAs(new FileInfo(reportFilePath));
            }
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = new System.Diagnostics.ProcessStartInfo(reportFilePath);
            proc.StartInfo.UseShellExecute = true;
            proc.Start();
        }
    }
}

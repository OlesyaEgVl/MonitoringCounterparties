using ContragentAnalyse.Model.Entities;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ContragentAnalyse.Reports
{
    class Wordreports
    {
        const string filepath = @"C:\Projects\CounterpartyMonitoring\ContragentAnalyse\bin\Debug\netcoreapp3.0\Anceta.docx";
        const string filepathCopy = @"C:\Projects\CounterpartyMonitoring\ContragentAnalyse\bin\Debug\netcoreapp3.0\AncetaCopy.docx";

        public  void ExportWordMethod(Client client, Employee employee) // SAVE WORD
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
                SetBookmarkText("FullName", client.FullName);
                SetBookmarkText("BIN", client.BIN);
                SetBookmarkText("DateAct", client.ActualizationDate.ToString());
                SetBookmarkText("Country", client.RegistrationRegion);
                SetBookmarkText("Contract", string.Join(',', client.ClientToContracts.Select(i => i.Contracts).Select(i => i.Name)));
                SetBookmarkText("Currency", string.Join(',', client.ClientToCurrency.Select(i => i.Currency).Select(i => i.Name)));
                SetBookmarkText("RestrictedAccounts", string.Join(',', client.RestrictedAccounts.Select(i => i.AccountNumber)));
                SetBookmarkText("COP", client.CardOP.ToString());
                SetBookmarkText("AdditionalBIN", client.AdditionalBIN);
                SetBookmarkText("DateNextScoring", client.NextScoringDate.ToString());
                SetBookmarkText("LevelRisk", client.Level);
                if(client.Scorings.Count!=0 && client.Scorings.Last().Criterias.Count != 0)
                {
                    SetBookmarkText("Criteria", string.Join(',', client.Scorings.Last().Criterias.Select(i => $"{i.Criteria.Name} {i.Criteria.Weight}")));
                }
                SetBookmarkText("Employee", employee.Name.ToString());
            }
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
    }
}

using ContragentAnalyse.Extension;
using ContragentAnalyse.Model.Entities;
using ContragentAnalyse.Model.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace ContragentAnalyse.Model.Implementation
{
    public class EquationProvider : IEquationProvider
    {
        char sessionName = 'A';
        bool connected = false;
        private IDataProvider dataProvider;

        /*public string reader()
        {
            
        }*/

        public EquationProvider(IDataProvider databaseProvider)
        {
            this.dataProvider = databaseProvider;
        }

        private void pEnter()
        {
            EUCL.SendStr("@E");
            EUCL.Wait();
        }

        private void Connect()
        {
            #region getConnection
            connected = true;
            sessionName = 'A';
            int attempCount = 0;
            Console.WriteLine("Connecting to Equation");
            Console.WriteLine($"Trying connect to {sessionName}...");
            while (EUCL.Connect(sessionName.ToString()) != 0)
            {
                if (attempCount++ > 5)
                {
                    connected = false;
                    Console.WriteLine("Connection failed");
                    break;
                }
                sessionName++;
                Console.WriteLine($"Trying connect to {sessionName}...");
            }
            
            if (connected)
            {
                EUCL.ClearScreen();
                Console.WriteLine("Connected.");
            }
            #endregion
        }

        private void Disconnect()
        {
            EUCL.Disconnect($"{sessionName}");
        }

        public Client GetClient(string BINStr)
        {
            Connect();
            if (!connected)
            {
                MessageBox.Show("Отсутствует подключение к Equation", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            } //вот и вся проверка
            else
            {
                Client clients = new Client();
                //Бежишь по eq и ищешь инфу
                //Возвращаешь заполненный инфой класс

                
                EUCL.SetCursorPos(21, 17);
                EUCL.SendStr("ПБА");
                pEnter();
                EUCL.SetCursorPos(3, 33);
                EUCL.SendStr($"{BINStr}");
                pEnter();
                /* string typeclient = EUCL.ReadScreen(4, 33, 80);// полное наименование
                 TypeClient tc = new TypeClient();
                 tc.Name = typeclient;
                 clients.TypeClient.add(tc);
                 clients.TypeClient.Name = typeclient;*/
                clients.TypeClient = dataProvider.GetClientType(EUCL.ReadScreen(4, 33, 2));
                string RKCBIK = EUCL.ReadScreen(21, 33, 9);//
                clients.RKC_BIK = RKCBIK;
                string fullname = EUCL.ReadScreen(6, 33, 35);// 
                clients.FullName = fullname.TrimSpaces(); //Удалять пробелы!!! + указать корректную длинну
                string shortname = EUCL.ReadScreen(11, 33, 35)+ EUCL.ReadScreen(12, 33, 35) + EUCL.ReadScreen(13, 33,35);// 
                clients.FullName = shortname;
                string Englname = EUCL.ReadScreen(14, 33, 35)+ EUCL.ReadScreen(15, 33, 35);// 
                clients.EnglName = Englname.TrimSpaces();
                string LicenceNumber = EUCL.ReadScreen(18, 33, 20);// 
                clients.LicenceNumber = LicenceNumber.TrimSpaces();
                string licenceDate = EUCL.ReadScreen(20, 33, 11);// 
               //Исправить. Проверка некорректная
                clients.LicenceEstDate = Convert.ToDateTime(licenceDate);
                string countrys = EUCL.ReadScreen(16, 33, 2);
                clients.Country = dataProvider.GetCountry(countrys.TrimSpaces());
                pEnter();
                string INN = EUCL.ReadScreen(10, 33, 12);// 
                clients.INN = INN.TrimSpaces();
                string ORGN = EUCL.ReadScreen(15, 33, 20);// 
                clients.OGRN = ORGN.TrimSpaces();
                string OGRNDate = EUCL.ReadScreen(15, 60, 11);// 
                clients.OGRN_Date = Convert.ToDateTime(OGRNDate);
                string regname = EUCL.ReadScreen(17, 33, 35);// 
                clients.RegName_RP = regname.TrimSpaces();
                string regdate = EUCL.ReadScreen(16, 60, 11);// 
                clients.RegDate_RP = Convert.ToDateTime(regdate);
                string regregion = EUCL.ReadScreen(20, 33, 35);// 
                clients.RegistrationRegion = regregion.TrimSpaces();
                pEnter();
                string becomeclientdate = EUCL.ReadScreen(5, 33, 11);// дата перехода в клиенты 
                clients.BecomeClientDate = Convert.ToDateTime(becomeclientdate.TrimSpaces());
                pEnter();
                string currencylicence = EUCL.ReadScreen(7, 33, 1);// валютная лицензия
                clients.CurrencyLicence = currencylicence.Equals("Y");
                string clientmanager = EUCL.ReadScreen(13, 39, 42);// КЛИНТ_МЕНЕД7ЖЕР имя и фамилию надо прописывать или код?
                clients.ClientManager = clientmanager.TrimSpaces();
                pEnter();
                pEnter();
                pEnter();
                string actdate = EUCL.ReadScreen(17, 29, 2)+'.'+ EUCL.ReadScreen(17, 31, 2) + '.'+ EUCL.ReadScreen(17, 33, 2);//дата актуализации
                Actualization act = new Actualization();
                act.DateActEKS = Convert.ToDateTime(actdate);
                if (clients != null) // get null ошибка
                {
                    clients.Actualization.Add(act);
                }
                
                
                pEnter();

                string levelrisk = EUCL.ReadScreen(5, 34, 11);//уровень риска - это нигде не хранится
                
                pEnter();
                if( !EUCL.ReadScreen(11, 4, 1).Equals(" "))
                {
                    pEnter();
                    for (int i = 1; i <= 24; i++)
                    {
                        for (int j = 1; j <= 80; j++)
                        {
                            if (BINStr == EUCL.ReadScreen(i, j, 6))
                            { clients.AdditionalBIN += EUCL.ReadScreen(i, 28, 6) + ", "; }
                        }
                    }
                    EUCL.SendStr("@8");
                    for (int i = 1; i <= 24; i++)
                    {
                        for (int j = 1; j <= 80; j++)
                        {
                            if (BINStr == EUCL.ReadScreen(i, j, 6))
                            { clients.AdditionalBIN += EUCL.ReadScreen(i, 28, 6) + ", "; }
                        }
                    }

                }

                EUCL.ClearScreen();
                EUCL.SetCursorPos(21, 17);
                EUCL.SendStr("PPP");
                pEnter();
                pEnter();
                EUCL.SetCursorPos(6, 2);
                EUCL.SendStr("1");
                // тут энтер проставляется автоматически
                EUCL.SetCursorPos(7, 69);
                EUCL.SendStr(BINStr);
                if (countrys.Equals("RU"))
                {
                    EUCL.SetCursorPos(5, 5);
                    EUCL.SendStr("30109");
                }
                else
                {
                    EUCL.SetCursorPos(5, 5);
                    EUCL.SendStr("30111");
                }
                pEnter();
                for (int i = 1; i <= 24; i++)
                {
                    if (EUCL.ReadScreen(i, 64, 6).Equals(" "))
                    {
                        // счёт открыт
                        clients.Currency = dataProvider.GetCurrency(EUCL.ReadScreen(i, 11, 3));//счета в валюте
                    }
                }
                EUCL.SendStr("@8"); // правильно ли тут сделала преход вниз
                EUCL.SendStr("@12");
               
                Disconnect();

                return clients;
            }
        }
        // это должно быть не тут, в эквейжене только эквейжн
        public void FillClient(Client client) 
        {
            throw new NotImplementedException(); // заглушка
            }


        }

        
    }


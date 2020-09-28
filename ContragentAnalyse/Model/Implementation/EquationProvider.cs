using ContragentAnalyse.Model.Entities;
using ContragentAnalyse.Model.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

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
            Client clients = new Client();
            //Бежишь по eq и ищешь инфу
            //Возвращаешь заполненный инфой класс
            Connect();
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
            string fullname = EUCL.ReadScreen(6, 33, 105);// 
            clients.FullName = fullname;
            string shortname = EUCL.ReadScreen(11, 33, 105);// 
            clients.FullName = shortname;
            string Englname = EUCL.ReadScreen(14, 33, 70);// 
            clients.EnglName = Englname;
            string LicenceNumber = EUCL.ReadScreen(18, 33, 20);// 
            clients.LicenceNumber = LicenceNumber;
            string licenceDate = EUCL.ReadScreen(20, 33, 11);// 
            clients.LicenceEstDate = Convert.ToDateTime(licenceDate);
            clients.Country = dataProvider.GetCountry(EUCL.ReadScreen(16, 33, 2));// страна регистрации-нужно сделать как типы клиента
            
            
            pEnter();
            string INN = EUCL.ReadScreen(10, 33, 12);// 
            clients.INN = INN;
            string ORGN = EUCL.ReadScreen(15, 33, 20);// 
            clients.OGRN = ORGN;
            string OGRNDate = EUCL.ReadScreen(15, 60, 11);// 
            clients.OGRN_Date = Convert.ToDateTime(OGRNDate);
            string regname = EUCL.ReadScreen(17, 33, 105);// 
            clients.RegName_RP = regname;
            string regdate = EUCL.ReadScreen(16, 60, 11);// 
            clients.RegDate_RP = Convert.ToDateTime(regdate);
            string regregion = EUCL.ReadScreen(20, 33, 35);// 
            clients.RegistrationRegion = regregion;
            pEnter();
            string becomeclientdate = EUCL.ReadScreen(5, 33, 11);// дата перехода в клиенты 
            clients.BecomeClientDate = Convert.ToDateTime(becomeclientdate);
            pEnter();
            string currencylicence = EUCL.ReadScreen(7, 33, 1);// валютная лицензия
            clients.CurrencyLicence = Convert.ToBoolean(currencylicence);
            string clientmanager = EUCL.ReadScreen(13, 39, 42);// КЛИНТ_МЕНЕДЖЕР имя и фамилию надо прописывать или код?
            clients.ClientManager = clientmanager;
            pEnter();
            pEnter();
           // pEnter();
            string actdate = EUCL.ReadScreen(17, 29, 6);//дата актуализации
            Actualization act = new Actualization();
            act.DateActEKS = Convert.ToDateTime(actdate);
            clients.Actualization.Add(act);
            pEnter();
            string levelrisk = EUCL.ReadScreen(5, 34, 11);//дата актуализации - это нигде не хранится

            Disconnect();

            return clients;
        }
        // это должно быть не тут, в эквейжене только эквейжн
        public void FillClient(Client client) 
        {
            throw new NotImplementedException(); // заглушка
            }


        }

        
    }


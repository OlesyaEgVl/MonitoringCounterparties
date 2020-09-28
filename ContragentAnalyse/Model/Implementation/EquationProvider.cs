﻿using ContragentAnalyse.Model.Entities;
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

        /*public string reader()
        {
            
        }*/


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
            string typeclient = EUCL.ReadScreen(4, 33, 80);// полное наименование
            clients.TypeClient.Name = typeclient;
            string RKCBIK = EUCL.ReadScreen(5, 33, 80);//
            clients.RKC_BIK = RKCBIK;
            string fullname = EUCL.ReadScreen(6, 33, 80);// 
            clients.FullName = fullname;
            string shortname = EUCL.ReadScreen(11, 33, 80);// 
            clients.FullName = shortname;
            string Englname = EUCL.ReadScreen(14, 33, 80);// 
            clients.EnglName = Englname;
            string LicenceNumber = EUCL.ReadScreen(18, 33, 80);// 
            clients.LicenceNumber = LicenceNumber;
            string licenceDate = EUCL.ReadScreen(20, 33, 80);// 
            clients.LicenceEstDate = Convert.ToDateTime(licenceDate);
            string country = EUCL.ReadScreen(16, 33, 80);// страна регистрации
            clients.Country.Name = country;
            pEnter();
            string INN = EUCL.ReadScreen(10, 33, 80);// 
            clients.INN = INN;
            string ORGN = EUCL.ReadScreen(15, 33, 80);// 
            clients.OGRN = ORGN;
            string OGRNDate = EUCL.ReadScreen(15, 60, 80);// 
            clients.OGRN_Date = Convert.ToDateTime(OGRNDate);
            string regname = EUCL.ReadScreen(17, 33, 80);// 
            clients.RegName_RP = regname;
            string regdate = EUCL.ReadScreen(16, 60, 80);// 
            clients.RegDate_RP = Convert.ToDateTime(regdate);
            string regregion = EUCL.ReadScreen(20, 33, 80);// 
            clients.RegistrationRegion = regregion;
            pEnter();
            string becomeclientdate = EUCL.ReadScreen(5, 33, 80);// дата перехода в клиенты 
            clients.BecomeClientDate = Convert.ToDateTime(becomeclientdate);
            pEnter();
            string currencylicence = EUCL.ReadScreen(7, 33, 80);// валютная лицензия
            clients.CurrencyLicence = Convert.ToBoolean(currencylicence);
            string clientmanager = EUCL.ReadScreen(13, 39, 80);// КЛИНТ_МЕНЕДЖЕР
            clients.ClientManager = clientmanager;
            pEnter();
            pEnter();
            string actdate = EUCL.ReadScreen(17, 29, 6);//дата актуализации
            Actualization act = new Actualization();
            act.DateActEKS = Convert.ToDateTime(actdate);
            clients.Actualization.Add(act);
            pEnter();
            string levelrisk = EUCL.ReadScreen(5, 34, 80);//дата актуализации - это нигде не хранится

            Disconnect();

            return clients;
        }
        public void FillClient(Client client) 
        {
            string connectionString = @"Server=A105512\\A105512;Database=CounterpartyMonitoring;Integrated Security=false;Trusted_Connection=True;MultipleActiveResultSets=True;User Id = CounterPartyMonitoring_user; Password = orppaAdmin123!";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.CommandText = @"INSERT INTO Client VALUES (@ClientManager, @Client_type_Id ,@BecomeClientDate,@ShortName,@FullName,@EnglName  ,@LicenceNumber,@LicenceEstDate,@RKC_BIK,@INN,@OGRN,@OGRN_Date,@RegName_RP,@RegDate_RP,@CurrencyLicence ,@RegistrationRegion,@NextScoringDate,@Country_Id)";
                command.Parameters.Add("@ClientManager", SqlDbType.NVarChar);
                command.Parameters.Add("@Client_type_Id", SqlDbType.Int);
                command.Parameters.Add("@BecomeClientDate", SqlDbType.Date);
                command.Parameters.Add("@ShortName", SqlDbType.NVarChar);
                command.Parameters.Add("@FullName", SqlDbType.NVarChar);
                command.Parameters.Add("@EnglName", SqlDbType.NVarChar);
                command.Parameters.Add("@LicenceNumber", SqlDbType.NVarChar);
                command.Parameters.Add("@LicenceEstDate", SqlDbType.Date);
                command.Parameters.Add("@RKC_BIK", SqlDbType.NVarChar);
                command.Parameters.Add("@INN", SqlDbType.NVarChar);
                command.Parameters.Add("@OGRN", SqlDbType.NVarChar);
                command.Parameters.Add("@OGRN_Date", SqlDbType.Date);
                command.Parameters.Add("@RegName_RP", SqlDbType.NVarChar);
                command.Parameters.Add("@RegDate_RP", SqlDbType.Date);
                command.Parameters.Add("@CurrencyLicence", SqlDbType.Bit);
                command.Parameters.Add("@RegistrationRegion", SqlDbType.NVarChar);
                command.Parameters.Add("@NextScoringDate", SqlDbType.Date);
                command.Parameters.Add("@Country_Id", SqlDbType.Int);
                // массив для хранения бинарных данных файла
                byte[] imageData;
                using (System.IO.FileStream fs = new System.IO.FileStream(connectionString, FileMode.Open))
                {
                    imageData = new byte[fs.Length];
                    fs.Read(imageData, 0, imageData.Length);
                }
                // передаем данные в команду через параметры
                command.Parameters["@ClientManager"].Value = client.ClientManager;
                command.Parameters["@Client_type_Id"].Value = client.Client_type_Id;
                command.Parameters["@BecomeClientDate"].Value = client.BecomeClientDate;
                command.Parameters["@ShortName"].Value = client.ShortName;
                command.Parameters["@FullName"].Value = client.FullName;
                command.Parameters["@EnglName"].Value = client.EnglName;
                command.Parameters["@LicenceNumber"].Value = client.LicenceNumber;
                command.Parameters["@LicenceEstDate"].Value = client.LicenceEstDate;
                command.Parameters["@RKC_BIK"].Value = client.RKC_BIK;
                command.Parameters["@INN"].Value = client.INN;
                command.Parameters["@OGRN"].Value = client.OGRN;
                command.Parameters["@OGRN_Date"].Value = client.OGRN_Date;
                command.Parameters["@RegName_RP"].Value = client.RegName_RP;
                command.Parameters["@RegDate_RP"].Value = client.RegDate_RP;
                command.Parameters["@CurrencyLicence"].Value = client.CurrencyLicence;
                command.Parameters["@RegistrationRegion"].Value = client.RegistrationRegion;
                command.Parameters["@NextScoringDate"].Value = client.NextScoringDate;
                command.Parameters["@Country_Id"].Value = client.Country_Id;


                command.ExecuteNonQuery();
                //throw new NotImplementedException(); // заглушка
            }


        }
    }
}
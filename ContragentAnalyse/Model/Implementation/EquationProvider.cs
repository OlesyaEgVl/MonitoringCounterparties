﻿using ContragentAnalyse.Extension;
using ContragentAnalyse.Model.Entities;
using ContragentAnalyse.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ContragentAnalyse.Model.Implementation
{
    public class EquationProvider : IEquationProvider
    {
        char sessionName = 'A';
        bool connected = false;
        private IDataProvider dataProvider;
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
            while (EUCL.Connect(sessionName.ToString()) != 0)
            {
                if (attempCount++ > 5)
                {
                    connected = false;
                    break;
                }
                sessionName++;
            }
            if (connected)
            {
                EUCL.ClearScreen();
            }
            #endregion
        }

        private void Disconnect()
        {
            EUCL.Disconnect($"{sessionName}");
        }

        public class NotConnectedException : Exception
        {
            public NotConnectedException(string message)
            : base(message)
            { }
        }

        public Client GetClient(string BINStr)
        {
            Connect();
            if (!connected)
            {
                throw new NotConnectedException("");
            }
            else
            {
                Client clients = new Client();
                EUCL.SetCursorPos(21, 17);
                EUCL.SendStr("ПБА");
                pEnter();
                EUCL.SetCursorPos(3, 33);
                EUCL.SendStr($"{BINStr}");
                pEnter();
                clients.BIN = BINStr;
                clients.TypeClient = dataProvider.GetClientType(EUCL.ReadScreen(4, 33, 2));
                string RKCBIK = EUCL.ReadScreen(21, 33, 9);
                clients.RKC_BIK = RKCBIK;
                string fullname = EUCL.ReadScreen(6, 33, 35);
                clients.FullName = fullname.TrimSpaces();
                string shortname = EUCL.ReadScreen(11, 33, 35) + EUCL.ReadScreen(12, 33, 35) + EUCL.ReadScreen(13, 33, 35);
                clients.ShortName = shortname.TrimSpaces();
                string Englname = EUCL.ReadScreen(14, 33, 35) + EUCL.ReadScreen(15, 33, 35);
                clients.EnglName = Englname.TrimSpaces();
                string LicenceNumber = EUCL.ReadScreen(18, 33, 20);
                clients.LicenceNumber = LicenceNumber.TrimSpaces();

                string licenceDate = EUCL.ReadScreen(20, 33, 11);
                if (EUCL.ReadScreen(20, 33, 1) != " ")
                {
                    clients.LicenceEstDate = Convert.ToDateTime(licenceDate);
                }
                string countrys = EUCL.ReadScreen(16, 33, 2);
                clients.Country = dataProvider.GetCountry(countrys.TrimSpaces());

                pEnter();
                string Mnemonic = EUCL.ReadScreen(5, 33, 4);// 
                clients.Mnemonic = Mnemonic.TrimSpaces();
                string INN = EUCL.ReadScreen(10, 33, 12);// 
                clients.INN = INN.TrimSpaces();
                string ORGN = EUCL.ReadScreen(15, 33, 20);// 
                clients.OGRN = ORGN.TrimSpaces();
                string OGRNDate = EUCL.ReadScreen(15, 60, 11);// 
                if (EUCL.ReadScreen(15, 60, 1) != " ")
                {
                    clients.OGRN_Date = Convert.ToDateTime(OGRNDate);
                }
                string regname = EUCL.ReadScreen(17, 33, 35);// 
                clients.RegName_RP = regname.TrimSpaces();
                string regdate = EUCL.ReadScreen(16, 60, 11);// 
                if (EUCL.ReadScreen(16, 60, 1) != " ")
                {
                    clients.RegDate_RP = Convert.ToDateTime(regdate);
                }
                string regregion = EUCL.ReadScreen(20, 33, 35);// 
                clients.RegistrationRegion = regregion.TrimSpaces();
                pEnter();
                string becomeclientdate = EUCL.ReadScreen(5, 33, 11);// дата перехода в клиенты 
                if (EUCL.ReadScreen(5, 33, 1) != " ")
                {
                    clients.BecomeClientDate = Convert.ToDateTime(becomeclientdate.TrimSpaces());
                }
                pEnter();
                string currencylicence = EUCL.ReadScreen(7, 33, 1);// валютная лицензия
                clients.CurrencyLicence = currencylicence.Equals("Y");
                string clientmanager = EUCL.ReadScreen(13, 39, 42);// КЛИНТ_МЕНЕД7ЖЕР имя и фамилию надо прописывать или код?
                clients.ClientManager = clientmanager.TrimSpaces();

                pEnter();
                pEnter();
                pEnter();
                string actdate = EUCL.ReadScreen(17, 29, 2) + '.' + EUCL.ReadScreen(17, 31, 2) + '.' + EUCL.ReadScreen(17, 33, 2);//дата актуализации
                if (EUCL.ReadScreen(17, 29, 1) != (" "))
                {
                    Actualization act = new Actualization();
                    act.DateActEKS = Convert.ToDateTime(actdate);
                    if (act.DateActEKS.AddMonths(10) > DateTime.Now)
                    {
                        act.Status = "Актуализирован";
                    }
                    else
                    if (act.DateActEKS.AddMonths(12) >= DateTime.Now && act.DateActEKS.AddMonths(10) < DateTime.Now)
                    {
                        act.Status = "Подлежит актуализации";
                    }
                    else
                    {
                        act.Status = "Заблокирован";
                    }
                    if (clients.Actualization == null) //Потом убрать (не нужна - Клиент всегда null)
                    {
                        clients.Actualization = new ObservableCollection<Actualization>();
                    }
                    clients.Actualization.Add(act);
                }
                pEnter();
                //Здесь был уровень риска
                pEnter();
                if (EUCL.ReadScreen(3, 6, 3).Equals("ИНН"))
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 5; i <= 20; i++)
                    {
                        string clientNameTemp = EUCL.ReadScreen(i, 40, 35);

                        if (clientNameTemp.IndexOf(BINStr) > -1)
                        {
                            sb.Append($"{EUCL.ReadScreen(i, 28, 6)}; ");
                        }


                        clients.AdditionalBIN = sb.ToString();

                    }
                    if (EUCL.ReadScreen(20, 79, 1).Equals("+"))
                    {
                        EUCL.SendStr("@v");
                        for (int i = 5; i <= 20; i++)
                        {
                            string clientNameTemp1 = EUCL.ReadScreen(i, 40, 35);
                            if (clientNameTemp1.IndexOf(clients.BIN) > -1)
                            {
                                sb.Append($"{EUCL.ReadScreen(i, 28, 6)}; ");
                            }
                            clients.AdditionalBIN = sb.ToString();
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
                EUCL.SetCursorPos(7, 69);
                EUCL.SendStr(clients.BIN);
                pEnter();
                EUCL.Wait();
                bool flagcurrency = true;
                string currencystring="";
                for (int ij = 0; ij <= 10; ij++)
                {
                    if (flagcurrency == true) 
                    {
                        for (int i = 8; i <= 20; i += 2)
                        {
                            if (string.IsNullOrWhiteSpace(EUCL.ReadScreen(i, 5, 24)))
                            {
                                break;
                            }
                            if (string.IsNullOrWhiteSpace(EUCL.ReadScreen(i, 64, 1)) && !currencystring.Contains(EUCL.ReadScreen(i, 11, 3))) //если счет открыт И счёт вообще существует
                            {
                                // счёт открыт
                                ClientToCurrency currtoclient = new ClientToCurrency();
                                Currency currenc = new Currency();
                                currtoclient.Currency = dataProvider.GetCurrencyByCode(EUCL.ReadScreen(i, 11, 3)); //Тут не должно быть List'a вообще
                                currtoclient.Client = clients;
                                if (clients.ClientToCurrency == null)
                                {
                                    clients.ClientToCurrency = new List<ClientToCurrency>();
                                }
                                clients.ClientToCurrency.Add(currtoclient);
                                currencystring += EUCL.ReadScreen(i, 11, 3) + ", ";
                            }
                        }
                       
                        if (EUCL.ReadScreen(21, 79, 1).Equals("+"))
                        {
                            EUCL.SendStr("@v");
                            EUCL.Wait();
                        }
                        else { flagcurrency = false; }
                    }
                }
                bool flagcontract= true;
                EUCL.ClearScreen();
                EUCL.SetCursorPos(21, 17);
                EUCL.SendStr("ДГП");
                pEnter();
                EUCL.Wait();
                EUCL.SetCursorPos(3, 30);
                EUCL.SendStr($"{BINStr}");
                EUCL.SetCursorPos(6, 30);
                EUCL.SendStr("*");
                pEnter();
                EUCL.Wait();
                clients.CardOP = false;
                clients.SEB = false;
                string contractstring = "";
                for (int ii = 0; ii <= 6; ii++)
                {
                    if (flagcontract == true)
                    {
                        for (int i = 6; i <= 19; i++)
                        {
                            if (EUCL.ReadScreen(i, 5, 1) != " " )
                            {
                                EUCL.SetCursorPos(i, 2);
                                EUCL.SendStr("1");
                                pEnter();
                                EUCL.Wait();
                                //проваливаемся в карточку
                                
                                
                                if (EUCL.ReadScreen(10, 30, 1) == " " && !contractstring.Contains(EUCL.ReadScreen(8, 41, 34).TrimSpaces()))
                                {
                                    ClientToContracts contractclient = new ClientToContracts();
                                    Contracts contract = new Contracts();
                                    contractclient.Contracts = dataProvider.GetContractByCode(EUCL.ReadScreen(8, 41, 34).TrimSpaces());
                                    contractclient.Client = clients;
                                    if (clients.ClientToContracts == null)
                                    {
                                        clients.ClientToContracts = new ObservableCollection<ClientToContracts>();
                                    }
                                    string datestart = EUCL.ReadScreen(9, 30, 2) + "." + EUCL.ReadScreen(9, 32, 2) + "." + EUCL.ReadScreen(9, 34, 2);
                                    if (datestart != "  .  .  ")
                                    {
                                        if (clients.CardOP != true && (EUCL.ReadScreen(8, 41, 3) == "Кор") && (EUCL.ReadScreen(10, 30, 1) == " ") && (Convert.ToDateTime(datestart) <= Convert.ToDateTime("01.01.16")))  //kop
                                        {
                                            clients.CardOP = true;
                                        }
                                    }

                                    if (EUCL.ReadScreen(8, 41, 12) == "Проверка СЭБ")
                                    {
                                        if ((Convert.ToDateTime(datestart).AddYears(1) >= DateTime.Now))
                                        {
                                            clients.SEB = true;
                                            clients.ClientToContracts.Add(contractclient);
                                            contractstring += EUCL.ReadScreen(8, 41, 34).TrimSpaces() + ",";
                                        }
                                    }
                                    else
                                    {
                                        clients.ClientToContracts.Add(contractclient);
                                        contractstring += EUCL.ReadScreen(8, 41, 34).TrimSpaces() + ",";
                                    }                         
                                }
                                
                                pEnter();
                                EUCL.Wait();
                                EUCL.SetCursorPos(6, 30);
                                EUCL.SendStr("*");
                                pEnter();
                                EUCL.Wait();
                            }
                        }
                        if (EUCL.ReadScreen(21, 79, 1).Equals("+"))
                        {
                            EUCL.SendStr("@v");
                            EUCL.Wait();
                        }
                        else { flagcontract = false; }
                    }
                }
                EUCL.ClearScreen();
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


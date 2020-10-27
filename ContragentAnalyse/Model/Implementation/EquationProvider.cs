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
                clients.BIN = BINStr;
                clients.TypeClient = dataProvider.GetClientType(EUCL.ReadScreen(4, 33, 2));
                string RKCBIK = EUCL.ReadScreen(21, 33, 9);//
                clients.RKC_BIK = RKCBIK;
                string fullname = EUCL.ReadScreen(6, 33, 35);// 
                clients.FullName = fullname.TrimSpaces(); //Удалять пробелы!!! + указать корректную длинну
                string shortname = EUCL.ReadScreen(11, 33, 35) + EUCL.ReadScreen(12, 33, 35) + EUCL.ReadScreen(13, 33, 35);// 
                clients.ShortName = shortname.TrimSpaces(); //WTF
                string Englname = EUCL.ReadScreen(14, 33, 35) + EUCL.ReadScreen(15, 33, 35);// 
                clients.EnglName = Englname.TrimSpaces();
                string LicenceNumber = EUCL.ReadScreen(18, 33, 20);// 
                clients.LicenceNumber = LicenceNumber.TrimSpaces();

                string licenceDate = EUCL.ReadScreen(20, 33, 11);// 
                if (EUCL.ReadScreen(20, 33, 1)!=" ")
                {
                    clients.LicenceEstDate = Convert.ToDateTime(licenceDate);
                }
                string countrys = EUCL.ReadScreen(16, 33, 2);
                clients.Country = dataProvider.GetCountry(countrys.TrimSpaces());
               
                pEnter();
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
                if (EUCL.ReadScreen(17, 29, 1)!=(" "))
                {
                    Actualization act = new Actualization();
                    act.DateActEKS = Convert.ToDateTime(actdate); //не совсем уверена, что верно сделала
                    //У актуализации обязательное поле статус. Ты обязана инициировать все обязательные поля.
                    if (act.DateActEKS.AddYears(1)>= DateTime.Now)
                    {
                        act.Status = "Заблокирован";
                    }
                    else 
                    if(act.DateActEKS.AddMonths(10) >= DateTime.Now && act.DateActEKS.AddMonths(12) < DateTime.Now)
                    {
                        act.Status = "Подлежит актуализации";
                    }
                    else
                    {
                        act.Status = "Актуализирован";
                    }
                    if (clients.Actualization == null) //Потом убрать (не нужна - Клиент всегда null)
                    {
                        clients.Actualization = new List<Actualization>();
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
                do
                {
                    for (int i = 8; i <= 20; i += 2)
                    {
                        if (string.IsNullOrWhiteSpace(EUCL.ReadScreen(i, 5, 24)))
                        {
                            break;
                        }
                        if (string.IsNullOrWhiteSpace(EUCL.ReadScreen(i, 64, 1))) //если счет открыт И счёт вообще существует
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
                        }
                    }
                    EUCL.SendStr("@v"); // правильно ли тут сделала преход вниз
                } while (EUCL.ReadScreen(21, 79, 1).Equals("+"));

                EUCL.ClearScreen();

                EUCL.SetCursorPos(21, 17);
                EUCL.SendStr("ДГП");
                pEnter();
                EUCL.SetCursorPos(3, 30);
                EUCL.SendStr($"{BINStr}");
                EUCL.SetCursorPos(6, 30);
                EUCL.SendStr("*");
                pEnter();
                if (EUCL.ReadScreen(6, 5, 1) != " ")
                {
                    for (int i = 6; i <= 19; i++)
                    {
                        if (EUCL.ReadScreen(i, 5, 1) != " ")
                        {
                            EUCL.SetCursorPos(i, 2);
                            EUCL.SendStr("1");
                            pEnter();
                            //проваливаемся в карточку
                            if (EUCL.ReadScreen(10, 30, 1) == " ")
                            {
                                ClientToContracts contractclient = new ClientToContracts();
                                Contracts contract = new Contracts();
                                contractclient.Contracts = dataProvider.GetContractByCode(EUCL.ReadScreen(8, 41, 34).TrimSpaces());
                                contractclient.Client = clients;
                                if (clients.ClientToContracts == null)
                                {
                                    clients.ClientToContracts = new List<ClientToContracts>();
                                }
                                clients.ClientToContracts.Add(contractclient);
                                if ((EUCL.ReadScreen(8, 41, 3) == "   ") && (Convert.ToDateTime(EUCL.ReadScreen(10, 30, 6)) <= Convert.ToDateTime("01.01.16")))  //kop
                                {
                                    clients.CardOP = true;
                                }
                                else
                                {
                                    clients.CardOP = true;
                                }
                            }
                            pEnter();
                            EUCL.SetCursorPos(6, 30);
                            EUCL.SendStr("*");
                            pEnter();
                        }
                    }
                    if (EUCL.ReadScreen(21, 79, 1).Equals("+"))
                    {
                        EUCL.SendStr("@v");
                        for (int i = 6; i <= 19; i++)
                        {
                            if (EUCL.ReadScreen(i, 5, 1) != " ")
                            {
                                EUCL.SetCursorPos(i, 2);
                                EUCL.SendStr("1");
                                pEnter();
                                //проваливаемся в карточку
                                if (EUCL.ReadScreen(10, 30, 1) == " ")
                                {
                                    ClientToContracts contractclient = new ClientToContracts();
                                    Contracts contract = new Contracts();
                                    contractclient.Contracts = dataProvider.GetContractByCode(EUCL.ReadScreen(8, 41, 34).TrimSpaces());
                                    contractclient.Client = clients;
                                    if (clients.ClientToContracts == null)
                                    {
                                        clients.ClientToContracts = new List<ClientToContracts>();
                                    }
                                    clients.ClientToContracts.Add(contractclient);
                                    if ((EUCL.ReadScreen(8, 41, 3) == "   ") && (Convert.ToDateTime(EUCL.ReadScreen(10, 30, 6)) <= Convert.ToDateTime("01.01.16")))  //kop
                                    {
                                        clients.CardOP = true;
                                    }
                                    else
                                    {
                                        clients.CardOP = true;
                                    }

                                }
                                pEnter();
                                EUCL.SetCursorPos(6, 30);
                                EUCL.SendStr("*");
                                pEnter();
                            }
                        }
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


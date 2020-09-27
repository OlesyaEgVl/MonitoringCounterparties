using System;
using System.Collections.Generic;
using System.Text;

namespace ContragentAnalyse.Model.Implementation
{
    class EUCLProgram
    {
        [System.Runtime.InteropServices.DllImport("PCSHLL32.dll")]
        public static extern uint hllapi(out uint Func, StringBuilder Data, out uint Length, out uint RetC);

    }
    /// <summary>
    /// Реализация основных функций Equation
    /// </summary>
    static class EUCL
    {
        /// <summary>
        /// Очистить экран (привести к начальному экрану)
        /// </summary>
        public static void ClearScreen()
        {
            if (ReadScreen(4, 3, 4).Substring(0, 4).Equals("####"))
            {
                SendStr("@f");
                Wait();
                SendStr("@E");
                Wait();
                SendStr("@E");
                Wait();
            }
            if (ReadScreen(4, 24, 3).Substring(0, 3).Equals("***"))
            {
                SendStr("@E");
                Wait();
                SendStr("@E");
                Wait();
            }
            if (ReadScreen(10, 2, 8).Substring(0, 8).Equals("EQUATION"))
            {
                SendStr("@E");
                Wait();
            }
            if (ReadScreen(21, 12, 4).Substring(0, 4).Equals("===>"))
            {
                return;
            }
            for (int i = 0; i < 100; i++)
            {
                SendStr("@3");
                Wait();
                if (ReadScreen(21, 12, 4).Substring(0, 4).Equals("===>"))
                {
                    return;
                }

            }
            SendStr("@3");
            Wait();
            SendStr("@3");
            Wait();
            SendStr("@f");
            Wait();
            SendStr("@E");
            Wait();
            SendStr("@E");
            Wait();
            return;
        } //Привести экран к начальному положению.
        /// <summary>
        /// Связаться с сессией Equation
        /// </summary>
        /// <param name="sessionID">Имя сессии (Буквенное обозначение), напр. "A"</param>
        /// <returns>0 - успешное соединение, 1 - ошибка при соединении</returns>
        public static uint Connect(string sessionID)
        {
            StringBuilder Data = new StringBuilder(4);
            //Data will contain the ID code of Session

            Data.Append(sessionID);
            uint rc = 0;
            uint f = 1; //function code
            uint l = 4; //lenght of data parameter
            return EUCLProgram.hllapi(out f, Data, out l, out rc);
            //return error code
        }
        /// <summary>
        /// Разорвать соединение с сессией Equation
        /// </summary>
        /// <param name="sessionID">Имя сессии (Буквенное обозначение), напр. "A"</param>
        /// <returns>0-успешно, 1-ошибка</returns>
        public static uint Disconnect(string sessionID)
        {
            StringBuilder Data = new StringBuilder(4);
            Data.Append(sessionID);
            uint rc = 0;
            uint f = 2;
            uint l = 4;
            return EUCLProgram.hllapi(out f, Data, out l, out rc);
        }
        /// <summary>
        /// Отправить строку
        /// </summary>
        /// <param name="cmd">Текст отправляемого сообщения, либо команды. Справочник "H:\WORK\Архив необычных операций\@ 7 ОФМА\Лебедев ИВ\Программы\УККО\Send_DOC\Документация\codes.cs"</param>
        /// <returns>0-успешно, 1-ошибка</returns>
        public static uint SendStr(string cmd)
        {
            StringBuilder Data = new StringBuilder(cmd.Length);
            //Data has the length of cmd string

            Data.Append(cmd);
            uint rc = 0;
            uint f = 3;
            uint l = (uint)cmd.Length;
            //l parameter contain the length of cmd string

            return EUCLProgram.hllapi(out f, Data, out l, out rc);
        }

        /// <summary>
        /// Прочитать текст с экрана
        /// </summary>
        /// <param name="Xposition">позиция курсора по координате X</param>
        /// <param name="Yposition">позиция курсора по координате Y></param>
        /// <param name="len">Длинна читаемого текста</param>
        /// <param name="size">Размер экрана 80 или 132</param>
        /// <returns>возвращает считанную строку</returns>
        public static string ReadScreen(int Xposition, int Yposition, int len, int size = 80)
        {
            StringBuilder Data = new StringBuilder(3000);
            //Initialization to a MAX char 
            //(> maximum number of char in a screen session)
            int position = (Xposition - 1) * size + Yposition;

            uint rc = (uint)position;
            //set initial position to start reading from

            uint f = 8;
            uint l = (uint)len;
            //set the number of chars that 
            //function will read from position

            uint r = EUCLProgram.hllapi(out f, Data, out l, out rc);
            return Data.ToString().Substring(0, len); //result

        }
        /// <summary>
        /// Ожидание готовности ввода Equation
        /// </summary>
        /// <returns>0-успешно, 1-ошибка</returns>
        public static uint Wait()
        {
            StringBuilder Data = new StringBuilder(0);
            uint rc = 0;
            uint f = 4;
            uint l = 0;
            uint r = EUCLProgram.hllapi(out f, Data, out l, out rc);
            return r;
        }
        /// <summary>
        /// Установить позицию каретки
        /// </summary>
        /// <param name="x">X координат</param>
        /// <param name="y">Y координат</param>
        /// <param name="size">Размер экрана 80 или 132</param>
        /// <returns></returns>
        public static UInt32 SetCursorPos(int x, int y, int size = 80)
        {
            int p = (x - 1) * size + y;
            StringBuilder Data = new StringBuilder(0);
            UInt32 rc = (UInt32)p;
            UInt32 f = 40;
            UInt32 l = 0;
            return EUCLProgram.hllapi(out f, Data, out l, out rc);
        }
    }
    /// <summary>
    /// Собранные функции, необходимые для работы программы
    /// </summary>
    public class Equation
    {
        /// <summary>
        /// Проверить соединение с Equation
        /// </summary>
        /// <param name="session">Название сессии (буквенное обозначение), например "B"</param>
        /// <returns>true - если Equation доступен, false, если соединение отсутствует</returns>
        public bool CheckConnection(string session)
        {
            if (EUCL.Connect(session) == 0)
            {

                if (EUCL.ReadScreen(3, 19, 3).Equals("###"))
                {
                    EUCL.Disconnect("A");
                    return false; // Сеанс в режиме ожидания
                }
                else
                {
                    EUCL.Disconnect("A");
                    return true;// Сеанс готов к работе
                }
            }
            else
                return false; //Сеанс закрыт
        }
    }
}

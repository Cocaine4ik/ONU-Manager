using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace ONU_Manager
{
    class ONUManager
    {
        static void Main(string[] args) {
                        
            string login; // переменная для логина
            string input; // переменная дял клиентского ввода
            string output; // переменная дял данных получаемых с сервера
            string sn; // переменная для серийного номера ONU
            string gponInfo;

            int oltNumber = 0; // номер олт
            int shelfNumber = 0; // номер платы на олт
            int ponNumber = 0; // номер пона на олт интерфейсе
            int onuNumber = 0; // номер слота для ону на поне
            int vlan = 1000; // vlan по умолчанию

            // Создание нового телнет соеденения по адресу "10.10.110.115" на порту номер "23"
            TelnetConnection tc = new TelnetConnection("10.10.110.115", 23);

            // залогиниться используя логин "admin", пароль "admin", с таймаутом в 100 мс
            // и показать ответ сервера
            login = tc.Login("admin", "admin", 300);
            Console.Write(login);
            Console.Write(tc.Read());

            // показаь незарегистрированые ONU
            tc.WriteLine("show gpon onu uncfg");

            // условие при котором ONU не надо регистрировать 
            output = tc.Read();
            if (output.Contains("No related information to show")) {

                Console.WriteLine("There are nothing to configure now!");
                Console.ReadKey(true);
            }
            else {
                // ловим отупут сервера и парсим только инфомрацию о местонахождении ONU и ее серийный номер
                String[] parseOutput = output.Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
                gponInfo = parseOutput[7]; // номер олт, платы, пона слота дял ону
                sn = parseOutput[8]; // серийный номер ону

                Console.WriteLine(gponInfo);

                // парсим информацию об ONU на номер олт, платы, пона в отдельности
                String[] parseGponInfo = gponInfo.Split(new char[] { '/', '_', ':'}, StringSplitOptions.RemoveEmptyEntries);
                // приводим String к int

                oltNumber = Int32.Parse(parseGponInfo[1]); // номера олт в числовом формате
                shelfNumber = Int32.Parse(parseGponInfo[2]); // номера платы в числовом формате
                ponNumber = Int32.Parse(parseGponInfo[3]); // номер пона в числовом формате

                tc.WriteLine("terminal length 0"); // снимаем ограничение на ввода для терминала

                String showGponOnuState = "show gpon onu state gpon-olt_" + oltNumber + "/"
                    + shelfNumber + "/" + ponNumber;
                String gponOnu = "gpon-onu_" + oltNumber + "/" + shelfNumber + "/" + ponNumber;

                tc.WriteLine(showGponOnuState); // получаем информацию о кол-ве ону и о налчиии свободных слотов на поне
                output = tc.Read();
                // Console.Write(output);

                // выбираем из всего вывода только занятых номера слотов на поне
                MatchCollection match = Regex.Matches(output, @":(\d+)");
                // обьявляем временные переменные которые помогут нам осуществить преобразование типов
                String delta = "";
                int num = 0;
                // создаем лист который будет содержать в себе информацию о занятых слотах на поне
                var slotList = new List<int>();

                // выбираем необходимые данные согласно регулярному выржанию заданному выше,
                // записвыаем каждый занятый слот на поне как отдельный елемент списка
                // int test = 0;
                foreach (Match m in match) {
                    delta += m.Groups[1];
                    num = Int32.Parse(delta);
                    slotList.Add(num);
                    delta = "";
                    num = 0;

                    // Console.WriteLine(slotList[test]);
                    // test++;
                }

                // создаем лист для записи свободных слотов на поне
                var freeSlotList = new List<int>();
                bool freeSlotFlag = false;

                // перебираем лист со всеми отображаемые слотами 
                for (int i = 0; i < slotList.Count; i++) {

                    // создаем вспомгательную переменную для записи разницы между проверяемым занятым  слотом и последующим занятым слотом
                    int temp = 0;

                    // првоерка чтобы при записи разницы в переменную temp не выйти за границу листа
                    if (i < slotList.Count - 1) {
                        temp = slotList[i + 1] - slotList[i];

                    }

                    // записываем в лист свободных слотов свободные слоты согласно разницы
                    if (temp > 1) {
                        for (int j = 1; j < temp; j++) {
                            freeSlotList.Add(slotList[i] + j);
                            freeSlotFlag = true;
                        }
                    }
                }
                // если пустых слотово нет записываем onuNumber следующий после последнего если он меньеше 128
                if (freeSlotFlag == false) {
                    if (slotList.Count + 1 < 128) {
                        onuNumber = slotList.Count + 1;
                    }
                    // если следующий равен 128 то предупреждаем об этом
                    else if (slotList.Count + 1 == 128) {
                        onuNumber = slotList.Count + 1;
                        Console.WriteLine("Warning! This is the last slot on PON!");
                    }
                    // если следующий больше 128 сворачиваем регистрацию и сообщаем об этом
                    else {
                        Console.WriteLine("Alert! This PON is full!");
                        tc.WriteLine("exit");
                    }
                }
                // записываем пустой слот в onuNumber
                else if (freeSlotFlag == true) {
                    onuNumber = freeSlotList[0];
                }
               /*
                for (int i = 0; i < freeSlotList.Count; i++) {
                    Console.WriteLine(freeSlotList[i]);
                }*/

                vlan = 1000 + (ponNumber * (shelfNumber - 1));
                Console.WriteLine(onuNumber);
                Console.WriteLine(oltNumber);
                Console.WriteLine(shelfNumber);
                Console.WriteLine(ponNumber);
                Console.WriteLine(vlan);
                
                // влючаем configure terminal
                tc.WriteLine("Configure terminal");
                Console.Write(tc.Read());
                

                // заходим на необходимый интерфейс
                tc.WriteLine("interface gpon-olt_" + oltNumber + "/" + shelfNumber + "/" + ponNumber);
                tc.WriteLine("onu " + onuNumber + " type universal sn " + sn); // регистририуем ону на слоте как универсальную
                Console.Write(tc.Read());
                
                // ставим скорость до 500 мб
                tc.WriteLine("onu " + onuNumber + " profile line 500m");
                Console.Write(tc.Read()); 
                
                 // ставим профель по стандарту
                 tc.WriteLine("onu " + onuNumber + " profile remote standart");
                 Console.Write(tc.Read());

                tc.WriteLine("exit");
                Console.Write(tc.Read());

                // Настройка оптичиеского порта

                // заходим на оптический порт
                tc.WriteLine("interface gpon-onu_" + oltNumber + "/" + shelfNumber + "/" + ponNumber + ":" + onuNumber);
                 Console.Write(tc.Read());

                //настраиваем влан на оптический порт
                tc.WriteLine("switchport vlan " + vlan + " tag");
                Console.Write(tc.Read());

                tc.WriteLine("exit");
                Console.Write(tc.Read());

                // Настройка ethernet порта

                // заходим на ethernet порт
                tc.WriteLine("pon-onu-mng gpon-onu_" + oltNumber + "/" + shelfNumber + "/" + ponNumber + ":" + onuNumber);
                Console.Write(tc.Read());

                // настраиваем  влан на ethernet порт
                tc.WriteLine("vlan port eth_0/1 mode tag vlan " + vlan);
                Console.Write(tc.Read());

                tc.WriteLine("exit");
                Console.Write(tc.Read());

                // првоеряем ли все настроено как надо
                tc.WriteLine("show running-config interface gpon-onu_" + oltNumber + "/" + shelfNumber + "/" + ponNumber + ":" + onuNumber);
                Console.Write(tc.Read());
                
                Console.ReadKey(true);
            }

        }
    }
}


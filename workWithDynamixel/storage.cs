using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting;
using System.Windows.Forms;

namespace workWithDynamixel
{
    public enum BaseReg
    {
        AR_BALL = 2,//done
        AR_BUTTON = 3,//done
        AR_LIGHT = 4,//done//haveArduino
        AR_DRIVER = 5,//done
        AR_DRIVER2 = 6,
        AR_ENCODER = 7,
        AR_IR_RX = 8,
        AR_LED = 9,//done
        AR_LINE = 10,
        AR_IMU_MAG = 16,
        AR_NMOS = 17,
        AR_NOISE = 18,
        AR_POT = 19,//done
        AR_PRESS = 20,
        AR_RGB = 21,//done//haveArduino
        AR_TEMP = 22,
        AR_uSW = 23,
        AR_BUZZER = 24//done//haveArduino
    }

    public enum baseNameOfServo
    {
        XL430_W250_T = 1060,
        AR_S430_01 = 1061,
    }

    internal class regs
    {
        public regs(int reg, int len, int val)
        {
            this.reg = reg;
            this.len = len;
            this.val = val;
        }

        public int reg { get; set; }
        public int len { get; set; }
        public int val { get; set; }
    }

    internal class dataForRead
    {
        public dataForRead(string registerName, int regToRead, int lengOfReg, bool readOnly)
        {
            this.registerName = registerName;
            this.regToRead = regToRead;
            this.lengOfReg = lengOfReg;
            this.readOnly = readOnly;
        }

        public string registerName { get; set; }
        public int regToRead { get; set; }
        public int lengOfReg { get; set; }
        public bool readOnly { get; set; }
        public string getRealData(int value, int ticks)
        {
            string gotResult = "";
            object temp = 0;
            switch (registerName)
            {
                case "Model Number":
                    return Enum.GetName(typeof(baseNameOfServo), value);
                case "ID":
                    gotResult = "ID " + value.ToString();
                    return gotResult;
                case "Baudrate":
                    switch (value)
                    {
                        case 0:
                            gotResult = "9600 bps";
                            break;
                        case 1:
                            gotResult = "57600 bps";
                            break;
                        case 2:
                            gotResult = "115200 bps";
                            break;
                        case 3:
                            gotResult = "1 Mbps";
                            break;
                        case 4:
                            gotResult = "2 Mbps";
                            break;
                        case 5:
                            gotResult = "3 Mbps";
                            break;
                        case 6:
                            gotResult = "4 Mbps";
                            break;
                        case 7:
                            gotResult = "4.5 Mbps";
                            break;
                    }
                    return gotResult;
                case "Return delay time":
                    temp = value * 2;
                    gotResult = temp.ToString() + " [μsec]";
                    return gotResult;
                case "Operation Mode":
                    switch (value)
                    {
                        case 1:
                            gotResult = "Velocity control";
                            break;
                        case 3:
                            gotResult = "Position control";
                            break;
                        case 4:
                            gotResult = "Extended Position Control";
                            break;
                        case 16:
                            gotResult = "PWM control";
                            break;
                    }
                    return gotResult;
                case "Shadow id":
                    if (value == 255) gotResult = "Disable";
                    else
                    {
                        gotResult = "Secondary ID " + value.ToString();
                    }
                    return gotResult;
                case "Protocol":
                    gotResult = "Protocol " + value.ToString() + ".0";
                    return gotResult;
                case "Homing offset":
                    double tempHomingOffset = 0;
                    switch (ticks)
                    {
                        case 4096:
                            tempHomingOffset = value * 0.087891;
                            break;
                        case 16383:
                            tempHomingOffset = value * 0.02197275;
                            break;
                    }
                    gotResult = tempHomingOffset.ToString() + " [°]";
                    return gotResult;
                case "Moving Threshold":
                    double tempMovingThreshold = value * 0.229;
                    gotResult = tempMovingThreshold.ToString() + " [rev/min]";
                    return gotResult;
                case "Temperature Limit":
                    gotResult = value.ToString() + " [°C]";
                    return gotResult;
                case "Max Voltage Limit":
                    temp = value * 0.1;
                    gotResult = temp.ToString() + " [V]";
                    return gotResult;
                case "Min Voltage Limit":
                    temp = value * 0.1;
                    gotResult = temp.ToString() + " [V]";
                    return gotResult;
                case "PWM Limit":
                    temp = value * 0.11299;
                    temp = Math.Ceiling((double)temp * 10) / 10;
                    gotResult = temp.ToString() + " [%]";
                    return gotResult;
                case "Velocity Limit":
                    temp = value * 0.229;
                    temp = Math.Ceiling((double)temp * 50) / 50;
                    gotResult = temp.ToString() + " [rev/min]";
                    return gotResult;
                case "Max Position Limit":
                    switch (ticks)
                    {
                        case 4096:
                            temp = value * 0.087891;
                            break;
                        case 16383:
                            temp = value * 0.02197275;
                            break;
                    }
                    temp = Math.Ceiling((double)temp * 50) / 50;
                    gotResult = temp.ToString() + " [°]";
                    return gotResult;
                case "Min Position Limit":
                    switch (ticks)
                    {
                        case 4096:
                            temp = value * 0.087891;
                            break;
                        case 16383:
                            temp = value * 0.02197275;
                            break;
                    }
                    temp = Math.Ceiling((double)temp * 50) / 50;
                    gotResult = temp.ToString() + " [°]";
                    return gotResult;
                case "Torque Enable":
                    if (value == 1) return "ON";
                    else return "OFF";
                case "LED":
                    if (value == 1) return "ON";
                    else return "OFF";
                case "Status Return Level":
                    switch (value)
                    {
                        case 0:
                            gotResult = "No return";
                            break;
                        case 1:
                            gotResult = "Return for read";
                            break;
                        case 2:
                            gotResult = "Return for all";
                            break;
                    }
                    return gotResult;
                case "Registered Instruction":
                    if (value == 1) gotResult = "Commands transmitted by REG_WRITE";
                    else gotResult = "No commands transmitted by REG_WRITE";
                    return gotResult;
                case "Bus Watchdog":
                    if (value == 0) gotResult = "Disable";
                    else
                    {
                        temp = value * 20;
                        gotResult = temp.ToString() + " [ms]";
                    }
                    return gotResult;
                case "Goal PWM":
                    temp = value * 0.11299;
                    temp = Math.Ceiling((double)temp * 10) / 10;
                    gotResult = temp.ToString() + " [%]";
                    return gotResult;
                case "Goal Velocity":
                    temp = value * 0.229;
                    temp = Math.Ceiling((double)temp * 50) / 50;
                    gotResult = temp.ToString() + " [rev/min]";
                    return gotResult;
                case "Profile Acceleration":
                    temp = value * 214.58;
                    gotResult = temp.ToString() + " [rev/min²]";
                    return gotResult;
                case "Profile Velocity":
                    temp = value * 0.229;
                    gotResult = temp.ToString() + " [rev/min]";
                    return gotResult;
                case "Goal Position":
                    switch (ticks)
                    {
                        case 4096:
                            temp = value * 0.087891;
                            break;
                        case 16383:
                            temp = value * 0.02197275;
                            break;
                    }
                    temp = Math.Ceiling((double)temp * 50) / 50;
                    gotResult = temp.ToString() + " [°]";
                    return gotResult;
                case "Realtime Tick":
                    gotResult = value + " [ms]";
                    return gotResult;
                case "Moving":
                    if (value == 1) return "Moving";
                    else return "Idle";
                case "Present PWM":
                    temp = value * 0.11299;
                    temp = Math.Ceiling((double)temp * 10) / 10;
                    gotResult = temp.ToString() + " [%]";
                    return gotResult;
                case "Present Load":
                    temp = value * 0.1;
                    gotResult = temp.ToString() + " [%]";
                    return gotResult;
                case "Present Velocity":
                    temp = value * 0.229;
                    temp = Math.Ceiling((double)temp * 50) / 50;
                    gotResult = temp.ToString() + " [rev/min]";
                    return gotResult;
                case "Present Position":
                    switch (ticks)
                    {
                        case 4096:
                            temp = value * 0.087891;
                            break;
                        case 16383:
                            temp = value * 0.02197275;
                            break;
                    }
                    temp = Math.Ceiling((double)temp * 50) / 50;
                    gotResult = temp.ToString() + " [°]";
                    return gotResult;
                case "Velocity Trajectory":
                    temp = value * 0.229;
                    temp = Math.Ceiling((double)temp * 50) / 50;
                    gotResult = temp.ToString() + " [rev/min]";
                    return gotResult;
                case "Position Trajectory":
                    switch (ticks)
                    {
                        case 4096:
                            temp = value * 0.087891;
                            break;
                        case 16383:
                            temp = value * 0.02197275;
                            break;
                    }
                    temp = Math.Ceiling((double)temp * 50) / 50;
                    gotResult = temp.ToString() + " [°]";
                    return gotResult;
                case "Present Input Voltage":
                    temp = value * 0.1;
                    gotResult = temp.ToString() + " [V]";
                    return gotResult;
                case "Present Temperature":
                    gotResult = value.ToString() + " [°C]";
                    return gotResult;
                default:
                    return "";
            }
        }
    }

    internal class groupReadWrite
    {
        public groupReadWrite(int group, int reg, int len) {
            this.groupNumber = group;
            this.registerLength = len;
            this.register = reg;
        }
        public int groupNumber { get; set; }
        public int register { get; set; }
        public int registerLength {  get; set; }
        public object valueToWrite { get; set; }
    }

    internal class storage
    {
        public static int lastId = 0;
        public static int openedPort = -1;
        public static string com_port = "";
        public static int listBoxLastIndex = -1;
        public static int gotBaudrate = 0;
        public static int usedProtocol = -1;
        public static int groupRead = -1;
        public static List<groupReadWrite> listGroupSyncRead = new List<groupReadWrite>();
        public static List<regs> regsToWriteWhileReading = new List<regs>();
        public PeripheryBase getClass(string type, int id, Form1 form)
        {
            switch (type)
            {
                case "AR_BUTTON":
                    return new AR_BUTTON(id);
                case "AR_LED":
                    return new AR_LED(id);
                case "AR_RGB":
                    return new AR_RGB(id);
                case "AR_BUZZER":
                    return new AR_BUZZER(id);
                case "AR_POT":
                    return new AR_POT(id);
                case "AR_LIGHT":
                    return new AR_LIGHT(id);
                case "AR_BALL":
                    return new AR_BALL(id);
                case "AR_TEMP":
                    return new AR_TEMP(id);
                case "AR_DRIVER":
                    return new AR_LIGHT(id);
                case "servo":
                    return new Servo(id); 
                default:
                    return new AR_TEMPLATE(id);
            }
        }

        public static int getTicks(int model_number)
        {
            switch(model_number)
            {
                case 1060:
                    return 4096;
                case 1061:
                    return 16383;
                default:
                    return 0;
            }
        }

        public static List<string> getRegInfo(string type)
        {
            List<string> list = new List<string>();
            for (int i = 0; i <= 23; i++)
            {
                if (i == 1) continue;
                list.Add("");
            }
            list[0] = "Базовый id. Доступ: R";
            list[1] = "Версия встроенного ПО. Доступ R";
            list[2] = "Адрес устройства. Доступ R/W";
            list[3] = "Скорость обмена данными. Доступ R";
            list[4] = "Return delay time";

            for(int i = 24; i < 50; i++)
            {
                switch(type)
                {
                    case "AR_BUTTON":
                        if (i == 27) list.Add("Состояние кнопки");
                        else list.Add("");
                        break;
                    case "AR_LED":
                        if (i == 26) list.Add("Интенсивность свечения светодиода");
                        else list.Add("");
                        break;
                    case "AR_RGB":
                        switch (i)
                        {
                            case 26:
                                list.Add("Интенсивность зеленого свечения");
                                break;
                            case 27:
                                list.Add("Интенсивность красного свечения");
                                break;
                            case 28:
                                list.Add("Интенсивность синего свечения");
                                break;
                            default:
                                list.Add("");
                                break;
                        }
                        break;
                    case "AR_BUZZER":
                        switch (i)
                        {
                            case 26:
                                list.Add("Значение частоты");
                                i++;
                                break;
                            case 28:
                                list.Add("Значение скважности");
                                break;
                            default:
                                list.Add("");
                                break;
                        }
                        break;
                    case "AR_POT":
                        switch(i)
                        {
                            case 26:
                                list.Add("Значение потенциометра");
                                break;
                            default:
                                list.Add("");
                                break;
                        }
                        break;
                    case "AR_LIGHT":
                        switch (i)
                        {
                            case 24: list.Add("Яркость фотодиода без фильтра. R"); i++;  break;
                            case 26: list.Add("Якрость фотодиода с красным фильтром. R"); i++; break;
                            case 28: list.Add("Яркость фотодиода с зеленым фильтром. R"); i++; break;
                            case 30: list.Add("Яркость фотодиода с синим фильтром. R"); i++; break;
                            case 32: list.Add("Освещение в люксах. R"); i++; break;
                            case 34: list.Add("Цветовая темп. в Кельвинах. R"); i++; break;
                            case 36: list.Add("Цветовые координаты по шкале HSV. R"); i++; break;
                            case 38: list.Add("Программное усиление. R"); break;
                            case 40: list.Add("Значение цветового оттенка. R"); break;
                            case 42: list.Add("Яркость светодиода подсветки. W"); break;
                            case 46: list.Add("Порог освещенности до церного. W"); i++; break;
                            case 48: list.Add("Минимальная разница между знач. К, С. З. W");break;
                            default:
                                list.Add("");
                                break;
                        }
                        break;
                    case "AR_BALL":
                        switch (i)
                        {
                            case 26:
                                list.Add("Состояние наклона по X");
                                break;
                            case 27:
                                list.Add("Состояние наклона по Y");
                                break;
                            default:
                                list.Add("");
                                break;
                        }
                        break;
                    case "AR_TEMP":
                        switch (i)
                        {
                            case 24:
                                list.Add("Целая часть от влажности");
                                break;
                            case 26:
                                list.Add("Десятичная часть от влажности");
                                break;
                            case 28:
                                list.Add("Целая часть от температуры");
                                break;
                            case 30:
                                list.Add("Десятичная часть от температуры");
                                break;
                            case 34:
                                list.Add("Значение температуры");i++;break;
                            default:
                                list.Add("");
                                break;
                        }
                        break;
                    case "AR_DRIVER":
                        switch (i)
                        {
                            case 24:
                                list.Add("Макс. мощность двигателя");i++;break;
                            case 27:
                                list.Add("Режим работы энкодера"); break;
                            case 28:
                                list.Add("Задает мощность двигателя"); i++; break;
                            case 32:
                                list.Add("Количество тиков энкодера"); i++; break;
                            case 34:
                                list.Add("Текущая скорость"); i++; break;
                            case 40:
                                list.Add("Задать скорость"); i++; break;
                            case 45:
                                list.Add("Длительность цикла управления");break;
                            case 47:
                                list.Add("ПИД регулятор. P");break;
                            case 48:
                                list.Add("ПИД регулятор. I");break;
                            case 49:
                                list.Add("ПИД регулятор. D");break;
                            default:
                                list.Add("");
                                break;
                        }
                        break;
                    default:
                        list.Add("");
                        break;
                }
            }

            return list;
        }

        public static List<groupReadWrite> fillSyncReadGroup(int modelNumber)
        {
            List<groupReadWrite> list = new List<groupReadWrite>();
            List<dataForRead> data = fillServoData(modelNumber);
            for (int i = 0; i < data.Count; i++)
            {
                list.Add(new groupReadWrite(baseDynamixel.getNewSyncReadGroup(data[i].regToRead, data[i].lengOfReg), data[i].regToRead, data[i].lengOfReg));
            }

            return list;
        }

        public static List<dataForRead> fillServoData(int modelNumber)
        {
            List<dataForRead> data = new List<dataForRead>();
            
            switch (modelNumber)
            {
                case 1060://XL_430_W_250
                    data.Add(new dataForRead("Model Number", 0, 2, true));
                    data.Add(new dataForRead("Model Information", 2, 4, true));
                    data.Add(new dataForRead("Firmware version", 6, 1, true));
                    data.Add(new dataForRead("ID", 7, 1, false));
                    data.Add(new dataForRead("Baudrate", 8, 1, false));
                    data.Add(new dataForRead("Return delay time", 9, 1, false));
                    data.Add(new dataForRead("Drive Mode", 10, 1, false));
                    data.Add(new dataForRead("Operation Mode", 11, 1, false));
                    data.Add(new dataForRead("Shadow id", 12, 1, false));
                    data.Add(new dataForRead("Protocol", 13, 1, false));
                    data.Add(new dataForRead("Homing offset", 20, 4, true));
                    data.Add(new dataForRead("Moving Threshold", 24, 4, true));
                    data.Add(new dataForRead("Temperature Limit", 31, 1, false));
                    data.Add(new dataForRead("Max Voltage Limit", 32, 2, false));
                    data.Add(new dataForRead("Min Voltage Limit", 34, 2, false));
                    data.Add(new dataForRead("PWM Limit", 36, 2, false));
                    data.Add(new dataForRead("Velocity Limit", 44, 4, false));
                    data.Add(new dataForRead("Max Position Limit", 48, 4, false));
                    data.Add(new dataForRead("Min Position Limit", 52, 4, false));
                    data.Add(new dataForRead("Startup Configuration", 60, 1, false));
                    data.Add(new dataForRead("Shutdown", 63, 1, false));
                    data.Add(new dataForRead("Torque Enable", 64, 1, false));
                    data.Add(new dataForRead("LED", 65, 1, false));
                    data.Add(new dataForRead("Status Return Level", 68, 1, false));
                    data.Add(new dataForRead("Registered Instruction", 69, 1, true));
                    data.Add(new dataForRead("Hardware Error Status", 70, 1, true));
                    data.Add(new dataForRead("Velocity I Gain", 76, 2, false));
                    data.Add(new dataForRead("Velocity P Gain", 78, 2, false));
                    data.Add(new dataForRead("Position D Gain", 80, 2, false));
                    data.Add(new dataForRead("Position I Gain", 82, 2, false));
                    data.Add(new dataForRead("Position D Gain", 84, 2, false));
                    data.Add(new dataForRead("Feedforward 2nd Gain", 88, 2, false));
                    data.Add(new dataForRead("Feedforward 1st Gain", 90, 2, false));
                    data.Add(new dataForRead("Bus Watchdog", 98, 1, false));
                    data.Add(new dataForRead("Goal PWM", 100, 2, false));
                    data.Add(new dataForRead("Goal Velocity", 104, 4, false));
                    data.Add(new dataForRead("Profile Acceleration", 108, 4, false));
                    data.Add(new dataForRead("Profile Velocity", 112, 4, false));
                    data.Add(new dataForRead("Goal Position", 116, 4, false));
                    data.Add(new dataForRead("Realtime Tick", 120, 2, true));
                    data.Add(new dataForRead("Moving", 122, 1, true));
                    data.Add(new dataForRead("Moving Status", 123, 1, true));
                    data.Add(new dataForRead("Present PWM", 124, 2, true));
                    data.Add(new dataForRead("Present Load", 126, 2, true));
                    data.Add(new dataForRead("Present Velocity", 128, 4, true));
                    data.Add(new dataForRead("Present Position", 132, 4, true));
                    data.Add(new dataForRead("Velocity Trajectory", 136, 4, true));
                    data.Add(new dataForRead("Position Trajectory", 140, 4, true));
                    data.Add(new dataForRead("Present Input Voltage", 144, 2, true));
                    data.Add(new dataForRead("Present Temperature", 146, 1, true));
                    data.Add(new dataForRead("Backup Ready", 147, 1, true));
                    break;
                case 1061://AR_S430_01
                    data.Add(new dataForRead("Model Number", 0, 2, true));
                    data.Add(new dataForRead("Model Information", 2, 4, true));
                    data.Add(new dataForRead("Firmware version", 6, 1, true));
                    data.Add(new dataForRead("ID", 7, 1, false));
                    data.Add(new dataForRead("Baudrate", 8, 1, false));
                    data.Add(new dataForRead("Return delay time", 9, 1, false));
                    data.Add(new dataForRead("Drive Mode", 10, 1, false));
                    data.Add(new dataForRead("Operation Mode", 11, 1, false));
                    data.Add(new dataForRead("Shadow id", 12, 1, false));
                    data.Add(new dataForRead("Protocol", 13, 1, false));
                    data.Add(new dataForRead("Homing offset", 20, 4, true));
                    data.Add(new dataForRead("Moving Threshold", 24, 4, true));
                    data.Add(new dataForRead("Temperature Limit", 31, 1, false));
                    data.Add(new dataForRead("Max Voltage Limit", 32, 2, false));
                    data.Add(new dataForRead("Min Voltage Limit", 34, 2, false));
                    data.Add(new dataForRead("PWM Limit", 36, 2, false));
                    data.Add(new dataForRead("Velocity Limit", 44, 4, false));
                    data.Add(new dataForRead("Max Position Limit", 48, 4, false));
                    data.Add(new dataForRead("Min Position Limit", 52, 4, false));
                    data.Add(new dataForRead("Startup Configuration", 60, 1, false));
                    data.Add(new dataForRead("Shutdown", 63, 1, false));
                    data.Add(new dataForRead("Torque Enable", 64, 1, false));
                    data.Add(new dataForRead("LED", 65, 1, false));
                    data.Add(new dataForRead("Status Return Level", 68, 1, false));
                    data.Add(new dataForRead("Registered Instruction", 69, 1, true));
                    data.Add(new dataForRead("Hardware Error Status", 70, 1, true));
                    data.Add(new dataForRead("Velocity I Gain", 76, 2, false));
                    data.Add(new dataForRead("Velocity P Gain", 78, 2, false));
                    data.Add(new dataForRead("Position D Gain", 80, 2, false));
                    data.Add(new dataForRead("Position I Gain", 82, 2, false));
                    data.Add(new dataForRead("Position D Gain", 84, 2, false));
                    data.Add(new dataForRead("Feedforward 2nd Gain", 88, 2, false));
                    data.Add(new dataForRead("Feedforward 1st Gain", 90, 2, false));
                    data.Add(new dataForRead("Bus Watchdog", 98, 1, false));
                    data.Add(new dataForRead("Goal PWM", 100, 4, false));
                    data.Add(new dataForRead("Goal Velocity", 104, 4, false));
                    data.Add(new dataForRead("Profile Acceleration", 108, 4, false));
                    data.Add(new dataForRead("Profile Velocity", 112, 4, false));
                    data.Add(new dataForRead("Goal Position", 116, 4, false));
                    data.Add(new dataForRead("Realtime Tick", 120, 2, true));
                    data.Add(new dataForRead("Moving", 122, 1, true));
                    data.Add(new dataForRead("Moving Status", 123, 1, true));
                    data.Add(new dataForRead("Present PWM", 124, 4, true));
                    data.Add(new dataForRead("Present Load", 126, 2, true));
                    data.Add(new dataForRead("Present Velocity", 128, 4, true));
                    data.Add(new dataForRead("Present Position", 132, 4, true));
                    data.Add(new dataForRead("Velocity Trajectory", 136, 4, true));
                    data.Add(new dataForRead("Position Trajectory", 140, 4, true));
                    data.Add(new dataForRead("Present Input Voltage", 144, 2, true));
                    data.Add(new dataForRead("Present Temperature", 146, 1, true));
                    data.Add(new dataForRead("Backup Ready", 147, 1, true));
                    break;
            }
            return data;
        }

        public static void firstDrawOfGrid(Form1 form, Dictionary<int, object> gotData, List<string> info)
        {
            if (form.dataGridView1.InvokeRequired)
            {
                form.dataGridView1.Invoke(new MethodInvoker(() =>
                {
                    form.dataGridView1.Rows.Clear();
                    form.dataGridView1.Columns.Clear();
                    form.dataGridView1.ColumnCount = 3;
                    form.dataGridView1.Columns[0].Name = "Регистр";
                    form.dataGridView1.Columns[0].Width = 100;
                    form.dataGridView1.Columns[1].Name = "Описание";
                    form.dataGridView1.Columns[1].Width = 239;
                    form.dataGridView1.Columns[2].Name = "Значения";
                    form.dataGridView1.Width = 500;
                    form.dataGridView1.ReadOnly = true;
                    form.dataGridView1.AllowUserToAddRows = false;

                    foreach (var pair in gotData)
                    {
                        form.dataGridView1.Rows.Add(pair.Key, " ", pair.Value);
                    }

                    foreach(DataGridViewRow row in form.dataGridView1.Rows)
                    {
                        try
                        {
                            row.Cells[1].Value = info[row.Index].ToString();
                        }
                        catch
                        {
                            row.Cells[1].Value = "";
                        }
                    }
                }));
            }
        }

        public static void firstDrawOfGrid(Form1 form, Dictionary<int, object> gotData, List<dataForRead> list)
        {
            if (form.dataGridView1.InvokeRequired)
            {
                form.dataGridView1.Invoke(new MethodInvoker(() =>
                {
                    form.dataGridView1.Rows.Clear();
                    form.dataGridView1.Columns.Clear();
                    form.dataGridView1.ColumnCount = 4;
                    form.dataGridView1.Columns[0].Name = "Регистр";
                    form.dataGridView1.Columns[0].Width = 100;
                    form.dataGridView1.Columns[1].Name = "Описание";
                    form.dataGridView1.Columns[1].Width = 120;
                    form.dataGridView1.Columns[2].Name = "Значения";
                    form.dataGridView1.Columns[2].Width = 70;
                    form.dataGridView1.Columns[3].Name = "Real";
                    form.dataGridView1.Columns[3].Width = 150;
                    form.dataGridView1.Width = 500;
                    form.dataGridView1.ReadOnly = true;
                    form.dataGridView1.AllowUserToAddRows = false;

                    foreach (var pair in gotData)
                    {
                        form.dataGridView1.Rows.Add(pair.Key, " ", pair.Value);
                    }

                    foreach (DataGridViewRow row in form.dataGridView1.Rows)
                    {
                        try
                        {
                            row.Cells[1].Value = list[row.Index].registerName;
                        }
                        catch
                        {
                            row.Cells[1].Value = "";
                        }
                    }
                }));
            }
        }

        public static void drawByData(Form1 form, Dictionary<int, object> gotData)
        {
            if (form.dataGridView1.InvokeRequired)
            {
                try
                {
                    form.dataGridView1.Invoke(new MethodInvoker(() =>
                    {
                        foreach (DataGridViewRow row in form.dataGridView1.Rows)
                        {
                            try
                            {
                                row.Cells[2].Value = gotData[Int32.Parse(row.Cells[0].Value.ToString())];
                            }
                            catch
                            {
                                row.Cells[2].Value = 0;
                            }
                        }
                    }));
                }
                catch
                {
                    return;
                }
            }
        }

        public static void drawByData(Form1 form, Dictionary<int, object> gotData, List<dataForRead> list, int ticks)
        {
            if (form.dataGridView1.InvokeRequired)
            {
                try
                {
                    form.dataGridView1.Invoke(new MethodInvoker(() =>
                    {
                        foreach (DataGridViewRow row in form.dataGridView1.Rows)
                        {
                            try
                            {
                                row.Cells[2].Value = gotData[Int32.Parse(row.Cells[0].Value.ToString())];
                                row.Cells[3].Value = list[row.Index].getRealData(Int32.Parse(row.Cells[2].Value.ToString()), ticks);
                            }
                            catch
                            {
                                row.Cells[2].Value = 0;
                            }
                        }
                    }));
                }
                catch
                {
                    return;
                }
            }
        }
    }
}

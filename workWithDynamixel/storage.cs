using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace workWithDynamixel
{
    public enum BaseReg
    {
        AR_BALL = 2,//done
        AR_BUTTON = 3,//done
        AR_LIGHT = 4,//done
        AR_DRIVER = 5,
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
        AR_RGB = 21,//done
        AR_TEMP = 22,
        AR_uSW = 23,
        AR_BUZZER = 24//done
    }

    internal class storage
    {
        public static int lastId = 0;
        public static int openedPort = -1;
        public static string com_port = "";
        public static int listBoxLastIndex = -1;
        public static int gotBaudrate = 0;
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
                default:
                    return null;
            }
        }
        public static List<string> getRegInfo(string type)
        {
            List<string> list = new List<string>();
            for (int i = 0; i <= 23; i++)
            {
                list.Add("");
            }
            list[0] = "Базовый id. Доступ: R/W";
            list[2] = "Версия встроенного ПО. Доступ R/W";
            list[3] = "Адрес устройства. Доступ R/W";
            list[4] = "Скорость обмена данными. Доступ R/W";
            list[5] = "Return delay time";

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
                                list.Add("Значение температуры");
                                break;
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

        public static void firstDrawOfGrid(Form1 form, Dictionary<int, int> gotData, List<string> info)
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

        public static void drawByData(Form1 form, Dictionary<int, int> gotData)
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
    }
}

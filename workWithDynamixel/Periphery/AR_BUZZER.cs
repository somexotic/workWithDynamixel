using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace workWithDynamixel
{
    internal class AR_BUZZER : PeripheryBase
    {
        public AR_BUZZER(int id)
        {
            gotId = id;
        }

        public override void createArduinoFile()
        {
            string fileName = "writeBuzzer.ino";
            string[] text =
            {
                "#include <DxlMaster.h> //Библиотека для работы с  Dynamixel",
                "",
                "DynamixelDevice buzzer(" + gotData[3].ToString() + "); //Инициализация устройства",
                "",
                "uint16_t freq = " + gotData[26].ToString() + "; //Частота звука",
                "uint8_t duty = " + gotData[28].ToString() + ";//Скважность звука",
                "",
                "void setup() {",
                "  DxlMaster.begin(57600); //Начало работы с Dynamixel устройствами",
                "  buzzer.init(); //Иницализация устройства",
                "  buzzer.write(26, freq); //Запись в регистр",
                "  buzzer.write(28, duty); //Запись в регистр",
                "}",
                "",
                "void loop() {",
                "}",
            };

            File.WriteAllLines(fileName, text);
        }

        public override void testDevice()
        {
            dyn.writeReg(gotId, 26, 255, 2);
            Thread.Sleep(200);
            dyn.writeReg(gotId, 28, 40, 1);
            MessageBox.Show("Buzzer должен издавать звук");
            Thread.Sleep(200);
            dyn.writeReg(gotId, 28, 0, 1);
        }

        public override bool needThreadPause()
        {
            return true;
        }
    }
}

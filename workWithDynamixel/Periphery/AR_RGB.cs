using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace workWithDynamixel
{
    internal class AR_RGB : PeripheryBase
    {
        public AR_RGB(int id)
        { 
            gotId = id;
        }

        public override void createArduinoFile()
        {
            string fileName = "writeRgb.ino";
            string[] text =
            {
                "#include <DxlMaster.h> //Библиотека для работы с  Dynamixel",
                "",
                "DynamixelDevice rgb(" + gotData[3].ToString() + "); //Инициализация устройства",
                "",
                "uint8_t red_value = 255; //Значение красного светодиода",
                "uint8_t green_value = 255;//Значение зеленого светодиода",
                "uint8_t blue_value = 255;//Значение синего светодиода",
                "",
                "void setup() {",
                "  DxlMaster.begin(57600); //Начало работы с Dynamixel устройствами",
                "  rgb.init(); //Иницализация устройства",
                "}",
                "",
                "void loop() {",
                "  rgb.write(26, green_value);",
                "  delay(200);",
                "  rgb.write(27, red_value);",
                "  delay(200);",
                "  rgb.write(28, blue_value);",
                "  delay(200);",
                "}",
            };

            File.WriteAllLines(fileName, text);
        }

        public override void testDevice()
        {
            dyn.writeReg(gotId, 26, 255, 1);
            MessageBox.Show("Зеленый светодиод должен загореться");
            Thread.Sleep(200);
            dyn.writeReg(gotId, 26, 0, 1);
            Thread.Sleep(200);
            dyn.writeReg(gotId, 27, 255, 1);
            MessageBox.Show("Красный светодиод должен загореться");
            Thread.Sleep(200);
            dyn.writeReg(gotId, 27, 0, 1);
            Thread.Sleep(200);
            dyn.writeReg(gotId, 28, 255, 1);
            MessageBox.Show("Синий светодиод должен загореться");
            Thread.Sleep(200);
            dyn.writeReg(gotId, 28, 0, 1);
        }

        public override bool needThreadPause()
        {
            return true;
        }
    }
}

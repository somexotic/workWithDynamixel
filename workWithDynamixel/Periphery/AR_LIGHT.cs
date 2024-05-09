using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace workWithDynamixel
{
    internal class AR_LIGHT : PeripheryBase
    {
        public AR_LIGHT(int id)
        {
            gotId = id;
        }

        public override void createArduinoFile()
        {
            string fileName = "readLight.ino";
            string[] text =
            {
                "#include <DxlMaster.h> //Библиотека для работы с  Dynamixel",
                "",
                "DynamixelDevice light(" + gotData[3].ToString() + "); //Инициализация устройства",
                "",
                "uint16_t bright_clear = 0;//Яркость фотодиода без фильтра.",
                "uint16_t bright_red = 0;//Якрость фотодиода с красным фильтром.",
                "uint16_t bright_green = 0;//Яркость фотодиода с зеленым фильтром.",
                "uint16_t bright_blue = 0;//Яркость фотодиода с синим фильтром.",
                "uint16_t bright_lux = 0;//Освещение в люксах.",
                "uint16_t bright_kelvin = 0;//Цветовая темп. в Кельвинах.",
                "uint16_t hsv = 0;//Цветовые координаты по шкале HSV.",
                "uint8_t gain = 0;//Программное усиление.",
                "uint8_t color = 0;//Значение цветового оттенка.",
                "uint8_t bright_light = 255;//Яркость светодиода подсветки.",
                "",
                "void setup() {",
                "  DxlMaster.begin(57600); //Начало работы с Dynamixel устройствами",
                "  light.init(); //Иницализация устройства",
                "  Serial.begin(115200);",
                "  light.write(42, bright_light);;",

                "}",
                "",
                "void loop() {",
                "  light.read(24, bright_clear);",
                "  light.read(26, bright_red);",
                "  light.read(28, bright_green);",
                "  light.read(30, bright_blue);",
                "  light.read(32, bright_lux);",
                "  light.read(34, bright_kelvin);",
                "  light.read(36, hsv);",
                "  light.read(38, gain);",
                "  light.read(40, color);",
                "  Serial.println(\"Яркость фотодиода без фильтра: \" + String(bright_clear));",
                "  Serial.println(\"Якрость фотодиода с красным фильтром: \" + String(bright_red));",
                "  Serial.println(\"Яркость фотодиода с зеленым фильтром: \" + String(bright_green));",
                "  Serial.println(\"Яркость фотодиода с синим фильтром: \" + String(bright_blue));",
                "  Serial.println(\"Освещение в люксах: \" + String(bright_lux));",
                "  Serial.println(\"Цветовая темп. в Кельвинах: \" + String(bright_kelvin));",
                "  Serial.println(\"Цветовые координаты по шкале HSV: \" + String(hsv));",
                "  Serial.println(\"Программное усиление: \" + String(gain));",
                "  Serial.println(\"Значение цветового оттенка: \" + String(color));",
                "  delay(200);",
                "}",
            };

            File.WriteAllLines(fileName, text);
        }

        public override void testDevice()
        {
            return;
        }

        public override bool needThreadPause()
        {
            return false;
        }
    }
}

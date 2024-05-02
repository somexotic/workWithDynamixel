using System;
using System.Collections.Generic;
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

        public override void getRegistersById(int id, Form1 form, CancellationToken token, ManualResetEvent pause)
        {
            getRegData("AR_RGB");
            var data = new dataStruct { prop1 = registers, id = id, regToRead = regToRead, lengOfReg = lengOfReg };
            gotData = dyn.getReg(data);
            if (firstDraw)
            {
                storage.firstDrawOfGrid(form, gotData, storage.getRegInfo("AR_RGB"));
                firstDraw = false;
            }
            while (!token.IsCancellationRequested)
            {
                gotData = dyn.getReg(data);
                storage.drawByData(form, gotData);
            }
        }

        public override void testDevice()
        {
            dyn.writeReg(gotId, 26, 255);
            MessageBox.Show("Зеленый светодиод должен загореться");
            Thread.Sleep(200);
            dyn.writeReg(gotId, 26, 0);
            Thread.Sleep(200);
            dyn.writeReg(gotId, 27, 255);
            MessageBox.Show("Красный светодиод должен загореться");
            Thread.Sleep(200);
            dyn.writeReg(gotId, 27, 0);
            Thread.Sleep(200);
            dyn.writeReg(gotId, 28, 255);
            MessageBox.Show("Синий светодиод должен загореться");
            Thread.Sleep(200);
            dyn.writeReg(gotId, 28, 0);
        }

        public override bool needThreadPause()
        {
            return true;
        }
    }
}

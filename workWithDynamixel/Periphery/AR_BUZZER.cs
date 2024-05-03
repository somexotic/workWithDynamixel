using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace workWithDynamixel
{
    internal class AR_BUZZER : PeripheryBase
    {
        public AR_BUZZER(int id)
        {
            gotId = id;
        }

        public override void getRegistersById(int id, Form1 form, CancellationToken token, ManualResetEvent pause)
        {
            getRegData("AR_BUZZER");
            var data = new dataStruct { prop1 = registers, id = id, regToRead = regToRead, lengOfReg = lengOfReg };
            gotData = dyn.getReg(data);
            if (firstDraw)
            {
                storage.firstDrawOfGrid(form, gotData, storage.getRegInfo("AR_BUZZER"));
                firstDraw = false;
            }
            while (!token.IsCancellationRequested)
            {
                if (pause.WaitOne())
                {
                    gotData = dyn.getReg(data);
                    lock (locker)
                    {
                        Monitor.Pulse(locker);
                    }
                    storage.drawByData(form, gotData);
                }
            }
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

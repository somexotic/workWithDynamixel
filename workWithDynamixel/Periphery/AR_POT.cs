using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace workWithDynamixel
{
    internal class AR_POT : PeripheryBase
    {
        public AR_POT(int id)
        {
            gotId = id;
        }

        public override void getRegistersById(int id, Form1 form, CancellationToken token, ManualResetEvent pause)
        {
            getRegData("AR_POT");
            var data = new dataStruct { prop1 = registers, id = id, regToRead = regToRead, lengOfReg = lengOfReg };
            gotData = dyn.getReg(data);
            if (firstDraw)
            {
                storage.firstDrawOfGrid(form, gotData, storage.getRegInfo("AR_POT"));
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
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            Thread thread = new Thread(() => waitForChange(token, source));
            thread.Start();
            MessageBox.Show("Покрутите потенциометр");
        }

        public override bool needThreadPause()
        {
            return false;
        }

        public void waitForChange(CancellationToken token, CancellationTokenSource source)
        {
            int lastValue = gotData[26];
            bool changed = false;
            lock (locker)
            {
                while (!token.IsCancellationRequested)
                {
                    while (lastValue == gotData[26])
                    {
                        Monitor.Wait(locker);
                        changed = true;
                    }
                    if(changed)
                    {
                        MessageBox.Show("Успешно");
                        source.Cancel();
                    }
                }
            }
        }
    }
}

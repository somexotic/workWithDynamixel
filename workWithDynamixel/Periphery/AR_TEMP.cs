using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace workWithDynamixel
{
    internal class AR_TEMP : PeripheryBase
    {
        public AR_TEMP(int id)
        {
            gotId = id;
        }

        public override void getRegistersById(int id, Form1 form, CancellationToken token, ManualResetEvent pause)
        {
            getRegData("AR_TEMP");
            var data = new dataStruct { prop1 = registers, id = id, regToRead = regToRead, lengOfReg = lengOfReg };
            gotData = dyn.getReg(data);
            if (firstDraw)
            {
                storage.firstDrawOfGrid(form, gotData, storage.getRegInfo("AR_TEMP"));
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
            throw new NotImplementedException();
        }

        public override bool needThreadPause()
        {
            throw new NotImplementedException();
        }
    }
}

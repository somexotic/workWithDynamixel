using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace workWithDynamixel
{
    internal class AR_BUTTON : PeripheryBase
    {
        public AR_BUTTON(int id)
        {
            gotId = id;
        }

        public override void getRegistersById(int id, Form1 form, CancellationToken token, ManualResetEvent pause)
        {
            getRegData("AR_BUTTON");
            var data = new dataStruct { prop1 = registers, id = id, regToRead = regToRead, lengOfReg = lengOfReg };
            gotData = dyn.getReg(data);
            if (firstDraw)
            {
                storage.firstDrawOfGrid(form, gotData, storage.getRegInfo("AR_BUTTON"));
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
            MessageBox.Show("Нажмите на кнопку");
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            Thread thread = new Thread(() => waitForChange(token, source));
            thread.Start();
        }

        public override bool needThreadPause()
        {
            return false;
        }

        public void waitForChange(CancellationToken token, CancellationTokenSource source)
        {
            lock (locker)
            {
                while (!token.IsCancellationRequested)
                {
                    while (gotData[27] != 1)
                    {
                        Monitor.Wait(locker);
                    }
                    if (gotData[27] == 1)
                    {
                        MessageBox.Show("Успешно");
                        source.Cancel();
                    }
                }
            }
        }
    }
}

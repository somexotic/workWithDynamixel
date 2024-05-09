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

        public override void createArduinoFile()
        {
            throw new NotImplementedException();
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

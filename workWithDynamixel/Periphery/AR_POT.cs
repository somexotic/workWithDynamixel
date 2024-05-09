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

        public override void createArduinoFile()
        {
            throw new NotImplementedException();
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

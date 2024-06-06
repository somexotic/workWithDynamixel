using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace workWithDynamixel
{
    internal abstract class PeripheryBase
    {
        public Dictionary<int, int> registers = new Dictionary<int, int>();
        protected Dictionary<int, object> gotData = new Dictionary<int, object>();
        protected baseDynamixel dyn = new baseDynamixel();
        public int gotId = 0;
        protected bool firstDraw = true;
        protected object locker = new object();
        public virtual void setTorque(bool status) { }
        public virtual void setLED(bool status) { }
        public void getRegistersById(Form1 form, CancellationToken token, ManualResetEvent pause, string peripheryType, CancellationTokenSource source)
        {
            getRegData(peripheryType);
            var data = new dataStruct { regToRead = regToRead, lengOfReg = lengOfReg, type = "periphery" };
            gotData = dyn.getReg(data);
            if (firstDraw)
            {
                storage.firstDrawOfGrid(form, gotData, storage.getRegInfo(peripheryType));
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
                    if(dyn.getZeroRegData() == 0)
                    {
                        source.Cancel();
                        MessageBox.Show("Device is disconnected. Stopping Thread");//Переделать на анимацию какую-то
                    }
                }
            }
        }

        public void getDataForServo(Form1 form, CancellationToken token, ManualResetEvent pause, CancellationTokenSource source)
        {
            int zeroRegdata = dyn.getZeroRegData();
            List<dataForRead> data = storage.fillServoData(zeroRegdata);
            int ticks = storage.getTicks(zeroRegdata);
            var structData = new dataStruct { servoData = data, type = "servo" };
            gotData = dyn.getReg(structData);
            dyn.setGroupSyncReadWrite(zeroRegdata);          
            if (firstDraw)
            {
                storage.firstDrawOfGrid(form, gotData, data);
                firstDraw = false;
            }
            while (!token.IsCancellationRequested)
            {
                if (pause.WaitOne())
                {
                    //gotData = dyn.getReg(structData);

                    if (storage.usedProtocol == 1)
                    {
                        gotData = dyn.getReg(structData);
                    }
                    else
                    {
                        gotData = dyn.startGroupSyncReadWrite();
                    }
                    storage.drawByData(form, gotData, data, ticks);
                    lock (locker)
                    {
                        Monitor.Pulse(locker);
                    }
                    if (dyn.getZeroRegData() == 0)
                    {
                        source.Cancel();
                        MessageBox.Show("Device is disconnected. Stopping Thread");//Переделать на анимацию какую-то
                    }
                }
            }
        }

        public abstract void testDevice();
        public abstract bool needThreadPause();
        public abstract void createArduinoFile();

        protected List<int> regToRead = new List<int>();
        protected List<int> lengOfReg = new List<int>();
        protected void getRegData(string type)
        {
            for(int i = 0; i <= 23; i++)
            {
                if (i == 1) continue;
                regToRead.Add(i);
                lengOfReg.Add(1);
            }
            switch (type)
            {
                case "AR_TEMP":
                    for(int i = 24; i < 50; i++)
                    {
                        switch (i)
                        {
                            case 34:
                                regToRead.Add(i); lengOfReg.Add(2); i++; break;
                            default:
                                regToRead.Add(i); lengOfReg.Add(1); break;
                        }
                    }
                    break;
                case "AR_BUZZER":
                    for (int i = 24; i < 50; i++)
                    {
                        switch (i)
                        {
                            case 26: regToRead.Add(i); lengOfReg.Add(2); i++; break;
                            default:
                                regToRead.Add(i); lengOfReg.Add(1); break;
                        }
                    }
                    break;
                case "AR_POT":
                    for(int i = 24; i < 50; i++)
                    {
                        switch (i)
                        {
                            case 26: regToRead.Add(i); lengOfReg.Add(2); i++; break;
                            default:
                                regToRead.Add(i); lengOfReg.Add(1); break;
                        }
                    }
                    break;
                case "AR_LIGHT":
                    for (int i = 24; i < 50; i++)
                    {
                        switch (i)
                        {
                            case 24: regToRead.Add(i); lengOfReg.Add(2); i++; break;
                            case 26: regToRead.Add(i); lengOfReg.Add(2); i++; break;
                            case 28: regToRead.Add(i); lengOfReg.Add(2); i++; break;
                            case 30: regToRead.Add(i); lengOfReg.Add(2); i++; break;
                            case 32: regToRead.Add(i); lengOfReg.Add(2); i++; break;
                            case 34: regToRead.Add(i); lengOfReg.Add(2); i++; break;
                            case 36: regToRead.Add(i); lengOfReg.Add(2); i++; break;
                            case 46: regToRead.Add(i); lengOfReg.Add(2); i++; break;
                            default:
                                regToRead.Add(i); lengOfReg.Add(1); break;
                        }
                    }
                    break;
                case "AR_DRIVER":
                    for (int i = 24; i < 50; i++)
                    {
                        switch (i)
                        {
                            case 24: regToRead.Add(i); lengOfReg.Add(2); i++; break;
                            case 28: regToRead.Add(i); lengOfReg.Add(2); i++; break;
                            case 32: regToRead.Add(i); lengOfReg.Add(2); i++; break;
                            case 34: regToRead.Add(i); lengOfReg.Add(2); i++; break;
                            case 40: regToRead.Add(i); lengOfReg.Add(2); i++; break;
                            default:
                                regToRead.Add(i); lengOfReg.Add(1); break;
                        }
                    }
                    break;
                default:
                    for (int i = 24; i < 50; i++)
                    {
                        regToRead.Add(i);
                        lengOfReg.Add(1);
                    }
                    break;
            }
        }
    }

    internal class dataStruct
    {
        public List<int> regToRead { get; set; }
        public List<int> lengOfReg { get; set; }
        public List<dataForRead> servoData { get; set; }
        public string type { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;

namespace workWithDynamixel
{
    internal abstract class PeripheryBase
    {
        public Dictionary<int, int> registers = new Dictionary<int, int>();
        protected Dictionary<int, int> gotData = new Dictionary<int, int>();
        protected baseDynamixel dyn = new baseDynamixel();
        public int gotId = 0;
        protected bool firstDraw = true;
        protected object locker = new object();
        public void getRegistersById(int id, Form1 form, CancellationToken token, ManualResetEvent pause, string peripheryType)
        {
            startReadData(peripheryType, id, form, token, pause);
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

        protected void startReadData(string peripheryType, int id, Form1 form, CancellationToken token, ManualResetEvent pause)
        {
            getRegData(peripheryType);
            var data = new dataStruct { prop1 = registers, id = id, regToRead = regToRead, lengOfReg = lengOfReg };
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
                }
            }
        }
    }

    internal class dataStruct
    {
        public Dictionary<int, int> prop1 { get; set; }
        public int id { get; set; }
        public List<int> regToRead { get; set; }
        public List<int> lengOfReg { get; set; }
        public Dictionary<int, int> result { get; set; }
    }
}
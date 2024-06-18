using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using dynamixel_sdk;

namespace workWithDynamixel
{
    internal class dynData
    {
        public dynData(int baseId, int uniqueId, string type, string name)
        {
            this.baseId = baseId;
            this.uniqueId = uniqueId;
            this.type = type;
            this.baseName = name;
        }
        public int baseId { get; set; }
        public int uniqueId { get; set; }
        public string type { get; set; }
        public string baseName { get; set; }
    }

    internal class baseDynamixel
    {
        public List<dynData> dynDataList = new List<dynData>();
        protected Dictionary<int, object> regData = new Dictionary<int, object>();
        protected dataStruct gotStruct = null;
        protected Form1 gotForm;
        protected object locker = new object();

        public baseDynamixel(Form1 form)
        {
            gotForm = form; 
        }
        public baseDynamixel () { }

        public async Task findDynamixel(string comPort, int baudrate, int len, int protocol)
        {
            if(dynDataList.Count > 0)
            {
                dynamixel.closePort(storage.openedPort);
            }
            dynDataList.Clear();
            storage.usedId.Clear();
            if (baudrate < 1) return;
            await Task.Run(() =>
            {
                try
                {
                    int port_num = dynamixel.portHandler(comPort);
                    dynamixel.packetHandler();
                    dynamixel.clearPort(port_num);
                    dynamixel.openPort(port_num);
                    dynamixel.setBaudRate(port_num, baudrate);
                    for (byte i = 1; i <= len; i++)
                    {

                        int baseId = dynamixel.read2ByteTxRx(port_num, protocol, i, 0);

                        if (baseId != 0)
                        {
                            if(baseId <= 100)
                            {
                                int value = dynamixel.read1ByteTxRx(port_num, protocol, i, 3);
                                dynDataList.Add(new dynData(baseId, value, "periphery", Enum.GetName(typeof(BaseReg), baseId)));
                                storage.usedId.Add(value, value);
                            }
                            else if (baseId > 100)
                            {
                                int value = dynamixel.read1ByteTxRx(port_num, protocol, i, 7);
                                dynDataList.Add(new dynData(1, value, "servo", Enum.GetName(typeof(baseNameOfServo), baseId)));
                                storage.usedId.Add(value, value);
                            }
                        }
                    }
                    if(dynDataList.Count > 0)
                    {
                        storage.openedPort = port_num;
                        storage.com_port = comPort;
                        storage.gotBaudrate = baudrate;
                        storage.usedProtocol = protocol;
                    }
                    else
                    {
                        dynamixel.closePort(port_num);
                    }
                }
                catch 
                {
                    Console.WriteLine("Error in baseDynamixel->findDynamixel");
                }
            });
        }

        public void reconnectToCurrentPort()
        {
            dynamixel.packetHandler();
            dynamixel.clearPort(storage.openedPort);
            dynamixel.openPort(storage.openedPort);
            dynamixel.setBaudRate(storage.openedPort, storage.gotBaudrate);
        }

        public void closeCurrentPort()
        {
            dynamixel.closePort(storage.openedPort);
        }

        public void checkIfNeedToWrite(string type)
        {
            int port = -1;
            if(type == "sync")
            {
                for (int i = 0; i < storage.regsToWriteWhileReading.Count; i++)
                {
                    port = dynamixel.groupSyncWrite(storage.openedPort, 2, (ushort)storage.regsToWriteWhileReading[i].reg, (ushort)storage.regsToWriteWhileReading[i].len);
                    dynamixel.groupSyncWriteAddParam(port, (byte)storage.lastId, (ushort)storage.regsToWriteWhileReading[i].val, (ushort)storage.regsToWriteWhileReading[i].len);
                    dynamixel.groupSyncWriteTxPacket(port);
                    dynamixel.groupSyncWriteClearParam(port);
                }
                storage.regsToWriteWhileReading.Clear();
            }
            else
            {
                for (int i = 0; i < storage.regsToWriteWhileReading.Count; i++)
                {
                    writeReg(storage.lastId, storage.regsToWriteWhileReading[i].reg, storage.regsToWriteWhileReading[i].val, storage.regsToWriteWhileReading[i].len);
                }
                storage.regsToWriteWhileReading.Clear();
            }
        }

        public void setGroupSyncReadWrite(int modelNumber)
        {
            storage.listGroupSyncRead = storage.fillSyncReadGroup(modelNumber);
            try
            {
                for (int i = 0; i < storage.listGroupSyncRead.Count; i++)
                {
                    dynamixel.groupSyncReadAddParam(storage.listGroupSyncRead[i].groupNumber, (byte)storage.lastId);
                }
            }
            catch
            {
                Console.WriteLine("Error in baseDynamixel->setGroupSyncRead");
            }   
        }

        public Dictionary<int, object> startGroupSyncReadWrite()
        {
            regData.Clear();
            try
            {
                storage.endGroupSyncRead = true;
                for (int i = 0; i < storage.listGroupSyncRead.Count; i++)
                {
                    dynamixel.groupSyncReadTxRxPacket(storage.listGroupSyncRead[i].groupNumber);
                    switch (storage.listGroupSyncRead[i].registerLength)
                    {
                        case 1:
                            regData.Add(storage.listGroupSyncRead[i].register, (byte)dynamixel.groupSyncReadGetData(storage.listGroupSyncRead[i].groupNumber, (byte)storage.lastId, (ushort)storage.listGroupSyncRead[i].register, (ushort)storage.listGroupSyncRead[i].registerLength));
                            break;
                        case 2:
                            regData.Add(storage.listGroupSyncRead[i].register, (ushort)dynamixel.groupSyncReadGetData(storage.listGroupSyncRead[i].groupNumber, (byte)storage.lastId, (ushort)storage.listGroupSyncRead[i].register, (ushort)storage.listGroupSyncRead[i].registerLength));
                            break;
                        case 4:
                            regData.Add(storage.listGroupSyncRead[i].register, dynamixel.groupSyncReadGetData(storage.listGroupSyncRead[i].groupNumber, (byte)storage.lastId, (ushort)storage.listGroupSyncRead[i].register, (ushort)storage.listGroupSyncRead[i].registerLength));
                            break;
                        default:
                            break;
                    }
                    checkIfNeedToWrite("sync");
                }
                storage.endGroupSyncRead = false;
            }
            catch
            {
                Console.WriteLine("Error in baseDynamixel->startGroupSyncReadWrite");
                return null;
            }
            return regData;
        }

        public bool testO = false;
        public int port = -1;
        public int port2 = -1;

        public static int getNewSyncReadGroup(int reg, int len)
        {
            int group = dynamixel.groupSyncRead(storage.openedPort, 2, (ushort)reg, (ushort)len);
            return group;
        }

        public void test()
        {
            if (!testO)
            {
                port = dynamixel.groupSyncRead(storage.openedPort, 2, 120, 2);
                port2 = dynamixel.groupSyncRead(storage.openedPort, 2, 31, 1);
                dynamixel.groupSyncReadAddParam(port, 1);
                dynamixel.groupSyncReadAddParam(port2, 1);
                testO = true;
            }
            dynamixel.groupSyncReadTxRxPacket(port);
            dynamixel.groupSyncReadTxRxPacket(port2);

            if (dynamixel.groupSyncReadIsAvailable(port, 1, 120, 2))
            {
                Console.WriteLine(dynamixel.groupSyncReadGetData(port, 1, 120, 2));
            }

            if (dynamixel.groupSyncReadIsAvailable(port, 1, 31, 1))
            {
                Console.WriteLine(dynamixel.groupSyncReadGetData(port2, 1, 31, 1));
            }
        }

        public async Task resetIds()
        {
            await Task.Run(() =>
            {
                for (byte i = 1; i < 255; i++)
                {
                    int baseId = dynamixel.read1ByteTxRx(storage.openedPort, storage.usedProtocol, i, 0);
                    if (baseId != 0)
                    { 
                        dynamixel.write1ByteTxRx(storage.openedPort, storage.usedProtocol, i, 3, (byte)baseId);
                    }
                }
                MessageBox.Show("Выполнено");
            });
        }

        public int getReg(int reg)
        {
            int gotData = dynamixel.read1ByteTxRx(storage.openedPort, storage.usedProtocol, (byte)storage.lastId, (ushort)reg);
            return gotData;
        }

        public void rebootItem()
        {
            dynamixel.reboot(storage.openedPort, storage.usedProtocol, (byte)storage.lastId);
        }

        public int getZeroRegData()
        {
            return dynamixel.read2ByteTxRx(storage.openedPort, storage.usedProtocol, (byte)storage.lastId, 0);
        }

        public Dictionary<int, object> getReg(object data)
        {
            gotStruct = (dataStruct)data;
            regData.Clear();
            try
            {
                if(gotStruct.type == "periphery")
                {
                    for (int i = 0; i < gotStruct.regToRead.Count; i++)
                    {
                        switch (gotStruct.lengOfReg[i])
                        {
                            case 1:
                                regData.Add(gotStruct.regToRead[i], dynamixel.read1ByteTxRx(storage.openedPort, storage.usedProtocol, (byte)storage.lastId, (ushort)gotStruct.regToRead[i]));
                                break;
                            case 2:
                                regData.Add(gotStruct.regToRead[i], dynamixel.read2ByteTxRx(storage.openedPort, storage.usedProtocol, (byte)storage.lastId, (ushort)gotStruct.regToRead[i]));
                                break;
                            case 4:
                                regData.Add(gotStruct.regToRead[i], dynamixel.read4ByteTxRx(storage.openedPort, storage.usedProtocol, (byte)storage.lastId, (ushort)gotStruct.regToRead[i]));
                                break;
                        }
                        checkIfNeedToWrite("regByReg");
                    }
                }
                else if (gotStruct.type == "servo")
                {
                    for(int i = 0; i < gotStruct.servoData.Count; i++)
                    {
                        switch(gotStruct.servoData[i].lengOfReg)
                        {
                            case 1:
                                regData.Add(gotStruct.servoData[i].regToRead, dynamixel.read1ByteTxRx(storage.openedPort, storage.usedProtocol, (byte)storage.lastId, (ushort)gotStruct.servoData[i].regToRead));
                                break;
                            case 2:
                                regData.Add(gotStruct.servoData[i].regToRead, dynamixel.read2ByteTxRx(storage.openedPort, storage.usedProtocol, (byte)storage.lastId, (ushort)gotStruct.servoData[i].regToRead));
                                break;
                            case 4:
                                regData.Add(gotStruct.servoData[i].regToRead, dynamixel.read4ByteTxRx(storage.openedPort, storage.usedProtocol, (byte)storage.lastId, (ushort)gotStruct.servoData[i].regToRead));
                                break;
                        }
                        checkIfNeedToWrite("regByReg");
                    }
                }
                return regData;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return regData;
        }

        public void writeReg(int id, int reg, int value, int byteSize)
        {
            try
            {
                switch(byteSize)
                {
                    case 1:
                        dynamixel.write1ByteTxRx(storage.openedPort, storage.usedProtocol, (byte)id, (ushort)reg, (byte)value);
                        break;
                    case 2:
                        dynamixel.write2ByteTxRx(storage.openedPort, storage.usedProtocol, (byte)id, (ushort)reg, (ushort)value);
                        break;
                    case 4:
                        dynamixel.write4ByteTxRx(storage.openedPort, storage.usedProtocol, (byte)id, (ushort)reg, (uint)value);
                        break;
                    default:
                        return;
                }
            }
            catch
            {
                return;
            }
        }
    }
}
using System;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Drawing;

namespace workWithDynamixel
{
    public partial class Form1 : Form
    {
        baseDynamixel dyn = null;
        PeripheryBase gotClass = null;
        storage store = new storage();
        public static Thread lastThread = null;
        protected bool gotClick = false;
        public static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        protected static CancellationToken cancellationToken = cancellationTokenSource.Token;
        protected ManualResetEvent pause = new ManualResetEvent(true);
        protected bool isDisconnected = false;
        protected string className = "";
        protected Thread read = null;
        public static int changeLed = 0;
        public static int changeTorque = 0;

        public Form1()
        {
            InitializeComponent();
            dyn = new baseDynamixel(this);
            string[] ports = SerialPort.GetPortNames();
            foreach(string port in ports)
            {
                comboBox1.Items.Add(port);
            }
            comboBox1.SelectedIndex = 0;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                cancellationTokenSource.Cancel();
                listBox1.Items.Clear();
                if (baud.Text == "") return;
                if (maxSearch.Text == "" || Int32.Parse(maxSearch.Text) < 1) return;
                if (comboBox1.SelectedItem.ToString() == "") return;
                if (Int32.Parse(protocol.Text) > 2 || Int32.Parse(protocol.Text) < 1) return;
                button1.Enabled = false;
                await dyn.findDynamixel(comboBox1.SelectedItem.ToString(), Int32.Parse(baud.Text), Int32.Parse(maxSearch.Text), Int32.Parse(protocol.Text));

                for(int i = 0; i < dyn.dynDataList.Count; i++)
                {
                    string str = "";

                    if (dyn.dynDataList[i].type == "periphery")
                    {
                        str += "P_";
                        str += dyn.dynDataList[i].baseName;
                        str += ": ";
                        str += dyn.dynDataList[i].uniqueId.ToString();
                    }
                    else if (dyn.dynDataList[i].type == "servo")
                    {
                        str = dyn.dynDataList[i].baseName + ": " + dyn.dynDataList[i].uniqueId.ToString();
                    }

                    listBox1.Items.Add(str);
                }
                button1.Enabled = true;
            }
            catch
            {
                button1.Enabled = true;
                Console.WriteLine("Error in Form1->button1_Click");
                return;
            }
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }
            comboBox1.SelectedIndex = 0;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!gotClick) return;
            dataGridView1.Visible = true;
            panelForServo.Visible = false;
            panelForPeriphery.Visible = false;
            if (listBox1.SelectedItem == null) return;
            storage.listBoxLastIndex = listBox1.SelectedIndex;
            string selected = listBox1.SelectedItem.ToString();
            string[] idStr = selected.Split(' ');
            string[] getName = selected.Split(':');
            int gotId = Int32.Parse(idStr[1]);
            storage.lastId = gotId;
            try
            {
                cancellationTokenSource.Cancel();
                Thread.Sleep(200);
                cancellationTokenSource = new CancellationTokenSource();
                cancellationToken = cancellationTokenSource.Token;
            }
            catch
            {
                Console.WriteLine("Error in Form1->listBox1_SelectedIndexChanged");
            }

            if (getName[0].ToString().StartsWith("P_"))
            {
                className = getName[0].ToString().Substring(2);
            }
            else
            {
                className = "servo";
            }

            gotClass = store.getClass(className, gotId, this);
            

            if (className == "servo")
            {
                read = new Thread(() => gotClass.getDataForServo(this, cancellationTokenSource.Token, pause, cancellationTokenSource));
                panelForServo.Visible = true;
                panelForServo.Left = 753;
                panelForServo.Top = 12;
                modelName.Text = dyn.dynDataList[listBox1.SelectedIndex].baseName;
            }
            else
            {
                read = new Thread(() => gotClass.getRegistersById(this, cancellationTokenSource.Token, pause, className, cancellationTokenSource));
                panelForPeriphery.Visible = true;
            }
                       
            lastThread = read;
            if(dyn.getReg(64) == 1)
            {
                torque.Checked = true;
                changeTorque = 1;
            }
            if(dyn.getReg(65) == 1)
            {
                LED.Checked = true;
                changeLed = 1;
            }
            read.Start();
            gotClick = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            cancellationTokenSource.Cancel();
            Thread.Sleep(100);
            cancellationTokenSource.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (regToWrite.Text == "" || Int32.Parse(regToWrite.Text) < 1) return;
            if (valueToWrite.Text == "" || Int32.Parse(valueToWrite.Text) < -1) return;
            if (byteSize.Text == "" || Int32.Parse(byteSize.Text) < 1 ) return;
            int byteSizeValue = Int32.Parse(byteSize.Text);
            if (byteSizeValue < 1 || byteSizeValue > 2) return;

            switch(regToWrite.Text)
            {
                case "0": return;
                case "1": return;
                case "2": return;
                case "4": return;
                case "5": return;
            }

            if (pause.Reset())
            {
                Thread.Sleep(250);
                dyn.writeReg(storage.lastId, Int32.Parse(regToWrite.Text), Int32.Parse(valueToWrite.Text), byteSizeValue);
            }
            if (Int32.Parse(regToWrite.Text) == 3)
            {
                storage.lastId = Int32.Parse(valueToWrite.Text);
                gotClass.gotId = Int32.Parse(valueToWrite.Text);
                string newStr = "";
                string[] str = listBox1.Items[storage.listBoxLastIndex].ToString().Split(' ');
                newStr += str[0] + " " + valueToWrite.Text;
                listBox1.Items[storage.listBoxLastIndex] = newStr;

            }
            pause.Set();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (gotClass.needThreadPause())
            {
                if (pause.Reset())
                {
                    Thread.Sleep(250);
                    gotClass.testDevice();
                }
                pause.Set();
            }
            else
            {
                gotClass.testDevice();
            }  
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            if (lastThread != null)
            {
                pause.Reset();
                Thread.Sleep(200);
                await dyn.resetIds();
                pause.Set();
            }
            else
            {
                await dyn.resetIds();
            }            
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                regToWrite.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            }
            catch
            {
                Console.WriteLine("Error in Form1->dataGridView1_CellClick");
            }
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                gotClick = true;
            }
            else
            {
                gotClick = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                gotClass.createArduinoFile();
            }
            catch
            {
                Console.WriteLine("Error in Form1->button5_Click");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            pause.Reset();
            Thread.Sleep(200);
            dyn.rebootItem();
            Thread.Sleep(200);
            pause.Set();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (!isDisconnected)
            {
                cancellationTokenSource.Cancel();
                Thread.Sleep(200);
                cancellationTokenSource = new CancellationTokenSource();
                cancellationToken = cancellationTokenSource.Token;
                dyn.closeCurrentPort();
                button7.Text = "Reconnect";
                isDisconnected = true;
            }
            else
            {
                dyn.reconnectToCurrentPort();
                if (className == "servo")
                {
                    read = new Thread(() => gotClass.getDataForServo(this, cancellationTokenSource.Token, pause, cancellationTokenSource));
                    panelForServo.Visible = true;
                    panelForServo.Left = 753;
                    panelForServo.Top = 12;
                    modelName.Text = dyn.dynDataList[listBox1.SelectedIndex].baseName;
                }
                else
                {
                    read = new Thread(() => gotClass.getRegistersById(this, cancellationTokenSource.Token, pause, className, cancellationTokenSource));
                    panelForPeriphery.Visible = true;
                }

                lastThread = read;
                read.Start();
                button7.Text = "Disconnect";
                isDisconnected = false;
            }
        }

        private void torque_CheckedChanged(object sender, EventArgs e)
        {
            if (changeTorque == 0)
            {
                storage.regsToWriteWhileReading.Add(new regs(64, 1, 1));
                changeTorque = 1;
            }
            else
            {
                storage.regsToWriteWhileReading.Add(new regs(64, 1, 0));
                changeTorque = 0;
            }
        }
        
        private void LED_CheckedChanged(object sender, EventArgs e)
        {
            if (changeLed == 0)
            {
                storage.regsToWriteWhileReading.Add(new regs(65, 1, 1));
                changeLed = 1;
            }
            else
            {
                storage.regsToWriteWhileReading.Add(new regs(65, 1, 0));
                changeLed = 0;
            }
        }
        int test = 1;
        private void button8_Click(object sender, EventArgs e)
        {
            if(test == 1)
            {
                storage.regsToWriteWhileReading.Add(new regs(116, 4, 3100));
                test = 0;
            }
            else
            {
                storage.regsToWriteWhileReading.Add(new regs(116, 4, 2047));
                test = 1;
            }
        }
    }
}

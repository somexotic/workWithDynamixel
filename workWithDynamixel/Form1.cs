using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace workWithDynamixel
{
    public partial class Form1 : Form
    {
        baseDynamixel dyn = null;
        PeripheryBase gotClass = null;
        storage store = new storage();
        public static Thread lastThread = null;
        public static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        protected static CancellationToken cancellationToken = cancellationTokenSource.Token;
        protected ManualResetEvent pause = new ManualResetEvent(true);

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
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                cancellationTokenSource.Cancel();
                listBox1.Items.Clear();
                if (baud.Text == "") return;
                if (maxSearch.Text == "" || Int32.Parse(maxSearch.Text) < 1) return;
                button1.Enabled = false;
                await dyn.findDynamixel(comboBox1.SelectedItem.ToString(), Int32.Parse(baud.Text), Int32.Parse(maxSearch.Text));
                
                for(int i = 0; i < dyn.listOfIds.Count; i++)
                {
                    string str = dyn.listOfNames[i] + ": " + dyn.listOfIds[i].ToString();
                    listBox1.Items.Add(str);
                }
                button1.Enabled = true;
            }
            catch (Exception ex)
            {
                button1.Enabled = true;
                Console.WriteLine(ex.Message);
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
            string selected = listBox1.SelectedItem.ToString();
            string[] idStr = selected.Split(' ');
            string[] getName = selected.Split(':');
            int gotId = Int32.Parse(idStr[1]);
            storage.lastId = gotId;
            try
            {
                if (lastThread != null)
                {
                    cancellationTokenSource.Cancel();
                    Thread.Sleep(200);
                    cancellationTokenSource = new CancellationTokenSource();
                    cancellationToken = cancellationTokenSource.Token;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            gotClass = store.getClass(getName[0].ToString(), gotId, this);
            Thread read = new Thread(() => gotClass.getRegistersById(gotId, this, cancellationTokenSource.Token, pause));
            lastThread = read;
            read.Start();
            panel1.Visible = true;
            testPanel.Visible = true;
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
            if (pause.Reset())
            {
                Thread.Sleep(250);
                dyn.writeReg(storage.lastId, Int32.Parse(regToWrite.Text), Int32.Parse(valueToWrite.Text));
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
            regToWrite.Text = e.RowIndex.ToString();
        }
    }
}

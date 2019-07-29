using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using UpperComputer2.Core;

namespace UpperComputer2
{
    public partial class Form1 : Form
    {
        private XInputController controller = null;
        public Form1()
        {



            InitializeComponent();
            this.timer1.Interval = 100;
            this.timer1.Stop();
            string[] baud = { "300", "1200", "2400", "4800", "9600", "19200", "38400", "57600" };
            comboBox2.Items.AddRange(baud);     //添加波特率列表
            string[] sps = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(sps);


            //设置默认值
            comboBox1.Text = "COM3";
            comboBox2.Text = "115200";
            //comboBox2.Text = "9600";
            comboBox3.Text = "8";
            comboBox4.Text = "None";
            comboBox5.Text = "1";

            //初始化
            //初始化手柄
            controller = new XInputController();
            if (controller.connected) {
                Console.WriteLine("你连上了！");
                //  this.textBox4.Text = "the handle has connected...\r\n";
                //this.button1.Enabled = true;
            }
            else
            {
                Console.WriteLine("太惨了你连上了！");

            }


        }
        private void Form1_Activated(object sender, EventArgs e)
        {
            this.Focus();
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            //trycatcj处理串口打开过程中的异常
            try
            {
                //将可能产生异常的代码放置在try块中
                //根据当前串口属性来判断是否打开
                if (serialPort1.IsOpen)
                {
                    //串口已经处于打开状态
                    serialPort1.Close();    //关闭串口
                    button1.Text = "打开串口";
                    button1.BackColor = Color.ForestGreen;
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox4.Enabled = true;
                    comboBox5.Enabled = true;
                    ReceptTb.Text = "";  //清空接收区
                    SendTb.Text = "";     //清空发送区
                    this.timer1.Stop();

                }
                else
                {
                    //串口已经处于关闭状态，则设置好串口属性后打开
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = false;
                    comboBox4.Enabled = false;
                    comboBox5.Enabled = false;
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                    serialPort1.DataBits = Convert.ToInt16(comboBox3.Text);

                    if (comboBox4.Text.Equals("None"))
                        serialPort1.Parity = System.IO.Ports.Parity.None;
                    else if (comboBox4.Text.Equals("Odd"))
                        serialPort1.Parity = System.IO.Ports.Parity.Odd;
                    else if (comboBox4.Text.Equals("Even"))
                        serialPort1.Parity = System.IO.Ports.Parity.Even;
                    else if (comboBox4.Text.Equals("Mark"))
                        serialPort1.Parity = System.IO.Ports.Parity.Mark;
                    else if (comboBox4.Text.Equals("Space"))
                        serialPort1.Parity = System.IO.Ports.Parity.Space;

                    if (comboBox5.Text.Equals("1"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.One;
                    else if (comboBox5.Text.Equals("1.5"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.OnePointFive;
                    else if (comboBox5.Text.Equals("2"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.Two;

                    serialPort1.Open();     //打开串口
                    button1.Text = "关闭串口";
                    button1.BackColor = Color.Firebrick;
                    this.timer1.Start();//开始接收手柄输入
                    this.Activate();
                }
            }
            catch (Exception ex)
            {
                //捕获可能发生的异常并进行处理

                //捕获到异常，创建一个新的对象，之前的不可以再用
                serialPort1 = new System.IO.Ports.SerialPort();
                //刷新COM口选项
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                button1.Text = "打开串口";
                button1.BackColor = Color.ForestGreen;
                MessageBox.Show(ex.Message);
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
            }
        }


        /// <summary>
        /// 启动
        /// </summary>
        private void SendBtn_Click(object sender, EventArgs e)
        {
            try
            {

                //首先判断串口是否开启
                if (serialPort1.IsOpen)
                {
                    //串口处于开启状态，将发送区文本发送
                    serialPort1.Write(SendTb.Text);
                    SendTb.Text = "";
                }
            }
            catch (Exception ex)
            {
                //捕获到异常，创建一个新的对象，之前的不可以再用
                serialPort1 = new System.IO.Ports.SerialPort();
                //刷新COM口选项
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                button1.Text = "打开串口";
                button1.BackColor = Color.ForestGreen;
                MessageBox.Show(ex.Message);
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
            }
        }


        private void SerialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                //因为要访问UI资源，所以需要使用invoke方式同步ui
                this.Invoke((EventHandler)(delegate
                {

                    //string str = serialPort1.ReadExisting();//字符串方式读
                    //ReceptTb.AppendText(str);
                    ReceptTb.AppendText(serialPort1.ReadExisting());
                    //Console.WriteLine(serialPort1.ReadExisting());
                    //ReceptTb.AppendText(serialPort1.ReadExisting());
                    //string[] strArray = str.Split(',');


                    //int byteNumber = serialPort1.BytesToRead;
                    //Console.Write("byte"+byteNumber);
                    //Thread.Sleep(20);


                    ////延时等待数据接收完毕。
                    //while ((byteNumber < serialPort1.BytesToRead) && (serialPort1.BytesToRead < 77))
                    //{
                    //    byteNumber = serialPort1.BytesToRead;
                    //    Thread.Sleep(20);

                    //}
                    //int n = serialPort1.BytesToRead; //记录下缓冲区的字节个数 
                    //byte[] buf = new byte[n]; //声明一个临时数组存储当前来的串口数据 
                    //serialPort1.Read(buf, 0, n); //读取缓冲数据到buf中，同时将这串数据从缓冲区移除 
                    //string str = System.Text.Encoding.Default.GetString(buf);
                    //string[] strArray = str.Split(',');


                    //int count = 0;
                    //for (int i = 0; i < strArray.Length; i++)
                    //{
                    //    if(count == 0)
                    //    {
                    //        textBox1.Text = strArray[i];
                    //    }else if(count == 1)
                    //    {
                    //        textBox2.Text = strArray[i];
                    //    }else if(count == 2)
                    //    {
                    //        textBox3.Text = strArray[i];
                    //    }
                    //    count++;
                    //    if(count == 3)
                    //    {
                    //        count = 0;
                    //    }


                    //    //textBox2.Text = strArray[i + 1];
                    //    //textBox3.Text = strArray[i + 2];
                    //    //Console.WriteLine(strArray[i]);
                    //}



                    ////StringBuilder sb = new StringBuilder();
                    ////for (int i = 0; i < n; i++)
                    ////{
                    ////    string s;
                    ////    if (buf[i] < 16)
                    ////        s = "0" + Convert.ToString(buf[i], 16).ToUpper() + " ";
                    ////    else
                    ////        s = Convert.ToString(buf[i], 16).ToUpper() + " ";


                    ////    sb.Append(s);
                    ////}

                    ////string str = sb.ToString();
                    ////float Num = float.Parse(str);


                    //Console.Write(str);

                }
                   )
                );

            }
            catch (Exception ex)
            {
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show(ex.Message);

            }
        }
        //按下上下左右进行反应
        //左箭头     37   
        //向上箭头    38
        //右箭头     39
        //向下箭头    40


        Boolean downnum = false;
        private void Button2_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("8888");





        }
        //Ascii转字符串
        public static string Ascii2Str(byte[] buf)
        {
            return System.Text.Encoding.ASCII.GetString(buf);
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //重点在于要重新聚焦
            String key = e.KeyValue.ToString();
            String signal = "";

            switch (key)
            {
                //case "65":
                case "37": signal = "l";
                    //按下左
                    break;
                // case "68":
                case "39": signal = "r";
                    //按下小键盘1以后
                    break;
                // case "87":
                case "38": signal = "f";
                    //按下向上键以后
                    break;
                //case "83":
                case "40": signal = "b";
                    //按下向下键以后
                    break;
            }

            try
            {

                //首先判断串口是否开启
                if (serialPort1.IsOpen)
                {
                    //串口处于开启状态，发送
                    serialPort1.Write(signal);
                }
            }
            catch (Exception ex)
            {
                //捕获到异常，创建一个新的对象，之前的不可以再用
                serialPort1 = new System.IO.Ports.SerialPort();
                //刷新COM口选项
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                button1.Text = "打开串口";
                button1.BackColor = Color.ForestGreen;
                MessageBox.Show(ex.Message);
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
            }
        }
        private void HandleControlForm_Load(object sender, EventArgs e)
        {

        }
        public int liCount = 0;
        /// <summary>
        /// 定时器处理
        /// </summary>
        private void Timer1_Tick(object sender, EventArgs e)
        {

            string input = null;
            mypoint pLeft = null;
            mypoint pRight = null;
            float tLeft;
            float tRight;
            controller.GetDirection(out tLeft, out tRight, out pLeft, out pRight, out input);
            if (input == null)
            {
                this.textBox1.Text += "the handle disconnect...\r\n";
                return;
            }
            // this.textBox4.Text += input + "\r\n";
            Console.WriteLine("888:" + input);
            Console.WriteLine("leftx:" + pLeft.X + " lefty:" + pLeft.Y);
            Console.WriteLine("rightx:" + pRight.X + " righty:" + pRight.Y);
            Console.WriteLine("leftt:" + tLeft + " rightt:" + tRight);
            //向下位机发送控制指令
            byte[] li;
            if (input == "li")
            {
                Console.WriteLine("sssssssssssssssssss");

                liCount++;
                if (liCount == 1)
                {
                    li = BitConverter.GetBytes(0);
                }
                else
                {
                    li = BitConverter.GetBytes(1);
                    liCount = 0;
                }
            }

            //字符串指令


            string str1 = "ol";
            string str2 = "or";
            byte data_ol = Convert.ToByte(tLeft);

            byte data_or = Convert.ToByte(tRight);
                


            byte[] data2_ol = new byte[] { data_ol, 0, 0 };
            byte[] data2_or = new byte[] { data_or, 0, 0 };
            byte[] data1_ol = Encoding.Default.GetBytes(str1);

            byte[] data3_ol = new byte[data1_ol.Length + data2_ol.Length];
            Array.Copy(data1_ol, 0, data3_ol, 0, data1_ol.Length);
            Array.Copy(data2_ol, 0, data3_ol, data1_ol.Length, data2_ol.Length);
            serialPort1.Write(data3_ol, 0, data3_ol.Length);



            byte[] data1_or = Encoding.Default.GetBytes(str2);

            byte[] data3_or = new byte[data1_or.Length + data2_or.Length];
            Array.Copy(data1_or, 0, data3_or, 0, data1_or.Length);
            Array.Copy(data2_or, 0, data3_or, data1_or.Length, data2_or.Length);
            serialPort1.Write(data3_or, 0, data3_or.Length);


            //转sbyte
            sbyte[] mySByte = new sbyte[data3_or.Length];

            for (int i = 0; i < data3_or.Length; i++)
            {
                if (data3_or[i] > 127)
                    mySByte[i] = (sbyte)(data3_or[i] - 256);
                else
                    mySByte[i] = (sbyte)data3_or[i];
            }


            //if (pLeft.Y > 100)
            //{
            //    pLeft.Y = 100;
            //}
            //else if (pLeft.Y < -100)
            //{
            //    pLeft.Y = -100;
            //}
            //else if (pRight.X > 100)
            //{
            //    pRight.X = 100;
            //}
            //else if (pRight.X < -100)
            //{
            //    pRight.X = -100;
            //}
            //else if (pRight.Y > 100)
            //{
            //    pRight.X = 100;
            //}
            //else if (pRight.Y < -100)
            //{
            //    pRight.X = -100;
            //}

            //string str3 = "mv";
            //sbyte data_z = Convert.ToSByte(pLeft.Y);
            //sbyte data_x = Convert.ToSByte(pRight.X);
            //sbyte data_y = Convert.ToSByte(pRight.Y);
            //sbyte[] data5 = new byte[] { data_x, data_y, data_z };
            //sbyte[] data4 = Encoding.Default.GetBytes(str3);

            //byte[] data6 = new byte[data4.Length + data5.Length];
            //Array.Copy(data4, 0, data6, 0, data4.Length);
            //Array.Copy(data5, 0, data6, data4.Length, data5.Length);
            //serialPort1.Write(data6, 0, data6.Length);


            //向小车发送控制指令
            //byte[] buffer = Encoding.UTF8.GetBytes(input);
            //streamToServer.Write(buffer, 0, buffer.Length);
            //if (input == "back")
            //{
            //    //关闭
            //    this.timer1.Stop();
            //    streamToServer.Close();
            //    client.Close();
            //    streamToServer = null;
            //    client = null;
            //    this.button1.Enabled = true;
            //    this.textBox1.Text += "shut down the connection...\r\n";
            //}
        }

        

        private void Form1_Load(object sender, EventArgs e)
        {
            ////手柄初始化
            //controller = new XInputController();
            //if (controller.connected)
            //{
            //    this.textBox1.Text = "the handle has connected...\r\n";
            //    this.button1.Enabled = true;
            //}
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (!downnum)
            {
                this.timer1.Start();
                downnum = true;
            }
            else
            {
                this.timer1.Stop();
                downnum = false;
            }

        }

    }
    }




     
    


using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;


namespace SocketTcpServer
{
    class information
    {
        public string Mass { get; set; }
        public string Auto_num { get; set; }
        public bool netto_brutto { get; set; } //Поздно пить боржоми когда легкие отпали!  итс трю
    }
    class didScreen
    {

        public static information iWantInf = new information();
        public static Bitmap getScreen()
        {
            Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(printscreen as Image);
            graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);

            Bitmap res = new Bitmap(printscreen.Width, printscreen.Height + 200);
            Graphics g = Graphics.FromImage(res);
            g.Clear(Color.Black);
            g.DrawImage(printscreen, 0, 0);

            string mass_label;

            if (iWantInf.netto_brutto == true)
            {
                mass_label = "Масса НЕТТО:";
            }
            else
            {
                mass_label = "Масса БРУТТО:";
            }
            string number_label = "Номер:";
            string number = iWantInf.Auto_num;
            string mass = iWantInf.Mass;
            string date_label = "Дата и время:";
            DateTime date = DateTime.Now;


            Font red = new Font("Arial", 15, FontStyle.Bold);
            Font white = new Font("Arial", 40, FontStyle.Bold);


            g.DrawString(mass_label, red, new SolidBrush(Color.Red),
                new RectangleF(printscreen.Width / 6 - g.MeasureString(mass_label, red).Width / 2, printscreen.Height, 0, 0));
            g.DrawString(mass, white, new SolidBrush(Color.White),
                new RectangleF(printscreen.Width / 6 - g.MeasureString(mass, white).Width / 2,
                               printscreen.Height + g.MeasureString(mass_label, red).Height + 10, 0, 0));

            g.DrawString(number_label, red, new SolidBrush(Color.Red),
                new RectangleF(printscreen.Width / 2 - g.MeasureString(number_label, red).Width / 2, printscreen.Height, 0, 0));
            g.DrawString(number, white, new SolidBrush(Color.White),
                new RectangleF(printscreen.Width / 2 - g.MeasureString(number, white).Width / 2, 
                               printscreen.Height + g.MeasureString(number_label, red).Height + 10, 0, 0));

            g.DrawString(date_label, red, new SolidBrush(Color.Red),
               new RectangleF(printscreen.Width / 6 * 5 - g.MeasureString(date_label, red).Width / 2, printscreen.Height, 0, 0));
            g.DrawString(date.ToShortDateString(), white, new SolidBrush(Color.White),
                new RectangleF(printscreen.Width / 6 * 5 - g.MeasureString(date.ToShortDateString(), white).Width / 2, 
                               printscreen.Height + g.MeasureString(date_label, red).Height + 10, 0, 0));
            g.DrawString(date.ToShortTimeString(), white, new SolidBrush(Color.White),
                new RectangleF(printscreen.Width / 6 * 5 - g.MeasureString(date.ToShortTimeString(), white).Width / 2,
                               printscreen.Height + g.MeasureString(date_label, red).Height + 20 + g.MeasureString(date.ToShortDateString(), white).Height, 0, 0));

            string path = "C:\\Users\\trape\\OneDrive\\Рабочий стол\\ScreenShoots\\" + Convert.ToString(date).Replace(':', '_') + ".jpg";
            Console.WriteLine(date.ToShortTimeString() + ": скриншот сохранен как - " + path);
            res.Save(path);

            return res;
        }
    }


    class Program
    {
        /*private static int SendVarData(Socket s, byte[] data)
        {
            int total = 0;
            int size = data.Length;
            int dataleft = size;
            int sent;
            byte[] datasize = new byte[4];
            datasize = BitConverter.GetBytes(size);
            sent = s.Send(datasize);
            while (total < size)
            {
                sent = s.Send(data, total, dataleft, SocketFlags.None);
                total += sent;
                dataleft -= sent;
            }
            return total;
        }*/

        static int port = 8005; // порт для приема входящих запросов
        static void Main(string[] args)
        {
            
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();

                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[1024]; //буфер для получаемых данны

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);

                    string jSon = builder.ToString();
                    didScreen.iWantInf = JsonConvert.DeserializeObject<information>(jSon);
                    didScreen.getScreen();
                    
                        // отправляем ответ
                    string message = "Скриншот успешно сделан";
                    data = Encoding.Unicode.GetBytes(message);
                    handler.Send(data);
                    // закрываем сокет
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

//Bitmap bmpScreenshot = new Bitmap(didScreen.getScreen());

//MemoryStream ms = new MemoryStream();

//bmpScreenshot.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

//byte[] bmpBytes = ms.GetBuffer();

//ms.Close();

//bytes = SendVarData(handler, bmpBytes);

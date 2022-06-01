using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.Threading;

namespace classForRandomMass
{
    class RandomMass
    {
        public static double randomNum()
        {
            Random rnd = new Random();
            double mass = rnd.Next(0, 99) + Math.Round(rnd.NextDouble(), 3);

            return mass;
        }
    }

    public class information
    {
        static int port = 8005;
        //Thread sendMessage = new Thread(new ThreadStart());
        public string serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
        public string Mass { get; set; }
        public string Auto_num { get; set; }
        public bool netto_brutto { get; set; } //Поздно пить боржоми когда легкие отпали!  итс трю


        
    }
    public class Client
    {
        static int port = 8005;
        static string ip = "127.0.0.1";

        //information inf = JsonConvert.DeserializeObject<information>(message);


        public void seeMessages(string message)
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // подключаемся к удаленному хосту
                socket.Connect(ipPoint);
                byte[] data = Encoding.Unicode.GetBytes(message);
                socket.Send(data);

                // получаем ответ
                data = new byte[256]; // буфер для ответа
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байт

                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);
                Console.WriteLine();
                MessageBox.Show("Ответ сервера: " + builder.ToString(), "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // закрываем сокет
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка запроса!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    /*class serverMessage
    {
        public static void ConnectSocket(string message, int port)
        {

            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            Socket asSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            asSocket.Bind(ipPoint);

            asSocket.Listen(10);
            while (true)
            {
                try
                {
                    Socket handler = asSocket.Accept();
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    byte[] data = new byte[256];

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);
                    data = Encoding.Unicode.GetBytes(message);
                    handler.Send(data);
                    MessageBox.Show("Сообщение доставленно успешно!", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка отправления сообщения!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }*/
}

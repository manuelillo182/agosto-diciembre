using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            bool band = true;
            TcpListener tcpListener = new TcpListener(System.Net.IPAddress.Any, 9898);
            Checador checador;
           
            while (band)
            {
                //Iniciamos la esucha
                tcpListener.Start();
                Console.WriteLine("Esto es una prueba y queda esperando en socket");
                Socket socketForClient = tcpListener.AcceptSocket();
           
                //Console.WriteLine("Esto es una prueba");
                if (socketForClient.Connected)
                {
                  

                    Console.WriteLine("se conecto");
                        NetworkStream networkStream = new NetworkStream(socketForClient);
                        StreamWriter streamWriter = new StreamWriter(networkStream);
                        StreamReader streamReader = new StreamReader(networkStream);

                        string theString = "Se esta ejecutando el proceso..favor de esperar";
                        try
                        {
                      
                            //Escribimos la data en el stream
                            streamWriter.WriteLine(theString);
                            //Ahora le decimos que la mande.
                            streamWriter.Flush();

                        Console.WriteLine("intenta recibir mensaje");
                        theString = streamReader.ReadLine();
                        Console.WriteLine(theString);
                        Console.WriteLine("recibio el mensaje");

                        //empieza el proceso con el checador
                        checador = new Checador();
                        checador.Aparato = 1;
                        string[] direcc = { "10.12.4.10", "10.7.6.199", "10.12.102.20", "10.4.4.10", "10.7.4.10", "10.2.6.10", "10.4.6.6","10.4.7.27","10.2.6.11","10.6.6.253",
                                 "10.14.4.10", "10.12.16.15", "10.12.4.11"};
                        checador.Puerto = 4370;

                        for (int i = 0; i <= direcc.Length - 1; i++)

                        {
                            checador.cDireccion = direcc[i];
                            //realiza ping para obtener estado de dispositivo
                            Ping HacerPing = new Ping();
                            PingReply RespuestaPing;
                            RespuestaPing = HacerPing.Send(checador.cDireccion);
                            //
                            if (RespuestaPing.Status == IPStatus.Success)
                            {
                                try
                                {
                                    checador.Conectar();
                                    Console.WriteLine("\n" + "ip:" + checador.cDireccion + "-puerto:" + checador.Puerto);
                                    if (checador.Conectado)
                                    {
                                        checador.GenerarListaDeChecador();
                                        checador.muestra();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Error al conectar al checador, error=44", "Error");
                                    }
                                }

#pragma warning disable CS0168 // La variable está declarada pero nunca se usa
                                catch (Exception ex)
#pragma warning restore CS0168 // La variable está declarada pero nunca se usa
                                {
                                    MessageBox.Show("Error al conectar al checador, error=" + checador.Error().ToString(), "Error");
                                }
                            }
                            else
                            {
                                Console.WriteLine("\n El dispositivo " + checador.cDireccion + " No esta conectado");

                                //Escribimos la data en el stream
                                streamWriter.WriteLine("El dispositivo " + checador.cDireccion + " No esta conectado");
                                //Ahora le decimos que la mande.
                                streamWriter.Flush();
                            }
                        }
                        MessageBox.Show("proceso terminado");

                        //Escribimos la data en el stream
                        streamWriter.WriteLine("e");
                        //Ahora le decimos que la mande.
                        streamWriter.Flush();

                        theString = streamReader.ReadLine();
                            Console.WriteLine(theString);
                        }
                        finally
                        {
                            streamReader.Close();
                            streamWriter.Close();
                            networkStream.Close();
                            socketForClient.Close();
                        }
                    
                   
                    tcpListener.Stop();

                }
            }
        }
    }
}

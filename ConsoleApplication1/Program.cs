using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ConsoleApplication1
{
    class Program
    {
        static Checador checador = new Checador();
        static void Main(string[] args)
        {
            bool band = true;
            TcpListener tcpListener = new TcpListener(System.Net.IPAddress.Any, 9898);
            string cadena_io = "Se esta ejecutando el proceso..favor de esperar";

        INICIO:
            while (band)
            {
                //Iniciamos la esucha
                tcpListener.Start();
                Console.WriteLine("Esto es una prueba y queda esperando en socket");
                Socket socketForClient = tcpListener.AcceptSocket();
                if (socketForClient.Connected)
                {
                    NetworkStream networkStream = new NetworkStream(socketForClient);
                    StreamWriter streamWriter = new StreamWriter(networkStream);
                    StreamReader streamReader = new StreamReader(networkStream);
                    streamWriter.AutoFlush = true;
                    try
                    {
                        try
                        {
                            cadena_io = streamReader.ReadLine();
                            Console.WriteLine(cadena_io);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("error de comunicacion con cliente");
                            goto INICIO;
                        }
                        Char delimitador = ',';//caracter para delimitar
                        String[] direcciones = cadena_io.Split(delimitador);//
                        try
                        {
                            streamWriter.WriteLine(cadena_io);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("error de comunicacion con cliente");
                            goto INICIO;
                        }
                        checa(direcciones, streamWriter);
                        try
                        {
                            streamWriter.WriteLine("s");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("error de comunicacion con cliente");
                            goto INICIO;
                        }
                    }
                    finally
                    {
                        streamReader.Close();
                        streamWriter.Close();
                        networkStream.Close();
                        socketForClient.Close();
                        Console.WriteLine("Cierra Reader,Writer,networkStream, y socket");
                    }
                    tcpListener.Stop();
                    Console.WriteLine("detiene tcplistener");
                }
            }
        }

        public static void checa(string[] direcciones, StreamWriter streamWriter)
        {
            checador.Aparato = 1;
            checador.Puerto = 4370;
            foreach (var ip in direcciones)
            {

                checador.cDireccion = ip;
                //realiza ping para obtener estado de dispositivo
                Ping HacerPing = new Ping();
                PingReply RespuestaPing = null;
                try
                {
                    for (int i = 0; i < 3; i++)
                    {
                        RespuestaPing = HacerPing.Send(checador.cDireccion);
                        if (RespuestaPing.Status == IPStatus.Success)
                        {
                            i = 4;
                        }
                        else if (i == 2)
                        {
                            streamWriter.WriteLine("intento de ping invalido (3veces)" + checador.cDireccion);
                            //continue;
                        }
                    }

                }
                catch (Exception e)
                {
                    streamWriter.WriteLine("error desconocido en ping:" + ip);
                    continue;
                }
                if (RespuestaPing.Status != IPStatus.Success)
                {
                    continue;
                }
                // if (RespuestaPing.Status == IPStatus.Success)
                //{
                try
                {
                    streamWriter.WriteLine("Analizando:" + ip);
                    checador.Conectar();
                    Console.WriteLine("\n" + "ip:" + checador.cDireccion + "-puerto:" + checador.Puerto);
                    if (checador.Conectado)
                    {
                        checador.GenerarListaDeChecador();
                        checador.muestra();
                    }
                    else
                    {
                        streamWriter.WriteLine("Error al conectar al checador, error=44", "Error");
                    }
                }
                catch (Exception ex)
                {
                    streamWriter.WriteLine("Error al conectar al checador, error=" + checador.Error().ToString(), "Error");
                }
                /* }
                 else
                 {
                     streamWriter.WriteLine("El dispositivo " + checador.cDireccion + " No esta conectado");
                 }*/
            }
        }
    }
}

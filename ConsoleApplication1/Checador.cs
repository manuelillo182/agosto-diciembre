using System;
using System.Collections.Generic;
using System.Globalization;
using Oracle.ManagedDataAccess.Client;

namespace ConsoleApplication1
{
    class Checador
    {
        zkemkeeper.CZKEM axCZKEM1 = new zkemkeeper.CZKEM();
        bool bIsConnected = false;//the boolean value identifies whether the device is connected
        int iMachineNumber = 1;//the serial number of the device.After connecting the device ,this value will be changed.
        int iUsuario = -1;
        string iArea = "";
        int iPuerto = 0;
        List<Check> iListaChecks;
        string icDireccion = "";
        string iServidorOracle = "";
        string wsDir = "";

        public string WsDir
        {
            set { wsDir = value; }
            get { return wsDir; }
        }
        public bool Conectado
        {
            set { bIsConnected = value; }
            get { return bIsConnected; }
        }
        public int Aparato
        {
            set { iMachineNumber = value; }
            get { return iMachineNumber; }
        }
        public int Usuario
        {
            set { iUsuario = value; }
            get { return iUsuario; }
        }
        public string Area
        {
            set { iArea = value; }
            get { return iArea; }
        }
        public int Puerto
        {
            set { iPuerto = value; }
            get { return iPuerto; }
        }
        public string cDireccion
        {
            set { icDireccion = value; }
            get { return icDireccion; }
        }
        public string ServidorOracle
        {
            set { iServidorOracle = value; }
            get { return iServidorOracle; }
        }
        public Checador()
        {
            iListaChecks = new List<Check>();

        }


        public void Conectar()
        {
            bIsConnected = axCZKEM1.Connect_Net(icDireccion, iPuerto);
        }

        public void Habilitar()
        {
            Conectado = this.axCZKEM1.EnableDevice(this.Aparato, true);
        }
        public void Deshabilitar()
        {
            Conectado = this.axCZKEM1.EnableDevice(this.Aparato, false);
        }

        public int Error()
        {
            int idwErrorCode = 0;
            this.axCZKEM1.GetLastError(ref idwErrorCode);

            return idwErrorCode;
        }

        public void GenerarListaDeChecador()
        {
            if (this.Conectado)
            {
                this.Deshabilitar();
                if (this.axCZKEM1.ReadGeneralLogData(this.Aparato))//read all the attendance records to the memory
                {
                    string sdwEnrollNumber = "";
                    int idwVerifyMode = 0;
                    int idwInOutMode = 0;
                    int idwYear = 0;
                    int idwMonth = 0;
                    int idwDay = 0;
                    int idwHour = 0;
                    int idwMinute = 0;
                    int idwSecond = 0;
                    int idwWorkcode = 0;
                    String fecha_chk = DateTime.Now.Day.ToString() + '-' + DateTime.Now.Month.ToString() + '-' + DateTime.Now.Year.ToString();
                    String fec_chk;
                    iListaChecks.Clear();
                    while (this.axCZKEM1.SSR_GetGeneralLogData(this.Aparato, out sdwEnrollNumber, out idwVerifyMode,
                               out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                    {
                        fec_chk = idwDay.ToString() + "-" + idwMonth.ToString() + "-" + idwYear.ToString();

                        if (fecha_chk == fec_chk)
                        {

                            if (int.Parse(sdwEnrollNumber) < 100000)
                            {
                                iListaChecks.Add(new Check(int.Parse(sdwEnrollNumber), DateTime.ParseExact(idwDay.ToString() + "-" + idwMonth.ToString() + "-" + idwYear.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString(), "d-M-yyyy H:m:s", CultureInfo.InvariantCulture)));
                            }
                        }
                    }
                }
                else
                {
                    Conectado = false;
                    System.Windows.Forms.MessageBox.Show("Error al obtener datos de checador, error=32", "Error");
                }
                this.Habilitar();
            }
            else
            {
                Conectado = false;
                System.Windows.Forms.MessageBox.Show("Error al obtener datos de checador, error=28", "Error");
            }
        }

        public void InsertarOracle()
        {

            string myOracleConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.12.1.20)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=nomina)));User Id=sirh;Password=sirh;";
            using (var oracleconn = new OracleConnection(myOracleConnectionString))
            {
                oracleconn.Open();
                using (var cmd_1 = oracleconn.CreateCommand())
                {
                    cmd_1.CommandText = "INSERT INTO paso_nom_checadores(NO_EMPLEADO, HORA_ENTRADA, HORA_SALIDA, TURNO, OBSERVACION, CREADO_POR, FECHA_CREACION, MODIFICADO_POR, FECHA_MODIFICACION, MODULO)" +
                                                         "VALUES(:noempleado, :horaentrada, NULL, 1, NULL, 457, :fechacreacion, 457, :fechacreacion, 'rec_hum')";
                    cmd_1.Parameters.Add("noempleado", OracleDbType.Int32);
                    cmd_1.Parameters.Add("horaentrada", OracleDbType.Date);
                    cmd_1.Parameters.Add("fechacreacion", OracleDbType.Date);
                    foreach (Check ss in iListaChecks)
                    {
                        cmd_1.Parameters["noempleado"].Value = ss.eID;
                        cmd_1.Parameters["horaentrada"].Value = ss.eHora;
                        cmd_1.Parameters["fechacreacion"].Value = DateTime.Now;
                        cmd_1.ExecuteNonQuery();
                    }
                }
                oracleconn.Close();
            }
        }

        public void muestra()
        {
            foreach (Check ss in iListaChecks)
            {
                Console.Write("eID:" + ss.eID);
                Console.Write(" eArea:" + ss.eArea);
                Console.Write(" eTipo:" + ss.eTipo);
                Console.Write(" eHora:" + ss.eHora);
                Console.Write(" eFecha Interna:" + ss.eFinterna);
                Console.Write(" eEnviado:" + ss.eEnviado);
                Console.Write(" eintID:" + ss.eintID);
                Console.Write(" eFechaPrev:" + ss.eFPRev + "\n");
            }
        }
    }//endclass
}

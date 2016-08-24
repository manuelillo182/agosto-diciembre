using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Check
    {
        private int? ID;
        private byte? Tipo = 0;
        private int intID;
        private DateTime? Hora;
        private DateTime? FPrev;
        private DateTime? FInterna;
        private int? Area;
        public int? eArea
        {
            get { return Area; }
            set { Area = value; }
        }
        public int eintID
        {
            get { return intID; }
            set { intID = value; }
        }
        private byte? Enviado;
        public byte? eEnviado
        {
            get { return Enviado; }
            set { Enviado = value; }
        }

        public int? eID
        {
            get { return ID; }
            set { ID = value; }
        }
        public byte? eTipo
        {
            get { return Tipo; }
            set { Tipo = value; }
        }

        public DateTime? eHora
        {
            get { return Hora; }
            set { Hora = value; }
        }
        public DateTime? eFPRev
        {
            get { return FPrev; }
            set { FPrev = value; }
        }
        public DateTime? eFinterna
        {
            get { return FInterna; }
            set { FInterna = value; }
        }
        public Check()
        {
        }
        public Check(int? tID, DateTime? thora)
        {
            ID = tID;
            Hora = thora;
            Tipo = 0;
        }
        public Check(int? tID, DateTime? thora, byte? tipo)
        {
            ID = tID;
            Hora = thora;
            Tipo = tipo;
        }
        public Check(int? tID, DateTime? thora, byte? tipo, DateTime? tprev)
        {
            ID = tID;
            Hora = thora;
            Tipo = tipo;
            FPrev = tprev;
        }
    }
}

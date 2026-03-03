using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libParcial1
{
    public abstract class clsParcial1
    {
        #region Atributos

        protected int intNroCta;
        protected string strFecCreac;
        protected int intTipoDoc;
        protected int intNroDcto;
        protected string strTitular;
        protected float fltSaldo;
        protected string rutaFile;
        protected string strError;

        #endregion

        #region Propiedades
        public int NroCta
        {
            get { return intNroCta; }
        }

        public string FecCreac
        {
            get { return strFecCreac; }
        }

        public string Error
        {
            get { return strError; }
        }

        public int TipoDoc
        {
            get { return intTipoDoc; }
            set { intTipoDoc = value; }
        }

        public int NroDcto
        {
            get { return intNroDcto; }
            set { intNroDcto = value; }
        }

        public string Titular
        {
            get { return strTitular; }
            set { strTitular = value; }
        }

        public float Saldo
        {
            get { return fltSaldo; }
            set { fltSaldo = value; }
        }
        #endregion

        #region Metodos
        public abstract bool Crear();
        public abstract bool Buscar(int nroCta);
        #endregion
    }

    public abstract class clsTipo : clsParcial1
    {
        public abstract bool Deposito(int nroCta, float valor);
        public abstract bool Retiro(int nroCta, float valor);
    }

    public class clsGeneral
    {
        public int UltimoId(string ruta)
        {
            int rpta = -1;
            string carpeta = Path.GetDirectoryName(ruta);
            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);
            if (File.Exists(ruta))
            {
                // 1. Leer todas las lineas del archivo
                string[] lineas = File.ReadAllLines(ruta);

                // 2. Verificar si hay lineas en el archivo
                if (lineas.Length > 0)
                {
                    // 3. Obtener el último registro
                    string ultimoRegistro = lineas.LastOrDefault();
                    string[] datos = ultimoRegistro.Split(':');
                    rpta = Convert.ToInt32(datos[0]);
                }
                else
                    rpta = 0;
            }
            return rpta;
        }

        public List<string> leerArchivo(string ruta, int nroCta)
        {
            List<string> rpta = new List<string>();
            try
            {
                string carpeta = Path.GetDirectoryName(ruta);
                // Verificamos si la carpeta existe, si no, se crea.
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);
                // Verificamos si el archivo existe, si no, se crea.
                if (!File.Exists(ruta))
                    return rpta;
                string[] lineas = File.ReadAllLines(ruta);
                if (lineas.Length > 0)
                {
                    foreach (string rgtro in lineas)
                    {
                        string[] datos = rgtro.Split(':');
                        if (int.Parse(datos[0]) == nroCta)
                        {
                            rpta.AddRange(datos);
                            break;
                        }
                    }
                }
                return rpta;
            }
            catch
            {
                return rpta;
            }
        }
    }

    public class clsAhorro : clsTipo
    {
        #region Atributos
        private int intTipoAhor;
        private float fltporcInt;
        #endregion

        #region Constructores
        public clsAhorro()
        {
            intNroCta = 0;
            strFecCreac = string.Empty;
            intTipoDoc = 0;
            intNroDcto = 0;
            intTipoAhor = 0;
            fltporcInt = 0;
            strTitular = string.Empty;
            fltSaldo = 0;
            rutaFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos", "ctaAhorros.txt");

        }
        public clsAhorro(int tipoDoc, int nroDoc, string titular, float valor, int tipoAhor, float porcint)
        {
            intTipoDoc = tipoDoc;
            intNroDcto = nroDoc;
            strTitular = titular;
            fltSaldo = valor;
            intTipoAhor = tipoAhor;
            fltporcInt = porcint;
            rutaFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos", "ctaAhorros.txt");
        }
        #endregion

        #region propiedades
        public int TipoAhor
        {
            get { return intTipoAhor; }
            set { intTipoAhor = value; }
        }

        public float PorcIntAhor
        {
            get { return fltporcInt; }
            set { fltporcInt = value; }
        }

        #endregion

        #region Metodos publicos
        public override bool Buscar(int nroCta)
        {
            try
            {
                if (nroCta <= 0)
                {
                    strError = "Número de cuenta nó válido";
                    return false;
                }
                clsGeneral og = new clsGeneral();
                List<string> datos = og.leerArchivo(rutaFile, nroCta);
                if (datos.Count <= 0)
                {
                    strError = "No se encontró la cuenta de ahorros #" + nroCta;
                    og = null;
                    return false;
                }
                strFecCreac = datos[1];
                intTipoDoc = int.Parse(datos[2]);
                intNroDcto = int.Parse(datos[3]);
                strTitular = datos[4];
                fltSaldo = float.Parse(datos[5]);
                intTipoAhor = int.Parse(datos[6]);
                fltporcInt = float.Parse(datos[7]);

                return true;
            }
            catch
            {

                strError = "Error en consultar la cuenta de ahorros";
                return false;
            }
        }
        public override bool Crear()
        {
            if (!Validar())
                return false;
            try
            {
                clsGeneral og = new clsGeneral();
                intNroCta = og.UltimoId(rutaFile) + 1;
                if (intNroCta <= 0)
                {
                    strError = "Error en consultar cuenta de ahorros";
                    return false;
                }
                strFecCreac = DateTime.Now.ToShortDateString();
                string rgtro = intNroCta + ":" + strFecCreac + ":" + intTipoDoc + ":" + intNroDcto + ":" +
                    strTitular + ":" + fltSaldo.ToString() + ":" + intTipoAhor + ":" + fltporcInt;

                StreamWriter grabar = new StreamWriter(rutaFile, true);
                grabar.WriteLine(rgtro);
                grabar.Close();
                return true;
            }
            catch (Exception)
            {

                strError = "Error en grabar";
                return false;
            }
        }
        public override bool Deposito(int nroCta, float valor)
        {
            bool encontrado = false;
            float saldoAct, newSaldo;

            try
            {
                List<string> lineas = File.ReadAllLines(rutaFile).ToList();
                for (int i = 0; i < lineas.Count(); i++)
                {
                    string[] campos = lineas[i].Split(':');
                    if (campos.Length > 0 && campos[0].Equals(nroCta.ToString()))
                    {
                        saldoAct = Convert.ToSingle(campos[5]);
                        newSaldo = saldoAct + valor;
                        string newRgro = campos[0] + ":" + campos[1] + ":" + campos[2] + ":" + campos[3] + ":" +
                            campos[4] + ":" + newSaldo + ":" + campos[6] + ":" + campos[7];
                        lineas[i] = newRgro;
                        File.WriteAllLines(rutaFile, lineas);
                        encontrado = true;
                        break;
                    }
                }
                if (!encontrado)
                {
                    strError = "No se emcontró la cuenta #" + nroCta;
                }
                return encontrado;
            }
            catch (Exception err)
            {

                strError = err.Message;
                return false;
            }
        }
        public override bool Retiro(int nroCta, float valor)
        {
            bool encontrado = false;
            float saldoAct, newSaldo;

            try
            {
                List<string> lineas = File.ReadAllLines(rutaFile).ToList();
                for (int i = 0; i < lineas.Count(); i++)
                {
                    string[] campos = lineas[i].Split(':');
                    if (campos.Length > 0 && campos[0].Equals(nroCta.ToString()))
                    {
                        saldoAct = Convert.ToSingle(campos[5]);
                        if (saldoAct >= valor)
                        {
                            newSaldo = saldoAct - valor;
                            string newRgro = campos[0] + ":" + campos[1] + ":" + campos[2] + ":" + campos[3] + ":" +
                                campos[4] + ":" + newSaldo + ":" + campos[6] + ":" + campos[7];
                            lineas[i] = newRgro;
                            File.WriteAllLines(rutaFile, lineas);
                            encontrado = true;
                            break;
                        }
                        else
                            strError = "Saldo insuficiente en la cuenta #" + nroCta;
                    }
                }
                if (!encontrado)
                {
                    strError = "No se emcontró la cuenta #" + nroCta;
                }
                return encontrado;
            }
            catch (Exception err)
            {

                strError = err.Message;
                return false;
            }
        }
        #endregion

        #region Metodos privados
        private bool Validar()
        {
            if (intTipoDoc <= 0)
            {
                strError = "El tipo de documento es incorrecto";
                return false;
            }
            if (intNroDcto <= 0)
            {
                strError = "El número de documento es incorrecto";
                return false;
            }
            if (strTitular.Trim() == "")
            {
                strError = "El titular es incorrecto";
                return false;
            }
            if (fltSaldo <= 0)
            {
                strError = "El saldo es incorrecto";
                return false;
            }
            if (intTipoAhor <= 0)
            {
                strError = "El tipo de ahorro es incorrecto";
                return false;
            }
            if (fltporcInt <= 0 || fltporcInt > 50)
            {
                strError = "El porcentaje de interes no pude ser menor a 0 ni superior a 50";
                return false;
            }

            return true;
        }
        #endregion

    }

    public class clsCorriente : clsTipo
    {
        #region Atributos
        private float fltLimSobreGiro;
        private string strRepresLeg;
        #endregion

        #region Constructores
        public clsCorriente()
        {
            intNroCta = 0;
            strFecCreac = string.Empty;
            intTipoDoc = 0;
            intNroDcto = 0;
            fltLimSobreGiro = 0;
            strRepresLeg = string.Empty;
            strTitular = string.Empty;
            fltSaldo = 0;
            rutaFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos", "ctaCorriente.txt");
        }
        public clsCorriente(int tipoDoc, int nroDoc, string titular, float valor, float LimSobreGiro, string repLeg)
        {
            intTipoDoc = tipoDoc;
            intNroDcto = nroDoc;
            strTitular = titular;
            fltSaldo = valor;
            fltLimSobreGiro = LimSobreGiro;
            strRepresLeg = repLeg;
            rutaFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos", "ctaCorriente.txt");
        }
        #endregion

        #region Propiedades
        public float LimSobreGiro
        {
            get { return fltLimSobreGiro; }
            set { fltLimSobreGiro = value; }
        }
        public string Represent
        {
            get { return strRepresLeg; }
            set { strRepresLeg = value; }
        }
        #endregion

        #region Metodos publicos
        public override bool Buscar(int nroCta)
        {
            try
            {
                if (nroCta <= 0)
                {
                    strError = "Número de cuenta nó válido";
                    return false;
                }
                clsGeneral og = new clsGeneral();
                List<string> datos = og.leerArchivo(rutaFile, nroCta);
                if (datos.Count <= 0)
                {
                    strError = "No se encontró la cuenta corriente #" + nroCta;
                    og = null;
                    return false;
                }
                strFecCreac = datos[1];
                intTipoDoc = int.Parse(datos[2]);
                intNroDcto = int.Parse(datos[3]);
                strTitular = datos[4];
                fltSaldo = float.Parse(datos[5]);
                fltLimSobreGiro = float.Parse(datos[6]);
                strRepresLeg = datos[7];

                return true;
            }
            catch
            {

                strError = "Error en consultar la cuenta corriente";
                return false;
            }
        }
        public override bool Crear()
        {
            if (!Validar())
                return false;
            try
            {
                clsGeneral og = new clsGeneral();
                intNroCta = og.UltimoId(rutaFile) + 1;
                if (intNroCta <= 0)
                {
                    strError = "Error en consultar cuenta corriente";
                    return false;
                }
                strFecCreac = DateTime.Now.ToShortDateString();
                string rgtro = intNroCta + ":" + strFecCreac + ":" + intTipoDoc + ":" + intNroDcto + ":" +
                    strTitular + ":" + fltSaldo.ToString() + ":" + fltLimSobreGiro + ":" + strRepresLeg;

                StreamWriter grabar = new StreamWriter(rutaFile, true);
                grabar.WriteLine(rgtro);
                grabar.Close();
                return true;
            }
            catch (Exception)
            {

                strError = "Error en grabar";
                return false;
            }
        }
        public override bool Deposito(int nroCta, float valor)
        {
            bool encontrado = false;
            float saldoAct, newSaldo;

            try
            {
                List<string> lineas = File.ReadAllLines(rutaFile).ToList();
                for (int i = 0; i < lineas.Count(); i++)
                {
                    string[] campos = lineas[i].Split(':');
                    if (campos.Length > 0 && campos[0].Equals(nroCta.ToString()))
                    {
                        saldoAct = Convert.ToSingle(campos[5]);
                        newSaldo = saldoAct + valor;
                        string newRgro = campos[0] + ":" + campos[1] + ":" + campos[2] + ":" + campos[3] + ":" +
                            campos[4] + ":" + newSaldo + ":" + campos[6] + ":" + campos[7];
                        lineas[i] = newRgro;
                        File.WriteAllLines(rutaFile, lineas);
                        encontrado = true;
                        break;
                    }
                }
                if (!encontrado)
                {
                    strError = "No se emcontró la cuenta #" + nroCta;
                }
                return encontrado;
            }
            catch (Exception err)
            {

                strError = err.Message;
                return false;
            }
        }
        public override bool Retiro(int nroCta, float valor)
        {
            bool encontrado = false;
            float saldoAct, newSaldo;

            try
            {
                List<string> lineas = File.ReadAllLines(rutaFile).ToList();
                for (int i = 0; i < lineas.Count(); i++)
                {
                    string[] campos = lineas[i].Split(':');
                    if (campos.Length > 0 && campos[0].Equals(nroCta.ToString()))
                    {
                        saldoAct = Convert.ToSingle(campos[5]);
                        if (saldoAct >= valor)
                        {
                            newSaldo = saldoAct - valor;
                            string newRgro = campos[0] + ":" + campos[1] + ":" + campos[2] + ":" + campos[3] + ":" +
                                campos[4] + ":" + newSaldo + ":" + campos[6] + ":" + campos[7];
                            lineas[i] = newRgro;
                            File.WriteAllLines(rutaFile, lineas);
                            encontrado = true;
                            break;
                        }
                        else
                            strError = "Saldo insuficiente en la cuenta #" + nroCta;
                    }
                }
                if (!encontrado)
                {
                    strError = "No se emcontró la cuenta #" + nroCta;
                }
                return encontrado;
            }
            catch (Exception err)
            {

                strError = err.Message;
                return false;
            }
        }
        #endregion

        #region Metodos privados
        private bool Validar()
        {
            if (intTipoDoc <= 0)
            {
                strError = "El tipo de documento no es valido";
                return false;
            }

            if (intNroDcto <= 0)
            {
                strError = "El numero de cuenta no es valido";
                return false;
            }
            if (strTitular.Trim() == " ")
            {
                strError = "El titular es incorrecto";
                return false;
            }
            if (fltSaldo < -fltLimSobreGiro)
            {
                strError = "El saldo supera el limite de sobregiros ";
                return false;
            }
            if (fltLimSobreGiro <= 0)
            {
                strError = "El limite de sobregiro no es valido";
                return false;
            }
            if (strRepresLeg.Trim() == " ")
            {
                strError = "El representante legal no es valido";
                return false;
            }

            return true;
        }
        #endregion
    }
}

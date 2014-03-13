using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AF0097_TabVentasMensualMuebles
{
    class ConexionSQL
    {
        public string connectionString, IP, DB, USER, PASS;
        private int CONT;

        public string LeeArchivo(string archivo)
        {
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(archivo))
                {
                    String linea;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    CONT = 0;
                    while ((linea = sr.ReadLine()) != null)
                    {
                        CONT += 1;
                        switch (CONT)
                        {
                            case 1:
                                IP = linea;
                                break;
                            case 2:
                                DB = linea;
                                break;
                            case 3:
                                USER = linea;
                                break;
                            case 4:
                                PASS = linea;
                                break;
                        }
                    }
                }
                connectionString = "Data Source=" + IP + ";Initial Catalog=" + DB + ";User ID=" + USER + ";Password=" + PASS;
                return connectionString;
            }
            catch (Exception Ex)
            {
                System.Windows.Forms.MessageBox.Show(Ex.Message, "ERROR", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return connectionString;
            }
        }
    }
}

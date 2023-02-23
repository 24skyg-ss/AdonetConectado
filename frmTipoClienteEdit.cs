using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace AdoNetConectado
{
    public partial class frmTipoClienteEdit : Form
    {
        int ID;
        string cadenaConexion = "server=DESKTOP-29K2QJ8\\MYSQLSERVER; DataBase=BancoBD; Integrated Security = true";
        public frmTipoClienteEdit(int id = 0)
        {
            InitializeComponent();
            this.ID = id;
        }

        private void aceptarCambios(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtNombre.Text))
            {
                MessageBox.Show("Debe ingresar un nombre válido", "Sistemas", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
        }

        private void cargarDatos(object sender, EventArgs e)
        {
            if(this.ID > 0)
            {
                this.Text = "Editar";
                mostrarDatos();
            }
        }

        void mostrarDatos()
        {
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqlCommand("SELECT * FROM TipoCliente WHERE ID = @id", conexion))
                {
                    comando.Parameters.AddWithValue("@id", this.ID);
                    using (var reader = comando.ExecuteReader())
                    {
                        if(reader != null && reader.HasRows)
                        {
                            reader.Read();
                            txtNombre.Text = reader[1].ToString();
                            txtDescripcion.Text = reader[2].ToString();
                            chkEstado.Checked = reader[3].ToString() == "1" ? true: false ;
                        }
                    }
                }
            }
        }
    }
}

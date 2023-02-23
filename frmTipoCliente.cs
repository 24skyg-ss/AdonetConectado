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
    /* 
     * Curso: Desarrollo de soluciones empresariales | 3F3 PRC | 3117-1626 |
     * Autor: Sebastian Nicolas Gamarra Doroteo
     * 
     * Actividad: 
     *      - Agregar un método para el botón Eliminar. 
     *              ( Hecho ) - Línea 151
     *      - Recargar los datos cuando se actualice la base de datos. 
     *              ( Hecho ) - Línea 96, Línea 139, Línea 177
     *      
     * Nota: 
     *      El nombre de los campos de la tabla TipoCliente son:
     *      - ID
     *      - Nombre
     *      - Descripcion
     *      - Estado
     *      Estos nombres corresponden a la tabla TipoCliente de una BD de prueba.
     *      
     *      En caso alguno de estos nombre no coincida con la base de datos local
     *      cambiar el nombre de estos campos al de los campos de la misma tabla 
     *      contenida en la base de datos local, a fin de prevenir errores en la 
     *      ejecución de los métodos.
     */

    public partial class frmTipoCliente : Form
    {
        string cadenaConexion = @"server=DESKTOP-29K2QJ8\MYSQLSERVER; DataBase=BancoBD; Integrated Security=true";
        public frmTipoCliente()
        {
            InitializeComponent();
        }

        private void cargarFormulario(object sender, EventArgs e)
        {
            cargarDatos();
        }


        private void cargarDatos()
        {
            // USING DELIMITA EL CICLO DE VIDA DE UNA VARIABLE
            using(var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using(var comando = new SqlCommand("SELECT * FROM TipoCliente", conexion))
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        if(lector != null && lector.HasRows)
                        {
                            while (lector.Read())
                            {
                                dgvDatos.Rows.Add(lector[0], lector[1], lector[2], lector[3]);
                            }
                        }
                    }
                }
            }
        }

        private void nuevoRegistro(object sender, EventArgs e)
        {
            frmTipoClienteEdit frm = new frmTipoClienteEdit();
            if(frm.ShowDialog() == DialogResult.OK)
            {
                string nombre = ((TextBox)frm.Controls["txtNombre"]).Text;
                string descripcion = ((TextBox) frm.Controls["txtDescripcion"]).Text;
                // OPERADOR TERNARIO
                var estado = ((CheckBox) frm.Controls["chkEstado"]).Checked == true ? 1: 0;

                using (var conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();
                    using(var comando = new SqlCommand("INSERT INTO TipoCliente (Nombre, Descripcion, Estado) " +
                        "VALUES (@nombre, @descripcion, @estado)", conexion))
                    {
                        comando.Parameters.AddWithValue("@nombre", nombre);
                        comando.Parameters.AddWithValue("@descripcion", descripcion);
                        comando.Parameters.AddWithValue("@estado", estado);
                        int resultado = comando.ExecuteNonQuery();
                        if(resultado > 0)
                        {
                            MessageBox.Show("Datos registrados.", "Sistemas", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            dgvDatos.Rows.Clear();
                            cargarDatos();
                        } else
                        {
                            MessageBox.Show("No se ha podido registrar los datos.", "Sistemas",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void editarRegistro(object sender, EventArgs e)
        {
            // VALIDAMOS QUE EXISTAN FILAS PARA EDITAR
            if (dgvDatos.RowCount > 0 && dgvDatos.CurrentRow != null)
            {
                // TOMAMOS EL ID DE LA FILA SELECCIONADA
                int idTipo = int.Parse(dgvDatos.CurrentRow.Cells[0].Value.ToString());
                var frm = new frmTipoClienteEdit(idTipo);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    string nombre = ((TextBox)frm.Controls["txtNombre"]).Text;
                    string descripcion = ((TextBox)frm.Controls["txtDescripcion"]).Text;
                    // OPERADOR TERNARIO
                    var estado = ((CheckBox)frm.Controls["chkEstado"]).Checked == true ? 1 : 0;

                    using (var conexion = new SqlConnection(cadenaConexion))
                    {
                        conexion.Open();
                        using (var comando = new SqlCommand("UPDATE TipoCliente SET Nombre = @nombre, " +
                            "Descripcion = @descripcion, Estado = @estado WHERE ID = @id", conexion))
                        {
                            comando.Parameters.AddWithValue("@nombre", nombre);
                            comando.Parameters.AddWithValue("@descripcion", descripcion);
                            comando.Parameters.AddWithValue("@estado", estado);
                            comando.Parameters.AddWithValue("@id", idTipo);
                            int resultado = comando.ExecuteNonQuery();
                            if(resultado > 0)
                            {
                                MessageBox.Show("Datos actualizados.", "Sistemas", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                dgvDatos.Rows.Clear();
                                cargarDatos();
                            } else
                            {
                                MessageBox.Show("No se ha podido actualizar los datos.", "Sistemas",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }            
        }

        private void eliminarRegistro(object sender, EventArgs e)
        {
            // VALIDAMOS QUE EXISTAN FILAS PARA EDITAR
            if (dgvDatos.RowCount > 0 && dgvDatos.CurrentRow != null)
            {
                // TOMAMOS EL ID DE LA FILA SELECCIONADA
                int idTipo = int.Parse(dgvDatos.CurrentRow.Cells[0].Value.ToString());

                // TOMAMOS EL NOMBRE DEL TIPO DE CLIENTE SELECCIONADO
                string nombre = dgvDatos.CurrentRow.Cells[1].Value.ToString();

                DialogResult frm = MessageBox.Show($"¿Desea eliminar la fila con la ID {idTipo} : {nombre}?", "Sistemas", 
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (frm == DialogResult.OK)
                {
                    using (var conexion = new SqlConnection(cadenaConexion))
                    {
                        conexion.Open();
                        using (var comando = new SqlCommand("DELETE FROM TipoCliente WHERE ID = @id", conexion))
                        {
                            comando.Parameters.AddWithValue("@id", idTipo);
                            int resultado = comando.ExecuteNonQuery();
                            if (resultado > 0)
                            {
                                MessageBox.Show("Datos actualizados.", "Sistemas",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                dgvDatos.Rows.Clear();
                                cargarDatos();
                            }
                            else
                            {
                                MessageBox.Show("No se ha podido eliminar el registro.", "Sistemas",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }

            }
        }
    }
}

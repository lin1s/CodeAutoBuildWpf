using CodeAutoBuildWpf.Emun;
using CodeAutoBuildWpf.Helper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodeAutoBuildWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetComboData();
        }

        private void SetComboData()
        {
            var ServerType = Tools.GetEmunList(typeof(SqlType));
            this.comboServerType.ItemsSource = ServerType;
            this.comboServerType.SelectedValuePath = nameof(ComboData.Key);
            this.comboServerType.DisplayMemberPath = nameof(ComboData.Value);
            this.comboServerType.SelectedIndex = 0;
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlType serverType = (SqlType)Convert.ToInt32(this.comboServerType.SelectedValue);
                string sqlConnection = txtConnectSql.Text;

                switch (serverType)
                {
                    case SqlType.SqlServer:
                        {
                            SqlConnection connection = new SqlConnection(sqlConnection);
                            SqlCommand cmd = new SqlCommand("SELECT NAME FROM SYS.TABLES", connection);
                            connection.Open();
                            SqlDataReader dr = cmd.ExecuteReader();
                            List<ComboData> tableList = new List<ComboData>();
                            while (dr.Read())
                            {
                                tableList.Add(new ComboData()
                                {
                                    Key = dr[0].ToString(),
                                    Value = dr[0].ToString()
                                });
                            }
                            connection.Close();

                            this.comboServerTable.ItemsSource = tableList;
                            this.comboServerTable.SelectedValuePath = nameof(ComboData.Key);
                            this.comboServerTable.DisplayMemberPath = nameof(ComboData.Value);
                            this.comboServerTable.SelectedIndex = 0;
                            MessageBox.Show("连接成功", "成功", MessageBoxButton.OK);
                        }
                        break;
                    default:
                        MessageBox.Show("尚未开放当前数据库的代码生成功能", "错误", MessageBoxButton.OK);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库连接字符串错误，请确认后再连接", "错误", MessageBoxButton.OK);
            }

        }

        private void comboServerTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SqlType serverType = (SqlType)Convert.ToInt32(this.comboServerType.SelectedValue);
                ComboData tableName = (ComboData)this.comboServerTable.SelectedItem;
                string sqlConnection = txtConnectSql.Text;
                if (tableName == null) return;

                switch (serverType)
                {
                    case SqlType.SqlServer:
                        {
                            SqlConnection connection = new SqlConnection(sqlConnection);
                            SqlCommand cmd = new SqlCommand("SELECT COLUMN_NAME,DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName", connection);
                            SqlParameter parameter = new SqlParameter("@TableName", tableName.Key);
                            cmd.Parameters.Add(parameter);
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dataTable = new DataTable();
                            dataTable.Columns.Add("checked", typeof(bool));
                            dataTable.Columns["checked"].DefaultValue = false;
                            connection.Open();
                            adapter.Fill(dataTable);
                            connection.Close();
                            gridColumn.ItemsSource = dataTable.AsDataView();
                            //gridColumn.Columns["checked"].HeaderText = "选择";
                            //gridColumn.Columns["COLUMN_NAME"].HeaderText = "列名";
                            //gridColumn.Columns["DATA_TYPE"].HeaderText = "列属性";

                            //gridColumn.Columns["COLUMN_NAME"].ReadOnly = true;
                            //gridColumn.Columns["DATA_TYPE"].ReadOnly = true;
                        }
                        break;
                    default:
                        MessageBox.Show("尚未开放当前数据库的代码生成功能", "错误", MessageBoxButton.OK);
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}

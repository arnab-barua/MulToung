using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Sql;
using System.ComponentModel;
using System.Windows.Markup;
using System.Text.RegularExpressions;

namespace MulToung
{
    /// <summary>
    /// Interaction logic for WordCatagory.xaml
    /// </summary>
    public partial class WordCatagory : Window
    {
        string conn = ConfigurationManager.ConnectionStrings["conn1"].ConnectionString;
        int m = 0;
        public WordCatagory()
        {
            InitializeComponent();
            loadForm();
        }

        private void cache(int per)
        {
            SqlConnection sqlConFill = new SqlConnection(conn);
            SqlCommand cmdFill = new SqlCommand();
            cmdFill.CommandText = "UPDATE extra SET Cat_Id=@CatagoryIde WHERE Ext_Id=1";
            cmdFill.Parameters.AddWithValue("@CatagoryIde", per);
            cmdFill.Connection = sqlConFill;
            sqlConFill.Open();
            cmdFill.ExecuteNonQuery();
            sqlConFill.Close();
        }

        private int existCheck(String wordper, int idck)
        {
            SqlConnection sqlConFill2 = new SqlConnection(conn);
            SqlCommand cmdFill2 = new SqlCommand();
            cmdFill2.CommandText = "SELECT Word_Id FROM Words WHERE Word=@Name AND Word_Id != @Id";
            cmdFill2.Parameters.AddWithValue("@Name", wordper);
            cmdFill2.Parameters.AddWithValue("@Id", idck);
            cmdFill2.Connection = sqlConFill2;
            sqlConFill2.Open();
            SqlDataReader rd = cmdFill2.ExecuteReader();
            String cat = "";
            if (rd.HasRows)
            {
                rd.Read();
                cat = cat + rd[0].ToString();
                rd.Close();
                sqlConFill2.Close();
                MessageBox.Show("This Word Already Exist at Id " + cat, " ", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return (Convert.ToInt32(cat));
            }
            else
            {
                rd.Close();
                sqlConFill2.Close();
                return (-1);
            }
        }

        private void loadForm()
        {
            SqlConnection sqlConFill = new SqlConnection(conn);
            SqlCommand cmdFill = new SqlCommand();
            cmdFill.CommandText = "SELECT * FROM Catagory_list WHERE Cat_Id IS NOT NULL";
            cmdFill.Connection = sqlConFill;
            sqlConFill.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmdFill);
            DataTable dt = new DataTable("Catagory_list");
            sda.Fill(dt);
            grdCatagory.ItemsSource = dt.DefaultView;
            sqlConFill.Close();

            cmdFill.CommandText = "SELECT * FROM Words WHERE Cat_Id=(SELECT Cat_Id FROM extra WHERE Ext_Id=1)";
            cmdFill.Connection = sqlConFill;
            sqlConFill.Open();
            SqlDataAdapter sda2 = new SqlDataAdapter(cmdFill);
            DataTable dt2 = new DataTable("Catagory_list");
            sda.Fill(dt2);
            wordList.ItemsSource = dt2.DefaultView;
            sqlConFill.Close();

            wordTxt.Focus();
        }

        private void change(object sender, RoutedEventArgs e)
        {
            String txt = Convert.ToString(((DataRowView)wordList.SelectedItem)["Word_Id"]);
            if (txt != null)
            {
                wordIdTxt.Text = Convert.ToString(((DataRowView)wordList.SelectedItem)["Word_Id"]);
                wordTxt.Text = Convert.ToString(((DataRowView)wordList.SelectedItem)["Word"]);
                singularTxt.Text = Convert.ToString(((DataRowView)wordList.SelectedItem)["singular"]);
                indefSingularTxt.Text = Convert.ToString(((DataRowView)wordList.SelectedItem)["indef_singular"]);
                defSingularTxt.Text = Convert.ToString(((DataRowView)wordList.SelectedItem)["def_singular"]);
                pluralTxt.Text = Convert.ToString(((DataRowView)wordList.SelectedItem)["plural"]);
                defPluralTxt.Text = Convert.ToString(((DataRowView)wordList.SelectedItem)["def_plural"]);
                catIdTxt.Text = Convert.ToString(((DataRowView)wordList.SelectedItem)["Cat_Id"]);
                noteTxt.Text = Convert.ToString(((DataRowView)wordList.SelectedItem)["note"]);
                m = 1;
            }
            //wordTxt.Focus();
            wordUpdate.IsEnabled = true;
            wordDelete.IsEnabled = true;
            wordInsert.IsEnabled = true;
        }

        private void wordInsert_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(wordTxt.Text) && !String.IsNullOrEmpty(catIdTxt.Text))
            {
                if (!Regex.Match(catIdTxt.Text, @"^[0-9]*$").Success)
                {
                    // phone number was incorrect
                    MessageBox.Show("Invalid Catagory ID", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                    catIdTxt.Focus();
                    return;
                }
                if (existCheck(wordTxt.Text, -1) != 1)
                {
                    SqlConnection sqlConFill2 = new SqlConnection(conn);
                    SqlCommand cmdFill2 = new SqlCommand();
                    sqlConFill2 = new SqlConnection(conn);
                    cmdFill2 = new SqlCommand();
                    cmdFill2.CommandText = "INSERT INTO Words(Cat_Id,Word,singular,indef_singular,def_singular,plural,def_plural,note) VALUES (@CatId,@wordnm,@singularnm,@indefsingnm,@defsingnm,@pluralnm,@defpluralnm,@notenm)";
                    cmdFill2.Parameters.AddWithValue("@CatId", catIdTxt.Text);
                    cmdFill2.Parameters.AddWithValue("@Wordnm", wordTxt.Text);
                    cmdFill2.Parameters.AddWithValue("@singularnm", singularTxt.Text);
                    cmdFill2.Parameters.AddWithValue("@indefsingnm", indefSingularTxt.Text);
                    cmdFill2.Parameters.AddWithValue("@defsingnm", defSingularTxt.Text);
                    cmdFill2.Parameters.AddWithValue("@pluralnm", pluralTxt.Text);
                    cmdFill2.Parameters.AddWithValue("@defpluralnm", defPluralTxt.Text);
                    cmdFill2.Parameters.AddWithValue("@notenm", noteTxt.Text);
                    cmdFill2.Connection = sqlConFill2;
                    sqlConFill2.Open();
                    cmdFill2.ExecuteNonQuery();
                    sqlConFill2.Close();
                    cache(Convert.ToInt32(catIdTxt.Text));
                    WordCatagory win2 = new WordCatagory();
                    win2.Show();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Word Or Type Id Cann't be Empty", " ", MessageBoxButton.OK, MessageBoxImage.Error);
                catIdTxt.Focus();
                return;
            }
        }

        private void wordUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(wordTxt.Text) && !String.IsNullOrEmpty(catIdTxt.Text))
            {
                if (!Regex.Match(catIdTxt.Text, @"^[0-9]*$").Success)
                {
                    // phone number was incorrect
                    MessageBox.Show("Invalid Catagory ID", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                    catIdTxt.Focus();
                    return;
                }
                if (existCheck(wordTxt.Text, Convert.ToInt32(wordIdTxt.Text)) == -1)
                {
                    SqlConnection sqlConFill2 = new SqlConnection(conn);
                    SqlCommand cmdFill2 = new SqlCommand();
                    cmdFill2.CommandText = "UPDATE Words SET Cat_Id=@CatId,Word=@wordnm,singular=@singularnm,indef_singular=@indefsingnm,def_singular=@defsingnm,plural=@pluralnm,def_plural=@defpluralnm,note=@notenm WHERE Word_Id=@wordId";
                    cmdFill2.Parameters.AddWithValue("@CatId", catIdTxt.Text);
                    cmdFill2.Parameters.AddWithValue("@Wordnm", wordTxt.Text);
                    cmdFill2.Parameters.AddWithValue("@singularnm", singularTxt.Text);
                    cmdFill2.Parameters.AddWithValue("@indefsingnm", indefSingularTxt.Text);
                    cmdFill2.Parameters.AddWithValue("@defsingnm", defSingularTxt.Text);
                    cmdFill2.Parameters.AddWithValue("@pluralnm", pluralTxt.Text);
                    cmdFill2.Parameters.AddWithValue("@defpluralnm", defPluralTxt.Text);
                    cmdFill2.Parameters.AddWithValue("@notenm", noteTxt.Text);
                    cmdFill2.Parameters.AddWithValue("@wordId", wordIdTxt.Text);
                    cmdFill2.Connection = sqlConFill2;
                    sqlConFill2.Open();
                    cmdFill2.ExecuteNonQuery();
                    sqlConFill2.Close();
                    cache(Convert.ToInt32(catIdTxt.Text));
                    WordCatagory win2 = new WordCatagory();
                    win2.Show();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Type Id or Name Cann't be Empty", " ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void wordDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(wordIdTxt.Text))
            {
                SqlConnection sqlConFill = new SqlConnection(conn);
                SqlCommand cmdFill = new SqlCommand();
                cmdFill.CommandText = "DELETE FROM Words WHERE Word_Id=@CatagoryId5";
                cmdFill.Parameters.AddWithValue("@CatagoryId5", wordIdTxt.Text);
                cmdFill.Connection = sqlConFill;
                sqlConFill.Open();
                cmdFill.ExecuteNonQuery();
                sqlConFill.Close();

                cache(Convert.ToInt32(catIdTxt.Text));
                WordCatagory win2 = new WordCatagory();
                win2.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Word Id Cann't be Empty");
            }
        }


        private void change2(object sender, RoutedEventArgs e)
        {
            if (m == 0)
            {
                //wordList.ItemsSource = null;
                String txt2 = Convert.ToString(((DataRowView)grdCatagory.SelectedItem)["Cat_Id"]);
                if (txt2 != null)
                {
                    typeIdTxt.Text = Convert.ToString(((DataRowView)grdCatagory.SelectedItem)["Cat_Id"]);
                    wordTypeTxt.Text = Convert.ToString(((DataRowView)grdCatagory.SelectedItem)["Cat_Name"]);
                }
                SqlConnection sqlConFill = new SqlConnection(conn);
                SqlCommand cmdFill = new SqlCommand();
                cmdFill.CommandText = "SELECT * FROM Words WHERE Cat_Id=@Id";
                cmdFill.Parameters.AddWithValue("@Id", typeIdTxt.Text);
                cmdFill.Connection = sqlConFill;
                sqlConFill.Open();
                SqlDataAdapter sda = new SqlDataAdapter(cmdFill);
                DataTable dt = new DataTable("Words");
                sda.Fill(dt);
                wordList.ItemsSource = dt.DefaultView;
                sqlConFill.Close();
                cache(Convert.ToInt32(typeIdTxt.Text));
                typeUpdate.IsEnabled = true;
                typeDelete.IsEnabled = true;
                typeCreate.IsEnabled = true;
                //wordTxt.Focus();
                //wordList.ItemsSource = null;
            }
            else
            {
                WordCatagory win2 = new WordCatagory();
                win2.Show();
                this.Close();
            }
        }


        private void typeCreate_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(wordTypeTxt.Text))
            {
                int sr = 0;
                SqlConnection sqlConFill2 = new SqlConnection(conn);
                SqlCommand cmdFill2 = new SqlCommand();
                cmdFill2.CommandText = "SELECT MAX(Cat_Id) FROM Catagory_list";
                cmdFill2.Connection = sqlConFill2;
                sqlConFill2.Open();
                SqlDataReader rd1 = cmdFill2.ExecuteReader();
                if (rd1.HasRows)
                {
                    rd1.Read();
                    var outputParam = rd1[0];
                    if (!(outputParam is DBNull))
                    {
                        sr = Convert.ToInt32(rd1[0]);
                    }
                    else
                    {
                        sr = 0;
                    }
                    rd1.Close();
                }
                sqlConFill2.Close();
                sr = sr + 1;

                cmdFill2.CommandText = "SELECT Cat_Id FROM Catagory_list WHERE Cat_Name=@Name";
                cmdFill2.Parameters.AddWithValue("@Name", wordTypeTxt.Text);
                cmdFill2.Connection = sqlConFill2;
                sqlConFill2.Open();
                SqlDataReader rd = cmdFill2.ExecuteReader();
                String cat = "";
                if (rd.HasRows)
                {
                    rd.Read();
                    cat = cat + rd[0].ToString();
                    rd.Close();
                    sqlConFill2.Close();
                    MessageBox.Show("This Word Type Already Exist at Id " + cat, " ", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    sqlConFill2.Close();
                    sqlConFill2 = new SqlConnection(conn);
                    cmdFill2 = new SqlCommand();
                    cmdFill2.CommandText = "INSERT INTO Catagory_list(Cat_Id,Cat_Name) VALUES (@CatagoryId,@CatagoryName)";
                    cmdFill2.Parameters.AddWithValue("@CatagoryId", sr.ToString());
                    cmdFill2.Parameters.AddWithValue("@CatagoryName", wordTypeTxt.Text);
                    cmdFill2.Connection = sqlConFill2;
                    sqlConFill2.Open();
                    cmdFill2.ExecuteNonQuery();
                    sqlConFill2.Close();
                    cache(0);
                    WordCatagory win2 = new WordCatagory();
                    win2.Show();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Catagory Name Cann't be Empty", " ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void typeUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(wordTypeTxt.Text) && !String.IsNullOrEmpty(typeIdTxt.Text))
            {
                SqlConnection sqlConFill = new SqlConnection(conn);
                SqlCommand cmdFill = new SqlCommand();
                cmdFill.CommandText = "UPDATE Catagory_list SET Cat_Id=@CatagoryId,Cat_Name=@CatagoryName WHERE Cat_Id=@CatagoryId";
                cmdFill.Parameters.AddWithValue("@CatagoryId", typeIdTxt.Text);
                cmdFill.Parameters.AddWithValue("@CatagoryName", wordTypeTxt.Text);
                cmdFill.Connection = sqlConFill;
                sqlConFill.Open();
                cmdFill.ExecuteNonQuery();
                sqlConFill.Close();
                WordCatagory win2 = new WordCatagory();
                win2.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Type Id or Name Cann't be Empty", " ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void typeDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(wordTypeTxt.Text) && !String.IsNullOrEmpty(typeIdTxt.Text))
            {
                SqlConnection sqlConFill = new SqlConnection(conn);
                SqlCommand cmdFill = new SqlCommand();
                cmdFill.CommandText = "DELETE FROM Catagory_list WHERE Cat_Id=@CatagoryId";
                cmdFill.Parameters.AddWithValue("@CatagoryId", typeIdTxt.Text);
                cmdFill.Connection = sqlConFill;
                sqlConFill.Open();
                cmdFill.ExecuteNonQuery();
                sqlConFill.Close();

                cmdFill.CommandText = "DELETE FROM Words WHERE Cat_Id=@CatagoryId3";
                cmdFill.Parameters.AddWithValue("@CatagoryId3", typeIdTxt.Text);
                cmdFill.Connection = sqlConFill;
                sqlConFill.Open();
                cmdFill.ExecuteNonQuery();
                sqlConFill.Close();

                cmdFill.CommandText = "UPDATE Catagory_list SET Cat_Id=Cat_Id-1 WHERE Cat_Id>@CatagoryId2";
                cmdFill.Parameters.AddWithValue("@CatagoryId2", typeIdTxt.Text);
                cmdFill.Connection = sqlConFill;
                sqlConFill.Open();
                cmdFill.ExecuteNonQuery();
                sqlConFill.Close();

                cmdFill.CommandText = "UPDATE Words SET Cat_Id=Cat_Id-1 WHERE Cat_Id>@CatagoryId7";
                cmdFill.Parameters.AddWithValue("@CatagoryId7", typeIdTxt.Text);
                cmdFill.Connection = sqlConFill;
                sqlConFill.Open();
                cmdFill.ExecuteNonQuery();
                sqlConFill.Close();

                cache(0);
                WordCatagory win2 = new WordCatagory();
                win2.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Catagory Id or Name Cann't be Empty");
            }
        }

    }
}
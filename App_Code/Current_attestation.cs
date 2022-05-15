using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web;

public class Current_attestation
{
    private int id_current_attestation { get; set; }
    private string student_book { get; set; }
    private string mark { get; set; }
    private DateTime datee { get; set; }
    private int educ_year { get; set; }
    private string discipline { get; set; }
    private int semestr { get; set; }
    private int control_type { get; set; }

    public Current_attestation(){}

    public Current_attestation(int id_current_attestation, string student_book, string mark, DateTime datee, int educ_year, string discipline, int semestr, int control_type)
    {
        this.id_current_attestation = id_current_attestation;
        this.student_book = student_book;
        this.mark = mark;
        this.datee = datee;
        this.educ_year = educ_year;
        this.discipline = discipline;
        this.semestr = semestr;
        this.control_type = control_type;
    }

    public static string getCurrentAttestation(string student_book)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["qwe"].ConnectionString;
        SqlConnection con = new SqlConnection(connectionString);

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri("http://172.17.2.1/");
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetAsync("univer_http/hs/Campus/curgrades?RegNumber=" + student_book);
            response.Wait();
            var result = response.Result;
            if (result.IsSuccessStatusCode)
            {
                var read = result.Content.ReadAsStringAsync();
                read.Wait();
                var buf = read.Result;
                string[] splitters = { "&&" };
                string[] grades = buf.Split(splitters, StringSplitOptions.None);
                for (int i = 0; i < grades.Count(); i++)
                {
                    con.Open();
                    string[] m = grades[i].Split(';');

                    var str = "insert into Current_attestation (student_book, discipline, id_control_type, mark, datee, semestr, educ_year, competitions, competitions_codes) values (@student_book, @discipline, @id_control_type, @mark, @datee, @semestr, @educ_year, @competitions, @competitions_codes)";
                    var com = new SqlCommand(str, con);
                    com.Parameters.Clear();
                    com.Parameters.AddWithValue("student_book", student_book);
                    com.Parameters.AddWithValue("discipline", m[1]);

                    switch (m[2])
                    {
                        case "Зачет":
                            com.Parameters.AddWithValue("id_control_type", 1);
                            break;
                        case "Экзамен":
                            com.Parameters.AddWithValue("id_control_type", 2);
                            break;
                        case "Зачет с оценкой":
                            com.Parameters.AddWithValue("id_control_type", 3);
                            break;
                    }

                    com.Parameters.AddWithValue("mark", m[3]);
                    com.Parameters.AddWithValue("datee", m[4]);
                    com.Parameters.AddWithValue("semestr", m[0]);
                    com.Parameters.AddWithValue("educ_year", m[5]);
                    com.Parameters.AddWithValue("competitions", m[6]);
                    com.Parameters.AddWithValue("competitions_codes", m[7]);
                    com.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        con.Open();
        var s = "Select Current_attestation.id_current_attest, Current_attestation.student_book, Current_attestation.mark, Current_attestation.datee, Current_attestation.educ_year, Current_attestation.discipline, Current_attestation.semestr, Control_type.control_type from Current_attestation " +
                "Inner join Control_type on Current_attestation.id_control_type = Control_type.id_control_type where educ_year = YEAR(getdate()) and student_book = '" + student_book + "'";
        var command = new SqlCommand(s, con);
        var reader = command.ExecuteReader();
        Marks marks = null;
        if (reader.Read())
        {
            marks = new Marks(
                reader.GetInt16(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetDateTime(3),
                reader.GetInt16(4),
                reader.GetString(5),
                reader.GetInt16(6),
                reader.GetInt16(7));
            string JsonResult = JsonConvert.SerializeObject(marks, Formatting.Indented);
            con.Close();
            return JsonResult;
        }
        else
        {
            con.Close();
            return ("Не удалось получить сведения об оценках");
        }

    }
}
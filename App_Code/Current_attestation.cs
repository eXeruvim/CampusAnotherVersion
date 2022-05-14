using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
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

    public static string getAttestation(string student_book)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["qwe"].ConnectionString;
        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        var s = "Select Current_attestation.id_current_attest, Current_attestation.student_book, Current_attestation.mark, Current_attestation.datee, Current_attestation.educ_year, Current_attestation.discipline, Current_attestation.semestr, Control_type.control_type from Current_attestation " +
                "Inner join Control_type on Current_attestation.id_control_type = Control_type.id_control_type where educ_year = YEAR(getdate()) and student_book = '" + student_book + "'";
        var command = new SqlCommand(s, con);
        var reader = command.ExecuteReader();
        Current_attestation current_attestation = null;
        if (reader.Read())
        {
            current_attestation = new Current_attestation(
                reader.GetInt16(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetDateTime(3),
                reader.GetInt16(4),
                reader.GetString(5),
                reader.GetInt16(6),
                reader.GetInt16(7));
            string JsonResult = JsonConvert.SerializeObject(current_attestation, Formatting.Indented);
            con.Close();
            return JsonResult;
        }
        else
        {
            con.Close();
            return ("Не удалось получить сведения о текущей аттестации");
        }

    }
}
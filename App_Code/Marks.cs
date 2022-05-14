using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

public class Marks
{
    private int id_marks { get; set; }
    private string student_book { get; set; }
    private string mark { get; set; }
    private DateTime datee { get; set; }
    private int educ_year { get; set; }
    private string discipline { get; set; }
    private int semestr { get; set; }
    private int control_type { get; set; }

    public Marks() {}

    public Marks(int id_marks, string student_book, string mark, DateTime datee, int educ_year, string discipline, int semestr, int control_type){
        this.id_marks = id_marks;
        this.student_book = student_book;
        this.mark = mark;
        this.datee = datee;
        this.educ_year = educ_year;
        this.discipline = discipline;
        this.semestr = semestr;
        this.control_type = control_type;
    }

    public static string getGradeSubject(string student_book)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["qwe"].ConnectionString;
        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        var s = "SELECT marks.id_marks, marks.student_book, marks.mark, marks.datee, marks.educ_year, marks.discipline, marks.semestr, control_type.control_type FROM Marks " +
                "Inner join Control_type on Marks.id_control_type = Control_type.id_control_type WHERE student_book = '" + student_book + "'";
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
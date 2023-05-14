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

namespace Final_Project_DBAS_2023
{
    /// <summary>
    /// Interaction logic for MakeNoteWindow.xaml
    /// </summary>
    public partial class MakeNoteWindow : Window
    {
        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DBAS-Final-G8;Integrated Security=True";

        private string queryStringMain = "SELECT PATIENT.PATIENT_NO, ADMISSION.ADMISSION_ID, NOTE.NOTE_ID, " +
            "NOTE.NOTE_TEXT FROM PATIENT, ADMISSION, NOTE " +
            "WHERE PATIENT.PATIENT_NO = @PATIENT_NO." +
            " AND PATIENT.PATIENT_NO = ADMISSION.PATIENT_NO " +
            "AND ADMISSION.ADMISSION_ID = NOTE.ADMISSION_ID ORDER BY NOTE.NOTE_ID DESC";

        public MakeNoteWindow()
        {
            InitializeComponent();
        }
    }
}

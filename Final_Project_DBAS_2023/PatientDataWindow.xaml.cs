using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for PatientDataWindow.xaml
    /// </summary>
    public partial class PatientDataWindow : Window
    {
        private string Physician_No;
        private int Physician_ID_Index;
        private int Patient_No;

        private string PatientNo = "";
        private string PatientFirstName = "";
        private string PatientMiddleName = "";
        private string PatientLastName = "";
        private string PatientAddress = "";
        private string PatientCity = "";
        private string PatientProvince = "";
        private string PatientPostalCode = "";
        private string PatientTelephone = "";
        private string PatientSex = "";
        private string PatientHealthCardNumber = "";
        private string PatientRoomNo = "";
        private string PatientBedDesig = "";
        private string PatientExtension = "";
        private string PatientDateAdmitted = "";
        private string PatientFinancialStatus = "";
        private string PatientDateDischarged = "";

        // Connection String
        private string connectionString = MainWindow.connectionString;

        private string queryStringMain = "SELECT PATIENT.PATIENT_NO, PATIENT.PATIENT_FIRST_NAME, PATIENT.PATIENT_MIDDLE_NAME, "
            + "PATIENT.PATIENT_LAST_NAME, PATIENT.PATIENT_ADDRESS, PATIENT.PATIENT_CITY, "
            + "PATIENT.PATIENT_PROV, PATIENT.PATIENT_POSTAL_CODE, PATIENT.PATIENT_SEX, "
            + "PATIENT.HCN, PATIENT.PATIENT_TELEPHONE, PATIENT.EXTENSION, "
            + "PATIENT.FINANCIAL_STATUS, ADMISSION.DATE_ADMITTED, ADMISSION.DISCHARGE_DATE, "
            + "LOCATION.ROOM_NO, LOCATION.BED_DESIG FROM PATIENT INNER JOIN ADMISSION "
            + "ON PATIENT.PATIENT_NO = ADMISSION.PATIENT_NO INNER JOIN LOCATION "
            + "ON ADMISSION.LOCATION_ID = LOCATION.LOCATION_ID WHERE PATIENT.PATIENT_NO = @PATIENTNO ORDER BY ADMISSION.DATE_ADMITTED DESC";

        public PatientDataWindow()
        {
            InitializeComponent();
        }

        public PatientDataWindow(string Passed_Physician_No, int Passed_Physician_ID_Index, int Passed_Patient_No)
        {
            Physician_No = Passed_Physician_No;
            Physician_ID_Index = Passed_Physician_ID_Index;
            Patient_No = Passed_Patient_No;
            InitializeComponent();
        }

        private void ContentLoaded_Event(object sender, EventArgs e)
        {
            System.Data.SqlClient.SqlConnection con;
            con = new System.Data.SqlClient.SqlConnection();
            con.ConnectionString = connectionString;

            // Query creates SQL command for Main query
            SqlCommand Query = new SqlCommand(queryStringMain, con);
            Query.Parameters.AddWithValue("@PATIENTNO", Patient_No);

            // Opens the Connection
            con.Open();

            // Executes the query and saves all returned data to SQLDataReader variable
            SqlDataReader reader = Query.ExecuteReader();
            try
            {
                // Reads from retrieved data one time
                reader.Read();

                PatientNo = reader["PATIENT_NO"].ToString();
                PatientFirstName = reader["PATIENT_FIRST_NAME"].ToString();
                PatientMiddleName = reader["PATIENT_MIDDLE_NAME"].ToString();
                PatientLastName = reader["PATIENT_LAST_NAME"].ToString();
                PatientAddress = reader["PATIENT_ADDRESS"].ToString();
                PatientCity = reader["PATIENT_CITY"].ToString();
                PatientProvince = reader["PATIENT_PROV"].ToString();
                PatientPostalCode = reader["PATIENT_POSTAL_CODE"].ToString();
                PatientSex = reader["PATIENT_SEX"].ToString();
                PatientHealthCardNumber = reader["HCN"].ToString();
                PatientTelephone = reader["PATIENT_TELEPHONE"].ToString();
                PatientRoomNo = reader["ROOM_NO"].ToString();
                PatientBedDesig = reader["BED_DESIG"].ToString();
                PatientExtension = reader["EXTENSION"].ToString();
                PatientDateAdmitted = reader["DATE_ADMITTED"].ToString();
                PatientFinancialStatus = reader["FINANCIAL_STATUS"].ToString();
                PatientDateDischarged = reader["DISCHARGE_DATE"].ToString();

                PatientNumberTextbox.Text = PatientNo;
                if (PatientMiddleName != "")
                {
                    PatientFullNameTextbox.Text = PatientLastName + ", " + PatientFirstName + " " + PatientMiddleName.Substring(0, 1) + ".";
                }
                else
                {
                    PatientFullNameTextbox.Text = PatientLastName + ", " + PatientFirstName;
                }
                PatientAddressTextbox.Text = PatientAddress;
                CityProvPCTextbox.Text = PatientCity + ", " + PatientProvince + " " + PatientPostalCode;
                TelephoneTextbox.Text = PatientTelephone;
                SexTextbox.Text = PatientSex;
                HCNTextbox.Text = PatientHealthCardNumber;
                LocationTextbox.Text = PatientRoomNo + PatientBedDesig;
                ExtensionTextbox.Text = PatientExtension;
                DateAdmittedTextbox.Text = PatientDateAdmitted;
                FinancialStatusTextbox.Text = PatientFinancialStatus;
                DateDischargedTextbox.Text = PatientDateDischarged;
            }
            catch
            {
                MessageBox.Content = "Error: Could not find associated patient";
            }
            finally
            {
                // Closes the Connection
                con.Close();
            }

        }
    

        private void WindowClosing_Event(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AdmissionsWindow Admissions = new AdmissionsWindow(Physician_No, Physician_ID_Index, Patient_No);
            Admissions.Show();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

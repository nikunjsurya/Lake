using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
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
    /// Interaction logic for PatientDisplayWindow.xaml
    /// </summary>
    public partial class PatientDisplayWindow : Window
    {

        private bool initialLoad = true;
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

        private bool isFocused = false;

        // Connection String
        private string connectionString = MainWindow.connectionString;

        private string queryStringMain = "SELECT PATIENT.PATIENT_NO, PATIENT.PATIENT_FIRST_NAME, PATIENT.PATIENT_MIDDLE_NAME, "
            + "PATIENT.PATIENT_LAST_NAME, PATIENT.PATIENT_ADDRESS, PATIENT.PATIENT_CITY, "
            + "PATIENT.PATIENT_PROV, PATIENT.PATIENT_POSTAL_CODE, PATIENT.PATIENT_SEX, "
            + "PATIENT.HCN, PATIENT.PATIENT_TELEPHONE, PATIENT.EXTENSION, "
            + "PATIENT.FINANCIAL_STATUS, ADMISSION.DATE_ADMITTED, ADMISSION.DISCHARGE_DATE, "
            + "LOCATION.ROOM_NO, LOCATION.BED_DESIG FROM PATIENT INNER JOIN ADMISSION "
            + "ON PATIENT.PATIENT_NO = ADMISSION.PATIENT_NO INNER JOIN LOCATION "
            + "ON ADMISSION.LOCATION_ID = LOCATION.LOCATION_ID WHERE ";

        private string queryStringPatientNo = "PATIENT.PATIENT_NO = @PATIENTNO ORDER BY ADMISSION.DATE_ADMITTED DESC";

        private string queryStringPatientName = "PATIENT.PATIENT_FIRST_NAME = @FIRSTNAME AND PATIENT.PATIENT_LAST_NAME " +
            "= @LASTNAME AND PATIENT_TELEPHONE = @TELEPHONE ORDER BY ADMISSION.DATE_ADMITTED DESC";
        public PatientDisplayWindow()
        {
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            ResetFields();
            if (PatientNoRadio.IsChecked == true)
            {
                // Search for matching record using PatientNo and store it meaningfully
                PatientNoSearch();
            }
            else
            {
                // Search for matching record using First and Last Name with Phone Number and store it meaningfully
                PatientNameSearch();
            }
        }

        private void PatientNoRadio_Checked(object sender, RoutedEventArgs e)
        {
            if(initialLoad == false)
            {
                PatientNoLabel.Visibility = Visibility.Visible;
                PatientNoTextbox.Visibility = Visibility.Visible;
                PatientNoTextbox.Text = "";
                FirstNameLabel.Visibility = Visibility.Hidden;
                FirstNameTextbox.Visibility = Visibility.Hidden;
                FirstNameTextbox.Text = "";
                LastNameLabel.Visibility = Visibility.Hidden;
                LastNameTextbox.Visibility = Visibility.Hidden;
                LastNameTextbox.Text = "";
                PhoneNumberLabel.Visibility = Visibility.Hidden;
                PhoneNumberTextbox.Visibility = Visibility.Hidden;
                PhoneNumberTextbox.Text = "";
            }
            initialLoad = false;
        }

        private void NameAndNumberRadio_Checked(object sender, RoutedEventArgs e)
        {
            PatientNoLabel.Visibility = Visibility.Hidden;
            PatientNoTextbox.Visibility = Visibility.Hidden;
            PatientNoTextbox.Text = "";
            FirstNameLabel.Visibility = Visibility.Visible;
            FirstNameTextbox.Visibility = Visibility.Visible;
            FirstNameTextbox.Text = "";
            LastNameLabel.Visibility = Visibility.Visible;
            LastNameTextbox.Visibility = Visibility.Visible;
            LastNameTextbox.Text = "";
            PhoneNumberLabel.Visibility = Visibility.Visible;
            PhoneNumberTextbox.Visibility = Visibility.Visible;
            PhoneNumberTextbox.Text = "";
        }

        private void PatientNoSearch()
        {
            if (PatientNoTextbox.Text.Trim() != "")
            {
                if(int.TryParse(PatientNoTextbox.Text.Trim(), out int res))
                {
                    // Create Connection
                    System.Data.SqlClient.SqlConnection con;
                    con = new System.Data.SqlClient.SqlConnection();
                    con.ConnectionString = connectionString;
                    Trace.WriteLine("Here 1");

                    // Query creates SQL command for Main plus the PatientNo String query
                    SqlCommand Query = new SqlCommand(queryStringMain + queryStringPatientNo, con);
                    Query.Parameters.AddWithValue("@PATIENTNO", PatientNoTextbox.Text.Trim());
                    Trace.WriteLine("Here 2");
                    Trace.WriteLine(PatientNoTextbox.Text.Trim());

                    // Opens the Connection
                    con.Open();
                    Trace.WriteLine("Here 3");
                    // Executes the query and saves all returned data to SQLDataReader variable
                    SqlDataReader reader = Query.ExecuteReader();

                    Trace.WriteLine("Here 4");
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

                        DisplayRetreivedPatient();
                    }
                    catch
                    {
                        // Displays relevant error to user
                        MessageBox.Content = "Error: Could not find associated patient";
                    }
                    finally
                    {
                        // Closes the Connection
                        con.Close();
                    }
                }
                else
                {
                    // Displays relevant error to user
                    MessageBox.Content = "Patient No must be Numeric";
                    PatientNoTextbox.Focus();
                }
            }
            else
            {
                // Displays relevant error to user
                MessageBox.Content = "You must enter a Patient Number";
                PatientNoTextbox.Focus();
            }
        }
        private void PatientNameSearch()
        {
            if (FirstNameTextbox.Text != "" && LastNameTextbox.Text != "" && PhoneNumberTextbox.Text != "")
            {
                
                if (PhoneNumberTextbox.Text.Length == 12)
                {
                    if (int.TryParse(PhoneNumberTextbox.Text.Substring(0,3).Trim(), out int res) && PhoneNumberTextbox.Text.Substring(3, 1).Trim() == " " && int.TryParse(PhoneNumberTextbox.Text.Substring(3, 3).Trim(), out res) && PhoneNumberTextbox.Text.Substring(7, 1).Trim() == "-" && int.TryParse(PhoneNumberTextbox.Text.Substring(8, 3).Trim(), out res))
                    {
                        // Create Connection
                        using (System.Data.SqlClient.SqlConnection con =
                        new System.Data.SqlClient.SqlConnection(connectionString))
                        {
                            // Query creates SQL command for Main, plus the PatientName String query
                            SqlCommand Query = new SqlCommand(queryStringMain + queryStringPatientName, con);
                            Query.Parameters.AddWithValue("@FIRSTNAME", FirstNameTextbox.Text.Trim());
                            Query.Parameters.AddWithValue("@LASTNAME", LastNameTextbox.Text.Trim());
                            Query.Parameters.AddWithValue("@TELEPHONE", PhoneNumberTextbox.Text.Trim());

                            // Opens the Connection
                            con.Open();
                            SqlDataReader reader = Query.ExecuteReader();
                            try
                            {
                                // Reads from retrieved data one time
                                reader.Read();

                                // Saves first returned set of values from query
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

                                // Calls the DisplayRetreivedPatient function
                                DisplayRetreivedPatient();
                            }
                            catch
                            {
                                // Displays relevant error to user
                                MessageBox.Content = "Error: Could not find associated patient";
                            }
                            finally
                            {
                                // Closes the Connection
                                con.Close();
                            }
                        }
                    }
                    else
                    {
                        // Displays relevant error to user
                        MessageBox.Content = "You must follow standard format for the Phone Number: 999 999-9999";
                        PhoneNumberTextbox.Focus();
                    }
                }
                else
                {
                    // Displays relevant error to user
                    MessageBox.Content = "You must follow standard format for the Phone Number: 999 999-9999";
                    PhoneNumberTextbox.Focus();
                }
            }
            else
            {
                if (FirstNameTextbox.Text == "")
                {
                    isFocused = true;
                    FirstNameTextbox.Focus();
                }
                if (LastNameTextbox.Text == "")
                {
                    if (!isFocused)
                    {
                        LastNameTextbox.Focus();
                        isFocused = true;
                    }
                }
                if (LastNameTextbox.Text == "")
                {
                    if (!isFocused)
                    {
                        PhoneNumberTextbox.Focus();
                        isFocused = true;
                    }
                }
                // Decides which relevant error to display to the user
                if (FirstNameTextbox.Text == "" && LastNameTextbox.Text == "" && PhoneNumberTextbox.Text == "")
                {
                    MessageBox.Content = "First Name Entry, Last Name Entry and Phone number must be filled.";
                }
                else if (FirstNameTextbox.Text == "" && LastNameTextbox.Text == "")
                {
                    MessageBox.Content = "First Name Entry and Last Name Entry must be filled.";
                }
                else if (FirstNameTextbox.Text == "" && PhoneNumberTextbox.Text == "")
                {
                    MessageBox.Content = "First Name Entry and Phone number must be filled.";
                }
                else if (LastNameTextbox.Text == "" && PhoneNumberTextbox.Text == "")
                {
                    MessageBox.Content = "Last Name Entry and Phone number must be filled.";
                }
                else if (FirstNameTextbox.Text == "")
                {
                    MessageBox.Content = "First Name Entry must be filled.";
                }
                else if (LastNameTextbox.Text == "")
                {
                    MessageBox.Content = "Last Name Entrymust be filled.";
                }
                else
                {
                    MessageBox.Content = "Phone number must be filled.";
                }
                isFocused = false;
            }
        }

        // Displays all the retrieved data in the textbox's
        private void DisplayRetreivedPatient()
        {
            PatientNumberTextbox.Text = PatientNo;
            if(PatientMiddleName != "")
            {
                PatientFullNameTextbox.Text = PatientLastName + ", " + PatientFirstName + " " + PatientMiddleName.Substring(0,1) + ".";
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


        private void PatientDisplayWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.MainWindow.Show();
        }

        // Resets all fields to being empty, or their default value
        private void ResetFields()
        {
            MessageBox.Content = "";
            PatientNo = "";
            PatientFirstName = "";
            PatientMiddleName = "";
            PatientLastName = "";
            PatientAddress = "";
            PatientCity = "";
            PatientProvince = "";
            PatientPostalCode = "";
            PatientTelephone = "";
            PatientSex = "";
            PatientHealthCardNumber = "";
            PatientRoomNo = "";
            PatientBedDesig = "";
            PatientExtension = "";
            PatientDateAdmitted = "";
            PatientFinancialStatus = "";
            PatientDateDischarged = "";
            isFocused = false;

            PatientNumberTextbox.Text = "";
            PatientFullNameTextbox.Text = "";
            PatientAddressTextbox.Text = "";
            CityProvPCTextbox.Text = "";
            TelephoneTextbox.Text = "";
            SexTextbox.Text = "";
            HCNTextbox.Text = "";
            LocationTextbox.Text = "";
            ExtensionTextbox.Text = "";
            DateAdmittedTextbox.Text = "";
            FinancialStatusTextbox.Text = "";
            DateDischargedTextbox.Text = "";
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

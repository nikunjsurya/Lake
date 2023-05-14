using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class AdmissionsWindow : Window
    {
        // Variables that are used to hold place of values for loading data based on situation
        private string CurrentPhysicianNo = "";
        private int CurrentPhysicianPosition = 0;
        private string CurrentPatientID = "";
        private bool foundRecord = false;
        private bool expectedExit = false;

        // List variables that hold data from the main query
        private List<string> PatientNo = new List<string>();
        private List<string> PatientFullName = new List<string>();
        private List<string> PatientLocation = new List<string>();
        private List<string> DateAdmitted = new List<string>();

        // List variables that hold data from the secondary query
        private List<string> PhysicianNo = new List<string>();
        private List<string> PhysicianFullName = new List<string>();

        // Connection String
        private string connectionString = MainWindow.connectionString;

        // Query String for collecting all patients currently under care of given physician
        private string queryStringPatientData = "SELECT PATIENT.PATIENT_NO, PATIENT.PATIENT_FIRST_NAME, "
            + "PATIENT.PATIENT_MIDDLE_NAME, PATIENT.PATIENT_LAST_NAME, "
            + "LOCATION.ROOM_NO, LOCATION.BED_DESIG, ADMISSION.DATE_ADMITTED "
            + "FROM PHYSICIAN, PATIENT, LOCATION, ADMISSION "
            + "WHERE PHYSICIAN.PHYSICIAN_NO = @PHYSICIAN_NO "
            + "AND PHYSICIAN.PHYSICIAN_NO = ADMISSION.PHYSICIAN_NO "
            + "AND ADMISSION.PATIENT_NO = PATIENT.PATIENT_NO "
            + "AND ADMISSION.LOCATION_ID = LOCATION.LOCATION_ID "
            + "AND ADMISSION.DISCHARGE_DATE IS NULL "
            + "ORDER BY DATE_ADMITTED DESC ";

        // Query String collects relevant data for all Physicians
        private string queryStringGetPhysicians = "SELECT PHYSICIAN_NO, PHYSICIAN.PHYSICIAN_FIRST_NAME, "
            + "PHYSICIAN.PHYSICIAN_MIDDLE_NAME, PHYSICIAN.PHYSICIAN_LAST_NAME "
            + "FROM PHYSICIAN ORDER BY PHYSICIAN_NO ASC ";

        // Initialization without passed values, used when called from MainWindow
        public AdmissionsWindow()
        {
            InitializeComponent();
        }

        // Initialization with passed values, used when called from SubWindows to get same position in Physician List
        public AdmissionsWindow(string Physician_ID, int Physician_ID_Index, int DataGridIndex)
        {
            InitializeComponent();
            CurrentPhysicianNo = Physician_ID;
            CurrentPhysicianPosition = Physician_ID_Index;
            CurrentPatientID = DataGridIndex.ToString();
        }

        // Loads data based on if there are passed values or not
        private void ContentLoaded_Event(object sender, EventArgs e)
        {
            if (CurrentPhysicianNo == "")
            {
                OpeningLoad();
            }
            else
            {
                OpeningLoad(CurrentPhysicianNo);
            }
        }

        // Made originally to export datagrid to excel file, but struggled to complete functionality
        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Content = "Sorry, that function is currently in development.";
        }

        // Submit button click response and displays data based on user entered Physician_No rather than presets
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // Resets MessageBox text to empty
            MessageBox.Content = "";

            // Make a variable to contain the index of the entered Physician
            int GivenNumberLocation = -1;

            // Searches between each PhysicianNo in the list
            for(int i = 0; i < PhysicianNo.Count; i++)
            {
                // Checks if the PhysicianNo at position i is equal to the given value
                if (PhysicianNoTextbox.Text == PhysicianNo.ElementAt(i))
                {
                    // Set that location to i
                    GivenNumberLocation = i;
                    break;
                }
            }
            // if the GivenNumberLocation variable is not -1
            if (GivenNumberLocation != -1)
            {
                //  Assign the found value to currenPhysicianPosition
                CurrentPhysicianPosition = GivenNumberLocation;

                // Displays all data associated
                DisplayData();
            }
            else
            {
                MessageBox.Content = "Physician does not exist.";
            }
        }

        // Next button click response
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // Resets MessageBox text to empty
            MessageBox.Content = "";
            // Checks if the current position is the end of the list
            if (CurrentPhysicianPosition != PhysicianNo.Count - 1)
            {
                // if not at end of list, increase position
                CurrentPhysicianPosition++;
            }
            else
            {
                // else, reset position to beginning of list
                CurrentPhysicianPosition = 0;
            }
            // Saves the Physician No textbox's text to the CurrentPhysicianPosition variable value
            PhysicianNoTextbox.Text = PhysicianNo.ElementAt(CurrentPhysicianPosition);

            // Displays all data associated
            DisplayData();
        }

        // Previous button click response
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Content = "";
            if (CurrentPhysicianPosition != 0)
            {
                CurrentPhysicianPosition--;
            }
            else
            {
                CurrentPhysicianPosition = PhysicianNo.Count - 1;
            }
            // Saves the Physician No textbox's text to the CurrentPhysicianPosition variable value
            PhysicianNoTextbox.Text = PhysicianNo.ElementAt(CurrentPhysicianPosition);

            // Displays all data associated
            DisplayData();
        }

        // Loads data when window is first opened without any passed values
        private void OpeningLoad()
        {
            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                // Query creates SQL command for secondary query
                SqlCommand Query = new SqlCommand(queryStringGetPhysicians, con);

                // Opens the Connection
                con.Open();
                
                // Executes the query and saves all returned data to SQLDataReader variable
                SqlDataReader reader = Query.ExecuteReader();

                // Loops through returned data until reaching the end
                while (reader.Read())
                {
                    // Saves all Physician No into the given List
                    PhysicianNo.Add(reader["PHYSICIAN_NO"].ToString());

                    // Saves data retreived formatted based on if there is a middle name or not.
                    if (reader["PHYSICIAN_MIDDLE_NAME"].ToString() != "")
                    {
                        PhysicianFullName.Add(reader["PHYSICIAN_LAST_NAME"].ToString() + ", " + reader["PHYSICIAN_FIRST_NAME"].ToString().Substring(0, 1) + ". " + reader["PHYSICIAN_MIDDLE_NAME"].ToString().Substring(0, 1) + ".");
                    }
                    else
                    {
                        PhysicianFullName.Add(reader["PHYSICIAN_LAST_NAME"].ToString() + ", " + reader["PHYSICIAN_FIRST_NAME"].ToString());
                    }
                }
                // Saves the Physician No textbox's text to the first value in the list
                PhysicianNoTextbox.Text = PhysicianNo.ElementAt(0);

                // Displays all data associated
                DisplayData();
            }
        }

        // Loads data when window is first opened with passed values
        private void OpeningLoad(string Physician_ID)
        {
            // Query creates SQL command for secondary query
            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                // Query creates SQL command for secondary query
                SqlCommand Query = new SqlCommand(queryStringGetPhysicians, con);

                // Opens the Connection
                con.Open();

                // Executes the query and saves all returned data to SQLDataReader variable
                SqlDataReader reader = Query.ExecuteReader();

                // Loops through returned data until reaching the end
                while (reader.Read())
                {
                    // Saves all Physician No into the given Lists
                    PhysicianNo.Add(reader["PHYSICIAN_NO"].ToString());

                    // Saves data retreived formatted based on if there is a middle name or not.
                    if (reader["PHYSICIAN_MIDDLE_NAME"].ToString() != "")
                    {
                        PhysicianFullName.Add(reader["PHYSICIAN_LAST_NAME"].ToString() + ", " + reader["PHYSICIAN_FIRST_NAME"].ToString().Substring(0, 1) + ". " + reader["PHYSICIAN_MIDDLE_NAME"].ToString().Substring(0, 1) + ".");
                    }
                    else
                    {
                        PhysicianFullName.Add(reader["PHYSICIAN_LAST_NAME"].ToString() + ", " + reader["PHYSICIAN_FIRST_NAME"].ToString());
                    }
                }
            }
            // Saves the Physician No textbox's text to the first value in the list
            PhysicianNoTextbox.Text = Physician_ID;

            // Displays all data associated
            DisplayData();
        }

        // Displays data based on value of CurrentPhysicianPosition variable
        private void DisplayData()
        {
            // Clears all PatientData Query data lists
            PatientNo.Clear();
            PatientFullName.Clear();
            PatientLocation.Clear();
            DateAdmitted.Clear();

            //
            DisplayNameTextbox.Text = PhysicianFullName.ElementAt(CurrentPhysicianPosition);
            DisplayNoTextbox.Text = PhysicianNo.ElementAt(CurrentPhysicianPosition);
            DisplayDateTextbox.Text = DateTime.Now.ToShortDateString();

            DataTable admissionReportTable = new DataTable();
            admissionReportTable.Rows.Clear();

            // Create Connection
            using (System.Data.SqlClient.SqlConnection con =
            new System.Data.SqlClient.SqlConnection(connectionString))
            {
                // Query creates SQL command for Main query
                SqlCommand Query = new SqlCommand(queryStringPatientData, con);
                Query.Parameters.AddWithValue("@PHYSICIAN_NO", PhysicianNoTextbox.Text.Trim());

                // Opens the Connection
                con.Open();

                // Executes the query and saves all returned data to SQLDataReader variable
                SqlDataReader reader = Query.ExecuteReader();
                try
                {
                    // Loops through returned data until reaching the end
                    while (reader.Read())
                    {
                        PatientNo.Add(reader["PATIENT_NO"].ToString());
                        if (reader["PATIENT_MIDDLE_NAME"].ToString() != "")
                        {
                            PatientFullName.Add(reader["PATIENT_LAST_NAME"].ToString() + ", " + reader["PATIENT_FIRST_NAME"].ToString().Substring(0, 1) + ". " + reader["PATIENT_MIDDLE_NAME"].ToString().Substring(0, 1) + ".");
                        }
                        else
                        {
                            PatientFullName.Add(reader["PATIENT_LAST_NAME"].ToString() + ", " + reader["PATIENT_FIRST_NAME"].ToString());
                        }
                        PatientLocation.Add(reader["ROOM_NO"].ToString() + reader["BED_DESIG"].ToString());
                        DateAdmitted.Add(DateTime.Parse(reader["DATE_ADMITTED"].ToString()).ToString("yyyy-MM-dd"));
                        foundRecord = true;
                    }

                    if (PatientNo != null)
                    {

                        admissionReportTable.Columns.Add("PATIENT-NO");
                        admissionReportTable.Columns.Add("PATIENT-NAME");
                        admissionReportTable.Columns.Add("LOCATION");
                        admissionReportTable.Columns.Add("DATE-ADMITTED");

                        for (int i = 0; i <= PatientNo.Count; i++)
                        {
                            if(PatientNo.ElementAt(i) == CurrentPatientID.ToString())
                            {
                                SelectedPatientTextbox.Text = CurrentPatientID.ToString();
                                ReportGrid.SelectedIndex = i;
                            }
                            admissionReportTable.Rows.Add(new object[] { PatientNo.ElementAt(i), PatientFullName.ElementAt(i), PatientLocation.ElementAt(i), DateAdmitted.ElementAt(i) });
                        }
                    }
                }
                catch
                {
                    bool existsInList = false;
                    for (int i = 0; i < PhysicianNo.Count; i++)
                    {
                        if (PhysicianNo.ElementAt(i) == PhysicianNoTextbox.Text.Trim())
                        {
                            existsInList = true;
                        }
                    }
                    if (existsInList && !foundRecord)
                    {
                        // Display relevant error to user
                        MessageBox.Content = "Physician has no current Admissions.";
                    }
                    else if (!foundRecord)
                    {
                        // Display relevant error to user
                        MessageBox.Content = "Physician does not exist.";
                    }
                }
                finally
                {
                    ReportGrid.ItemsSource = admissionReportTable.DefaultView;

                    // Closes the Connection
                    con.Close();
                    foundRecord = false;
                }
            }
        }

        // NewSelectionMade Event Response
        private void NewSelectionMade(object sender, SelectionChangedEventArgs e)
        {
            // Gets the Selected Index as an integer value 
            int.TryParse(ReportGrid.SelectedIndex.ToString(), out int ReportGridIndex);
            try
            {
                CurrentPatientID = PatientNo.ElementAt(ReportGridIndex);
                SelectedPatientTextbox.Text = PatientNo.ElementAt(ReportGridIndex);
            }
            catch
            {
                SelectedPatientTextbox.Text = "";
            }
        }

        // ViewData Button Response
        private void ViewDataButton_Click(object sender, RoutedEventArgs e)
        {
            // Gets the Selected Index as an integer value
            int.TryParse(ReportGrid.SelectedIndex.ToString(), out int ReportGridIndex);
            try
            {
                // Gets patient at the index position in list
                int PatientNoNum = int.Parse(PatientNo.ElementAt(ReportGridIndex));

                // Creates, Opens and then Displays a NoteViewerWindow with some data passed to it
                PatientDataWindow PatientData = new PatientDataWindow(DisplayNoTextbox.Text.Trim(), CurrentPhysicianPosition, PatientNoNum);
                PatientData.Show();

                // Sets expectedExit to true, to represent a SubWindow being the cause of the close
                expectedExit = true;

                // Closes the Window
                this.Close();
            }
            catch
            {
                // Checks if there are any patients retrieved
                if (PatientNo.Count != 0)
                {
                    // Display relevant error to user
                    MessageBox.Content = "You must select a Patient from the list.";
                }
            }
        }

        // Treatments Button Response
        private void TreatmentsButton_Click(object sender, RoutedEventArgs e)
        {
            // Gets the Selected Index as an integer value
            int.TryParse(ReportGrid.SelectedIndex.ToString(), out int ReportGridIndex);
            try
            {
                // Gets patient at the index position in list
                int PatientNoNum = int.Parse(PatientNo.ElementAt(ReportGridIndex));

                // Creates, Opens and then Displays a NoteViewerWindow with some data passed to it
                TreatmentWindow NoteViewer = new TreatmentWindow(DisplayNoTextbox.Text.Trim(), CurrentPhysicianPosition, PatientNoNum);
                NoteViewer.Show();

                // Sets expectedExit to true, to represent a SubWindow being the cause of the close
                expectedExit = true;

                // Closes the Window
                this.Close();
            }
            catch
            {
                // Checks if there are any patients retrieved
                if (PatientNo.Count != 0)
                {
                    // Display relevant error to user
                    MessageBox.Content = "You must select a Patient from the list.";
                }
            }
        }

        // Notes Button Response
        private void NotesButton_Click(object sender, RoutedEventArgs e)
        {
            // Gets the Selected Index as an integer value
            int.TryParse(ReportGrid.SelectedIndex.ToString(), out int ReportGridIndex);
            try
            {
                // Gets patient at the index position in list
                int PatientNoNum = int.Parse(PatientNo.ElementAt(ReportGridIndex));

                // Creates, Opens and then Displays a NoteViewerWindow with some data passed to it
                NoteViewerWindow NoteViewer = new NoteViewerWindow(DisplayNoTextbox.Text.Trim(), CurrentPhysicianPosition, PatientNoNum);
                NoteViewer.Show();

                // Sets expectedExit to true, to represent a SubWindow being the cause of the close
                expectedExit = true;

                // Closes the Window
                this.Close();
            }
            catch
            {
                // Checks if there are any patients retrieved
                if (PatientNo.Count != 0)
                {
                    // Display relevant error to user
                    MessageBox.Content = "You must select a Patient from the list.";
                }
            }
        }

        // Window Closing Event Response
        private void AdmissionsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Checks if the Exit was a SubWindow closing it, or the exit button or closing it manually
            if (expectedExit == false)
            {
                // Shows the main application
                Application.Current.MainWindow.Show();
            }
        }

        // Exit Button Response
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Shows the main application
            this.Close();
        }

    }
}
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
    /// Interaction logic for NoteViewerWindow.xaml
    /// </summary>
    public partial class NoteViewerWindow : Window
    {
        private int Patient_No;
        private string Physician_No;
        private int Physician_ID_Index;

        private int counter = 0;

        private string AdmissionId;
        private List<string> NoteId = new List<string>();
        private List<string> NoteText = new List<string>();

        // Connection String

        private string connectionString = MainWindow.connectionString;

        // GetNotes Query String, Gets relevant note data for a given patient
        private string queryStringGetNotes = "SELECT  NOTE.NOTE_ID, " +
            "NOTE.NOTE_TEXT FROM ADMISSION, NOTE " +
            "WHERE ADMISSION.PATIENT_NO = @PATIENT_NO AND " +
            "ADMISSION.ADMISSION_ID = NOTE.ADMISSION_ID ORDER BY NOTE.NOTE_ID DESC";

        // AdmissionID Query String, gets the ID for the given patients most recent admission
        private string queryStringAdmissionID = "SELECT ADMISSION.ADMISSION_ID" +
            " FROM ADMISSION WHERE ADMISSION.PATIENT_NO = @PATIENT_NO" +
            " ORDER BY ADMISSION_ID DESC";

        // Insert Query String, submits given data into database using given values
        private string queryStringInsert = "INSERT INTO NOTE(NOTE_ID, ADMISSION_ID, NOTE_TEXT) " +
            "VALUES (NEXT VALUE FOR NOTE_SEQ, @ADMISSION_ID, @NOTE_TEXT)";

        // Constructor
        public NoteViewerWindow(string Passed_Physician_No, int Passed_Physician_ID_Index, int Passed_Patient_No)
        {
            Physician_No = Passed_Physician_No;
            Physician_ID_Index = Passed_Physician_ID_Index;
            Patient_No = Passed_Patient_No;
            InitializeComponent();
        }

        // Content Loaded Event Response
        private void ContentLoaded_Event(object sender, EventArgs e)
        {
            // Sets DisplayNo Textbox's text to be Patient_No
            DisplayNoTextbox.Text = Patient_No.ToString();

            // Gets and displays the data
            GetData();
        }

        // Function to get and display all note data for given patient
        private void GetData()
        {
            // Resets MessageBox text to empty
            MessageBox.Content = "";
            using (System.Data.SqlClient.SqlConnection con =
            new System.Data.SqlClient.SqlConnection(connectionString))
            {
                // Query creates SQL command for Main query
                SqlCommand Query = new SqlCommand(queryStringGetNotes, con);
                Query.Parameters.AddWithValue("@PATIENT_NO", Patient_No);

                // Opens the Connection
                con.Open();

                // Executes the query and saves all returned data to SQLDataReader variable
                SqlDataReader reader = Query.ExecuteReader();
                try
                {
                    // Loops through returned data until reaching the end
                    while (reader.Read())
                    {
                        // Adds found data to relevant lists
                        NoteId.Add(reader["NOTE_ID"].ToString());
                        NoteText.Add(reader["NOTE_TEXT"].ToString());
                    }
                    // Sets NoteWriter and DisplayID text to related text using counter variable
                    NoteWriter.Text = NoteText[counter];
                    DisplayIdTextbox.Text = NoteId[counter];
                }
                catch
                {
                    // If there are no notes, display relevant error
                    MessageBox.Content = "Patient has no Notes.";
                }
                finally
                {
                    // Closes the Connection
                    con.Close();
                }
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            // Resets MessageBox text to empty
            MessageBox.Content = "";

            // Checks if there are any notes at all
            if (NoteText.Count == 0)
            {
                // Display appropriate error to user
                MessageBox.Content = "This Patient currently has no notes.";
            }

            // Checks if there is only one note to view
            else if (NoteText.Count == 1)
            {
                // Display appropriate error to user
                MessageBox.Content = "Only one note exists for this patient.";
            }

            // Otherwise, 
            else
            {
                // Checks if the current note is not the first element
                if (counter - 1 >= 0)
                {
                    // Reduces the counter by one
                    counter--;
                }
                else
                {
                    // Sends user to the last element in the note list
                    counter = NoteText.Count - 1;
                }

                // Sets the NoteWriter to the text and id at counter position
                NoteWriter.Text = NoteText.ElementAt(counter);
                DisplayIdTextbox.Text = NoteId.ElementAt(counter);
            }
        }

        // Next button click response
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // Resets MessageBox text to empty
            MessageBox.Content = "";

            // Checks if there are any notes at all
            if (NoteText.Count == 0)
            {
                // Display appropriate error to user
                MessageBox.Content = "This Patient currently has no notes.";
            }

            // Checks if there is only one note to view
            else if (NoteText.Count == 1)
            {
                // Display appropriate error to user
                MessageBox.Content = "Only one note exists for this patient.";
            }

            // Otherwise, 
            else
            {
                // Checks to ensure the counter would not reach the end of the list
                if (counter + 1 <= NoteText.Count - 1)
                {
                    // Reduces the counter by one
                    counter++;
                }
                else
                {
                    // Sends user to the first element in the note list
                    counter = 0;
                }

                // Sets the NoteWriter to the text and id at counter position
                NoteWriter.Text = NoteText[counter];
                DisplayIdTextbox.Text = NoteId[counter];
            }
        }

        // Submit button click response
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // Resets MessageBox text to empty
            MessageBox.Content = "";
            
            // Checks if NoteWriter is empty
            if (NoteWriter.Text == "")
            {
                // Display appropriate error to user
                MessageBox.Content = "Cannot make an empty Note.";
            }
            else
            {
                // Otherwise, Initiate result variable
                int result = 0;

                using (System.Data.SqlClient.SqlConnection con =
                new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    // Query creates SQL command for secondary query
                    SqlCommand Query = new SqlCommand(queryStringAdmissionID, con);
                    Query.Parameters.AddWithValue("@PATIENT_NO", Patient_No);

                    // Opens the Connection
                    con.Open();

                    // Executes the query and saves all returned data to SQLDataReader variable
                    SqlDataReader reader = Query.ExecuteReader();
                    try
                    {
                        // Reads from retrieved data one time
                        reader.Read();

                        // Saves first returned value from list
                        AdmissionId = (reader["ADMISSION_ID"].ToString());

                        // Closes the Connection
                        con.Close();
                        try
                        {
                            // Opens the Connection
                            con.Open();

                            // Query creates SQL command for Insert query
                            SqlCommand Query2 = new SqlCommand(queryStringInsert, con);

                            // Adds parametered values to query
                            Query2.Parameters.AddWithValue("@ADMISSION_ID", AdmissionId);
                            Query2.Parameters.AddWithValue("@NOTE_TEXT", NoteWriter.Text);

                            // Executes the query, result will be 1 if successful
                            result = Query2.ExecuteNonQuery();

                            // Returns default visibility of controls
                            PreviousButton.Visibility = Visibility.Visible;
                            NextButton.Visibility = Visibility.Visible;
                            CreateButton.Visibility = Visibility.Visible;

                            CancelButton.Visibility = Visibility.Hidden;
                            SubmitButton.Visibility = Visibility.Hidden;

                            // Resets counter to 0
                            counter = 0;

                            // Displays all information, including the just added Note
                            GetData();

                            // Prevents user from overwriting the NoteWriter Text
                            NoteWriter.IsReadOnly = true;

                            // Displays appropriate message to user
                            MessageBox.Content = "Note added to patient file.";
                        }
                        catch
                        {
                            // Check Error
                            if (result < 0)
                            {
                                // Display appropriate error to user
                                MessageBox.Content = "Error inserting data into Database!";
                            }
                        }
                    }
                    catch
                    {
                        // Should not be possible, but it would be good to have in case of critical error
                        MessageBox.Content = "Patient has no Admissions.";
                    }
                    finally
                    {
                        // Closes the Connection
                        con.Close();
                    }
                }
            }
        }

        // Cancel button click response
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Content = "Note creation cancelled.";
            NoteWriter.IsReadOnly = true;
            if (NoteText.Count != 0)
            {
                // Sets NoteWriter and DisplayID text to related text using counter variable
                NoteWriter.Text = NoteText[counter];
                DisplayIdTextbox.Text = NoteId[counter];
            }
            else
            {
                // Resets MessageBox text to empty
                NoteWriter.Text = "";
                DisplayIdTextbox.Text = "";
            }

            // Returns default visibility of controls
            PreviousButton.Visibility = Visibility.Visible;
            NextButton.Visibility = Visibility.Visible;
            CreateButton.Visibility = Visibility.Visible;

            CancelButton.Visibility = Visibility.Hidden;
            SubmitButton.Visibility = Visibility.Hidden;
        }

        // Create button click response
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            // Resets MessageBox text to empty
            MessageBox.Content = "";
            DisplayIdTextbox.Text = "";
            NoteWriter.Text = "";

            // sets visibility of controls for Create Display
            PreviousButton.Visibility = Visibility.Hidden;
            NextButton.Visibility = Visibility.Hidden;
            CreateButton.Visibility = Visibility.Hidden;

            CancelButton.Visibility = Visibility.Visible;
            SubmitButton.Visibility = Visibility.Visible;

            // Allows the user to write a note in the NoteWriter area
            NoteWriter.IsReadOnly = false;
        }

        // Exit button click response
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Closes the Window
            this.Close();
        }

        // Window Closing response
        private void NoteViewerWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Creates and opens a new Admissions tab with relevant data
            AdmissionsWindow Admissions = new AdmissionsWindow(Physician_No, Physician_ID_Index, Patient_No);
            Admissions.Show();
        }

        // TextChanged event response, checks the current number of characters to ensure it is less than or equal to database maximum
        private void CheckLength_Event(object sender, TextChangedEventArgs e)
        {
            if(NoteWriter.Text.Length == 750)
            {
                MessageBox.Content = "Maximum Note length reached.";
            }
        }
    }
}

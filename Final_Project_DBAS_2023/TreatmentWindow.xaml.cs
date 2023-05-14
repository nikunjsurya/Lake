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
    /// Interaction logic for TreatmentWindow.xaml
    /// </summary>
    public partial class TreatmentWindow : Window
    {
        // Holds the totals for each payment method to display for each Cost Center
        private int Selected_Treatment_Index = 0;
        private string Physician_No;
        private int Physician_ID_Index;
        private int Patient_No;

        // Holds a value that says whether the user is in the create treatment view
        bool createBool = false;

        // Holds the payment method types, this would be able to be changed in the future, but otherwise works as intended
        // This COULD be completed programatically storing it in the database, but this works as is
        private List<string> PaymentMethods = new List<string>() { "Assure", "SELF", "ESI", "OTHER" };

        // Holds the data retrieved from the Main query
        private List<string> ItemDesc = new List<string>();
        private List<string> BilledItemCost = new List<string>();
        private List<string> DateCharged = new List<string>();
        private List<string> TreatmentID = new List<string>();
        private List<string> TreatmentDate= new List<string>();
        private List<string> TreatmentResults = new List<string>();
        private List<string> TreatmentNotes = new List<string>();
        private List<string> BillingPaymentMethod = new List<string>();

        // Holds the data retrieved from the GetItems query
        private List<string> AllItemID = new List<string>();
        private List<string> AllItemDesc = new List<string>();
        private List<string> AllItemCost = new List<string>();

        // Holds the ID for the most recent Billing instance
        private int BillingID;

        // Holds the currently Selected Date, by default this is the current date
        DateTime SelectedDate = DateTime.Now;

        // Connection String
        private string connectionString = MainWindow.connectionString;

        // Query that collects all information on all of patients previous and upcoming treatments
        private string queryStringMain = "SELECT ITEM.ITEM_DESC, BILLED_ITEM.BILLED_ITEM_COST, " +
            "BILLED_ITEM.DATE_CHARGED, TREATMENT.TREATMENT_ID, TREATMENT.TREATMENT_DATE, BILLED_ITEM.PAYMENT_METHOD, " +
            "TREATMENT.TREATMENT_RESULTS, TREATMENT.TREATMENT_NOTES " +
            "FROM PATIENT, ADMISSION, BILLING, BILLED_ITEM, ITEM, TREATMENT " +
            "WHERE PATIENT.PATIENT_NO = @PATIENT_NO AND ADMISSION.PATIENT_NO = PATIENT.PATIENT_NO " +
            "AND BILLING.ADMISSION_ID = ADMISSION.ADMISSION_ID " +
            "AND BILLED_ITEM.BILLING_ID = BILLING.BILLING_ID " +
            "AND BILLED_ITEM.ITEM_ID = ITEM.ITEM_ID " +
            "AND TREATMENT.BILLED_ITEM_ID = BILLED_ITEM.BILLED_ITEM_ID  " +
            "ORDER BY DATE_CHARGED DESC";

        // Query that collects all items that are associated with administerable treatments
        // Items under COST CENTER codes 100 and 115 are daily costs associated with location, not administerable treatments, and thus are excluded
        private string queryStringGetItems = "SELECT ITEM_ID, ITEM_DESC, ITEM_COST FROM ITEM " +
                                         "WHERE COST_CENTER_ID NOT LIKE '100' AND COST_CENTER_ID NOT LIKE '115'";

        // Query that collects all billings associated with a given patient
        private string queryStringGetBilling = "SELECT BILLING_ID from PATIENT " +
            "INNER JOIN ADMISSION ON PATIENT.PATIENT_NO = ADMISSION.PATIENT_NO " +
            "INNER JOIN BILLING ON ADMISSION.ADMISSION_ID = BILLING.ADMISSION_ID " +
            "WHERE PATIENT.PATIENT_NO = @PATIENT_NO " +
            "ORDER BY BILLING.BILLING_ID DESC";

        // Query that collects the count of all billed items
        private string queryStringCountBilledItems = "SELECT COUNT(*) FROM BILLED_ITEM";

        // Query that inserts a billed item with given information
        private string queryStringInsertBilledItem = "INSERT INTO BILLED_ITEM (BILLED_ITEM_ID, BILLING_ID, " +
            "ITEM_ID, DATE_CHARGED, BILLED_ITEM_COST, PAYMENT_METHOD) " +
            "VALUES (NEXT VALUE FOR BILLED_ITEM_SEQ, @BILLING_ID, " +
            "@ITEM_ID, @DATE_CHARGED, @BILLED_ITEM_COST, @PAYMENT_METHOD);";

        // Query that inserts a treatment with given information
        private string queryStringInsertTreatment = "INSERT INTO TREATMENT (TREATMENT_ID, BILLED_ITEM_ID, " +
            "TREATMENT_DATE, TREATMENT_NOTES) VALUES (NEXT VALUE FOR TREATMENT_SEQ, " +
            "@BILLED_ITEM_ID, @TREATMENT_DATE, @TREATMENT_NOTES);";

        // Query that updates an existing treatment
        private string queryStringUpdate = "UPDATE TREATMENT SET " +
            "TREATMENT_RESULTS = @TREATMENT_RESULT, TREATMENT_NOTES = @TREATMENT_NOTE " +
            "WHERE TREATMENT_ID = @TREATMENT_ID";

        // Initialization of window without being passed any data
        // Should not be used by the system as-is
        public TreatmentWindow()
        {
            InitializeComponent();
        }

        // Initialization of window with passed data to use in finding what patient to search treatments of, and send back to Admission's Window
        public TreatmentWindow(string Passed_Physician_No, int Passed_Physician_ID_Index, int Passed_Patient_No)
        {
            // Saves passed data to variables
            Physician_No = Passed_Physician_No;
            Physician_ID_Index = Passed_Physician_ID_Index;
            Patient_No = Passed_Patient_No;

            InitializeComponent();
        }

        // Once all Components are loaded, loads the data into them
        private void FinishedLoading_Event(object sender, EventArgs e)
        {
            DisplayDateTextbox.Text = DateTime.Now.Date.ToString();

            using (System.Data.SqlClient.SqlConnection con =
            new System.Data.SqlClient.SqlConnection(connectionString))
            {
                // Query creates SQL command for GetBilling query
                SqlCommand Query = new SqlCommand(queryStringGetBilling, con);
                Query.Parameters.AddWithValue("@PATIENT_NO", Patient_No);

                // Opens the Connection
                con.Open();

                // Executes the query and saves all returned data to SQLDataReader variable
                SqlDataReader reader = Query.ExecuteReader();
                try
                {
                    // Reads from retrieved data one time
                    reader.Read();

                    // Collects the first retrieved Billing_ID 
                    BillingID = int.Parse(reader["BILLING_ID"].ToString());

                    // Closes the Connection
                    con.Close();
                }
                catch
                {
                    // Displays error to user. This should not appear but is good practice to report
                    MessageBox.Content = "No billing associated, contact Admin.";
                }
                // Updates datepicker to current date
                TreatmentDatePicker.SelectedDate = SelectedDate;
            }
            // Inserts all values of Item Descriptions into the ComboBox
            LoadComboBox();

            // Updates all components to display relevant data
            DisplayTreatmentData();
        }

        // Updates all components to display relevant data
        private void DisplayTreatmentData()
        {
            // Clears all data lists to be refilled in this function
            ItemDesc.Clear();
            BilledItemCost.Clear();
            DateCharged.Clear();
            TreatmentID.Clear();
            TreatmentDate.Clear();
            TreatmentResults.Clear();
            TreatmentNotes.Clear();
            BillingPaymentMethod.Clear();

            // Creates the DataTable
            DataTable TreatmentTable = new DataTable();

            // Create Connection
            using (System.Data.SqlClient.SqlConnection con =
            new System.Data.SqlClient.SqlConnection(connectionString))
            {
                // Query creates SQL command for Main query
                SqlCommand Query = new SqlCommand(queryStringMain, con);
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
                        // Saves all retrieved data into the associated Lists
                        ItemDesc.Add(reader["ITEM_DESC"].ToString());
                        BilledItemCost.Add(reader["BILLED_ITEM_COST"].ToString());
                        DateCharged.Add(DateTime.Parse(reader["DATE_CHARGED"].ToString()).ToString("yyyy-MM-dd"));
                        TreatmentID.Add(reader["TREATMENT_ID"].ToString());
                        TreatmentDate.Add(DateTime.Parse(reader["TREATMENT_DATE"].ToString()).ToString("yyyy-MM-dd"));
                        TreatmentResults.Add(reader["TREATMENT_RESULTS"].ToString());
                        TreatmentNotes.Add(reader["TREATMENT_NOTES"].ToString());
                        BillingPaymentMethod.Add(reader["PAYMENT_METHOD"].ToString());
                    }
                    // If list is not empty, make associated columns
                    if (ItemDesc != null)
                    {
                        TreatmentTable.Columns.Add("Treatment Description");
                        TreatmentTable.Columns.Add("Cost");
                        TreatmentTable.Columns.Add("Date Charged");
                        TreatmentTable.Columns.Add("Treatment Date");
                        TreatmentTable.Columns.Add("Payment Method");

                        // For each item in list, add a row with each piece of data
                        for (int i = 0; i <= ItemDesc.Count; i++)
                        {
                            TreatmentTable.Rows.Add(new object[] { ItemDesc.ElementAt(i), BilledItemCost.ElementAt(i), 
                                DateCharged.ElementAt(i), TreatmentDate.ElementAt(i), BillingPaymentMethod.ElementAt(i) });
                        }
                    }
                }
                catch
                {
                    // If there were no items found, tell user the patient has no treatments
                    MessageBox.Content = "Patient has no treatments on file";
                }
                finally
                {
                    // Update DataTable
                    TreatmentDataGrid.ItemsSource = TreatmentTable.DefaultView;

                    // Closes the Connection
                    con.Close();
                }
            }
        }

        // Displays and hides controls based selected treatment to display more information on treatment
        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Updates visibility to represent different view
                DetailsButton.Visibility = Visibility.Hidden;
                CreateButton.Visibility = Visibility.Hidden;
                TreatmentDataGrid.Visibility = Visibility.Hidden;

                ResultLabel.Visibility = Visibility.Visible;
                ResultStackPanel.Visibility = Visibility.Visible;
                NoteLabel.Visibility = Visibility.Visible;
                NoteStackPanel.Visibility = Visibility.Visible;

                BackButton.Visibility = Visibility.Visible;
                SubmitButton.Visibility = Visibility.Visible;

                // Displays collected data to user on selected treatment
                ResultWriter.Text = TreatmentResults.ElementAt(Selected_Treatment_Index);
                NoteWriter.Text = TreatmentNotes.ElementAt(Selected_Treatment_Index);
            }
            catch
            {
                // If there was no selected treatment, tell user to select one
                MessageBox.Content = "You must select a Treatment from the list.";
            }
        }

        // Back button click response
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Updates visibility to represent different view
            DetailsButton.Visibility = Visibility.Visible;
            CreateButton.Visibility = Visibility.Visible;
            TreatmentDataGrid.Visibility = Visibility.Visible;

            ResultLabel.Visibility = Visibility.Hidden;
            ResultStackPanel.Visibility = Visibility.Hidden;
            NoteLabel.Visibility = Visibility.Hidden;
            NoteStackPanel.Visibility = Visibility.Hidden;
            BackButton.Visibility = Visibility.Hidden;
            SubmitButton.Visibility = Visibility.Hidden;
        }

        // Create button click response
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            // updates CreateBool to true, making Submit button complete a different task
            createBool = true;

            // Updates visibility to represent different view
            DetailsButton.Visibility = Visibility.Hidden;
            CreateButton.Visibility = Visibility.Hidden;
            TreatmentDataGrid.Visibility = Visibility.Hidden;

            NoteLabel.Visibility = Visibility.Visible;
            NoteStackPanel.Visibility = Visibility.Visible;
            PaymentLabel.Visibility = Visibility.Visible;
            PaymentComboBox.Visibility = Visibility.Visible;
            TreatmentLabel.Visibility = Visibility.Visible;
            TreatmentComboBox.Visibility = Visibility.Visible;
            DatePickerLabel.Visibility = Visibility.Visible;
            TreatmentDatePicker.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Visible;
            SubmitButton.Visibility = Visibility.Visible;

            // Updates Writer TextBox's to be empty
            ResultWriter.Text = "";
            NoteWriter.Text = "";

        }

        // Submit button click response
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // Will change based on returned values from insert queries
            int result = 0;

            // If we are in the Create Treatment View, do the following
            if (createBool)
            {

                using (System.Data.SqlClient.SqlConnection con =
                new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    try
                    {
                        // Holds the index for the selected item in the ComboBox
                        int ItemIndex = TreatmentComboBox.SelectedIndex;

                        // Query creates SQL command for InsertBilledItem query
                        SqlCommand Query = new SqlCommand(queryStringInsertBilledItem, con);

                        Query.Parameters.AddWithValue("@BILLING_ID", BillingID);
                        Query.Parameters.AddWithValue("@ITEM_ID", AllItemID.ElementAt(ItemIndex));
                        Query.Parameters.AddWithValue("@DATE_CHARGED", DateTime.Now.ToString("yyyy-MM-dd"));
                        Query.Parameters.AddWithValue("@BILLED_ITEM_COST", AllItemCost.ElementAt(ItemIndex));
                        Query.Parameters.AddWithValue("@PAYMENT_METHOD", PaymentComboBox.SelectedValue.ToString());

                        // Opens the Connection
                        con.Open();

                        // Executes the Query
                        result = Query.ExecuteNonQuery();

                        // Closes the Connection
                        con.Close();
                        try
                        {
                            // Query creates SQL command for CountBilledItems query
                            Query = new SqlCommand(queryStringCountBilledItems, con);

                            // Opens the Connection
                            con.Open();

                            int BilledItemCount = int.Parse(Query.ExecuteScalar().ToString());

                            // Closes the Connection
                            con.Close();
                            try
                            {
                                // Query creates SQL command for InsertTreatment query
                                Query = new SqlCommand(queryStringInsertTreatment, con);

                                Query.Parameters.AddWithValue("@BILLED_ITEM_ID", BilledItemCount);
                                Query.Parameters.AddWithValue("@TREATMENT_DATE", TreatmentDatePicker.SelectedDate.Value.Date.ToString("yyyy-MM-dd"));
                                Query.Parameters.AddWithValue("@TREATMENT_NOTES", NoteWriter.Text);

                                // Opens the Connection
                                con.Open();

                                // Executes the query
                                result = Query.ExecuteNonQuery();

                                // Closes the Connection
                                con.Close();

                                // Resets creatBool to default value to show it is not longer creating treatment items
                                createBool = false;

                                // Resets control to default visibility
                                DetailsButton.Visibility = Visibility.Visible;
                                CreateButton.Visibility = Visibility.Visible;
                                TreatmentDataGrid.Visibility = Visibility.Visible;

                                NoteStackPanel.Visibility = Visibility.Hidden;
                                CancelButton.Visibility = Visibility.Hidden;
                                SubmitButton.Visibility = Visibility.Hidden;
                                PaymentLabel.Visibility = Visibility.Hidden;
                                PaymentComboBox.Visibility = Visibility.Hidden;
                                TreatmentLabel.Visibility = Visibility.Hidden;
                                TreatmentComboBox.Visibility = Visibility.Hidden;
                                DatePickerLabel.Visibility = Visibility.Hidden;
                                TreatmentDatePicker.Visibility = Visibility.Hidden;

                                // Runs DisplayTreatmentData function
                                DisplayTreatmentData();
                            }
                            catch
                            {
                                // Displays relevant error to user
                                MessageBox.Content = "Error inserting data into Database! (Level 3)";
                                
                            }
                        }
                        catch
                        {
                            // Displays relevant error to user
                            MessageBox.Content = "Error inserting data into Database! (Level 2)";
                        }
                    }
                    catch
                    {
                        // Check Error
                        if (result < 0)
                        {
                            // Displays relevant error to user
                            MessageBox.Content = "Error inserting data into Database! (Level 1)";
                        }
                    }
                }
            }

            // Otherwise, do the following
            else
            {
                // Resets MessageBox text to empty
                MessageBox.Content = "";

                using (System.Data.SqlClient.SqlConnection con =
                new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    try
                    {
                        // Query creates SQL command for Update query
                        SqlCommand Query = new SqlCommand(queryStringUpdate, con);
                        Query.Parameters.AddWithValue("@TREATMENT_ID", TreatmentID.ElementAt(Selected_Treatment_Index));
                        Query.Parameters.AddWithValue("@TREATMENT_RESULT", ResultWriter.Text);
                        Query.Parameters.AddWithValue("@TREATMENT_NOTE", NoteWriter.Text);

                        // Opens the Connection
                        con.Open();

                        // Executes the Query
                        result = Query.ExecuteNonQuery();

                        // Resets to control visibility to defaults
                        DetailsButton.Visibility = Visibility.Visible;
                        CreateButton.Visibility = Visibility.Visible;
                        TreatmentDataGrid.Visibility = Visibility.Visible;

                        ResultLabel.Visibility = Visibility.Hidden;
                        ResultStackPanel.Visibility = Visibility.Hidden;
                        NoteLabel.Visibility = Visibility.Hidden;
                        NoteStackPanel.Visibility = Visibility.Hidden;
                        BackButton.Visibility = Visibility.Hidden;
                        CancelButton.Visibility = Visibility.Hidden;
                        SubmitButton.Visibility = Visibility.Hidden;

                        // Runs DisplayTreatmentData function
                        DisplayTreatmentData();

                        // Displays message to user to inform them the Update is complete
                        MessageBox.Content = "Treatment Data Updated.";
                    }
                    catch
                    {
                        // Check Error
                        if (result < 0)
                        {
                            // Displays relevant error to user
                            MessageBox.Content = "Error updating data into Database!";
                        }
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
            // Resets creatBool to default value to show it is not longer creating treatment items
            createBool = false;

            // Resets to control visibility to defaults
            DetailsButton.Visibility = Visibility.Visible;
            CreateButton.Visibility = Visibility.Visible;
            TreatmentDataGrid.Visibility = Visibility.Visible;

            ResultLabel.Visibility = Visibility.Hidden;
            ResultStackPanel.Visibility = Visibility.Hidden;
            NoteLabel.Visibility = Visibility.Hidden;
            NoteStackPanel.Visibility = Visibility.Hidden;

            PaymentLabel.Visibility = Visibility.Hidden;
            PaymentComboBox.Visibility = Visibility.Hidden;
            TreatmentLabel.Visibility = Visibility.Hidden;
            TreatmentComboBox.Visibility = Visibility.Hidden;
            DatePickerLabel.Visibility = Visibility.Hidden;
            TreatmentDatePicker.Visibility = Visibility.Hidden;

            CancelButton.Visibility = Visibility.Hidden;
            SubmitButton.Visibility = Visibility.Hidden;
        }

        // DatePicker Date changed response
        private void TreatmentDatePicker_Changed(object sender, SelectionChangedEventArgs e)
        {
            if(TreatmentDatePicker.SelectedDate.Value.Date < DateTime.Now.Date)
            {
                MessageBox.Content = "Cannot schedule a treatment before today";
                TreatmentDatePicker.SelectedDate = DateTime.Now;
            }
            else
            {
                try
                {
                    SelectedDate = TreatmentDatePicker.SelectedDate.Value.Date;
                }
                catch
                {
                    MessageBox.Content = "Error changing selected date.";
                }
            }
        }

        // 
        private void NewSelectionMade(object sender, SelectionChangedEventArgs e)
        {
            int.TryParse(TreatmentDataGrid.SelectedIndex.ToString(), out int TreatmentDataGridIndex);
            Selected_Treatment_Index = TreatmentDataGridIndex;
            try
            {
                SelectedTreatmentTextbox.Text = TreatmentID.ElementAt(Selected_Treatment_Index);
            }
            catch
            {
                SelectedTreatmentTextbox.Text = "";
            }
        }

        private void LoadComboBox()
        {
            PaymentComboBox.ItemsSource = PaymentMethods;

            using (System.Data.SqlClient.SqlConnection con =
            new System.Data.SqlClient.SqlConnection(connectionString))
            {
                // Query creates SQL command for GetItems query
                SqlCommand Query = new SqlCommand(queryStringGetItems, con);

                // Opens the Connection
                con.Open();

                // Executes the query and saves all returned data to SQLDataReader variable
                SqlDataReader reader = Query.ExecuteReader();
                try
                {
                    // Loops through returned data until reaching the end
                    while (reader.Read())
                    {
                        AllItemID.Add(reader["ITEM_ID"].ToString());
                        AllItemDesc.Add(reader["ITEM_DESC"].ToString());
                        AllItemCost.Add(reader["ITEM_COST"].ToString());
                    }
                    TreatmentComboBox.ItemsSource = AllItemDesc;
                }
                catch
                {
                    MessageBox.Content = "Error loading Item list.";
                }
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

        private void CheckNoteLength_Event(object sender, TextChangedEventArgs e)
        {
            if (NoteWriter.Text.Length == 750)
            {
                MessageBox.Content = "Maximum Note length reached.";
            }
        }

        private void CheckResultLength_Event(object sender, TextChangedEventArgs e)
        {
            if (NoteWriter.Text.Length == 255)
            {
                MessageBox.Content = "Maximum Result length reached.";
            }
        }
    }
}
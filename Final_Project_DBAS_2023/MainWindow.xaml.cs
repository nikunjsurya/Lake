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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Final_Project_DBAS_2023
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        #region Variables
        // Connection String
        public static string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DBAS-Final-G8;Integrated Security=True";

        //private string queryStringAdmissionCount = "SELECT COUNT(*) FROM ADMISSION WHERE ADMISSION_ID >= @ADMISSION_ID";
        // Above could be made to search through list since last completed admission id, which would be useful for a database with Tens of Thousands of admissions
        private string queryStringAdmissionCount = "SELECT COUNT(*) FROM ADMISSION";

        // GetBedBillings Query String, gets a list of all bed billings for a given admission, only care about the most recent one however
        private string queryStringGetBedBillings = "SELECT ADMISSION.DATE_ADMITTED, ADMISSION.DISCHARGE_DATE, BILLED_ITEM.BILLING_ID, " +
                "BILLED_ITEM.DATE_CHARGED, BILLED_ITEM.ITEM_ID, PATIENT.FINANCIAL_STATUS, BILLED_ITEM.BILLED_ITEM_COST " +
                "FROM PATIENT INNER JOIN ADMISSION ON ADMISSION.PATIENT_NO = PATIENT.PATIENT_NO " +
                "INNER JOIN BILLING ON BILLING.ADMISSION_ID = ADMISSION.ADMISSION_ID " +
                "INNER JOIN BILLED_ITEM ON BILLED_ITEM.BILLING_ID = BILLING.BILLING_ID " +
                "WHERE ADMISSION.ADMISSION_ID = @ADMISSION_ID AND BILLED_ITEM.ITEM_ID >= 2000 " +
                "AND BILLED_ITEM.ITEM_ID < 2005 ORDER BY DATE_CHARGED DESC";

        // InsertBilledItems Query String, inserts a new billed item into database using given data, and the previously found BILLING_ID
        private string queryStringInsertBilledItems = "INSERT INTO BILLED_ITEM (BILLED_ITEM_ID, BILLING_ID, ITEM_ID, " +
            "DATE_CHARGED, BILLED_ITEM_COST, PAYMENT_METHOD) VALUES " +
            "(NEXT VALUE FOR BILLED_ITEM_SEQ, @BILLINGID, @ITEMID, @DATECHARGED, @ITEMCOST, @PAYMENTMETHOD);";

        private string DateAdmitted;
        private string DischargeDate;
        private string Billing_ID;
        private string Date_Charged;
        private string Item_ID;
        private string Item_Cost;
        private string Financial_Status;

        #endregion

        #region Initial Load Functions
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ContentLoaded_Event(object sender, EventArgs e)
        {
            using (System.Data.SqlClient.SqlConnection con =
            new System.Data.SqlClient.SqlConnection(connectionString))
            {
                try
                {
                    // Query creates SQL command for AdmissionCount query
                    SqlCommand Query = new SqlCommand(queryStringAdmissionCount, con);

                    // Opens the Connection
                    con.Open();

                    int AdmissionCount = int.Parse(Query.ExecuteScalar().ToString());
                    Trace.WriteLine(AdmissionCount);

                    // Closes the Connection
                    con.Close();

                    for (int i = 1; i <= AdmissionCount; i++)
                    {
                        DateTime currentDateVar;
                        DateTime dischargeDateVar;
                        TimeSpan difference;

                        // Query creates SQL command for GetBedBillings query
                        Query = new SqlCommand(queryStringGetBedBillings, con);
                        Query.Parameters.AddWithValue("@ADMISSION_ID", i);

                        // Opens the Connection
                        con.Open();

                        // Executes the query and saves all returned data to SQLDataReader variable
                        SqlDataReader reader = Query.ExecuteReader();

                        // Reads from retrieved data one time
                        reader.Read();

                        // Saves first returned set of data from query
                        DateAdmitted = reader["DATE_ADMITTED"].ToString();
                        DischargeDate = reader["DISCHARGE_DATE"].ToString();
                        Billing_ID = reader["BILLING_ID"].ToString();
                        Date_Charged = reader["DATE_CHARGED"].ToString();
                        Item_ID = reader["ITEM_ID"].ToString();
                        Item_Cost = reader["BILLED_ITEM_COST"].ToString();
                        Financial_Status = reader["FINANCIAL_STATUS"].ToString();

                        // Closes the Connection
                        con.Close();

                        // Gets the last charged date
                        currentDateVar = DateTime.Parse(Date_Charged);

                        try
                        {
                            // If there is a discharge date, set dischargeDateVar to it
                            dischargeDateVar = DateTime.Parse(DischargeDate);
                        }
                        catch
                        {
                            // Otherwise, set dischargeDateVar to current date
                            dischargeDateVar = DateTime.Now;
                        }

                        // Get the difference of the days
                        difference = dischargeDateVar - currentDateVar;

                        // Check there is one or more days between last charged date and dischargeDateVar date
                        if (difference.Days >= 1)
                        {
                            // For each days difference, insert another bed billing
                            for (int o = 0; o < difference.Days; o++)
                            {
                                currentDateVar = currentDateVar.AddDays(1);

                                // Query creates SQL command for InsertBilledItems query
                                Query = new SqlCommand(queryStringInsertBilledItems, con);

                                Query.Parameters.AddWithValue("@BILLINGID", Billing_ID);
                                Query.Parameters.AddWithValue("@ITEMID", Item_ID);
                                Query.Parameters.AddWithValue("@DATECHARGED", currentDateVar.Date.ToString("yyyy-MM-dd"));
                                Query.Parameters.AddWithValue("@ITEMCOST", Item_Cost);
                                Query.Parameters.AddWithValue("@PAYMENTMETHOD", Financial_Status);

                                // Opens the Connection
                                con.Open();

                                // Executes the insert query
                                int result = Query.ExecuteNonQuery();

                                // Closes the Connection
                                con.Close();
                            }
                        }
                        else
                        {
                            // Closes the Connection
                            con.Close();
                        }
                    }
                }
                catch
                {
                    // Display relevant error to user
                    MessageBox.Content = "Error found while updating bed billings";
                }
                finally
                {
                    // Closes the Connection
                    con.Close();
                }
            }
        }
        #endregion

        #region Patient Functions
        private void AddPatient_Click(object sender, RoutedEventArgs e)
        {
            DevErrorFunction();
        }

        private void AdmitPatient_Click(object sender, RoutedEventArgs e)
        {
            DevErrorFunction();
        }

        private void DisplayPatient_Click(object sender, RoutedEventArgs e)
        {
            // Resets MessageBox text to empty
            MessageBox.Content = "";
            PatientDisplayWindow patientDisplay = new PatientDisplayWindow();
            this.Hide();
            patientDisplay.Show();
        }

        private void BillPatient_Click(object sender, RoutedEventArgs e)
        {
            DevErrorFunction();
        }

        private void DischargePatient_Click(object sender, RoutedEventArgs e)
        {
            DevErrorFunction();
        }

        #endregion
        #region Room Function
        private void RoomUtilization_Click(object sender, RoutedEventArgs e)
        {
            // Resets MessageBox text to empty
            MessageBox.Content = "";
            RoomUtilizationWindow RoomWindow = new RoomUtilizationWindow();
            this.Hide();
            RoomWindow.Show();
        }
        #endregion

        #region Physician Functions
        private void PhysicianPatient_Click(object sender, RoutedEventArgs e)
        {
            // Resets MessageBox text to empty
            MessageBox.Content = "";
            AdmissionsWindow Admissions = new AdmissionsWindow();
            this.Hide();
            Admissions.Show();
        }

        private void AddPhysician_Click(object sender, RoutedEventArgs e)
        {
            DevErrorFunction();
        }
        #endregion

        #region Revenue Functions 
        private void RevenueAnalysis_Click(object sender, RoutedEventArgs e)
        {
            // Resets MessageBox text to empty
            MessageBox.Content = "";
            RevenueAnalysisWindow RevenueAnalysis = new RevenueAnalysisWindow();
            this.Hide();
            RevenueAnalysis.Show();
        }

        private void DailyRevenue_Click(object sender, RoutedEventArgs e)
        {
            // Resets MessageBox text to empty
            MessageBox.Content = "";
            DailyRevenueWindow DailyRevenue = new DailyRevenueWindow();
            this.Hide();
            DailyRevenue.Show();
        }
        #endregion

        #region Other Function
        //Dev Error Function
        private void DevErrorFunction()
        {
            MessageBox.Content = "Sorry, that function is currently in development.";
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}

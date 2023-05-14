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
    /// Interaction logic for DailyRevenueWindow.xaml
    /// </summary>
    public partial class DailyRevenueWindow : Window
    {
        private bool initialLoad = true;
        private double totalDues = 0;

        //private string NewLine = "&#x0a";
        private string CurrentPatientNo = "";
        private List<string> PatientNo = new List<string>();
        private List<string> PatientFullName = new List<string>();
        private List<string> PatientLocation = new List<string>();
        private List<string> CostCenterID = new List<string>();
        private List<string> ItemID = new List<string>();
        private List<string> ItemDesc = new List<string>();
        private List<string> BillingCost = new List<string>();
        private List<string> BillingPaymentMethod = new List<string>();

        // Connection String
        private string connectionString = MainWindow.connectionString;

        // Gets the revenue for each patient that has any costs that day
        private string queryStringGetDaysRevenue = "SELECT PATIENT.PATIENT_NO, PATIENT.PATIENT_FIRST_NAME, PATIENT.PATIENT_MIDDLE_NAME, "
            + "PATIENT.PATIENT_LAST_NAME, LOCATION.ROOM_NO, LOCATION.BED_DESIG, "
            + "COST_CENTER.COST_CENTER_ID, ITEM.ITEM_ID, ITEM.ITEM_DESC, "
            + "BILLED_ITEM.BILLED_ITEM_COST, BILLED_ITEM.PAYMENT_METHOD "
            + "FROM PATIENT INNER JOIN ADMISSION ON PATIENT.PATIENT_NO = ADMISSION.PATIENT_NO  "
            + "INNER JOIN LOCATION ON ADMISSION.LOCATION_ID = LOCATION.LOCATION_ID "
            + "INNER JOIN BILLING ON ADMISSION.ADMISSION_ID = BILLING.ADMISSION_ID "
            + "INNER JOIN BILLED_ITEM ON BILLING.BILLING_ID = BILLED_ITEM.BILLING_ID "
            + "INNER JOIN ITEM ON BILLED_ITEM.ITEM_ID = ITEM.ITEM_ID "
            + "INNER JOIN COST_CENTER ON ITEM.COST_CENTER_ID = COST_CENTER.COST_CENTER_ID "
            + "WHERE BILLED_ITEM.DATE_CHARGED = @DATE ORDER BY PATIENT_NO ASC";

        // Constructor
        public DailyRevenueWindow()
        {
            InitializeComponent();
        }

        // 
        private void ContentLoaded_Event(object sender, EventArgs e)
        {
            DisplayDailyRevenueReport();
        }

        private void DailyRevenueWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.MainWindow.Show();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DailyRevenueCalender_Changed(object sender, SelectionChangedEventArgs e)
        {
            DisplayDailyRevenueReport();
        }

        private void DisplayDailyRevenueReport()
        {

            DataTable dailyReportTable = new DataTable();
            dailyReportTable.Rows.Clear();
            DateTime date;
            if (initialLoad == false)
            {
                date = DailyRevenueCalender.SelectedDate.Value;
            }
            else
            {
                date = DateTime.Now;
                initialLoad = false;
            }

            if (date > DateTime.Now)
            {
                DailyRevenueCalender.SelectedDate = DateTime.Now;
                // Displays relevant error to user
                MessageBox.Content = "Cannot look ahead of current date";
            }
            else
            {
                // Create Connection
                using (System.Data.SqlClient.SqlConnection con =
                new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    // Query creates SQL command for Main query
                    SqlCommand Query = new SqlCommand(queryStringGetDaysRevenue, con);
                    Query.Parameters.AddWithValue("@DATE", date.ToString("yyyy-MM-dd"));
                    ReportHeaderLabel.Content = "Report for: " + date.ToShortDateString();

                    // Opens the Connection
                    con.Open();

                    // Executes the query and saves all returned data to SQLDataReader variable
                    SqlDataReader reader = Query.ExecuteReader();
                    try
                    {
                        // Loops through returned data until reaching the end
                        while (reader.Read())
                        {
                            // Saves the read data to relevant lists
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
                            CostCenterID.Add(reader["COST_CENTER_ID"].ToString());
                            ItemID.Add(reader["ITEM_ID"].ToString());
                            ItemDesc.Add(reader["ITEM_DESC"].ToString());
                            BillingCost.Add(reader["BILLED_ITEM_COST"].ToString());
                            BillingPaymentMethod.Add(reader["PAYMENT_METHOD"].ToString());
                        }

                        // Checks if there were no Patients found
                        if (PatientNo != null)
                        {
                            // Adds columns
                            dailyReportTable.Columns.Add("PATIENT-NO");
                            dailyReportTable.Columns.Add("PATIENT-NAME");
                            dailyReportTable.Columns.Add("LOC");
                            dailyReportTable.Columns.Add("FIN SOURCE");
                            dailyReportTable.Columns.Add("COST-CENTER");
                            dailyReportTable.Columns.Add("ITEM-CODE");
                            dailyReportTable.Columns.Add("ITEM DESCRIPTION");
                            dailyReportTable.Columns.Add("CHARGE");
                            dailyReportTable.Columns.Add("TOTAL");

                            // Creates a formattedTotal variable
                            string formattedTotals = "";

                            try
                            {
                                // For each item in the PatientNo list, complete the following
                                for (int i = 0; i <= PatientNo.Count; i++)
                                {
                                    // Parses the collected string to a double value
                                    double CostDouble = double.Parse(BillingCost.ElementAt(i));

                                    // Makes a formatted string to ensure 2 decimal places are displayed
                                    string formattedCost = String.Format("{0:0.00}", CostDouble);
                                    // Adds cost of current item to the totals
                                    totalDues += CostDouble;
                                    // If the value in CurrentPatientNo is not the current, add a different formatted row that displays the Patient data and the BilledItem info
                                    if (CurrentPatientNo != PatientNo.ElementAt(i))
                                    {
                                        dailyReportTable.Rows.Add(new object[] { PatientNo.ElementAt(i), PatientFullName.ElementAt(i), PatientLocation.ElementAt(i), BillingPaymentMethod.ElementAt(i), CostCenterID.ElementAt(i), ItemID.ElementAt(i), ItemDesc.ElementAt(i), formattedCost, "" });
                                        CurrentPatientNo = PatientNo.ElementAt(i);
                                    }
                                    // Otherwise, it displays just the BilledItem info
                                    else
                                    {
                                        dailyReportTable.Rows.Add(new object[] { "", "", "", BillingPaymentMethod.ElementAt(i), CostCenterID.ElementAt(i), ItemID.ElementAt(i), ItemDesc.ElementAt(i), formattedCost, "" });
                                    }
                                    // Formats the calculated totals to ensure 2 decimal places are displayed
                                    formattedTotals = String.Format("{0:0.00}", totalDues);

                                    // If the next patient is is not the same as the current one, format and print out the totals line
                                    if (CurrentPatientNo != PatientNo.ElementAt(i + 1))
                                    {
                                        formattedTotals = String.Format("{0:0.00}", totalDues);
                                        dailyReportTable.Rows.Add(new object[] { "", "", "", "", "", "", "", "", formattedTotals });
                                        totalDues = 0;
                                    }
                                }
                            }
                            catch
                            { 
                                // Displays the last patients totals line
                                dailyReportTable.Rows.Add(new object[] { "", "", "", "", "", "", "", "", formattedTotals });
                                totalDues = 0;
                            }

                        }
                    }
                    catch
                    {
                        if (CurrentPatientNo == "")
                        {
                            // Displays relevant error to user
                            MessageBox.Content = "No Records on this day";
                        }
                    }
                    finally
                    {
                        ReportGrid.ItemsSource = dailyReportTable.DefaultView;
                        CurrentPatientNo = "";
                        PatientNo.Clear();
                        PatientFullName.Clear();
                        PatientLocation.Clear();
                        BillingPaymentMethod.Clear();
                        CostCenterID.Clear();
                        ItemID.Clear();
                        ItemDesc.Clear();
                        BillingCost.Clear();

                        // Closes the Connection
                        con.Close();
                    }
                }
            }
        }
        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            // Displays relevant error to user
            MessageBox.Content = "Sorry, that function is currently in development.";
        }
    }
}

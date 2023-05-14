using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for RevenueAnalysisWindow.xaml
    /// </summary>
    public partial class RevenueAnalysisWindow : Window
    {
        // Holds all retreived billed items information
        private List<string> BilledItemCost = new List<string>();
        private List<string> PaymentMethod = new List<string>();
        private List<string> CostCenterID = new List<string>();
        private List<string> CostCenterName = new List<string>();

        // Holds the payment method types, this would be able to be changed in the future, but otherwise works as intended
        // This COULD be completed programatically storing it in the database, but this works as is
        private List<string> PaymentTypeId = new List<string>() { "ASSURE", "ESI", "SELF", "OTHER" };

        // Made to find the date period for display and retrieval query
        private int DatePosition = 0;

        // Holds the end date and start date which by default is the current day and 6 days prior respectively
        // Doing this gives the user a 7 day (1 week) period
        private string StartDate = DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd");
        private string EndDate = DateTime.Now.ToString("yyyy-MM-dd");

        // Connection String
        private string connectionString = MainWindow.connectionString;

        // Query for retriving all required information for Revenue Analysis
        string queryStringMain = "SELECT BILLED_ITEM.BILLED_ITEM_COST, BILLED_ITEM.PAYMENT_METHOD, " +
            "COST_CENTER.COST_CENTER_ID, COST_CENTER.COST_CENTER_NAME FROM COST_CENTER " +
            "INNER JOIN ITEM ON ITEM.COST_CENTER_ID = COST_CENTER.COST_CENTER_ID " +
            "INNER JOIN BILLED_ITEM ON BILLED_ITEM.ITEM_ID = ITEM.ITEM_ID " +
            "WHERE BILLED_ITEM.DATE_CHARGED >=  @STARTDATE " +
            "AND BILLED_ITEM.DATE_CHARGED <= @ENDDATE " +
            "ORDER BY COST_CENTER.COST_CENTER_ID ASC, " +
            "BILLED_ITEM.BILLED_ITEM_ID ASC";

        public RevenueAnalysisWindow()
        {
            InitializeComponent();
        }

        // Once all Components are loaded, loads the data into them
        private void ContentLoaded_Event(object sender, EventArgs e)
        {
            LoadData();
        }

        // Loads data into different components
        private void LoadData()
        {
            // Resets MessageBox text to empty
            MessageBox.Content = "";
            // Sets the date range textbox's
            StartDateTextbox.Text = StartDate;
            EndDateTextbox.Text = EndDate;

            // Clears the lists for new selection
            BilledItemCost.Clear();
            PaymentMethod.Clear();
            CostCenterID.Clear();
            CostCenterName.Clear();

            // Connects the the database
            using (System.Data.SqlClient.SqlConnection con =
            new System.Data.SqlClient.SqlConnection(connectionString))
            {
                try
                {
                    // Query creates SQL command for Main query
                    SqlCommand Query = new SqlCommand(queryStringMain, con);

                    // Sets the query variables 
                    Query.Parameters.AddWithValue("@STARTDATE", StartDate);
                    Query.Parameters.AddWithValue("@ENDDATE", EndDate);

                    // Opens the Connection
                    con.Open();

                    // Executes the query and saves all returned data to SQLDataReader variable
                    SqlDataReader reader = Query.ExecuteReader();

                    // Loops through returned data until reaching the end
                    while (reader.Read())
                    {
                        BilledItemCost.Add(reader["BILLED_ITEM_COST"].ToString());
                        PaymentMethod.Add(reader["PAYMENT_METHOD"].ToString());
                        CostCenterID.Add(reader["COST_CENTER_ID"].ToString());
                        CostCenterName.Add(reader["COST_CENTER_NAME"].ToString());
                    }

                    // Closes the Connection
                    con.Close();
                }
                catch
                {
                    MessageBox.Content = "Error loading occupied locations.";
                }
            }
            // Creates a DataTable instance to display on DataGrid
            DataTable RevenueAnalysisTable = new DataTable();

            // Adds columns to the DataTable
            RevenueAnalysisTable.Columns.Add("COST-CENTER");
            RevenueAnalysisTable.Columns.Add("NAME");
            RevenueAnalysisTable.Columns.Add("NO-OF-TRANS");
            RevenueAnalysisTable.Columns.Add("TOTAL CHARGES");
            RevenueAnalysisTable.Columns.Add("ASSURE");
            RevenueAnalysisTable.Columns.Add("ESI");
            RevenueAnalysisTable.Columns.Add("SELF PAY");
            RevenueAnalysisTable.Columns.Add("OTHER");
            try
            {
                // Saves the first Cost Center ID to use as a 'counter'
                string currentCostCenter = CostCenterID.ElementAt(0);

                // Holds the totals for each payment method to display for each Cost Center
                double AssurePayments = 0;
                double ESIPayments = 0;
                double SelfPayments = 0;
                double OtherPayments = 0;

                // Holds the total charges of all above variables for each Cost Center
                double totalCharges = 0;

                // Creates total charges 
                int counter = 0;

                // Loops through BilledItems, adding up totals for each Cost center, and adds the 
                // row of data to the DataGrid once getting to the next CostCenterID, or reaching the end of the list
                for (int i = 0; i < BilledItemCost.Count; i++)
                {
                    // Checks if a new CostCenterID is found
                    if (currentCostCenter != CostCenterID.ElementAt(i) && counter != 0)
                    {
                        // Calculates totalCharges
                        totalCharges = AssurePayments + ESIPayments + SelfPayments + OtherPayments;

                        // Adds Row of data to DataTable
                        RevenueAnalysisTable.Rows.Add(new object[] { currentCostCenter, CostCenterName.ElementAt(i-1), counter.ToString(),
                        String.Format("{0:0.00}", totalCharges), String.Format("{0:0.00}", AssurePayments), String.Format("{0:0.00}", ESIPayments),
                        String.Format("{0:0.00}", SelfPayments), String.Format("{0:0.00}", OtherPayments) });

                        currentCostCenter = CostCenterID.ElementAt(i);

                        AssurePayments = 0;
                        ESIPayments = 0;
                        SelfPayments = 0;
                        OtherPayments = 0;

                        counter = 0;
                    }

                    counter++;

                    // Checks which PaymentType the current BilledItem is using
                    if (PaymentMethod.ElementAt(i) == PaymentTypeId[0])
                    {
                        AssurePayments += double.Parse(BilledItemCost.ElementAt(i));
                    }
                    if (PaymentMethod.ElementAt(i) == PaymentTypeId[1])
                    {
                        ESIPayments += double.Parse(BilledItemCost.ElementAt(i));
                    }
                    if (PaymentMethod.ElementAt(i) == PaymentTypeId[2])
                    {
                        SelfPayments += double.Parse(BilledItemCost.ElementAt(i));
                    }
                    if (PaymentMethod.ElementAt(i) == PaymentTypeId[3])
                    {
                        OtherPayments += double.Parse(BilledItemCost.ElementAt(i));
                    }
                }
                // Checks if there is any data
                if (counter != 0)
                {
                    // Calculates totalCharges
                    totalCharges = AssurePayments + ESIPayments + SelfPayments + OtherPayments;

                    // Adds Row of data to DataTable
                    RevenueAnalysisTable.Rows.Add(new object[] { currentCostCenter, CostCenterName.ElementAt(CostCenterName.Count - 1), counter.ToString(),
                String.Format("{0:0.00}", totalCharges), String.Format("{0:0.00}", AssurePayments), String.Format("{0:0.00}", ESIPayments),
                String.Format("{0:0.00}", SelfPayments), String.Format("{0:0.00}", OtherPayments) });
                }
            }
            catch
            {
                // Display relevant error to user
                MessageBox.Content = "There are no transactions for this week";
            }
            // Assign DataGrid source to the recently made DataTable
            RevenueAnalysisDataGrid.ItemsSource = RevenueAnalysisTable.DefaultView;
        }

        // Previous Button Response
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            // Increases the DatePosition
            DatePosition++;

            // Sets the Start and End date to the corresponding days based on DatePosition
            StartDate = DateTime.Now.AddDays(-1 * (6 + (7 * DatePosition))).ToString("yyyy-MM-dd");
            EndDate = DateTime.Now.AddDays(-1 * (7 * DatePosition)).ToString("yyyy-MM-dd");

            // Enable both NextButton and ReturnButton, as they would both be available
            NextButton.IsEnabled = true;
            ReturnButton.IsEnabled = true;
            
            // Calls the LoadData function
            LoadData();
        }

        // Next Button Response
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // Decreases the DatePosition
            DatePosition--;

            // Sets the Start and End date to the corresponding days based on DatePosition
            StartDate = DateTime.Now.AddDays(-1 * (6 + (7 * DatePosition))).ToString("yyyy-MM-dd");
            EndDate = DateTime.Now.AddDays(-1 * (7 * DatePosition)).ToString("yyyy-MM-dd");

            // Checks if the current DatePosition can go any further than the current day
            if (DatePosition == 0)
            {
                // Disables both NextButton and ReturnButton, as they would both be available
                NextButton.IsEnabled = false;
                ReturnButton.IsEnabled = false;
            }

            // Calls the LoadData function
            LoadData();
        }

        // Return Button Response
        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            // Resets the DatePosition to 0
            DatePosition = 0;

            // Sets the Start and End date to the corresponding days based on DatePosition
            StartDate = DateTime.Now.AddDays(-1 * (6 + (7 * DatePosition))).ToString("yyyy-MM-dd");
            EndDate = DateTime.Now.AddDays(-1 * (7 * DatePosition)).ToString("yyyy-MM-dd");

            // Disables both NextButton and ReturnButton, as they would both be available
            NextButton.IsEnabled = false;
            ReturnButton.IsEnabled = false;

            // Calls the LoadData function
            LoadData();
        }

        // Window Closing Event Response
        private void RevenueAnalysisWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Shows the main application
            Application.Current.MainWindow.Show();
        }

        // Exit Button Response
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Closes the Window
            this.Close();
        }
    }
}

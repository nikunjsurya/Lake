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
    /// Interaction logic for RoomUtilizationWindow.xaml
    /// </summary>
    public partial class RoomUtilizationWindow : Window
    {
        // Lists that hold data returned from the GetUsed query 
        private List<string> UsedLocationID = new List<string>();
        private List<string> UsedItemID = new List<string>();
        private List<string> PatientNo = new List<string>();
        private List<string> PatientName = new List<string>();
        private List<string> DateAdmitted = new List<string>();

        // Lists that hold data returned from the GetAllLoc query
        private List<string> AllLocationID = new List<string>();
        private List<string> AllRoomNo = new List<string>();
        private List<string> AllBedDesig = new List<string>();
        private List<string> AllItemID = new List<string>();
        
        // Arrays that hold data for Location Types, correlating between the Item Code, and the Type's short form
        // This could also be admitted to the database as a seperate record or column in the items field, but not necessary at this time
        private string[] LocationType = { "SP", "PR", "IC", "W3", "W4" };
        private string[] LocationTypeId = { "2000", "2001", "2002", "2003", "2004" };

        // Variables used to record all bed counts and all room counts
        private int RoomCount = 0;
        private int BedCount = 0;

        // Variables used to record used bed counts and used room counts
        private int UsedRoomCount = 0;
        private int UsedBedCount = 0;

        // Arrays used for counting the total type counts and used count types
        private int[] LocationTypeCount = { 0, 0, 0, 0, 0 };
        private int[] UsedLocationTypeCount = { 0, 0, 0, 0, 0 };

        // Connection String
        private string connectionString = MainWindow.connectionString;

        // Query that will return relevant data for locations that are in use
        private string queryStringGetUsed = "SELECT LOCATION.LOCATION_ID, PATIENT.PATIENT_NO, " +
            "LOCATION.ITEM_ID, PATIENT.PATIENT_FIRST_NAME, PATIENT.PATIENT_MIDDLE_NAME, " +
            "PATIENT.PATIENT_LAST_NAME, ADMISSION.DATE_ADMITTED " +
            "FROM PATIENT INNER JOIN ADMISSION ON ADMISSION.PATIENT_NO = PATIENT.PATIENT_NO " +
            "INNER JOIN LOCATION ON LOCATION.LOCATION_ID = ADMISSION.LOCATION_ID " +
            "WHERE LOCATION.LOC_AVAILABILITY = 1 AND ADMISSION.DISCHARGE_DATE IS NULL " +
            "ORDER BY LOCATION.ROOM_NO ASC, LOCATION.BED_DESIG ASC";

        // Query that will return relevant data for all locations, used or not
        private string queryStringGetAllLoc = "SELECT LOCATION_ID, ROOM_NO, " +
            "BED_DESIG, ITEM_ID FROM LOCATION " +
            "ORDER BY ROOM_NO ASC, BED_DESIG ASC";

        // Query that gets the number of beds that have discharging patients for the current day
        private string queryStringCountDischarges = "SELECT count(*) from ADMISSION where DISCHARGE_DATE = @DATE";

        public RoomUtilizationWindow()
        {
            InitializeComponent();
        }

        // Event is fired by window when all the components on the window are loaded
        private void ContentLoaded_Event(object sender, EventArgs e)
        {
            // Updates DisplayDateTextbox to current date
            DisplayDateTextbox.Text = DateTime.Now.Date.ToString("yyyy-MM-dd");

            // Creates a connection using connection string
            using (System.Data.SqlClient.SqlConnection con =
            new System.Data.SqlClient.SqlConnection(connectionString))
            {
                try
                {
                    // Query creates SQL command for GetUsed query
                    SqlCommand Query = new SqlCommand(queryStringGetUsed, con);

                    // Opens the Connection
                    con.Open();

                    // Executes the query and saves all returned data to SQLDataReader variable
                    SqlDataReader reader = Query.ExecuteReader();

                    // Loops through returned data until reaching the end
                    while (reader.Read())
                    {
                        // Gathers read data and adds to relevant lists
                        UsedLocationID.Add(reader["LOCATION_ID"].ToString());
                        PatientNo.Add(reader["PATIENT_NO"].ToString());
                        UsedItemID.Add(reader["ITEM_ID"].ToString());
                        DateAdmitted.Add(DateTime.Parse(reader["DATE_ADMITTED"].ToString()).ToString("yyyy-MM-dd"));

                        // reads the patients name, and saves them in local variables
                        string PatientLastName = reader["PATIENT_LAST_NAME"].ToString();
                        string PatientFirstName = reader["PATIENT_FIRST_NAME"].ToString();
                        string PatientMiddleName = reader["PATIENT_MIDDLE_NAME"].ToString();

                        // Formats the name based on if they have a middle name in the system
                        if (PatientMiddleName != "")
                        {
                            PatientName.Add(PatientLastName + ", " + PatientFirstName + " " + PatientMiddleName.Substring(0, 1) + ".");
                        }
                        else
                        {
                            PatientName.Add(PatientLastName + ", " + PatientFirstName);
                        }
                    }
                    // Closes the Connection
                    con.Close();

                }
                catch
                {
                    // Displays relevant error to user
                    MessageBox.Content = "Error loading occupied locations.";
                }
                try
                {
                    // Query creates SQL command for GetAllLoc query
                    SqlCommand Query = new SqlCommand(queryStringGetAllLoc, con);

                    // Opens the Connection
                    con.Open();

                    // Executes the query and saves all returned data to SQLDataReader variable
                    SqlDataReader reader = Query.ExecuteReader();

                    while (reader.Read())
                    {
                        AllLocationID.Add(reader["LOCATION_ID"].ToString());
                        AllRoomNo.Add(reader["ROOM_NO"].ToString());
                        AllBedDesig.Add(reader["BED_DESIG"].ToString());
                        AllItemID.Add(reader["ITEM_ID"].ToString());
                    }

                    // Closes the Connection
                    con.Close();
                }
                catch
                {
                    // Displays relevant error to user
                    MessageBox.Content = "Error loading locations.";
                }
                try
                {
                    // Query creates SQL command for CountDischarges query
                    SqlCommand Query = new SqlCommand(queryStringCountDischarges, con);

                    // Opens the Connection
                    con.Open();
                    Query.Parameters.AddWithValue("@DATE", DateTime.Now.Date.ToString("yyyy-MM-dd"));

                    int DischargeCount = int.Parse(Query.ExecuteScalar().ToString());
                    DischargingTextbox.Text = DischargeCount.ToString();

                    // Closes the Connection
                    con.Close();
                }
                catch
                {
                    // Displays relevant error to user
                    MessageBox.Content = "Error loading discharges.";
                }
                LoadData();
            }
        }

        private void LoadData()
        {
            DataTable RoomUtilizationTable = new DataTable();

            RoomUtilizationTable.Columns.Add("LOCATION");
            RoomUtilizationTable.Columns.Add("TYPE");
            RoomUtilizationTable.Columns.Add("PATIENT-NO");
            RoomUtilizationTable.Columns.Add("PATIENT-NAME");
            RoomUtilizationTable.Columns.Add("DATE-ADMITTED");

            int counter = 0;
            string currentRoom = "";
            string lastUsedRoom = "";

            for (int i = 0; i < AllLocationID.Count; i++)
            {
                bool newRoom = false;
                BedCount++;
                int TypeTracker = 4;

                if (currentRoom != AllRoomNo.ElementAt(i))
                {
                    newRoom = true;
                    currentRoom = AllRoomNo.ElementAt(i);
                    RoomCount++;
                }
                if (AllItemID.ElementAt(i) == LocationTypeId[0])
                {
                    TypeTracker = 0;
                }
                else if (AllItemID.ElementAt(i) == LocationTypeId[1])
                {
                    TypeTracker = 1;
                }
                else if (AllItemID.ElementAt(i) == LocationTypeId[2])
                {
                    TypeTracker = 2;
                }
                else if (AllItemID.ElementAt(i) == LocationTypeId[3])
                {
                    TypeTracker = 3;
                }

                if (newRoom)
                {
                    LocationTypeCount[TypeTracker]++;
                }

                if (counter < UsedLocationID.Count)
                {
                    if (UsedLocationID.ElementAt(counter) == AllLocationID.ElementAt(i))
                    {
                        RoomUtilizationTable.Rows.Add(new object[] { AllRoomNo.ElementAt(i) + AllBedDesig.ElementAt(i), LocationType[TypeTracker], PatientNo.ElementAt(counter), PatientName.ElementAt(counter), DateAdmitted.ElementAt(counter) });

                        UsedBedCount++;
                        counter++;

                        if (lastUsedRoom != AllRoomNo.ElementAt(i))
                        {
                            lastUsedRoom = AllRoomNo.ElementAt(i);
                            UsedLocationTypeCount[TypeTracker]++;
                            UsedRoomCount++;
                        }
                    }
                    else
                    {
                        RoomUtilizationTable.Rows.Add(new object[] { AllRoomNo.ElementAt(i) + AllBedDesig.ElementAt(i), LocationType[TypeTracker], "", "", "" });
                    }
                }
                else
                {
                    RoomUtilizationTable.Rows.Add(new object[] { AllRoomNo.ElementAt(i) + AllBedDesig.ElementAt(i), LocationType[TypeTracker], "", "", "" });
                }
            }
            RoomUtilizationGrid.ItemsSource = RoomUtilizationTable.DefaultView;

            // Updates all the related textboxes to display collected data
            UsedBedsTextbox.Text = UsedBedCount.ToString();
            TotalBedsTextbox.Text = BedCount.ToString();

            UsedRoomsTextbox.Text = UsedRoomCount.ToString();
            TotalRoomsTextbox.Text = RoomCount.ToString();

            TotalUsedBedsTextbox.Text = UsedBedCount.ToString();

            UsedLocationTypePRTextbox.Text = UsedLocationTypeCount[0].ToString();
            LocationTypePRTextbox.Text = LocationTypeCount[0].ToString();

            UsedLocationTypeSPTextbox.Text = UsedLocationTypeCount[1].ToString();
            LocationTypeSPTextbox.Text = LocationTypeCount[1].ToString();

            UsedLocationTypeICTextbox.Text = UsedLocationTypeCount[2].ToString();
            LocationTypeICTextbox.Text = LocationTypeCount[2].ToString();

            UsedLocationTypeW3Textbox.Text = UsedLocationTypeCount[3].ToString();
            LocationTypeW3Textbox.Text = LocationTypeCount[3].ToString();

            UsedLocationTypeW4Textbox.Text = UsedLocationTypeCount[4].ToString();
            LocationTypeW4Textbox.Text = LocationTypeCount[4].ToString();
        }

        private void RoomUtilizationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.MainWindow.Show();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

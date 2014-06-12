using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DMV_GUI
{
    public partial class Form1 : Form
    {

        MotorVehicle[] vehicleArray = new MotorVehicle[50]; //Define Array of MotorVehicle objects
        int mvArrayCounter = 0; //Initialize array counter  
        public static string textFile = "log-"+(DateTime.Now.ToString("dd-MM-yyyy"))+".txt"; //Define dynamic time-dependant name of textfile

        public Form1()
        {
            InitializeComponent();
        }

        private void onLoad(object sender, EventArgs e)
        {
            VehicleTypeChange(null, null); //Run function to check the default radio button

            if (!File.Exists(textFile)) //Check if our textfile already exists
            {
                FileStream fileStream = new FileStream(textFile, FileMode.Create, FileAccess.Write); //Create a new texfile
                fileStream.Close(); //Close the file, so that other methods can acces it
            }



            if (!Directory.Exists(@"C:\OldFiles\")) Directory.CreateDirectory(@"C:\OldFiles\");
            string[] txtFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.txt");
            foreach (string fname in txtFiles)
            {
                DateTime d = File.GetLastWriteTime(fname);
  
                if (d > DateTime.Now.AddHours(-24))
                {
                    FileStream file = new FileStream(fname, FileMode.Append, FileAccess.Write);
                    try
                    {
                        StreamWriter writer = new StreamWriter(file);
                        try
                        {
                            writer.WriteLine("------- Below is text created after " + DateTime.Now.ToString("dd-MM-yyyy") + " ----------------");
                            writer.WriteLine("This is a test line");
                            writer.Close();
                        }
                        catch (Exception exc)
                        { throw exc; }
                        finally
                        {
                            writer.Dispose();
                        }
                    }
                    catch (Exception exc) {
                        throw exc;
                    }
                    finally
                    {
                        file.Close();
                        file.Dispose();
                    }

                } else
                {
                     File.Move(fname, @"C:\OldFiles\"); 
                }

            }
        }

        private void VehicleTypeChange(object sender, EventArgs e) //Method for radio button selector. Displays required fileds for diferent types of motor Vehicles
        {
            if (rbTruck.Checked)
            {
                customLabel01.Visible = customTb01.Visible = true;
                customLabel02.Visible = customTb02.Visible = customLabel03.Visible = rbYes.Visible = rbNo.Visible = false;
                customLabel01.Text = "maximum weight";
            }
            else if (rbBus.Checked)
            {
                customLabel01.Visible = customTb01.Visible = true;
                customLabel02.Visible = customTb02.Visible = customLabel03.Visible = rbYes.Visible = rbNo.Visible = false;
                customLabel01.Text = "Company name";
            }
            else if (rbCar.Checked)
            {
                customLabel01.Visible = customTb01.Visible = customLabel02.Visible = customLabel03.Visible = customTb02.Visible = rbYes.Visible = rbNo.Visible = true;
                customLabel01.Text = "Car Color";
                customLabel02.Text = "Number of airbags";
                customLabel03.Text = "Does the car have AC?";
            }
            else if (rbTaxi.Checked)
            {
                customLabel01.Visible = customTb01.Visible = customLabel02.Visible = customLabel03.Visible = customTb02.Visible = rbYes.Visible = rbNo.Visible = customLabel04.Visible = rbYes2.Visible = rbNo2.Visible = true;
                customLabel01.Text = "Car Color";
                customLabel02.Text = "Number of airbags";
                customLabel03.Text = "Cab has AC?";
                customLabel04.Text = "Driver has licence?";
            }
        }

        private void RegisterVehicleClick(object sender, EventArgs e) //Button Click method. Creates objects, puts them in our array, then displays them in log as well as stores them in out textfile
        {
            MotorVehicle mv = null;
            if (rbTruck.Checked)
            {
                mv = new Truck(tbVIN.Text, tbMake.Text, tbModel.Text, (int)NoOfWheels.Value, (int)NoOfSeats.Value, datePicker.Value, Convert.ToDouble(customTb01.Text));
            }
            else if (rbBus.Checked)
            {
                mv = new Bus(tbVIN.Text, tbMake.Text, tbModel.Text, (int)NoOfWheels.Value, (int)NoOfSeats.Value, datePicker.Value, customTb01.Text);                
            }
            else if (rbCar.Checked)
            {
                mv = new Car(tbVIN.Text, tbMake.Text, tbModel.Text, (int)NoOfWheels.Value, (int)NoOfSeats.Value, datePicker.Value, customTb01.Text, rbYes.Checked, Convert.ToInt32(customTb02.Text));                
            }
            else if (rbTaxi.Checked)
            {
                mv = new Taxi(tbVIN.Text, tbMake.Text, tbModel.Text, (int)NoOfWheels.Value, (int)NoOfSeats.Value, datePicker.Value, customTb01.Text, rbYes.Checked, Convert.ToInt32(customTb02.Text), rbYes2.Checked);
            }

            vehicleArray[mvArrayCounter++] = mv; //Append newest object to array
            
            rtLog.Clear();

           

            foreach(MotorVehicle m in vehicleArray) //Display and store in textfile
            {
                if (m != null)
                {
                    rtLog.AppendText(m.show() + "\n\n");
                    FileStream file = new FileStream(textFile, FileMode.Append, FileAccess.Write);
                    try
                    {
                        StreamWriter writer = new StreamWriter(file);
                        try 
                        {
                            writer.WriteLine(m.show());
                            writer.Close();
                        }
                        catch (Exception exc)
                        { 
                            throw exc;
                        }
                        finally
                        {
                            writer.Dispose();
                        }
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                    finally
                    {
                        file.Close();  
                        file.Dispose();
                    }


                    
                }
            }
        }
        
        private void ShowLastVehicleFromFile(object sender, EventArgs e) //Get from textfile and display in Richtextbox
        {
            using (FileStream fileStream = new FileStream(textFile, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    string allFileLines;
                    string[] currentLine = new String[50];
                    int lineCounter = 0;

                    while((allFileLines = streamReader.ReadLine()) != null) {
                        try
                        {
                            currentLine[lineCounter++] = allFileLines;
                        }
                        catch
                        {
                            break;
                        }
                    }

                    rtLog.AppendText(lineCounter.ToString() + ": " + currentLine[lineCounter-1] + "\n\n");
                    streamReader.Close();
                }
                fileStream.Close(); 
            }        
        }

        private void vehicleTypeGroupBox_Enter(object sender, EventArgs e)
        {

        }

        private void tbVIN_TextChanged(object sender, EventArgs e)
        {

        }        
    }
}

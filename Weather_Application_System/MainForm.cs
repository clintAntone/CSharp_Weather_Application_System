using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net;
using static Weather_Application_System.WeatherForecast;

namespace Weather_Application_System
{
    public partial class MainForm : Form
    {
        string APIKEY = "345f7b0c9af779cf03ff3daa414147f4";
        double latitude;
        double longitude;

        public MainForm()
        {
            InitializeComponent();
        }

        DateTime convertDatetime(long sec)
        {
            DateTime day = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).ToLocalTime();
            day = day.AddSeconds(sec).ToLocalTime();

            return day;
        }

        void getWeather()
        {
            using (WebClient web = new WebClient())
            {
                string url = string.Format("https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}", txtCityName.Text, APIKEY);
                Console.WriteLine(url);
                WeatherInfo.root Info;
                try
                {
                    var json = web.DownloadString(url);
                    Info = JsonConvert.DeserializeObject<WeatherInfo.root>(json);
                }
                catch(Exception e)
                {
                    MessageBox.Show("Error:" + e.Message.ToString(), "Invalid City Name",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }

                lblCondition.Visible = true;
                lblCityName.Visible = true;

                picWeather.ImageLocation = "https://openweathermap.org/img/w/" + Info.weather[0].icon + ".png";
                lblCondition.Text = Info.weather[0].main;
                lblDetails.Text = String.Concat("''" + Info.weather[0].description + "''");
                lblSunrise.Text = convertDatetime(Info.sys.sunrise).ToShortTimeString();
                lblSunset.Text = convertDatetime(Info.sys.sunset).ToShortTimeString();
                lblCountry.Text = Info.sys.country.ToString();
                
                double temp;
                temp = Info.main.temp - 273.15;
                lblTemp.Text = String.Concat(temp.ToString() + " celsius");

                lblWindspeed.Text = Info.wind.speed.ToString();
                lblPressure.Text = Info.main.pressure.ToString();
                lblHumidity.Text = Info.main.humidity.ToString();

                lblCityName.Text = String.Format("City of " + Info.name + ", " + Info.sys.country.ToString());

                latitude = Info.coord.lat;
                longitude = Info.coord.lon;

                getForecast();

            }
        }

        void getForecast()
        {
            using (WebClient web = new WebClient())
            {
                string url = string.Format("https://api.openweathermap.org/data/2.5/onecall?lat={0}&lon={1}&exclude=current,minutely,hourly,alerts&appid={2}", latitude, longitude, APIKEY);
                var json = web.DownloadString(url);
                WeatherForecast.ForecastInfo ForecastInfo = JsonConvert.DeserializeObject<WeatherForecast.ForecastInfo>(json);

                ForecastUC forecast;
                for (int i=0; i < 8;  i++)
                {
                    forecast = new ForecastUC();
                    forecast.picWeatherIcon.ImageLocation =  "https://openweathermap.org/img/w/" + ForecastInfo.daily[i].weather[0].icon + ".png";
                    forecast.lblMainWeather.Text = ForecastInfo.daily[i].weather[0].main;
                    forecast.lblDescription.Text = ForecastInfo.daily[i].weather[0].description;
                    forecast.lblDateTime.Text = convertDatetime(ForecastInfo.daily[i].dt).DayOfWeek.ToString();

                    flowForecast.Controls.Add(forecast);
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            getWeather();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

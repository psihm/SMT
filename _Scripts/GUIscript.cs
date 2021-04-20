using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Xml;
using System.IO;

/* 
 * Pulls weather from wunderground and displays it on GUI
 * If weather change is detected, skybox is changed to match
 * Note: Lighting from skybox selected in editor is used
 * Todo: Pull time to change skybox based on time of day
 */
public class GUIscript : MonoBehaviour
{
    //public static bool updated = false;

    // Weather Underground API settings
    private const string WEATHER_UNDERGROUND_API_KEY = "d6b661301bfa7d5c";
    private const string WEATHER_STATION_NAME = "IWINNIPE49"; // change to RRC weather station name and switch lines 38 & 39 IVANCOUV38

    //string place = "";
    public static string weather = "";
    public static string oldWeather = "";
    public static string temperature = "";
    public static string relativeHumidity = "";
    public static string windDirection = "";
    public static string windKPH = "";
    

    public static bool clear = false;
    public static bool cloud = false;
    public static bool rain = false;

    

    // Skyboxes
    public Material skyboxCloudy;
    public Material skyboxClear;
    public Material skyboxRain;


    void GetWeatherData()
    {
        // URL to TWB weather station XML
        //string inputXML = "http://api.wunderground.com/api/" + WEATHER_UNDERGROUND_API_KEY + "/conditions/q/pws:" + WEATHER_STATION_NAME + ".xml";
        string inputXML = "http://api.wunderground.com/api/" + WEATHER_UNDERGROUND_API_KEY + "/conditions/q/CA/Winnipeg.xml";

        //bool fullNameRetrieved = false;    // Need to use a flag. There are two "full" XML elements; we just want the first.

        WebClient webClient = new WebClient();
        string weatherData = "";

        try
        {
            weatherData = webClient.DownloadString(inputXML);
        }
        catch
        {
            Debug.Log("Could not receive XML document from Weather Underground.");
        }

        using (XmlReader reader = XmlReader.Create(new StringReader(weatherData)))
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        /*if (reader.Name.Equals("full") && !fullNameRetrieved)
                        {
                            reader.Read();
                            place = reader.Value;
                            fullNameRetrieved = true;
                        }*/
                        if (reader.Name.Equals("weather"))
                        {
                            reader.Read();
                            weather = reader.Value;
                            if (weather.Contains("Thunderstorm"))
                                weather = "Thunderstorms";
                        }
                        else if (reader.Name.Equals("temp_c"))
                        {
                            reader.Read();
                            temperature = reader.Value;
                        }
                        else if (reader.Name.Equals("relative_humidity"))
                        {
                            reader.Read();
                            relativeHumidity = reader.Value;
                        }
                        else if (reader.Name.Equals("wind_dir"))
                        {
                            reader.Read();
                            windDirection = reader.Value;
                        }
                        else if (reader.Name.Equals("wind_kph"))
                        {
                            reader.Read();
                            windKPH = reader.Value;
                        }

                        break;
                }
            }
        }

        //string[] formattedData = { "Weather for " + place, weather, temperature, "Rel. humidity: " + relativeHumidity, "Wind " + windDirection + " at " + windKPH + " KPH" };
        
        //Debug.Log("Location: " + place);
        //Debug.Log("Weather: " + weather);
        //Debug.Log("Temperature: " + temperature);
        //Debug.Log("Relative Humidity: " + relativeHumidity);
        //Debug.Log("Wind: " + windDirection + " at " + windKPH + " KPH");
        

        if (weather != oldWeather) // If there is a weather change, change the skybox
        {
            clear = false;
            cloud = false;
            rain = false;
            ChangeSkybox();
        }
            

        oldWeather = weather;

        //updated = true;
    }

    void ChangeSkybox()
    {
        if (weather.Contains("Cloud") || weather == "Overcast")
        {
            RenderSettings.skybox = skyboxCloudy;
            cloud = true;
        }
        else if (weather == "Clear")
        {
            RenderSettings.skybox = skyboxClear;
            clear = true;
        }
        else if (weather.Contains("Light") || weather.Contains("Heavy") || weather.Contains("Rain") || weather.Contains("Thunderstorm") || weather.Contains("Freezing") || weather.Contains("Snow") || weather.Contains("Ice") || weather.Contains("Hail") || weather.Contains("Drizzle"))
        {
            RenderSettings.skybox = skyboxRain;
            rain = true;
        }
        else
        {
            RenderSettings.skybox = skyboxCloudy;
            cloud = true;
        } 
    }

    // Use this for initialization
    void Start()
    {
        InvokeRepeating("GetWeatherData", 0.0f, 150.0f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    
}

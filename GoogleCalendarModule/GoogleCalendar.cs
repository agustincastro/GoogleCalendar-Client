using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

namespace GoogleCalendarModule
{
    class GoogleCalendar : IGoogleCalendar
    {

        internal class Event
        {
            public string summary { get; set; }
            public string description { get; set; }
            public string location { get; set; }
            public GoogleDate start { get; set; }
            public GoogleDate end { get; set; }
        }

        internal class GoogleDate
        {
            public string dateTime { get; set; }
        }

        internal class Invitation
        {
            public Attendee[] attendees { get; set; }
            public GoogleDate start { get; set; }
            public GoogleDate end { get; set; }
            public string summary { get; set; }
            public string description { get; set; }
            public string location { get; set; }
        }

        internal class Attendee
        {
            public string email { get; set; }

        }


        public async Task<string> GetAllCalendars(string accessToken)
        {
            try
            {
                using (var client = new HttpClient()) //Implementacion utilizando HttpClient
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var responseString = await client.GetStringAsync("https://www.googleapis.com/calendar/v3/users/me/calendarList").ConfigureAwait(false);
                    return responseString;
                }
                #region Legacy HttpWebRequest Implementation
                // Implementation using httpwebrequest (Legacy)
                //var urlBuilder = new System.Text.StringBuilder();           
                //urlBuilder.Append("https://");
                //urlBuilder.Append("www.googleapis.com");
                //urlBuilder.Append("/calendar/v3/users/me/calendarList");
                //urlBuilder.Append("?minAccessRole=writer");
                //var httpWebRequest = HttpWebRequest.Create(urlBuilder.ToString())
                //    as HttpWebRequest;

                //httpWebRequest.CookieContainer = new CookieContainer();
                //httpWebRequest.Headers["Authorization"] =
                //    string.Format("Bearer {0}", accessToken);
                //var response = httpWebRequest.GetResponse();

                //Encoding enc = System.Text.Encoding.GetEncoding(1252);
                //StreamReader loResponseStream = new StreamReader(response.GetResponseStream(), enc);

                //string Response = loResponseStream.ReadToEnd();

                //loResponseStream.Close();
                //response.Close();
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception("GoogleCalendarModule, GetAllCalendars exception. Message:" + ex.Message);
            }
        }



        public async Task<string> CreateEvent(string accessToken, DateTime startDate, DateTime endDate, string title, string description, string location)
        {
            try
            {
                using (var client = new HttpClient()) //Implementacion utilizando HttpClient
                {
                    // Oauth2 Access token for authentication with Google Api
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    string start = startDate.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"); //YYYY-MM-DDTHH:MM:SS.MMMZ (Unix timestamp)
                    string end = endDate.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");

                    Event googleEvent = new Event()
                    {
                        description = description,
                        start = new GoogleDate() { dateTime = start },
                        end = new GoogleDate() { dateTime = end },
                        summary = title,
                        location = location
                    };
                    var jsonEvent = new JavaScriptSerializer().Serialize(googleEvent);
                    var jsoncontent = new StringContent(jsonEvent.ToString(), Encoding.UTF8, "application/json");

                    string url = "https://www.googleapis.com/calendar/v3/calendars/" + ConfigManager.GetInstance.GooogleCalendarId + "/events";
                    var response = await client.PostAsync(url, jsoncontent).ConfigureAwait(continueOnCapturedContext: false);
                    var responseString = await response.Content.ReadAsStringAsync();

                    //ExceptionManager.LogException(new Exception("Create Event: " + title + " startDate: " + startDate + " endDate: " + endDate + " Response: " + responseString));

                    JObject joResponse = JObject.Parse(responseString);
                    return joResponse["id"].ToString();
                }
                #region Legacy HttpWebRequest Implementation
                // Implementation using httpwebrequest (Legacy) there is no need to make asyncronous call with await
                //var urlBuilder = new System.Text.StringBuilder();
                //urlBuilder.Append("https://");
                //urlBuilder.Append("www.googleapis.com");
                //urlBuilder.Append("/calendar/v3/calendars/");
                //urlBuilder.Append("4bfsl6e3359d2q9v2037uhfv3g%40group.calendar.google.com");
                //urlBuilder.Append("/events");
                //urlBuilder.Append("?minAccessRole=writer");
                //var httpWebRequest = HttpWebRequest.Create(urlBuilder.ToString())
                //    as HttpWebRequest;
                //httpWebRequest.CookieContainer = new CookieContainer();
                //httpWebRequest.Headers["Authorization"] =
                //    string.Format("Bearer {0}", accessToken);
                ////2016-04-08T17:00:00.000Z
                //httpWebRequest.Method = "POST";
                //string postData = "{ \"end\": {\"dateTime\" : \"2016-04-08T17:00:00.000Z\"}, \"start\": {\"dateTime\" : \"2016-04-08T19:00:00.000Z\"}, \"description\": \"Una pija\" }";
                //byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                //// Set the ContentType property of the WebRequest.
                //httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                //// Set the ContentLength property of the WebRequest.
                //httpWebRequest.ContentLength = byteArray.Length;
                //// Get the request stream.
                //Stream dataStream = httpWebRequest.GetRequestStream();
                //// Write the data to the request stream.
                //dataStream.Write(byteArray, 0, byteArray.Length);
                //// Close the Stream object.
                //dataStream.Close();
                //var response = httpWebRequest.GetResponse();
                //Encoding enc = System.Text.Encoding.GetEncoding(1252);
                //StreamReader loResponseStream = new StreamReader(response.GetResponseStream(), enc);
                //string Response = loResponseStream.ReadToEnd();
                //loResponseStream.Close();
                //response.Close();
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception("GoogleCalendarModule, CreateEvent exception. Message:" + ex.Message);
            }
        }



        public async Task<bool> DeleteEvent(string accessToken, string eventId)
        {
            try
            {
                using (var client = new HttpClient())  //Implementation using HttpClient
                {
                    // Oauth2 Access token for authentication with Google Api
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    //Query parameter 'sendNotifications' refers to the notifications via email to the atendees of the removed event
                    string url = "https://www.googleapis.com/calendar/v3/calendars/" + ConfigManager.GetInstance.GooogleCalendarId + "/events/" + eventId + "?sendNotifications=True";
                    var response = await client.DeleteAsync(url).ConfigureAwait(continueOnCapturedContext: false);
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GoogleCalendarModule, DeleteEvent exception. Message:" + ex.Message);
            }
        }



        public async Task<bool> AddAttendee(string accessToken, string eventId, List<string> atendeesGmail, DateTime startDate, DateTime endDate, string title, string description, string location)
        {
            try
            {
                atendeesGmail = atendeesGmail.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                using (var client = new HttpClient()) //Implementation using HttpClient
                {
                    // Oauth2 Access token for authentication with Google Api
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    string start = startDate.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"); //YYYY-MM-DDTHH:MM:SS.MMMZ
                    string end = endDate.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                    Invitation googleInvitation = new Invitation()
                    {
                        attendees = atendeesGmail.Select(x => new Attendee() { email = x }).ToArray(),
                        description = description,
                        start = new GoogleDate() { dateTime = start },
                        end = new GoogleDate() { dateTime = end },
                        summary = title,
                        location = location
                    };
                    var jsonEvent = new JavaScriptSerializer().Serialize(googleInvitation);
                    var jsoncontent = new StringContent(jsonEvent.ToString(), Encoding.UTF8, "application/json");

                    string url = "https://www.googleapis.com/calendar/v3/calendars/" + ConfigManager.GetInstance.GooogleCalendarId + "/events/" + eventId;
                    var response = await client.PutAsync(url, jsoncontent).ConfigureAwait(continueOnCapturedContext: false);
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GoogleCalendarModule, AddAttendee exception. Message:" + ex.Message);
            }
        }


        public string RefreshAuthToken(string refreshToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var content = new FormUrlEncodedContent(new[] 
                    {
                        new KeyValuePair<string, string>("client_id", ConfigManager.GetInstance.ClientId),
                        new KeyValuePair<string, string>("client_secret", ConfigManager.GetInstance.ClientSecret),
                        new KeyValuePair<string, string>("refresh_token", refreshToken),
                        new KeyValuePair<string, string>("grant_type", "refresh_token")
                    });
                    var result = client.PostAsync("https://www.googleapis.com/oauth2/v4/token", content).Result;
                    string resultContent = result.Content.ReadAsStringAsync().Result;
                    JObject joResponse = JObject.Parse(resultContent);
                    return joResponse["access_token"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GoogleCalendarModule, RefreshAuthToken exception. Message:" + ex.Message);
            }
        }


    }
}

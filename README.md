###Client for consuming Google Calendar API REST services


Implemented in C#, an interface provides the functions to:

* **Retrieve all calendars:** Returns a json string containing all Google calendars

* **Create an event in a particular calendar**

* **Refresh authentication tokens (JWT):** Returns a new auth token by using existing refresh token

* **Add an attendee:** Invites another Google account to an event already created. The attendee gets the existing event replicated inside their Google Calendar.

* **Delete event:** Deletes a specific event from the user´s Google calendar(user associated with the accessToken), if the event has attendees they are notified to their mail and the event is also deleted from their calendars.

This is an alternative to the .NET Client library with few dependencies. The implementation also provides a legacy implementation with HttpWebRequest (Commented).

**The credentials needed to interact with the API are obtained by registering your application in Google developers Console. The default calendar name is "primary" and all dates are sent using the UNIX timestamp format.**

**The ones provided inside ConfigManager are for illustration purposes only**

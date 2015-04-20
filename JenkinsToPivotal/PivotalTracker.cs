using System;
using System.IO;
using System.Net;

namespace JenkinsToPivotal
{
    /// <summary>
    /// Class that send information to pivotal tracker.
    /// </summary>
    internal static class PivotalTracker
    {
        #region Constants
        private const string PIVOTAL_TRACKER_URL = "https://www.pivotaltracker.com/services/v5/source_commits";
        #endregion

        #region Public Methods
        /// <summary>
        /// Post a specific change to PivotalTracker Api.
        /// </summary>
        /// <param name="json">The json to be send.</param>
        /// <param name="pivotalTrackerApiToken">The pivotal tracker API token.</param>
        public static void Post(string json, string pivotalTrackerApiToken)
        {
            try
            {
                WebRequest request = WebRequest.CreateHttp(PIVOTAL_TRACKER_URL);
                request.Method = "POST";
                request.Headers.Add("X-TrackerToken", pivotalTrackerApiToken);
                request.ContentType = "application/json";

                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(json);
                }

                request.GetResponse();
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion
    }
}

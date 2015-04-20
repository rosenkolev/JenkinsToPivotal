using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace JenkinsToPivotal
{
    /// <summary>
    /// Connect to Jenkins and return the repository changes.
    /// </summary>
    internal static class Jenkins
    {
        #region Constants
        private const string JENKINS_SUFFIX_GET_URL = "api/xml?wrapper=changes&xpath=//changeSet/item";
        #endregion

        #region Public Methods
        /// <summary>
        /// Connect to Jenkins CI API and return the repository changes for a given JOB.
        /// </summary>
        /// <param name="jenkinsBuildUrl">The jenkins build Url.</param>
        /// <param name="jenkinsUsername">The jenkins username.</param>
        /// <param name="jenkinsToken">The jenkins token.</param>
        public static Changes Get(string jenkinsBuildUrl, string jenkinsUsername, string jenkinsToken)
        {
            Stream responseStream = null;
            try
            {
                Console.WriteLine("Connecting Jenkins...");

                var jenkinsUrl = Path.Combine(jenkinsBuildUrl, JENKINS_SUFFIX_GET_URL);
                WebRequest request = WebRequest.CreateHttp(jenkinsUrl);
                SetBasicAuthHeader(request, jenkinsUsername, jenkinsToken);
                responseStream = request.GetResponse().GetResponseStream();

                // Deserialize
                if (responseStream != null && responseStream.CanRead)
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(Changes));
                    return (Changes)deserializer.Deserialize(responseStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }
        #endregion

        #region Inner Methods
        /// <summary>
        /// Sets the basic authentication header for a WebRequest.
        /// </summary>
        /// <param name="req">The web request.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="userPassword">The user password.</param>
        private static void SetBasicAuthHeader(WebRequest req, string userName, string userPassword)
        {
            string authInfo = userName + ":" + userPassword;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            req.Headers["Authorization"] = "Basic " + authInfo;
        }
        #endregion
    }
}

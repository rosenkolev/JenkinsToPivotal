using System;
using System.Collections.Generic;

namespace JenkinsToPivotal
{
    class Program
    {
        #region Constants
        private const string ArgJenkinsBuildUrl = "--jenkins-build-url";
        private const string ArgJenkinsUsername = "--jenkins-username";
        private const string ArgJenkinsToken = "--jenkins-token";
        private const string ArgPivotalToken = "--pivotal-token";
        private const string UnknownArgumentError = "Unknown argument {0} found.";
        private const string Help = @"
  > JenkinsToPivotal [options]

  Options:
    {1}: The path of a specific build in a Jenkins Job. Ex: http://my.jenkins.domain/job/my_job/job_build/
    {2}: The Jenkins username taken from the API Token configuration.
    {3}: The Jenkins token taken from the API Token configuration.
    {4}: The Pivotak toke taken from the pivotal token configuration.
  Example:
    JenkinsToPivotal {1} http://jenkins.company.com/job/backbone/11/ {2} joe.doun {3} A2c2d4521 {4} E3sd1234dsWV";
        
        private static readonly Dictionary<string, string> _arguments = new Dictionary<string, string>()
        {
            { ArgJenkinsBuildUrl, string.Empty },
            { ArgJenkinsUsername, string.Empty },
            { ArgJenkinsToken, string.Empty },
            { ArgPivotalToken, string.Empty }
        };
        #endregion

        #region Program
        /// <summary>
        /// The program start method.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(
                    Help,
                    Environment.NewLine,
                    ArgJenkinsBuildUrl,
                    ArgJenkinsUsername,
                    ArgJenkinsToken,
                    ArgPivotalToken);
            }
            else
            {
                for (int i = 0; i < args.Length; i += 2)
                {
                    if (_arguments.ContainsKey(args[i]))
                    {
                        _arguments[args[i]] = args[i + 1];
                    }
                    else
                    {
                        Console.WriteLine(UnknownArgumentError, args[i]);
                        Environment.Exit(1);
                    }
                }
                
                var jenkinsChanges = Jenkins.Get(
                    _arguments[ArgJenkinsBuildUrl],
                    _arguments[ArgJenkinsUsername],
                    _arguments[ArgJenkinsToken]);
                
                if (jenkinsChanges != null)
                {
                    foreach (Item commitItem in jenkinsChanges.Items)
                    {
                        PivotalTracker.Post(
                            TransformJenkinsChangesToPivotalJson(commitItem, _arguments[ArgJenkinsBuildUrl]),
                            _arguments[ArgPivotalToken]);
                    }
                }
            }
        }
        #endregion

        #region Inner Methods
        private static string TransformJenkinsChangesToPivotalJson(Item commitItem, string jenkinsBuildUrl)
        {
            return string.Format("{{\"source_commit\":{{\"commit_id\":\"{0}\",\"message\":\"{1}\",\"url\":\"{2}\",\"author\":\"{3}\"}}}}",
                commitItem.CommitRevision,
                EscapeJson(commitItem.Message),
                jenkinsBuildUrl,
                commitItem.Author);
        }
                
        private static string EscapeJson(string input)
        {
            return input.Replace("\n", "\\n")
                        .Replace("\r", "\\r")
                        .Replace("\t", "\\t");
        }
        #endregion
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace SonarqubeIssueExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            var issues = new List<CCIssue>();
            var client = GetWebClient();
            var projects = JsonConvert.DeserializeObject<Projects>(client.DownloadString($"http://localhost:9000/api/components/search_projects?ps=50&facets=reliability_rating%2Csecurity_rating%2Csqale_rating%2Ccoverage%2Cduplicated_lines_density%2Cncloc%2Calert_status%2Clanguages%2Ctags&f=analysisDate%2CleakPeriodDate"));
            foreach (var component in projects.components)
            {
                GetIssues(issues, component.key);
            }
            File.WriteAllText("commentedcode.txt", JsonConvert.SerializeObject(issues));
        }

        private static void GetIssues(List<CCIssue> issues, string component)
        {
            for (int i = 1; i < 200; i++)
            {
                WebClient client = GetWebClient();
                var response = JsonConvert.DeserializeObject<Issues>(client.DownloadString($"http://localhost:9000/api/issues/search?s=FILE_LINE&projects={component}&resolved=false&rules=csharpsquid%3AS125&ps=100&facets=severities%2Ctypes&additionalFields=_all&p={i++}"));

                if (response.issues.Count == 0)
                {
                    break;
                }

                foreach (var issue in response.issues)
                {
                    var issueType = GetIssueType(issue.message);

                    if (issueType == null)
                        issueType = issue.message;

                    var file = issue.component.Split(':')[1];
                    var existingIssue = issues.FirstOrDefault(x => x.IssueType == issueType && x.FileName == file);
                    if (existingIssue == null)
                    {
                        issues.Add(new CCIssue() { FileName = file, IssueType = issueType, Locations = new List<IssueLocation>(), AddedOn = DateTime.Now });
                        existingIssue = issues.FirstOrDefault(x => x.IssueType == issueType && x.FileName == file);
                    }

                    if (issue.flows.Count > 0)
                    {
                        foreach (var flow in issue.flows)
                        {
                            var subIssue = JsonConvert.DeserializeObject<SubIssues>(flow.ToString());

                            foreach (var location in subIssue.locations)
                            {
                                var issueLocation = new IssueLocation()
                                {
                                    StartLine = location.textRange.startLine,
                                    EndLine = location.textRange.endLine,
                                    StartOffset = location.textRange.startOffset,
                                    Url = $"https://github.com/trilogy-group/km-all-projects/blob/develop/{file}#L{location.textRange.startLine}"
                                };

                                if (!existingIssue.Locations.Any(x => x.Url == issueLocation.Url))
                                {
                                    existingIssue.Locations.Add(issueLocation);
                                }
                            }
                        }
                    }
                    else
                    {
                        var issueLocation = new IssueLocation()
                        {
                            StartLine = issue.textRange.startLine,
                            EndLine = issue.textRange.endLine,
                            StartOffset = issue.textRange.startOffset,
                            Url = $"https://github.com/trilogy-group/km-all-projects/blob/develop/{file}#L{issue.textRange.startLine}"
                        };

                        if (!existingIssue.Locations.Any(x => x.Url == issueLocation.Url))
                        {
                            existingIssue.Locations.Add(issueLocation);
                        }
                    }

                }
            }
        }

        private static WebClient GetWebClient()
        {
            var client = new WebClient();
            client.Headers.Add("Authorization", "Basic YWRtaW46YWRtaW4=");
            return client;
        }

        private static string GetIssueType(string issueType)
        {
            if (issueType == "Missing curly brace.")
            {
                return "BRP :: Missing curly braces";
            }

            if (issueType == "Remove this commented out code.")
            {
                return "Dead Code";
            }

            if (issueType.StartsWith("Define a constant"))
            {
                return "BRP Issue";
            }

            if (issueType == "Add the \"@Override\" annotation above this method signature")
            {
                return "BRP :: \"@Override\" should be used on overriding and implementing methods";
            }

            if (issueType.Contains("Make this \"public static "))
            {
                return "BRP::Public fields should be constants";
            }

            return null;
        }
    }
}

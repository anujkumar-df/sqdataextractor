using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SonarqubeIssueExtractor
{
    class Program
  {
    static void Main(string[] args)
    {
      var issues = new List<CCIssue>();
      for (int i = 1; i < 200; i++)
      {
        var client = new WebClient();
        client.Headers.Add("Authorization", "Basic YWRtaW46YWRtaW4=");
        var response = JsonConvert.DeserializeObject<RootObject>(client.DownloadString($"http://localhost:9000/api/issues/search?s=FILE_LINE&projects=ecnaccounts&resolved=false&rules=csharpsquid%3AS1192&ps=100&facets=severities%2Ctypes&additionalFields=_all&p={i++}"));

        if(response.issues.Count == 0)
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
            issues.Add(new CCIssue() { FileName = file, IssueType = issueType, Locations = new List<IssueLocation>() });
            existingIssue = issues.FirstOrDefault(x => x.IssueType == issueType && x.FileName == file);
          }
          existingIssue.Locations.Add(new IssueLocation()
          {
            StartLine = issue.textRange.startLine,
            EndLine = issue.textRange.endLine,
            StartOffset = issue.textRange.startOffset,
            Url = $"https://github.com/trilogy-group/km-all-projects/blob/develop/{file}#L{issue.textRange.startLine}"
          });
        }
      }
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

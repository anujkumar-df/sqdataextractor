﻿using System;
using System.Collections.Generic;

namespace SonarqubeIssueExtractor
{


    public class Location
    {
        public string component { get; set; }
        public TextRange textRange { get; set; }
    }

    public class CCIssue
    {
        public string IssueType { get; set; }
        public string FileName { get; set; }
        public List<IssueLocation> Locations { get; set; }
        public DateTime AddedOn { get; set; }
    }

    public class IssueLocation
    {
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public int StartOffset { get; set; }
        public string Url { get; internal set; }
    }

    public class Paging
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public int total { get; set; }
    }
    public class TextRange
    {
        public int startLine { get; set; }
        public int endLine { get; set; }
        public int startOffset { get; set; }
        public int endOffset { get; set; }
    }

    public class Issue
    {
        public string key { get; set; }
        public string rule { get; set; }
        public string severity { get; set; }
        public string component { get; set; }
        public string project { get; set; }
        public int line { get; set; }
        public string hash { get; set; }
        public TextRange textRange { get; set; }
        public List<object> flows { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string effort { get; set; }
        public string debt { get; set; }
        public string author { get; set; }
        public List<string> tags { get; set; }
        public List<string> transitions { get; set; }
        public List<string> actions { get; set; }
        public List<object> comments { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime updateDate { get; set; }
        public string type { get; set; }
        public string organization { get; set; }
        public bool fromHotspot { get; set; }
    }

    public class Component
    {
        public string organization { get; set; }
        public string key { get; set; }
        public string uuid { get; set; }
        public bool enabled { get; set; }
        public string qualifier { get; set; }
        public string name { get; set; }
        public string longName { get; set; }
        public string path { get; set; }
        public string id { get; set; }
        public bool isFavorite { get; set; }
        public DateTime analysisDate { get; set; }
        public List<object> tags { get; set; }
        public string visibility { get; set; }
    }

    public class Rule
    {
        public string key { get; set; }
        public string name { get; set; }
        public string lang { get; set; }
        public string status { get; set; }
        public string langName { get; set; }
    }

    public class User
    {
        public string login { get; set; }
        public string name { get; set; }
        public bool active { get; set; }
    }

    public class Language
    {
        public string key { get; set; }
        public string name { get; set; }
    }

    public class Value
    {
        public string val { get; set; }
        public int count { get; set; }
    }

    public class Facet
    {
        public string property { get; set; }
        public List<Value> values { get; set; }
    }

    public class Issues
    {
        public int total { get; set; }
        public int p { get; set; }
        public int ps { get; set; }
        public Paging paging { get; set; }
        public int effortTotal { get; set; }
        public int debtTotal { get; set; }
        public List<Issue> issues { get; set; }
        public List<Component> components { get; set; }
        public List<Rule> rules { get; set; }
        public List<User> users { get; set; }
        public List<Language> languages { get; set; }
        public List<Facet> facets { get; set; }
    }

    public class SubIssues
    {
        public List<Location> locations { get; set; }
    }





    public class Projects
    {
        public Paging paging { get; set; }
        public List<object> organizations { get; set; }
        public List<Component> components { get; set; }
        public List<Facet> facets { get; set; }
    }
}

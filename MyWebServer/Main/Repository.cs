using System;
using System.Collections.Generic;
using RequestHandlers;
using DataHandlers;
using ExceptionFabric;
using System.Xml.Linq;
using Events;
using Configurate;
using System.Linq;
using System.Text;

public static class Repository
{
    public delegate void WriteLogParam(string text, params object[] obj);
    public static XElement ConfigBody { get; private set; }
    public static Dictionary<string, ABSHttpHandler> ReqestsHandlers { get; private set; }
    public static Dictionary<string, ABSMIME> DataHandlers { get; private set; }
    public static Dictionary<string, ABSExceptionFabric> ExceptionFabrics { get; private set; }
    public static List<ABSGrub> Eventers { get; private set; }
    public static WebServerConfig Configurate { get; private set; }

    private static Action<string> write_log;
    private static WriteLogParam write_log_param;
    private static string Xdoc_puth;
    private static string host_name;

    static Repository()
    {
        ReqestsHandlers = new Dictionary<string, ABSHttpHandler>();
        DataHandlers = new Dictionary<string, ABSMIME>();
        ExceptionFabrics = new Dictionary<string, ABSExceptionFabric>();
        Eventers = new List<ABSGrub>();
    }

    public static void RepositoryConstruct(string puth, string name, Action<string> WriteLog, WriteLogParam LogParam)
    {
        Xdoc_puth = puth;
        host_name = name;
        write_log = WriteLog;
        write_log_param = LogParam;
    }

    public static void Write(string text) {
        write_log(text);
    }
    public static void Write(string text, params object[] obj) {
        write_log_param(text, obj);
    }
    public static void WriteLine(string text) {
        write_log(text + "\r\n");
    }
    public static void WriteLine(string text, params object[] obj) {
        write_log_param(text+"\r\n", obj);
    }

    public static string HostInfo()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("\r\n### {0} ###", host_name);
        sb.Append("\r\nHTTP Handlers:");
        foreach (KeyValuePair<string, ABSHttpHandler> handler in Repository.ReqestsHandlers)
        {
            sb.AppendFormat("\r\n\tHTTP type {0}", handler.Key.ToString());
        }
        sb.Append("\r\nMIME Handlers:");
        foreach (KeyValuePair<string, ABSMIME> handler in Repository.DataHandlers)
        {
            sb.AppendFormat("\r\n\tMIME handler extension {0}", handler.Key);
        }
        sb.Append("\r\nAvailable exceptions");
        foreach (KeyValuePair<string, ABSExceptionFabric> handler in Repository.ExceptionFabrics)
        {
            sb.AppendFormat("\r\n\tException: {0}", handler.Key);
        }
        return sb.ToString();
    }

    public static void Start()
    {
        XDocument all_configs = XDocument.Load(Xdoc_puth);
        ConfigBody = (from el in all_configs.Root.Elements()
                      where el.Element("name").Value == host_name
                      select el).First();
        Configurate = new WebServerConfig();
        Configurate.Configurate();
    }

    public static void Stop() {
        Configurate.Dispose();
        Configurate = null;
    }
}

*Внесение изменений*  
git add .  
git commit -m "Сообщение о том, что сделали"  
git push origin muster.  
git pull origin master - получить последние изменения других контрибьютеров  
git log - Вся история проекта  
git log -u - Вся история проекта + изменения в коде  
git status - Для просмотра статуса репозитория(внесённые изменения) - работает локально  
Для перезаписи локальных изменений удалёнными  
git fetch --all  
git reset --hard origin/имя_ветки  

### About  
There is a web-server written on C#, you can easily extend this server through your own modules, like HTTP or mime handlers, exception and resource viewer. Each of these modules should be compiled, and copied onto the directory, specified in the config

######Web-server support commands:  
* lshost - display available hosts
* host <your_host_name> <command> - send <command> to host with name <your_host_name>
* exit - end web-server work
  
######Host support commands:  
* loadintplug - load internal plugins
* loadexplug - load external plugins
* info - display HTTP handlers, MIME handler, Exceptions and status
* start - run host
* stop - stop host
* status - display run/stop state of host
  
Web-server send command to host by order loadexplug -> loadintplug -> start on default  

It's necessary to implement abstract class Host.HttpHandler.IHttpHandler to add a new HTTP handler. This class will notify program with the needed HTTP version and method. Moreover, it contains the actual logic which is supposed to handle request headers and parameters.  
*code example for http handler*
```cs
public class AnythingHttpHandler : IHttpHandler {
      public override string HandlerType {
        get {
          //this type of http request. Reaction on starting line string of http. Like return "GET";
        }
      }
      public override string HandlerVersion {
        get {
          //this version of http request. Like return "HTTP/1.1";
        }
      }

      public override void Parse(ref Reqest output, string[] request, string URL) {
        //in this block you write your logic of http handle. URL this is url of resource,
        //request - http headers
        //output this is object of Reqest(just created). In this object you must fill next variables:
        //  varibles - this is parameters which stay after url (in GET requests for example). Add like this: output.varibles.Add(var_name, var var_val);
        //  cookies on this moment is not using
        //  preferens - all remaining variables in http headers perhaps you use their in any place
      }
}
```

Then we could set up a mime handle. this handler implements the  Host.MIME.IMIME interface. the mentioned interface should inform the main program about mime type(for response), file extension and also consist code which handles data from the file.  
*code example for mime handler*
```cs
public class HttpMIME : IMIME {
    private string[] _file_extensions = { ".html" }; //extension of handle's file
    public string MIME_Type { get { return "text/html"; } } //type of data for response
    public string[] file_extensions { get { return _file_extensions; } }

    public byte[] Handle(ref Response response, ref Reqest request, ref Reader read) {//this method can include anything logic of handling data. In example it return data from reader of data. But also it can read somehow data, handle it anything and return. This method is invoke before formation of http response.
      return read.data;
    }
}
```

For adding exception you must implement abstract class Host.ServerExceptions.ExceptionCode. It inform program about Exception code, fatal is it and also it can implement two methods: first of their can add headers into headers of request, second can add data to message body(in default first of this nothing do and the second return html page with error code)
*code example for exceptions*
```cs
public class OK : ExceptionCode
{
  public OK()
  {
    Code = "200 OK";
    _IsFatal = false;
  }
}
```

For adding resource viewer you must implement interface Host.DirReader.IDirectoryReader and in config write next row:
```xml
<allow_browse_folders is_work="true" browser="NameOfResourceViewer"></allow_browse_folders>
<!--NameOfResourceViewer - name of your class-->
<!--is_work - can use it in program-->
```
This component implement method which accept IItem(can be LinkDirectory or LinkFile).

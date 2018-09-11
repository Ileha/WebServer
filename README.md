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

###### Web-server supports commands:  
* lshost - display available hosts
* host <your_host_name> <host_command> - send <host_command> to host with name <your_host_name>
* exit - end web-server work
  
###### Host supports commands:  
* loadintplug - load internal plugins
* loadexplug - load external plugins
* info - display HTTP handlers, MIME handler, Exceptions and status
* start - run host
* stop - stop host
* status - display run/stop state of host
  
In start Web-server sends command to host in order loadexplug -> loadintplug -> start on default.  

It's necessary to implement abstract class RequestHandlers.ABSHttpHandler to add a new HTTP handler. This class will notify program with the needed HTTP version and method. Moreover, it contains the actual logic which is supposed to handle request headers and http data.  
*code example for http handler*
```cs
public class AnythingHttpHandler : IHttpHandler {
	//this type of http request. Reaction on starting line string of http.
	public override string HandlerType { get { return "GET"; } }
	//this version of http request.
	public override string HandlerVersion { get { return "HTTP/1.1"; } }

	public override void ParseHeaders(ref Reqest output, Stream reqest)
	{
		//in output you must set:
		//						output.URL
		//						write data to output.Data
		//                      add headers to output.headers
		//						add cookies to output.cookies
	}
}
```

Then we could set up a mime handle. this handler implements abstract class the  DataHandlers.ABSMIME. the mentioned interface should inform the main program about mime type(for response), file extension and also consist code which handles data from the file.  
*code example for mime handler*
```cs
public class ExampleMIME : IMIME {
	//array with file extensions
    public override string[] file_extensions { get { } }
	
	public override void Handle(IConnetion Connection, Action<string, string> add_to_http_header_request) {
		//IConnetion consist:
		//					information about RemoteEndPoint and LocalEndPoint
		//					input and output steam with data
		//					user data 
		//					request data in stream 
		//					type of connection
		
		//add_to_http_header_request Action for adding http headers to response
	}
}
```

For adding exception you must implement abstract classes ExceptionFabric.ABSExceptionFabric and ExceptionFabric.ExceptionCode. First class is fabric of your exception, it implements two method string name { get; } and ExceptionCode Create(params object[] data). In second class you must set Code field for inform program about Exception code and also can implement method void ExceptionHandleCode(Response response, IConnetion data); this function work like Handle in DataHandlers.ABSMIME but it`s have more access to response. In default it clear response and write inform data to IConnetion.
*code example for exceptions*
```cs
	public class ExampleExceptionFabric : ABSExceptionFabric {
		public ExampleExceptionFabric() {}
		public override string name { get { return "ExampleExceptionName"; } }
        public override ExceptionCode Create(params object[] data) {
            return new OK();
        }
    }
	public class ExampleException : ExceptionCode {

        public ExampleException() {
			Code = "ExampleExceptionCode ExampleExceptionName";
		}
		
		//default function:
        public override void ExceptionHandleCode(Response response, IConnetion data) {
			response.Clear();
            response.AddToHeader("Content-Type", "text/html; charset=UTF-8", AddMode.rewrite);
			byte[] _data = Encoding.UTF8.GetBytes("<html><body><h2>An error has occurred code of error " + Code + "</h2></body></html>");
            data.OutputData.Write(_data, 0, _data.Length);
        }
	
```

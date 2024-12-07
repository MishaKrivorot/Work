This is a client-server software complex that implements the layer 7 network protocol.
The server part is a Windows Service, which receives network requests from the client on TCP port 44000 and sends it responses.
The client part is a Windows Form Application, which
has one text field for the search filter and three buttons - "Perform 1st request", "Perform 2nd request" and "Close program".

On the server, data is stored and processed in the registry branch
HKEY_LOCAL_MACHINE\Software\SEARCH-Server

As a result of its execution on both sides, there will be 4 files:
Request-1.XML
Response-1.XML
Request-2.XML
Response-2.XML

You can only create parameters manually on the server. 
The procedure for determining the name of the client student consists of two consecutive requests. 
With the first request, the client sends the XML file Request-1.XML to the server, which contains only the search filter. 
The server receives the request and creates a list of surnames (without first names) that match the filter. The list can be empty. 
The server generates and sends the XML file Responce-1.XML with this list to the client. The client receives and displays the list of surnames on the user's screen and allows him to select only one surname from this list. 
If the client receives an empty list, he must change the search filter and repeat the first request. During the second request, the client sends the server an XML file with one selected surname. 
The server finds the student's name using the known surname and returns to the client an XML file with the student's surname and first name. The client receives this information and displays it on the user's screen.

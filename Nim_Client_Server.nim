#[
    Injector em Nim
    Criado por Marcone em 2022
    Telegram @thegrapevine
]#

# Importando os Módulos
import net, threadpool, selectors


proc conecta(c: Socket, a: string)  =
    echo("[#] Client Received: " & a)
    
    var rq = c.recv(8192)
    echo("[#] Client Request: " & rq)

    var s = newSocket(buffered = false)
    s.connect("nl-2.hostip.co", Port(443))

    c.send("HTTP/1.1 200 Established\r\n\r\n")
    
    var io = newSelector[int]()
    io.registerHandle(c.getFd, {Read}, 0)
    io.registerHandle(s.getFd, {Read}, 0)

    try:
        while true:
            for i in io.select(-1):
                if i.fd == s.getFd().int:
                    # Download
                    var data = s.recv(8192)
                    if data.len == 0: raise
                    c.send(data)
                elif i.fd == c.getFd().int:
                    # Upload
                    var data = c.recv(8192)
                    if data.len == 0: raise
                    s.send(data)
    except:
        echo("[#] Client Disconnected!")
    finally:
        s.close()
        c.close()

# Listen
var l = newSocket(buffered = false)
l.bindAddr(Port(8088))
l.listen()

echo("[#] Listen on IP and Port 127.0.0.1:8088\n")

var c: Socket
var a: string

while true:
    l.acceptAddr(c, a)
    spawn conecta(c, a)
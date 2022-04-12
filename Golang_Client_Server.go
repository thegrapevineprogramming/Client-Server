/*
	Injector em Golang
	Criado por Marcone em 2022
	Telegram: @thegrapevine
*/

package main

import "fmt"
import "net"

func conecta(c net.Conn){
	fmt.Println("[#] Client Received!")

	rq := make([]byte, 8192)
    tam, _ := c.Read(rq)
    fmt.Println("[#] Client Request: " + string(rq[0:tam]))

	s, _ := net.Dial("tcp", "nl-2.hostip.co:443")

	c.Write([]byte("HTTP/1.1 200 Established\r\n\r\n"))

	conn := true

	go func(){
		for conn {
			data := make([]byte, 8192)
			tm, err := s.Read(data)
			if (err != nil){conn = false; break}
			c.Write(data[0:tm])
		}
	}()

	func(){ 
		for conn {
			data := make([]byte, 8192)
			tm, err := c.Read(data)
			if (err != nil){conn = false; break}
			s.Write(data[0:tm])
		}
	}()

	fmt.Println("[#] Client Disconnected!")
}

func main(){
	// Listen
	l, _ := net.Listen("tcp", "127.0.0.1:8088")
	fmt.Println("[#] Listen on IP and Port 127.0.0.1:8088")

	for {
		c, _ := l.Accept()
		go conecta(c)
	}
}
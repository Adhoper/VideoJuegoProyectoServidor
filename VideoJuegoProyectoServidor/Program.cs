﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.WebSockets;
using System.Threading;

namespace VideoJuegoProyectoServidor
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var httpListener = new HttpListener();
            
            httpListener.Prefixes.Add("http://localhost:1600/");

            httpListener.Start();

            Console.WriteLine("Se ha iniciado el servidor");


            while (true)
            {
                // Espera una solicitud HTTP entrante
                var context = await httpListener.GetContextAsync();

                // Verifica si la solicitud es una solicitud WebSocket
                if (context.Request.IsWebSocketRequest)
                {
                    // Acepta la conexión WebSocket
                    var webSocketContext = await context.AcceptWebSocketAsync(null);

                    // Maneja la conexión WebSocket de forma asincrónica
                    await HandleWebSocketAsync(webSocketContext.WebSocket);
                }
                else
                {
                    // Rechaza las solicitudes que no sean WebSocket
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }


        }

        static async Task HandleWebSocketAsync(WebSocket webSocket)
        {

            // Buffer para recibir mensajes WebSocket
            byte[] buffer = new byte[1024];

            // Recibe el primer mensaje del cliente
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            // Bucle principal que maneja la comunicación WebSocket
            while (!result.CloseStatus.HasValue)
            {
                // Decodifica el mensaje recibido como una cadena UTF-8
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Mensaje recibido: {message}");

                    // Valida que el mensaje sea iniciar juego
                    if (message == "juego")
                    {

                    // Envía una actualización de estado al cliente
                    string response = $"\u001b[36mSe Recibio el Mensaje.\u001b[0m";
                    byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
                    await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                    // ejecuta el ascensor

                }
                    else
                    {
                        // Si el piso solicitado no es válido, envía un mensaje de error al cliente
                        string errorMessage = "\u001b[31mEl piso especificado no es válido.\u001b[0m";
                        byte[] errorBuffer = Encoding.UTF8.GetBytes(errorMessage);
                        await webSocket.SendAsync(new ArraySegment<byte>(errorBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                

                // Recibe el siguiente mensaje del cliente
                buffer = new byte[1024];
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            // Cierra la conexión WebSocket
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}

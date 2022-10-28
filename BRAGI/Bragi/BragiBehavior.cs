using System;
using WebSocketSharp.Server;
using WebSocketSharp;
using BRAGI.Valhalla;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BRAGI.Bragi;

public class BragiBehavior : WebSocketBehavior
{
    /// <summary>
    /// Handles the incoming messages when received via websocket.
    /// If an UUID is present in the deserialized object, a response will be given.
    /// </summary>
    /// <param name="e"></param>
    protected override async void OnMessage(MessageEventArgs e)
    {
        Console.WriteLine("Received Websocket Message: " + e.Data);
        WebSocketResponse? resp = null;
        try
        {
            WebSocketMessage? mes = JsonSerializer.Deserialize<WebSocketMessage>(e.Data, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true});
            if (mes == null) Console.WriteLine("Deserialized Message is NULL");
            else
            {
                try
                {
                    if (mes.UUID != null) resp = new WebSocketResponse(mes.UUID, RETURNCODE.EXCEPTION);
                    if (mes.Command == null || !BragiCommands.Commands.ContainsKey(mes.Command))
                    {
                        Console.WriteLine("Received unknown Command: " + mes.Command);
                        if (resp != null) resp.Code = RETURNCODE.UNKNOWN_COMMAND;
                    }
                    else
                    {
                        if (resp == null)
                        {
                            await BragiCommands.CallFunction(mes.Command, mes.Params);
                        }
                        else
                        {
                            resp.Data = await BragiCommands.CallFunction(mes.Command, mes.Params);
                            resp.Code = RETURNCODE.OK;
                        }
                    }
                }
                catch (CommandException ex)
                {
                    Console.WriteLine(ex);
                    if (resp != null)
                    {
                        resp.Data = new object[] { ex.ErrorCode, ex.Message };
                        resp.Code = RETURNCODE.COMMAND_ERROR;
                    }
                }
                catch (InvalidParameterException ex)
                {
                    Console.WriteLine(ex);
                    if (resp != null)
                    {
                        resp.Data = ex.Message;
                        resp.Code = RETURNCODE.INVALID_PARAMETER;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    if (resp != null)
                    {
                        resp.Data = ex.Message;
                        resp.Code = RETURNCODE.EXCEPTION;
                    }
                }
                finally
                {
                    if (resp != null) Send(JsonSerializer.Serialize(resp));
                }
            }
        }
        catch(Exception er)
        {
            if (er is JsonException)
            {
                Console.WriteLine("Error while trying to deserialize object: " + e.Data);
                
            }
            Console.WriteLine(er);
        }
    }

    protected override void OnClose(CloseEventArgs e)
    {
        Console.WriteLine("A Hero has left Valhalla with runes: " + e.Code);
        if (e.Code != ((ushort)CloseStatusCode.PolicyViolation))
        {
            if (Valhalla.Valhalla.Instance!.Bragi != null)
            {
                Valhalla.Valhalla.Instance.Bragi.CleanUp();
            }
            Valhalla.Valhalla.Instance.HeroArrived = false;
        }
    }

    protected override void OnOpen()
    {
        if (!Valhalla.Valhalla.Instance!.HeroArrived)
        {
            Valhalla.Valhalla.Instance.HeroArrived = true;
            Console.WriteLine("Client connected.");
        }
        else
        {
            Console.WriteLine("Client already connected. Dropping duplicated connection...");
            Sessions.CloseSession(ID, CloseStatusCode.PolicyViolation, "Duplicated Connection");
        }
    }
}

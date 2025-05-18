using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using Newtonsoft.Json.Linq;
using WindFrostBot.SDK;
using static Mysqlx.Expect.Open.Types.Condition.Types;

public class BotClient
{
    private static string ?appId;
    private static string ?clientSecret;
    private static string ?accessToken;//当前使用的Token
    private static string gatewayUrl = "wss://api.sgroup.qq.com/websocket";//Gateway地址
    private static int heartbeatInterval;//心跳包间隙
    private static string ?sessionId;
    private static int sequenceNumber;//当前seq参数
    private static ClientWebSocket ?_webSocket;
    public event EventHandler<MessageEventArgs> OnMessageReceived; //私聊事件
    public event EventHandler<MessageEventArgs> OnGroupMessageReceived; //群聊事件
    private Thread receiveThread;
    private Thread heartThread;
    public bool Connected = false;
    public BotClient(string appid, string clientsecret)
    {
        clientSecret = clientsecret;
        appId = appid;
        new Task(async () => { await Init(); }).Start();
    }
    public void Dispose()
    {
         _webSocket?.Dispose();
    }
    public async Task Init()
    {
        await GetAccessToken();
        await ConnectWebSocket();
    }

    public static async Task GetAccessToken()
    {
        using (var client = new HttpClient())
        {
            var requestContent = new StringContent(
                $"{{\"appId\": \"{appId}\", \"clientSecret\": \"{clientSecret}\"}}",
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("https://bots.qq.com/app/getAppAccessToken", requestContent);
            var responseString = await response.Content.ReadAsStringAsync();

            var tokenResponse = JObject.Parse(responseString);
            accessToken = tokenResponse["access_token"].ToString();
            Message.Info($"Access Token: {accessToken}");
        }
    }
    public async Task ConnectWebSocket()
    {
        _webSocket = new ClientWebSocket();
        await _webSocket.ConnectAsync(new Uri(gatewayUrl), CancellationToken.None);
        Message.Info("Connection opened.");
        Connected = true;
        SendIdentify();//发送鉴定
        receiveThread = new Thread(new ThreadStart(Listener));
        receiveThread.Start();
    }
    public async void Listener()
    {
        var buffer = new byte[1024 * 4];
        while (_webSocket.State == WebSocketState.Open && Connected)
        {
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            try
            {
                if (!string.IsNullOrEmpty(message))
                {
                    //Message.Erro(message);
                    var data = JObject.Parse(message);
                    ProcessMessage(data);
                }
                else
                {
                    Message.Erro("Recevive Empty Message...");
                }
            }
            catch
            {

            }
        }
    }
    public static async void SendIdentify()
    {
        var identifyPayload = new JObject
        {
            ["op"] = 2,
            ["d"] = new JObject
            {
                ["token"] = "QQBot " + accessToken,
                ["intents"] = 1 << 25,
                ["shard"] = new JArray { 0, 1 },
                ["properties"] = new JObject
                {
                    ["$os"] = "windows",
                    ["$browser"] = "my_library",
                    ["$device"] = "my_library"
                }
            }
        };

        await SendMessageAsync(identifyPayload.ToString());
        Message.Info("Identify sent.");
    }
    public async void Hearbeat()
    {
        while(_webSocket?.State == WebSocketState.Open && Connected)
        {
            try
            {
                var heartbeatPayload = new JObject
                {
                    ["op"] = 1,
                    ["d"] = sequenceNumber
                };
                await SendMessageAsync(heartbeatPayload.ToString());
                await Task.Delay(heartbeatInterval);
            }
            catch
            {

            }
        }
    }
    private async void ProcessMessage(JObject message)
    {
        int op = message["op"].Value<int>();
        if(message["s"] != null)
        {
            sequenceNumber = message["s"].Value<int>();
        }
        switch (op)
        {
            case 10://心跳包
                heartbeatInterval = message["d"]["heartbeat_interval"].Value<int>();
                heartThread = new Thread(new ThreadStart(Hearbeat));
                heartThread.Start();
                break;
            case 7:
                Message.Erro("Recevive Reconnecting Message...");
                Connected = false;
                Dispose();
                //_webSocket = new ClientWebSocket();
                //await _webSocket.ConnectAsync(new Uri(gatewayUrl), CancellationToken.None);
                //Connected = true;
                //await GetAccessToken();
                //SendIdentify();
                //Message.Info("Reconnect to Server Successfully!");
                break;
            case 0://群聊消息
                string eventType = message["t"].Value<string>();
                var data = message["d"];
                switch (eventType)
                {
                    case "READY"://准备完成后的事件
                        sessionId = data["session_id"].Value<string>();
                        break;
                    case "GROUP_AT_MESSAGE_CREATE"://群消息事件
                        HandleGroupAtMessageCreate(data);
                        break;
                    case "GROUP_ADD_ROBOT"://机器人被添加到群的事件
                        string memberopenid = data["op_member_openid"].Value<string>();
                        string groupopenid = data["group_openid"].Value<string>();
                        var groupevent = new GroupMessageEvent(groupopenid, memberopenid);
                        MainSDK.OnGroupAdd.ExecuteAll(groupevent);
                        break;
                    case "GROUP_DEL_ROBOT"://机器人被移除群聊事件
                        memberopenid = data["op_member_openid"].Value<string>();
                        groupopenid = data["group_openid"].Value<string>();
                        groupevent = new GroupMessageEvent(groupopenid, memberopenid);
                        MainSDK.OnGroupRemove.ExecuteAll(groupevent);
                        break;
                    case "GROUP_MSG_REJECT"://群聊主动消息关闭事件
                        memberopenid = data["op_member_openid"].Value<string>();
                        groupopenid = data["group_openid"].Value<string>();
                        break;
                    case "GROUP_MSG_RECEIVE"://群聊主动消息开启事件
                        memberopenid = data["op_member_openid"].Value<string>();
                        groupopenid = data["group_openid"].Value<string>();
                        break;
                    case "FRIEND_ADD"://私聊好友添加事件
                        var openid = data["openid"].Value<string>();
                        break;
                    case "FRIEND_DEL"://私聊好友删除事件
                        openid = data["openid"].Value<string>();
                        break;
                    case "C2C_MSG_RECEIVE"://开启私聊主动消息
                        openid = data["openid"].Value<string>();
                        break;
                    case "C2C_MSG_REJECT"://关闭私聊主动消息
                        openid = data["openid"].Value<string>();
                        break;
                    case "C2C_MESSAGE_CREATE"://私聊事件
                        HandleMessageCreate(data);
                        break;
                }
                break;
            case 9://处理无效的会话
                Message.Info("Received INVALID_SESSION");
                // await ConnectWebSocket();
                break;
            case 11:
                //心跳包成功后返回
                break;
        }
    }
    public static async Task SendMessageAsync(string message)
    {
        var messageBuffer = Encoding.UTF8.GetBytes(message);
        await _webSocket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }
    public async void SendKeyboard(string keyid, MessageEventArgs args, int seq = 1)
    {
        try
        {
            await GetAccessToken();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("QQBot", accessToken);
                client.DefaultRequestHeaders.Add("X-Union-Appid", appId);
                var postData = new JObject
                {
                    ["content"] = "",
                    ["msg_type"] = 2, // MD
                    ["msg_id"] = args.MsgId,
                    ["msg_seq"] = seq,
                    ["keyboard"] = new JObject
                    {
                        ["id"] = keyid
                    }
                };
                var content = new StringContent(postData.ToString(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"https://api.sgroup.qq.com/v2/groups/{args.GroupOpenId}/messages", content);
                var responseString = await response.Content.ReadAsStringAsync();

                Message.Info("Sent GroupKeyBoardMessage Response: " + responseString);
            }
        }
        catch(Exception ex)
        {
            Message.Erro(ex.ToString());
        }
    }
    #region Message
    public void HandleMessageCreate(JToken data)//处理私聊被动消息部分
    {
        var author = data["author"]["user_openid"].Value<string>();
        var content = data["content"].Value<string>();
        var msgId = data["id"].Value<string>(); // 获取消息ID

        Message.Info($"Received Message from {author} : {content}");

        OnMessageReceived?.Invoke(this, new MessageEventArgs("无", content, msgId, author));
        //SendMessage(groupOpenId, $"{content}", msgId);
    }
    public async void SendMessage(string message, MessageEventArgs args, int seq = 1)
    {
        await GetAccessToken();
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("QQBot", accessToken);
            client.DefaultRequestHeaders.Add("X-Union-Appid", appId);

            var postData = new JObject
            {
                ["content"] = message,
                ["msg_type"] = 0, // 文本消息
                ["msg_id"] = args.MsgId,
                ["msg_seq"] = seq
            };
            var content = new StringContent(postData.ToString(), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"https://api.sgroup.qq.com/v2/users/{args.Author}/messages", content);
            var responseString = await response.Content.ReadAsStringAsync();

            Message.Info("Sent Message Response: " + responseString);
        }
    }
    #endregion

    #region Group
    public void HandleGroupAtMessageCreate(JToken data)//处理群聊被动消息部分
    {
        var author = data["author"]["member_openid"].Value<string>();
        var content = data["content"].Value<string>();
        var groupOpenId = data["group_openid"].Value<string>();
        var msgId = data["id"].Value<string>(); // 获取消息ID

        Message.Info($"Received GroupMessage from {author} in group {groupOpenId} : {content}");

        OnGroupMessageReceived?.Invoke(this, new MessageEventArgs(groupOpenId, content, msgId, author));
    }
    public async void SendGroupMessage(string message, MessageEventArgs args, int seq = 1)
    {
        await GetAccessToken();
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("QQBot", accessToken);
            client.DefaultRequestHeaders.Add("X-Union-Appid", appId);

            var postData = new JObject
            {
                ["content"] = message,
                ["msg_type"] = 0, // 文本消息
                ["msg_id"] = args.MsgId,
                ["msg_seq"] = seq
            };
            var content = new StringContent(postData.ToString(), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"https://api.sgroup.qq.com/v2/groups/{args.GroupOpenId}/messages", content);
            var responseString = await response.Content.ReadAsStringAsync();

            Message.Info("Sent GroupMessage Response: " + responseString);
        }
    }
    public async Task SendGroupMedia(MessageEventArgs args, string fileUrl,int seq = 1)
    {
        try
        {
            await GetAccessToken();
            Message.Info("尝试发送: " + fileUrl);

            if (string.IsNullOrEmpty(fileUrl))
            {
                throw new Exception("Failed to upload the file to the server.");
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("QQBot", accessToken);
                client.DefaultRequestHeaders.Add("X-Union-Appid", appId);

                var postData = new JObject
                {
                    ["file_type"] = 1,
                    ["url"] = fileUrl,
                    ["srv_send_msg"] = false,
                    ["msg_seq"] = seq
                };

                var content = new StringContent(postData.ToString(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"https://api.sgroup.qq.com/v2/groups/{args.GroupOpenId}/files", content);
                var responseString = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(responseString);

                //Message.Info(responseString);

                if (string.IsNullOrEmpty(jsonResponse["file_info"]?.Value<string>()))
                {
                    throw new Exception("Image upload failed: " + jsonResponse["message"]?.Value<string>());
                }

                string fileInfo = jsonResponse["file_info"]?.Value<string>();
                if (string.IsNullOrEmpty(fileInfo))
                {
                    throw new Exception("Failed to retrieve file_info from the response.");
                }

                var messageData = new JObject
                {
                    //["content"] = "", // Ensure content is not null or empty
                    ["msg_id"] = args.MsgId,
                    ["msg_type"] = 7,
                    ["media"] = new JObject
                    {
                        ["file_info"] = fileInfo
                    }
                };

                var messageContent = new StringContent(messageData.ToString(), Encoding.UTF8, "application/json");
                var messageResponse = await client.PostAsync($"https://api.sgroup.qq.com/v2/groups/{args.GroupOpenId}/messages", messageContent);
                var messageResponseString = await messageResponse.Content.ReadAsStringAsync();

                Message.Info("Sent Media GroupMessage Response: " + messageResponseString);
            }
        }
        catch (Exception ex)
        {
            Message.Erro("Error: " + ex.Message);
        }
    }
    public async Task SendGroupMedia(MessageEventArgs args, byte[] data,string text, int seq = 1, string name = "upload.jpg")
    {
        try
        {
            await GetAccessToken();
            string fileUrl = UploadImageToServer(data, name).Result;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("QQBot", accessToken);
                client.DefaultRequestHeaders.Add("X-Union-Appid", appId);
                var postData = new JObject
                {
                    ["file_type"] = 1,
                    ["url"] = fileUrl,
                    ["srv_send_msg"] = false,
                    ["msg_seq"] = seq
                };

                var content = new StringContent(postData.ToString(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"https://api.sgroup.qq.com/v2/groups/{args.GroupOpenId}/files", content);
                var responseString = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(responseString);

                if (string.IsNullOrEmpty(jsonResponse["file_info"]?.Value<string>()))
                {
                    throw new Exception("Image upload failed: " + jsonResponse["message"]?.Value<string>());
                }

                string fileInfo = jsonResponse["file_info"]?.Value<string>();
                if (string.IsNullOrEmpty(fileInfo))
                {
                    throw new Exception("Failed to retrieve file_info from the response.");
                }
                var messageData = new JObject
                {
                    ["content"] = text,
                    ["msg_id"] = args.MsgId,
                    ["msg_type"] = 7,
                    ["media"] = new JObject
                    {
                        ["file_info"] = fileInfo
                    },
                    ["image"] = fileUrl
                };

                var messageContent = new StringContent(messageData.ToString(), Encoding.UTF8, "application/json");
                var messageResponse = await client.PostAsync($"https://api.sgroup.qq.com/v2/groups/{args.GroupOpenId}/messages", messageContent);
                var messageResponseString = await messageResponse.Content.ReadAsStringAsync();
                FileServerDelete(name);
                Message.Info("Sent TextMedia Message Response: " + messageResponseString);
            }
        }
        catch (Exception ex)
        {
            Message.Erro("Error: " + ex.Message);
        }
    }
    public async Task SendGroupMedia(MessageEventArgs args, byte[] data, int seq = 1 ,string name = "upload.jpg")
    {
        try
        {
            await GetAccessToken();
            string fileUrl = UploadImageToServer(data, name).Result;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("QQBot", accessToken);
                client.DefaultRequestHeaders.Add("X-Union-Appid", appId);

                // Get file info for the uploaded file
                var postData = new JObject
                {
                    ["file_type"] = 1,
                    ["url"] = fileUrl,
                    ["srv_send_msg"] = false,
                    ["msg_seq"] = seq
                };

                var content = new StringContent(postData.ToString(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"https://api.sgroup.qq.com/v2/groups/{args.GroupOpenId}/files", content);
                var responseString = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(responseString);

                //Message.Info(responseString);

                if (string.IsNullOrEmpty(jsonResponse["file_info"]?.Value<string>()))
                {
                    throw new Exception("Image upload failed: " + jsonResponse["message"]?.Value<string>());
                }

                string fileInfo = jsonResponse["file_info"]?.Value<string>();
                if (string.IsNullOrEmpty(fileInfo))
                {
                    throw new Exception("Failed to retrieve file_info from the response.");
                }

                var messageData = new JObject
                {
                    //["content"] = "",
                    ["msg_id"] = args.MsgId,
                    ["msg_type"] = 7,
                    ["media"] = new JObject
                    {
                        ["file_info"] = fileInfo
                    }
                };

                var messageContent = new StringContent(messageData.ToString(), Encoding.UTF8, "application/json");
                var messageResponse = await client.PostAsync($"https://api.sgroup.qq.com/v2/groups/{args.GroupOpenId}/messages", messageContent);
                var messageResponseString = await messageResponse.Content.ReadAsStringAsync();
                FileServerDelete(name);
                Message.Info("Sent Media Message Response: " + messageResponseString);
            }
        }
        catch (Exception ex)
        {
            Message.Erro("Error: " + ex.Message);
        }
    }
    #endregion

    #region FileServer
    public void FileServerDelete(string name = "upload.jpg")
    {
        HttpClient httpClient = new HttpClient();
        string requestUrl = $"{MainSDK.BotConfig.FileServerUrl}/delete/{name}";
        httpClient.DeleteAsync(requestUrl);
    }
    public async Task<string> UploadImageToServer(byte[] data, string name = "upload.jpg")
    {
        using (var client = new HttpClient())
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(data, 0, data.Length);
                var fileContent = new ByteArrayContent(ms.ToArray());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                var content = new MultipartFormDataContent();
                content.Add(fileContent, "file", $"{name}");
                var response = await client.PostAsync($"{MainSDK.BotConfig.FileServerUrl}/upload", content);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(responseString);
                return jsonResponse["fileUrl"]?.ToString();
            }
        }
    }
    public async Task<string> UploadFileToServer(string filePath)
    {
        using (var client = new HttpClient())
        {
            var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            content.Add(fileContent, "file", Path.GetFileName(filePath));

            var response = await client.PostAsync($"{MainSDK.BotConfig.FileServerUrl}/upload", content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            var jsonResponse = JObject.Parse(responseString);
            return jsonResponse["fileUrl"]?.ToString();
        }
    }
    #endregion
}
public class MessageEventArgs : EventArgs
{
    public string Author { get; }
    public string GroupOpenId { get; }
    public string Content { get; }
    public string MsgId { get; }
    public List<Attachment> Attachments { get; }

    public MessageEventArgs(string groupOpenId, string content, string msgId, string author)
    {
        GroupOpenId = groupOpenId;
        Content = content;
        MsgId = msgId;
        Author = author;
    }
}
public class Attachment
{

}

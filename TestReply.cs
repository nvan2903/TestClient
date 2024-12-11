using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using Newtonsoft.Json;
using TestClient;

namespace TestCient
{
    class TestReply
    {
        static void Main(string[] args)
        {
            string server = "localhost"; // Đổi thành địa chỉ server của bạn
            int port = 25; // Cổng SMTP mặc định là 25

            try
            {
                using TcpClient client = new TcpClient(server, port);
                using NetworkStream stream = client.GetStream();
                using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                // Đọc lời chào từ server
                string response = reader.ReadLine();
                Console.WriteLine($"Server: {response}");

                // Gửi lệnh HELO
                var heloCommand = new { Command = "HELO" };
                writer.WriteLine(JsonConvert.SerializeObject(heloCommand));
                response = reader.ReadLine();
                Console.WriteLine($"Server: {response}");

                // Gửi lệnh MAIL FROM
                var mailFromCommand = new
                {
                    Command = "MAIL FROM",
                    Email = "trungnd.21it@vku.udn.vn"
                };
                writer.WriteLine(JsonConvert.SerializeObject(mailFromCommand));
                response = reader.ReadLine();
                Console.WriteLine($"Server: {response}");

                // Gửi lệnh RCPT TO
                var rcptToCommand = new
                {
                    Command = "RCPT TO",
                    Email = "vantn.21it@vku.udn.vn"
                };
                writer.WriteLine(JsonConvert.SerializeObject(rcptToCommand));
                response = reader.ReadLine();
                Console.WriteLine($"Server: {response}");

                // Gửi lệnh REPLY
                var replyCommand = new
                {
                    Command = "REPLY",
                    Mailid = 307
                };
                writer.WriteLine(JsonConvert.SerializeObject(replyCommand));
                response = reader.ReadLine();
                Console.WriteLine($"Server: {response}");

                // Gửi lệnh DATA
                var dataCommand = new
                {
                    Command = "DATA",
                    Subject = "Trung Reply Mail To Van",
                    Content = "Trung has reply"
                };
                writer.WriteLine(JsonConvert.SerializeObject(dataCommand));
                response = reader.ReadLine();
                Console.WriteLine($"Server: {response}");

                // Gửi lệnh ATTACH (đính kèm tệp)
                var attachCommand = new
                {
                    Command = "ATTACH",
                    FilePath = "Attachment.txt" // Thay đường dẫn bằng file thực tế
                };
                writer.WriteLine(JsonConvert.SerializeObject(attachCommand));
                response = reader.ReadLine();
                Console.WriteLine($"Server: {response}");

                // Gửi lệnh QUIT
                var quitCommand = new { Command = "QUIT" };
                writer.WriteLine(JsonConvert.SerializeObject(quitCommand));
                response = reader.ReadLine();
                Console.WriteLine($"Server: {response}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        // Hàm gửi lệnh và xử lý phản hồi
        static void SendCommand(StreamWriter writer, StreamReader reader, object command)
        {
            string jsonCommand = JsonConvert.SerializeObject(command);
            writer.WriteLine(jsonCommand);
            Console.WriteLine($"Client: {jsonCommand}");
        }

        // Hàm đọc và in phản hồi từ server
        static void ReadAndPrintServerResponse(StreamReader reader)
        {
            try
            {
                string response = reader.ReadLine();
                while (response != null)
                {
                    ServerResponse aPIService = new ServerResponse();
                    aPIService = ServerResponse.FromJson(response);

                    Console.WriteLine($"Server Status = {aPIService.Status}");
                    Console.WriteLine($"Server Message = {aPIService.Message}");
                    if (aPIService.Identify != null)
                    {
                        status = aPIService.Status;
                        identify = aPIService.Identify;
                    }

                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading response: {ex.Message}");
            }
        }

        // Hàm định dạng JSON
        static string FormatJson(string response)
        {
            try
            {
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(response);
                return JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            }
            catch
            {
                return response; // Nếu không parse được JSON, trả về chuỗi gốc
            }
        }
    }
}

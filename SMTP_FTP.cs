using System;
using System.ComponentModel.Design;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace TestClient
{
    class SMTP_FTP
    {
        private static string status;
        private static string identify;

        static void Main(string[] args)
        {

            string server = "localhost"; // Đổi thành địa chỉ server của bạn
            int port = 25; // Cổng SMTP mặc định là 25
            int ftpPort = 21; // Cổng FTP mặc định là 21
            try
            {
                using TcpClient client = new TcpClient(server, port);
                using NetworkStream stream = client.GetStream();
                using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                // Đọc lời chào từ server
                ReadAndPrintServerResponse(reader);

                // Gửi lệnh HELO
                var heloCommand = new { Command = "HELO" };
                SendCommand(writer, reader, heloCommand);
                ReadAndPrintServerResponse(reader);

                // Gửi lệnh MAIL FROM
                var mailFromCommand = new
                {
                    Command = "MAIL FROM",
                    Email = "vantn.21it@vku.udn.vn"
                };
                SendCommand(writer, reader, mailFromCommand);
                ReadAndPrintServerResponse(reader);

                // Gửi lệnh RCPT TO
                var rcptToCommand = new
                {
                    Command = "RCPT TO",
                    Email = "trungnd.21it@vku.udn.vn"
                };
                SendCommand(writer, reader, rcptToCommand);
                ReadAndPrintServerResponse(reader);

                // Gửi lệnh DATA
                var dataCommand = new
                {
                    Command = "DATA",
                    Subject = "CHECK SMTP AND FTP",
                    Content = "Van sent mail for trung to check smtp and ftp"
                };
                SendCommand(writer, reader, dataCommand);
                ReadAndPrintServerResponse(reader);


                // Gửi lệnh ATTACH (đính kèm tệp)
                var attachCommand = new
                {
                    Command = "ATTACH",
                    Filename = "pic1.jpg" // Thay đường dẫn bằng file thực tế
                };
                SendCommand(writer, reader, attachCommand);
                ReadAndPrintServerResponse(reader);

                // Gửi lệnh QUIT
                var quitCommand = new { Command = "QUIT" };
                SendCommand(writer, reader, quitCommand);
                ReadAndPrintServerResponse(reader);



                // Kiểm tra giá trị Identify và Status
                if (status == "OK" && !string.IsNullOrEmpty(identify))
                {
                    Console.WriteLine($"Server responded with Identify: {identify}, Status: {status}. Initiating FTP connection...");

                    // Tạo socket mới để gửi lệnh FTP
                    using TcpClient ftpClient = new TcpClient(server, ftpPort);
                    using NetworkStream ftpStream = ftpClient.GetStream();
                    using StreamReader ftpReader = new StreamReader(ftpStream, Encoding.UTF8);
                    using StreamWriter ftpWriter = new StreamWriter(ftpStream, Encoding.UTF8) { AutoFlush = true };


                    // Gửi lệnh FTP (ví dụ)
                    var ftpCommand = new
                    {
                        Command = "FTP",
                        Sender = "vantn.21it@vku.udn.vn",
                        Recipient = "trungnd.21it@vku.udn.vn"
                    };
                    SendCommand(ftpWriter, ftpReader, ftpCommand);
                    ReadAndPrintServerResponse(ftpReader);


                    // Prepare file path and PUT command
                    string filePath = "D:\\MailBox\\pic1.jpg"; // File path to send
                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine($"File not found: {filePath}");
                        return;
                    }

                    var putCommand = new
                    {
                        Command = "PUT",
                        Identify = identify,
                        Filename = Path.GetFileName(filePath)
                    };

                    // Send the PUT command
                    SendCommand(ftpWriter, ftpReader, putCommand);

                    // Open the file for reading
                    using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    byte[] buffer = new byte[64 * 1024]; // Buffer size for efficient data transfer
                    int bytesRead;

                    try
                    {
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ftpStream.Write(buffer, 0, bytesRead); // Write file data to the FTP stream
                        }
                        ftpStream.Flush(); // Ensure all data is sent to the server
                        Console.WriteLine("File upload completed.");
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Error during file transfer: {ex.Message}");
                    }
                    ReadAndPrintServerResponse(ftpReader);

                    // Send QUIT command to FTP server
                    var ftpQuitCommand = new { Command = "QUIT" };
                    SendCommand(ftpWriter, ftpReader, ftpQuitCommand);
                    ReadAndPrintServerResponse(ftpReader);


                }
                else
                {
                    Console.WriteLine("Invalid server response. Status is not OK or Identify is missing.");
                }



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

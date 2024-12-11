//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading.Tasks;
//using TestCient;

//namespace TestClient
//{
//    internal class IMAP_FTP
//    {
//        static void Main(string[] args)
//        {

//            string server = "localhost"; // Đổi thành địa chỉ server của bạn
//            int imapPort = 143; // Cổng SMTP mặc định là 25
//            int ftpPort = 21; // Cổng FTP mặc định là 21
//            try
//            {
//                using TcpClient client = new TcpClient(server, imapPort);
//                using NetworkStream stream = client.GetStream();
//                using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
//                using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };



//                // Gửi lệnh CAPABILITY
//                var capabilityCommand = new { Command = "CAPABILITY" };
//                SendCommand(writer, reader, capabilityCommand);
//                ReadAndPrintServerResponse(reader);

//                // Gửi lệnh REGISTER
//                var registerCommand = new
//                {
//                    Command = "REGISTER",
//                    Username = "vinv.21it",
//                    Fullname = "Nguyễn Văn Vĩ",
//                    Password = "12345678"
//                };
//                SendCommand(writer, reader, registerCommand);
//                ReadAndPrintServerResponse(reader);

//                // Gửi lệnh LOGIN
//                var loginCommand = new
//                {
//                    Command = "LOGIN",
//                    Username = "trungnd.21it",
//                    Password = "12345678"
//                };
//                SendCommand(writer, reader, loginCommand);
//                ReadAndPrintServerResponse(reader);

//                //// Gửi lệnh CHNAME để đổi tên
//                //var chnameCommand = new
//                //{
//                //    Command = "CHNAME",
//                //    Newfullname = "Văn Tào Nguyên"
//                //};
//                //writer.WriteLine(JsonConvert.SerializeObject(chnameCommand));
//                //response = reader.ReadLine();
//                //Console.WriteLine($"Server: {response}");

//                //// Gửi lệnh CHPASS để đổi mật khẩu
//                //var chpassCommand = new
//                //{
//                //    Command = "CHPASS",
//                //    Oldpassword = "12345678",
//                //    Newpassword = "87654321"
//                //};
//                //writer.WriteLine(JsonConvert.SerializeObject(chpassCommand));
//                //response = reader.ReadLine();
//                //Console.WriteLine($"Server: {response}");

//                // Gửi lệnh SELECT để lấy danh sách email trong INBOX
//                var selectCommand = new
//                {
//                    Command = "SELECT",
//                    Mailbox = "INBOX"
//                };
//                SendCommand(writer, reader, selectCommand);
//                //ReadAndPrintServerResponse(reader);
//                try
//                {
//                    string response = reader.ReadLine();
//                    while (response != null)
//                    {
//                        ServerResponse aPIService = new ServerResponse();
//                        aPIService = ServerResponse.FromJson(response);

//                        List<Mail> mails = aPIService.Mails;

//                        Console.WriteLine($"Server Status = {aPIService.Status}");
//                        Console.WriteLine($"Server Message = {aPIService.Message}");

//                        foreach(Mail mail in mails)
//                        {
//                            Console.WriteLine($"Mail ID: {mail.Id}");
//                            Console.WriteLine($"Sender: {mail.Sender}");
//                            Console.WriteLine($"Receiver: {mail.Receiver}");
//                            Console.WriteLine($"Owner: {mail.Owner}");
//                            Console.WriteLine($"IsRead: {mail.IsRead}");
//                            Console.WriteLine($"Attachment: {mail.Attachment}");
//                            Console.WriteLine($"Subject: {mail.Subject}");
//                            Console.WriteLine($"Content: {mail.Content}");
//                            Console.WriteLine($"-----------------------------------");
//                        }


//                        break;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"Error reading response: {ex.Message}");
//                }


//                //// Send SELECT SENT command
//                //var selectSentCommand = new
//                //{
//                //    Command = "SELECT",
//                //    Mailbox = "SENT"
//                //};
//                //SendCommand(writer, reader, selectSentCommand);
//                //ReadAndPrintServerResponse(reader);


//                //// Send SELECT TRASH command
//                //var selectTrashCommand = new
//                //{
//                //    Command = "SELECT",
//                //    Mailbox = "TRASH"
//                //};
//                //SendCommand(writer, reader, selectTrashCommand);
//                //ReadAndPrintServerResponse(reader);

//                //// Send SELECT ALL command
//                //var selectAllCommand = new
//                //{
//                //    Command = "SELECT",
//                //    Mailbox = "ALL"
//                //};
//                //string jsonSelectAllCommand = JsonConvert.SerializeObject(selectAllCommand);
//                //writer.WriteLine(jsonSelectAllCommand);
//                //SendCommand(writer, reader, selectAllCommand);
//                //ReadAndPrintServerResponse(reader);



//                // Gửi lệnh FETCH để lấy thông tin email
//                var fetchCommand = new
//                {
//                    Command = "FETCH",
//                    Mailid = 307
//                };
//                SendCommand(writer, reader, fetchCommand);
//                //ReadAndPrintServerResponse(reader);
//                try
//                {
//                    var response = reader.ReadLine();
//                    while (response != null)
//                    {
//                        ServerResponse aPIService = new ServerResponse();
//                        aPIService = ServerResponse.FromJson(response);

//                        Console.WriteLine($"Server Status = {aPIService.Status}");
//                        Console.WriteLine($"Server Message = {aPIService.Message}");

//                        Mail mail = aPIService.Mails[0];

//                        //Console.WriteLine($"Mail ID: {mail.Id}");
//                        Console.WriteLine($"Sender: {mail.Sender}");
//                        Console.WriteLine($"Receiver: {mail.Receiver}");
//                        //Console.WriteLine($"Owner: {mail.Owner}");
//                        Console.WriteLine($"IsRead: {mail.IsRead}");
//                        //Console.WriteLine($"Attachment: {mail.Attachment}");
//                        Console.WriteLine($"Subject: {mail.Subject}");
//                        //Console.WriteLine($"Content: {mail.Content}");
//                        Console.WriteLine($"Created At : {mail.CreatedAt}");

//                        break;
//                    }

//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"Error reading response: {ex.Message}");
//                }





//                // Gửi lệnh LOGOUT
//                var logoutCommand = new { Command = "LOGOUT" };
//                SendCommand(writer, reader, logoutCommand);
//                ReadAndPrintServerResponse(reader);




//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error: {ex.Message}");
//            }


//        }

//        // Hàm gửi lệnh và xử lý phản hồi
//        static void SendCommand(StreamWriter writer, StreamReader reader, object command)
//        {
//            string jsonCommand = JsonConvert.SerializeObject(command);
//            writer.WriteLine(jsonCommand);
//            Console.WriteLine($"Client: {jsonCommand}");
//        }

//        // Hàm đọc và in phản hồi từ server
//        static void ReadAndPrintServerResponse(StreamReader reader)
//        {
//            try
//            {
//                string response = reader.ReadLine();
//                while (response != null)
//                {
//                    ServerResponse aPIService = new ServerResponse();
//                    aPIService = ServerResponse.FromJson(response);

//                    Console.WriteLine($"Server Status = {aPIService.Status}");
//                    Console.WriteLine($"Server Message = {aPIService.Message}");

//                    break;
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error reading response: {ex.Message}");
//            }
//        }

//        // Hàm định dạng JSON
//        static string FormatJson(string response)
//        {
//            try
//            {
//                var jsonObject = JsonConvert.DeserializeObject<dynamic>(response);
//                return JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
//            }
//            catch
//            {
//                return response; // Nếu không parse được JSON, trả về chuỗi gốc
//            }
//        }


//    }
//}


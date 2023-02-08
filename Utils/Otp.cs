using AntalyaTaksiAccount.Models;

using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AntalyaTaksiAccount.Utils
{
    public class Otp
    {

        private readonly ATAccountContext _aTAccountContext;
        string rootDir = "./Content/OtpMessage/";
        public Otp(ATAccountContext aTAccountContext)
        {
            _aTAccountContext = aTAccountContext;
            
        }

        public string SendOtp(VerimorOtpSend verimorOtpSend)
        {
            var smsIstegi = new SmsIstegi();
            smsIstegi.username = "908502420134";
            smsIstegi.password = "6PrP7SY2Wd";
            smsIstegi.source_addr = "IPOS";
            smsIstegi.messages = new Mesaj[] { new Mesaj(verimorOtpSend.Mesaj, verimorOtpSend.Phone) };
            IstegiGonder(smsIstegi);
            /**/
            return JsonConvert.SerializeObject(verimorOtpSend);
        }

        private void IstegiGonder(SmsIstegi istek)
        {
            string payload = JsonConvert.SerializeObject(istek);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            wc.Headers["Content-Type"] = "application/json";
            try
            {
                string campaign_id = wc.UploadString("http://sms.verimor.com.tr/v2/send.json", payload);
                string mesaj = "Mesaj Gönderildi, kampanya_id" + campaign_id;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError) // 400 hataları
                {
                    var responseBody = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    string mesaj = "Mesaj gönderilemedi, dönen hata: " + responseBody;
                }
                else // diğer hatalar
                {
                    var mesaj = ex.Status;
                    throw;
                }
            }
        }

        public string CheckOtpSendMethod(CheckOtpDto checkOtpDto)
        {
            int number = 0;

            Random random = new Random();
            number = random.Next(100000, 900000);

            VerimorOtpSend otpSend = new VerimorOtpSend();
            otpSend.Mesaj = "Guvenliginiz icin onay kodunuzu kimse ile paylasmayiniz onay kodunuz: " + number;
            otpSend.Phone = "90" + checkOtpDto.Phone;
            var res = SendOtp(otpSend);
            /**/
            try
            {
                checkOtpDto.OtpMessage = $"{number}";
                string serializeOtp = JsonConvert.SerializeObject(checkOtpDto);


                if (!Directory.Exists(rootDir))
                {
                    Directory.CreateDirectory(rootDir);
                }

                string fileName = $"{checkOtpDto.UserID}_{checkOtpDto.Phone}.json";
                string filePath = Path.Combine(rootDir, fileName);
                File.WriteAllText(filePath, serializeOtp);

            }
            catch (Exception ex) 
            {

                var mesaj = ex.Message;
            }

            return JsonConvert.SerializeObject(otpSend);

        }



        public string CheckOtpVerification(CheckOtpDto checkOtpDto)
        {
           
            if (Directory.Exists(rootDir))
            {
                string newfile = $"{rootDir}{checkOtpDto.UserID}_{checkOtpDto.Phone}.json";
                string messageJson = File.ReadAllText(newfile);

                CheckOtpDto readOtp = JsonConvert.DeserializeObject<CheckOtpDto>(messageJson);

                if (checkOtpDto.OtpMessage == readOtp.OtpMessage && checkOtpDto.Phone == readOtp.Phone)
                {
                    File.Delete(newfile);
                    AllUser user = _aTAccountContext.AllUsers.Where(p => p.Phone == checkOtpDto.Phone).First();

                    return JsonConvert.SerializeObject(checkOtpDto);
                }
                return JsonConvert.SerializeObject(checkOtpDto);
            }
            return JsonConvert.SerializeObject(checkOtpDto);
        }
        class Mesaj
        {
            public string msg { get; set; }
            public string dest { get; set; }

            public Mesaj() { }

            public Mesaj(string msg, string dest)
            {
                this.msg = msg;
                this.dest = dest;
            }
        }
        class SmsIstegi
        {
            public string username { get; set; }
            public string password { get; set; }
            public string source_addr { get; set; }
            public Mesaj[] messages { get; set; }
        }
    }
}


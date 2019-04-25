using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PaytureTest
{
    [TestClass]
    public class PaytureApiTest
    {
        private const string URL = "https://sandbox3.payture.com/api/Block";
        [DataTestMethod]
        [DataRow(1050, "Merchant", 5218851946955484, 12, 21, "Ivan Ivanov", 123, true)]
        [DataRow(1755, "Merchant", 3300000000000001, 12, 22, "Ivan Ivanov", 521, false)]
        [DataRow(2184, "Merchant", 5218851946955484, 12, 21, "Ivan Ivanov", 123, true)]

        public void BlockTest(int amount, string key, long PAN, int eMonth, int eYear, string cardHolder, int secureCode, bool successStatus)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            //building uri
            string orderId = Guid.NewGuid().ToString("N");//generate Globally Unique Identifier
            var builder = new UriBuilder(URL);
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["Key"] = key;
            query["OrderId"] = orderId;
            query["Amount"] = amount.ToString();
            string payInfo = $"PAN={PAN}; EMonth={eMonth}; EYear={eYear}; CardHolder={cardHolder}; " +
                $"SecureCode={secureCode}; OrderId = {orderId}; Amount = {amount};";
            query["PayInfo"] = payInfo;
            builder.Query = query.ToString();
            string urlParameters = builder.ToString();
            Console.WriteLine(urlParameters);

            //Sending request
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                var responseText = response.Content.ReadAsStringAsync().Result;  
                Console.WriteLine(responseText);
                string blockSuccess;
                
                //Finding "Success" parameter
                Match match = Regex.Match(responseText, "(?<=Success=\")(.*?)(?=\")");
                if (match.Success)
                {
                    blockSuccess = match.Value;
                }
                else
                {
                    throw new Exception("Incorrect response");
                }
                                
                Assert.IsTrue(blockSuccess == successStatus.ToString());
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            client.Dispose();
        }
    }
}

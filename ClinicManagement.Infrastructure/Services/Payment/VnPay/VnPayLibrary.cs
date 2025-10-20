using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ClinicManagement.Infrastructure.Services.Payment.VNPAY
{
    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData = new(new VnPayCompare());
        private readonly SortedList<string, string> _responseData = new(new VnPayCompare());

        public const string VERSION = "2.1.0";

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                _requestData[key] = value;
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                _responseData[key] = value;
        }

        // ✅ Tạo URL thanh toán VNPay đúng chuẩn
        public string CreateRequestUrl(string baseUrl, string hashSecret, bool encodeHash = false)
        {
            var queryString = new StringBuilder();
            var hashData = new StringBuilder();

            foreach (var kv in _requestData)
            {
                if (string.IsNullOrWhiteSpace(kv.Value)) continue;

                var encodedKey = WebUtility.UrlEncode(kv.Key);
                var encodedValue = WebUtility.UrlEncode(kv.Value);

                queryString.Append($"{encodedKey}={encodedValue}&");

                if (encodeHash)
                {
                    if (kv.Key == "vnp_IpnUrl")
                        hashData.Append($"{encodedKey}={kv.Value}&");
                    else
                        hashData.Append($"{encodedKey}={encodedValue}&");
                }
                else
                {
                    hashData.Append($"{kv.Key}={kv.Value}&");
                }
            }

            if (queryString.Length > 0) queryString.Length--;
            if (hashData.Length > 0) hashData.Length--;

            string secureHash = ComputeHmacSHA512(hashSecret, hashData.ToString());

            return $"{baseUrl}?{queryString}&vnp_SecureHashType=SHA512&vnp_SecureHash={secureHash}";
        }

        // ✅ Validate chữ ký trả về từ VNPay
        public bool ValidateSignature(string inputHash, string secretKey, bool encodeHash = false)
        {
            var clone = new SortedList<string, string>(_responseData, new VnPayCompare());
            clone.Remove("vnp_SecureHashType");
            clone.Remove("vnp_SecureHash");

            var hashData = new StringBuilder();

            foreach (var kv in clone)
            {
                if (string.IsNullOrWhiteSpace(kv.Value)) continue;

                if (encodeHash)
                    hashData.Append($"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}&");
                else
                    hashData.Append($"{kv.Key}={kv.Value}&");
            }

            if (hashData.Length > 0)
                hashData.Length--;

            var computedHash = ComputeHmacSHA512(secretKey, hashData.ToString());

            Console.WriteLine("🔍 RawData: " + hashData);
            Console.WriteLine("🔑 Computed: " + computedHash);
            Console.WriteLine("🔐 Input:    " + inputHash);

            return computedHash.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private static string ComputeHmacSHA512(string key, string input)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(input);

            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(inputBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }


    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            var comparer = System.Globalization.CompareInfo.GetCompareInfo("en-US");
            return comparer.Compare(x, y, System.Globalization.CompareOptions.Ordinal);
        }
    }
}

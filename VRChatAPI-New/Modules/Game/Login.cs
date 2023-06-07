using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VRChatAPI_New.Exception;
using VRChatAPI_New.Models;

namespace VRChatAPI_New.Modules.Game
{
    public static class Login
    {
        public static async Task<VRChatUserInfo> LoginWithTokenAsync(string auth, string twoFactorAuth)
        {
            StaticGameValues.CheckSetup();

            if (!string.IsNullOrEmpty(twoFactorAuth))
            {
                StaticGameValues.CookieContainer.Add(StaticGameValues.ApiUrl, new Cookie("twoFactorAuth", twoFactorAuth));
                StaticGameValues.TwoFactorKey = twoFactorAuth;
            }
            StaticGameValues.CookieContainer.Add(StaticGameValues.ApiUrl, new Cookie("auth", auth));
            StaticGameValues.AuthKey = auth;

            var webResponse = await HttpRequests.GetStringAsync("auth/user?organization=vrchat").ConfigureAwait(false);
            if (webResponse.ToLower().Contains("missing credentials"))
            {
                throw new InvalidLogin("Invalid token");
            }
            VRChatUserInfo info = JsonConvert.DeserializeObject<VRChatUserInfo>(webResponse);
            info.Details = new LoginAuth
            {
                AuthKey = auth,
                TwoFactorKey = twoFactorAuth
            };
            return info;
        }

        public static async Task<VRChatUserInfo> LoginWithCredentials(string username, string password, string twoFactorCode)
        {
            StaticGameValues.CheckSetup();

            StaticGameValues.HttpClient.DefaultRequestHeaders.Remove("Authorization");
            StaticGameValues.HttpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Uri.EscapeDataString(username)}:{Uri.EscapeDataString(password)}"))}");
            var webResponse = await HttpRequests.GetStringAsync("auth/user?organization=vrchat").ConfigureAwait(false);
            if (webResponse.ToLower().Contains("missing credentials") || webResponse.ToLower().Contains("invalid username/email"))
            {
                throw new InvalidLogin("Invalid user credentials");
            }

            if (webResponse.ToLower().Contains("requirestwofactorauth"))
            {
                TwoFactorAuth twoFactorAuth = JsonConvert.DeserializeObject<TwoFactorAuth>(webResponse);
                string twoFactorType = twoFactorAuth.RequiresTwoFactorAuth.FirstOrDefault().ToLower();
                if (string.IsNullOrEmpty(twoFactorCode))
                {
                    twoFactorCode = AskInputAuthCode(twoFactorType);
                }

                if (string.IsNullOrEmpty(twoFactorCode))
                {
                    StaticGameValues.HttpClient.DefaultRequestHeaders.Remove("Authorization");
                    throw new NoTwoFactor("No two factor code has been entered");
                }

                StaticGameValues.HttpClient.DefaultRequestHeaders.Authorization = null;
                var twoFactor = JsonConvert.SerializeObject(new _2FACode(twoFactorCode));
                var twoFactorResponse = await HttpRequests.PostAsync($"auth/twofactorauth/{twoFactorType}/verify?organization=vrchat", StaticGameValues.JsonToHtmlContent(twoFactor)).ConfigureAwait(false);
                if (twoFactorResponse.ToLower().Contains("false"))
                {
                    StaticGameValues.HttpClient.DefaultRequestHeaders.Remove("Authorization");
                    throw new NoTwoFactor("Couldn't verify 2FA!");
                }
            }
            var authedResponse = await HttpRequests.GetStringAsync("auth/user?organization=vrchat").ConfigureAwait(false);
            ProcessCookies();
            VRChatUserInfo info = JsonConvert.DeserializeObject<VRChatUserInfo>(authedResponse);
            info.Details = new LoginAuth { AuthKey = StaticGameValues.AuthKey, TwoFactorKey = StaticGameValues.TwoFactorKey };
            return info;
        }

        private static void ProcessCookies()
        {
            foreach (Cookie cookie in StaticGameValues.CookieContainer.GetCookies(StaticGameValues.ApiUrl))
            {
                if (cookie.Name.Equals("auth", StringComparison.OrdinalIgnoreCase))
                {
                    StaticGameValues.AuthKey = cookie.Value;
                }
                if (cookie.Name.Equals("twoFactorAuth", StringComparison.OrdinalIgnoreCase))
                {
                    StaticGameValues.TwoFactorKey = cookie.Value;
                }
            }
        }

        public async static void Logout()
        {
            StaticGameValues.AuthKey = null;
            StaticGameValues.TwoFactorKey = null;
            await HttpRequests.PutAsync("logout?organization=vrchat", new StringContent("{}", Encoding.UTF8, "application/json")).ConfigureAwait(false);
        }

        private static string AskInputAuthCode(string twoFactorType)
        {
            if (twoFactorType == "emailotp")
            {
                MessageBox.Show("VRChat has emailed you your 2FA code.\nPlease check and then enter it on the next prompt");
            }
            string input = Interaction.InputBox($"Enter your 2FA Code for type {twoFactorType}", "2FA Code", "");
            return input;
        }
    }
}
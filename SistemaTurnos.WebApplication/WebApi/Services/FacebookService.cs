using RestSharp;
using SistemaTurnos.WebApplication.WebApi.Dto.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaTurnos.WebApplication.WebApi.Services
{
    public class FacebookService
    {
        public static string GetUserId(string token)
        {
            var client = new RestClient("https://graph.facebook.com/me?fields=id&access_token=" + token);
            var request = new RestRequest("", Method.GET);
            var response = client.Execute<TokenResponseDto>(request);
            return response.Data.id;
        }
    }
}

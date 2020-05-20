using LT.HttpClientHelper.TestConsole.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace LT.HttpClientHelper.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test async method
            TestCall();

            Console.ReadKey();
        }

        public static async void TestCall()
        {
            var httpClient = new MainHttpClient("https://xx.xxxxx.xxx/OAuth/api/");
            var classToPost = new SigIn { UserName = "MRighi", Password = "password1" };


            var resultSignIn = await httpClient.Invoke<SigIn, SignInResult>("authentication/sign-in",
                                                                System.Net.Http.HttpMethod.Post,
                                                                classToPost);
            if (resultSignIn.Response.IsSuccessStatusCode)
            {

                //RICERCO DA AUTENTICATO
                var auth = new AuthenticationHeaderValue(resultSignIn.Data.TokenType, resultSignIn.Data.AccessToken);
                var resultSearch = await httpClient.Invoke<SigIn, List<UserContract>>("user/search",
                                                                    System.Net.Http.HttpMethod.Post,
                                                                    classToPost,
                                                                    auth);

                var classToPost2 = new UserContract();

                var deleteResult = await httpClient.InvokeNoResponse<UserContract>("user/delete",
                                                                    System.Net.Http.HttpMethod.Post,
                                                                    classToPost2,
                                                                    auth);

                var user = await httpClient.InvokeNoRequest<UserContract>("user/get",
                                                                    System.Net.Http.HttpMethod.Get,
                                                                    "?id=1",
                                                                    auth);

            }
        }

    }
}

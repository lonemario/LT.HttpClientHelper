using LT.HttpClientHelper.TestConsole.Models;
using LT.HttpClientHelper.TestConsole.Models.Response;
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

        //public static async void TestCall()
        //{
        //    var httpClient = new MainHttpClient("http://192.168.44.69/OAuth/api/");
        //    var classToPost = new SigIn { UserName = "MRighi", Password = "password1" };


        //    var resultSignIn = await httpClient.Invoke<SigIn, SignInResult>("authentication/sign-in",
        //                                                        System.Net.Http.HttpMethod.Post,
        //                                                        classToPost);
        //    if (resultSignIn.Response.IsSuccessStatusCode)
        //    {
        //        //var token = resultSignIn.Data.TokenType + " " + resultSignIn.Data.AccessToken;
        //        //var token = "Bearer " + resultSignIn.Data.AccessToken;

        //        //RICERCO DA AUTENTICATO
        //        var auth = new AuthenticationHeaderValue(resultSignIn.Data.TokenType, resultSignIn.Data.AccessToken);
        //        var resultSearch = await httpClient.Invoke<SigIn, List<UserContract>>("user/search",
        //                                                            System.Net.Http.HttpMethod.Post,
        //                                                            classToPost,
        //                                                            auth);


        //        var deleteResoult = await httpClient.Invoke<SigIn>("user/delete",
        //                                                            System.Net.Http.HttpMethod.Post,
        //                                                            classToPost,
        //                                                            auth);
        //    }
        //}


        public static async void TestCall()
        {
            var httpClient = new MainHttpClient("https://XXXXXXXXXXXXXX/ws/XXXXXXX.dll");

            var query = string.Empty;
            //if (first.HasValue)
            //    query = string.Format("first={0}&", first.Value.ToString());
            //if (skip.HasValue)
            //    query = string.Format("skip={0}&", skip.Value.ToString());
            //if (idSessione.HasValue)
            //    query = string.Format("ID_SESSIONE={0}&", idSessione.Value.ToString());
            //if (!string.IsNullOrWhiteSpace(ordine))
            //    query = string.Format("ordine={0}&", ordine.ToUpper());
            //if (!string.IsNullOrWhiteSpace(query))
            //{
            //    var i = query.Length - 1;
            //    query = string.Format("/pagamenti/elenco/gestionali?{0}", query.Substring(0, i));
            //}
            //else
            //    query = "/pagamenti/elenco/gestionali";

            AuthenticationHeaderValue header = new AuthenticationHeaderValue("Cookie", "dtsessionid=BAD85A76119045CCBA37FFDCC4334A24");

            var resultRequest = await httpClient.Invoke<RichiestaPagamentoResponse>("/pagamenti/elenco/gestionali",
                                                                System.Net.Http.HttpMethod.Get,
                                                                "",
                                                                header);


            //if (resultRequest.Data != null)
            //    return resultRequest.Data;

            //var result = new RichiestaPagamentoResponse();
            //var message = new List<MessaggiNode>();
            //message.Add(new MessaggiNode
            //{
            //    COD = 54,
            //    MEX = resultRequest.Response.ReasonPhrase
            //});
            //result.MEX_ERR = message;
            //return result;

        }
    }
}

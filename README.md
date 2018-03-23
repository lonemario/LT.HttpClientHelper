# LT.HttpClientHelper
[![NuGet](https://img.shields.io/nuget/v/Nuget.Core.svg)](https://www.nuget.org/packages/LT.HttpClientHelper)

Helper to manage the json serialization/deserialization entities over http in .NET Standard 2.0

## Prerequisites

### .NETStandard 2.0
```
Newtonsoft.Json (>= 11.0.1)
```

### Example 
```c#
public static async void Authenticate()
{
    var httpClient = new MainHttpClient("http://myOaut.com/OAuth/api/");
    var classToPost = new SigIn { UserName = "MRighi", Password = "password1" };


    var resultSignIn = await httpClient.Invoke<SigIn, SignInResult>("authentication/sign-in",
                                                        System.Net.Http.HttpMethod.Post,
                                                        classToPost);
    if (resultSignIn.Response.IsSuccessStatusCode)
    {

        //AUTHENTICATED SEARCH
        var auth = new AuthenticationHeaderValue(resultSignIn.Data.TokenType, resultSignIn.Data.AccessToken);
        var resultSearch = await httpClient.Invoke<SigIn, List<UserContract>>("user/search",
                                                            System.Net.Http.HttpMethod.Post,
                                                            null,
                                                            auth);
    }
}
```
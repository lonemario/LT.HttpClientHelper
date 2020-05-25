using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using LT.HttpClientHelper.Models;

namespace LT.HttpClientHelper
{
    public interface IMainHttpClient
    {
        Uri BaseAddress { get; }
        string BaseAddressString { get; set; }

        void Dispose();
        Task<HttpResponseMessage<TResponse>> Invoke<TRequest, TResponse>(string partialUrl, HttpMethod method, TRequest request = null, AuthenticationHeaderValue authentication = null, string acceptedResponseType = null, Encoding encodingType = null, string contentMediaType = null)
            where TRequest : class, new()
            where TResponse : class, new();
        Task<HttpResponseMessage> Invoke<TRequest>(string partialUrl, HttpMethod method, TRequest request, AuthenticationHeaderValue authentication = null, string acceptedResponseType = null, Encoding encodingType = null, string contentMediaType = null) where TRequest : class, new();
        Task<HttpResponseMessage<TResponse>> Invoke<TResponse>(string partialUrl, HttpMethod method, string queryStringParameter = null, AuthenticationHeaderValue authentication = null, string acceptedResponseType = null, Encoding encodingType = null, string contentMediaType = null) where TResponse : class, new();
        Task<HttpResponseMessage<TResponse>> InvokeNoRequest<TResponse>(string partialUrl, HttpMethod method, string queryStringParameter = null, AuthenticationHeaderValue authentication = null, string acceptedResponseType = null, Encoding encodingType = null, string contentMediaType = null) where TResponse : class, new();
        HttpResponseMessage<TResponse> InvokeNoRequestSync<TResponse>(string partialUrl, HttpMethod method, string queryStringParameter = null, AuthenticationHeaderValue authentication = null, string acceptedResponseType = null, Encoding encodingType = null, string contentMediaType = null) where TResponse : class, new();
        Task<HttpResponseMessage> InvokeNoResponse<TRequest>(string partialUrl, HttpMethod method, TRequest request = null, AuthenticationHeaderValue authentication = null, string acceptedResponseType = null, Encoding encodingType = null, string contentMediaType = null) where TRequest : class, new();
        HttpResponseMessage InvokeNoResponseSync<TRequest>(string partialUrl, HttpMethod method, TRequest request = null, AuthenticationHeaderValue authentication = null, string acceptedResponseType = null, Encoding encodingType = null, string contentMediaType = null) where TRequest : class, new();
        HttpResponseMessage<TResponse> InvokeSync<TRequest, TResponse>(string partialUrl, HttpMethod method, TRequest request = null, AuthenticationHeaderValue authentication = null, string acceptedResponseType = null, Encoding encodingType = null, string contentMediaType = null)
            where TRequest : class, new()
            where TResponse : class, new();
    }
}
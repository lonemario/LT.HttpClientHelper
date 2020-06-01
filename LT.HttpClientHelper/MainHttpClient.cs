using LT.HttpClientHelper.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LT.HttpClientHelper
{
    /// <summary>
    /// Main client for service requests
    /// </summary>
    public class MainHttpClient : IDisposable, IMainHttpClient
    {
        #region Private fields
        private bool _IsDisposed;
        private HttpClient _Client;
        #endregion

        /// <summary>
        /// Client base address
        /// </summary>
        public Uri BaseAddress => _Client?.BaseAddress;

        /// <summary>
        /// Client base address in string format
        /// </summary>
        public string BaseAddressString
        {
            get {
                return _Client?.BaseAddress.OriginalString;
            }
            set {
                var _baseUrl = value.Trim();
                //Aggiungi / infondo se non presente
                if (_baseUrl.Substring(_baseUrl.Length - 1) != "/")
                    _baseUrl = _baseUrl + "/";
                if (_Client != null)
                    _Client.BaseAddress = new Uri(_baseUrl);
            }
        }

        /// <summary>
        /// Constructor whit base url
        /// </summary>
        /// <param name="baseUrl">Base URL for API service</param>
        public MainHttpClient(string baseUrl)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentNullException(nameof(baseUrl));

            var _baseUrl = baseUrl.Trim();
            //Aggiungi / infondo se non presente
            if (_baseUrl.Substring(_baseUrl.Length - 1) != "/")
                _baseUrl = _baseUrl + "/";

            //Inizializzo il client
            _Client = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };

            //Imposto i default
            _Client.DefaultRequestHeaders.Accept.Clear();
            _Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Costructor with no parameters, remember to set BaseAddress
        /// </summary>
        public MainHttpClient()
        {
            //Inizializzo il client
            _Client = new HttpClient();

            //Imposto i default
            _Client.DefaultRequestHeaders.Accept.Clear();
            _Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Send a request on remote server without send and return classes
        /// </summary>
        /// <param name="partialUrl"></param>
        /// <param name="method"></param>
        /// <param name="authentication"></param>
        /// <param name="acceptedResponseType"></param>
        /// <param name="encodingType"></param>
        /// <param name="contentMediaType"></param>
        /// <returns>Returns task with response</returns>
        public async Task<HttpResponseMessage> InvokeBase(string partialUrl,
            HttpMethod method, AuthenticationHeaderValue authentication = null,
            string acceptedResponseType = null, Encoding encodingType = null, string contentMediaType = null)            
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(partialUrl)) throw new ArgumentNullException(nameof(partialUrl));
            if (_Client.BaseAddress == null) throw new Exception("Remember to set BaseAddress!");

            var _partialUrl = partialUrl.Trim();
            //Se presente elimina lo / all'inizio
            if (_partialUrl.Substring(0, 1) == "/")
                partialUrl = _partialUrl.Substring(1);

            //Creo il messaggio di request con l'url e il verb
            HttpRequestMessage message = new HttpRequestMessage(method, partialUrl);

            //Aggiungo l'header "Accept"
            message.Headers.Accept.Clear();
            if (String.IsNullOrWhiteSpace(acceptedResponseType))
                acceptedResponseType = "application/json";
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptedResponseType));

            if (encodingType == null)
                encodingType = Encoding.UTF8;

            if (contentMediaType == null)
                contentMediaType = "application/json";

            //se il content è 'application/json' aggiungo nel body un oggetto json vuoto
            if (contentMediaType.Trim().ToLower() == "application/json")
                message.Content = new StringContent("{}",
                    Encoding.UTF8, "application/json");

            //Se ho un token autorizzativo
            if (authentication != null)
                message.Headers.Authorization = authentication;

            //Eseguo la chiamata del client
            return await _Client.SendAsync(message);

        }


        /// <summary>
        /// Send a request on remote server
        /// </summary>
        /// <typeparam name="TRequest">Type of request</typeparam>
        /// <typeparam name="TResponse">Type or response</typeparam>
        /// <param name="partialUrl">Partial URL</param>
        /// <param name="method">HTTP method</param>
        /// <param name="request">Request</param>
        /// <param name="authentication">Authentication header</param>
        /// <param name="acceptedResponseType">default is "application/json"</param>
        /// <param name="encodingType">default is UTF8 </param>
        /// <param name="contentMediaType">default is "application/json"</param>
        /// <returns>Returns task with response</returns>
        public async Task<HttpResponseMessage<TResponse>> Invoke<TRequest, TResponse>(string partialUrl,
            HttpMethod method, TRequest request = null, AuthenticationHeaderValue authentication = null, 
            string acceptedResponseType = null, Encoding encodingType = null, string contentMediaType = null )
            where TRequest : class, new()
            where TResponse : class, new()
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(partialUrl)) throw new ArgumentNullException(nameof(partialUrl));
            if (_Client.BaseAddress == null) throw new Exception("Remember to set BaseAddress!");

            var _partialUrl = partialUrl.Trim();
            //Se presente elimina lo / all'inizio
            if (_partialUrl.Substring(0,1) == "/")
                partialUrl = _partialUrl.Substring(1);

            //Creo il messaggio di request con l'url e il verb
            HttpRequestMessage message = new HttpRequestMessage(method, partialUrl);

            //Aggiungo l'header "Accept"
            message.Headers.Accept.Clear();
            if (String.IsNullOrWhiteSpace(acceptedResponseType))
                acceptedResponseType = "application/json";
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptedResponseType));

            if (encodingType == null)
                encodingType = Encoding.UTF8;

            if (contentMediaType == null)
                contentMediaType = "application/json";

            if (request == null)
            {
                //se il content è 'application/json' aggiungo nel body un oggetto json vuoto
                if (contentMediaType.Trim().ToLower() == "application/json")
                    message.Content = new StringContent("{}",
                        Encoding.UTF8, "application/json");
            }
            else
            {
                //Serializzo in formato JSON e imposto il contenuto (se diverso da null)
                message.Content = new StringContent(
                    JsonConvert.SerializeObject(request),
                    encodingType, contentMediaType);
            }

            //Se ho un token autorizzativo
            if (authentication != null)
                message.Headers.Authorization = authentication;

            //Eseguo la chiamata del client
            var response = await _Client.SendAsync(message);

            //Eseguo la creazione della response
            HttpResponseMessage<TResponse> contractResponse = new HttpResponseMessage<TResponse>(response);

            //Se la chiamata ha avuto successo (200 - Ok)
            if (response.IsSuccessStatusCode)
            {
                //Recupero il contento presente nella response
                string jsonContent = await response.Content.ReadAsStringAsync();

                //Deserializzo la response (se valida)
                if (!string.IsNullOrEmpty(jsonContent))
                    contractResponse.Data = JsonConvert.DeserializeObject<TResponse>(jsonContent);

                //Ritorno la response
                return contractResponse;
            }

            //Ritorno semplicemente la response
            return contractResponse;
        }


        /// <summary>
        /// DEPRECATED - use InvokeNoResponse<TRequest> instead.
        /// Send a request on remote server
        /// </summary>
        /// <typeparam name="TRequest">Type of request</typeparam>
        /// <param name="partialUrl">Partial URL</param>
        /// <param name="method">HTTP method</param>
        /// <param name="request">Request</param>
        /// <param name="authentication">Authentication header</param>
        /// <param name="acceptedResponseType">default is "application/json"</param>
        /// <param name="encodingType">default is UTF8 </param>
        /// <param name="contentMediaType">default is "application/json"</param>
        /// <returns>Returns task with response</returns>
        [Obsolete("Invoke<TRequest> is deprecated, please use InvokeNoResponse<TRequest> instead.")]
        public async Task<HttpResponseMessage> Invoke<TRequest>(string partialUrl,
            HttpMethod method, TRequest request, AuthenticationHeaderValue authentication = null,
            string acceptedResponseType = null, Encoding encodingType = null, string contentMediaType = null)
            where TRequest : class, new()
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(partialUrl)) throw new ArgumentNullException(nameof(partialUrl));
            if (_Client.BaseAddress == null) throw new Exception("Remember to set BaseAddress!");

            var _partialUrl = partialUrl.Trim();
            //Se presente elimina lo / all'inizio
            if (_partialUrl.Substring(0, 1) == "/")
                partialUrl = _partialUrl.Substring(1);

            //Creo il messaggio di request con l'url e il verb
            HttpRequestMessage message = new HttpRequestMessage(method, partialUrl);

            //Aggiungo l'header "Accept"
            message.Headers.Accept.Clear();
            if (String.IsNullOrWhiteSpace(acceptedResponseType))
                acceptedResponseType = "application/json";
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptedResponseType));

            if (encodingType == null)
                encodingType = Encoding.UTF8;

            if (contentMediaType == null)
                contentMediaType = "application/json";

            if (request == null)
            {
                //se il content è 'application/json' aggiungo nel body un oggetto json vuoto
                if (contentMediaType.Trim().ToLower() == "application/json")
                    message.Content = new StringContent("{}",
                        Encoding.UTF8, "application/json");
            }
            else
            {
                //Serializzo in formato JSON e imposto il contenuto (se diverso da null)
                message.Content = new StringContent(
                    JsonConvert.SerializeObject(request),
                    encodingType, contentMediaType);
            }

            //Se ho un token autorizzativo
            if (authentication != null)
                message.Headers.Authorization = authentication;

            //Eseguo la chiamata del client
            return await _Client.SendAsync(message);

        }


        /// <summary>
        /// Send a request on remote server without return a class
        /// </summary>
        /// <typeparam name="TRequest">Type of request</typeparam>
        /// <param name="partialUrl">Partial URL</param>
        /// <param name="method">HTTP method</param>
        /// <param name="request">Request</param>
        /// <param name="authentication">Authentication header</param>
        /// <param name="acceptedResponseType">default is "application/json"</param>
        /// <param name="encodingType">default is UTF8 </param>
        /// <param name="contentMediaType">default is "application/json"</param>
        /// <returns>Returns task with response</returns>
        public async Task<HttpResponseMessage> InvokeNoResponse<TRequest>(string partialUrl,
            HttpMethod method, TRequest request = null, AuthenticationHeaderValue authentication = null,
            string acceptedResponseType = null, Encoding encodingType = null, string contentMediaType = null)
            where TRequest : class, new()
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(partialUrl)) throw new ArgumentNullException(nameof(partialUrl));
            if (_Client.BaseAddress == null) throw new Exception("Remember to set BaseAddress!");

            var _partialUrl = partialUrl.Trim();
            //Se presente elimina lo / all'inizio
            if (_partialUrl.Substring(0, 1) == "/")
                partialUrl = _partialUrl.Substring(1);

            //Creo il messaggio di request con l'url e il verb
            HttpRequestMessage message = new HttpRequestMessage(method, partialUrl);

            //Aggiungo l'header "Accept"
            message.Headers.Accept.Clear();
            if (String.IsNullOrWhiteSpace(acceptedResponseType))
                acceptedResponseType = "application/json";
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptedResponseType));

            if (encodingType == null)
                encodingType = Encoding.UTF8;

            if (contentMediaType == null)
                contentMediaType = "application/json";

            if (request == null)
            {
                //se il content è 'application/json' aggiungo nel body un oggetto json vuoto
                if (contentMediaType.Trim().ToLower() == "application/json")
                    message.Content = new StringContent("{}",
                        Encoding.UTF8, "application/json");
            }
            else
            {
                //Serializzo in formato JSON e imposto il contenuto (se diverso da null)
                message.Content = new StringContent(
                    JsonConvert.SerializeObject(request),
                    encodingType, contentMediaType);
            }

            //Se ho un token autorizzativo
            if (authentication != null)
                message.Headers.Authorization = authentication;

            //Eseguo la chiamata del client
            return await _Client.SendAsync(message);

        }

        /// <summary>
        /// DEPRECATED - use InvokeNoRequest<TResponse> instead.
        /// Send a request on remote server
        /// </summary>
        /// <typeparam name="TResponse">Type of response</typeparam>
        /// <param name="partialUrl">Partial URL</param>
        /// <param name="method">HTTP method</param>
        /// <param name="queryStringParameter">Query string parameters</param>
        /// <param name="authentication">Authentication header</param>
        /// <param name="acceptedResponseType">default is "application/json"</param>
        /// <param name="encodingType">default is UTF8 </param>
        /// <param name="contentMediaType">default is "application/json"</param>
        /// <returns>Returns task with response</returns>
        [Obsolete("Invoke<TResponse> is deprecated, please use InvokeNoRequest<TResponse> instead.")]
        public async Task<HttpResponseMessage<TResponse>> Invoke<TResponse>(string partialUrl,
            HttpMethod method, string queryStringParameter = null, AuthenticationHeaderValue authentication = null,
            string acceptedResponseType = null, Encoding encodingType = null, string contentMediaType = null)
            where TResponse : class, new()
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(partialUrl)) throw new ArgumentNullException(nameof(partialUrl));
            if (_Client.BaseAddress == null) throw new Exception("Remember to set BaseAddress!");

            var _partialUrl = partialUrl.Trim();
            //Se presente elimina lo / all'inizio
            if (_partialUrl.Substring(0, 1) == "/")
                partialUrl = _partialUrl.Substring(1);

            //Se presenti aggiungo i query string parameters
            if (!string.IsNullOrWhiteSpace(queryStringParameter))
            {
                //se non presente il ? lo aggiungo
                if (queryStringParameter.Substring(0, 1) != "?")
                    partialUrl = partialUrl + "?" + queryStringParameter;
                else
                    partialUrl = partialUrl + queryStringParameter;
            }

            //Creo il messaggio di request con l'url e il verb
            HttpRequestMessage message = new HttpRequestMessage(method, partialUrl);

            //Aggiungo l'header "Accept"
            message.Headers.Accept.Clear();
            if (String.IsNullOrWhiteSpace(acceptedResponseType))
                acceptedResponseType = "application/json";
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptedResponseType));

            if (encodingType == null)
                encodingType = Encoding.UTF8;

            //if (contentMediaType == null)
            //    contentMediaType = "application/json";

            //if (request == null)
            //{

            //se il content è 'application/json' aggiungo nel body un oggetto json vuoto
            //if (contentMediaType.Trim().ToLower() == "application/json")
            //    message.Content = new StringContent("{}",
            //        Encoding.UTF8, "application/json");
            //}
            //else
            //{
            //    //Serializzo in formato JSON e imposto il contenuto (se diverso da null)
            //    message.Content = new StringContent(
            //        JsonConvert.SerializeObject(request),
            //        encodingType, contentMediaType);
            //}

            //Se ho un token autorizzativo
            if (authentication != null)
            {
                message.Headers.Authorization = authentication;
                message.Headers.Add(authentication.Scheme, authentication.Parameter);
            }
                

            //Eseguo la chiamata del client
            var response = await _Client.SendAsync(message);

            //Eseguo la creazione della response
            HttpResponseMessage<TResponse> contractResponse = new HttpResponseMessage<TResponse>(response);

            //Se la chiamata ha avuto successo (200 - Ok)
            if (response.IsSuccessStatusCode)
            {
                //Recupero il contento presente nella response
                string jsonContent = await response.Content.ReadAsStringAsync();

                //Deserializzo la response (se valida)
                if (!string.IsNullOrEmpty(jsonContent))
                    contractResponse.Data = JsonConvert.DeserializeObject<TResponse>(jsonContent);

                //Ritorno la response
                return contractResponse;
            }

            //Ritorno semplicemente la response
            return contractResponse;
        }

        /// <summary>
        /// Send a request on remote server without send a class
        /// </summary>
        /// <typeparam name="TResponse">Type of response</typeparam>
        /// <param name="partialUrl">Partial URL</param>
        /// <param name="method">HTTP method</param>
        /// <param name="queryStringParameter">Query string parameters</param>
        /// <param name="authentication">Authentication header</param>
        /// <param name="acceptedResponseType">default is "application/json"</param>
        /// <param name="encodingType">default is UTF8 </param>
        /// <param name="contentMediaType">default is "application/json"</param>
        /// <returns>Returns task with response</returns>
        public async Task<HttpResponseMessage<TResponse>> InvokeNoRequest<TResponse>(string partialUrl,
            HttpMethod method, string queryStringParameter = null, AuthenticationHeaderValue authentication = null,
            string acceptedResponseType = null, Encoding encodingType = null, string contentMediaType = null)
            where TResponse : class, new()
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(partialUrl)) throw new ArgumentNullException(nameof(partialUrl));
            if (_Client.BaseAddress == null) throw new Exception("Remember to set BaseAddress!");

            var _partialUrl = partialUrl.Trim();
            //Se presente elimina lo / all'inizio
            if (_partialUrl.Substring(0, 1) == "/")
                partialUrl = _partialUrl.Substring(1);

            //Se presenti aggiungo i query string parameters
            if (!string.IsNullOrWhiteSpace(queryStringParameter))
            {
                //se non presente il ? lo aggiungo
                if (queryStringParameter.Substring(0, 1) != "?")
                    partialUrl = partialUrl + "?" + queryStringParameter;
                else
                    partialUrl = partialUrl + queryStringParameter;
            }

            //Creo il messaggio di request con l'url e il verb
            HttpRequestMessage message = new HttpRequestMessage(method, partialUrl);

            //Aggiungo l'header "Accept"
            message.Headers.Accept.Clear();
            if (String.IsNullOrWhiteSpace(acceptedResponseType))
                acceptedResponseType = "application/json";
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptedResponseType));

            if (encodingType == null)
                encodingType = Encoding.UTF8;

            //if (contentMediaType == null)
            //    contentMediaType = "application/json";

            //if (request == null)
            //{

            //se il content è 'application/json' aggiungo nel body un oggetto json vuoto
            //if (contentMediaType.Trim().ToLower() == "application/json")
            //    message.Content = new StringContent("{}",
            //        Encoding.UTF8, "application/json");
            //}
            //else
            //{
            //    //Serializzo in formato JSON e imposto il contenuto (se diverso da null)
            //    message.Content = new StringContent(
            //        JsonConvert.SerializeObject(request),
            //        encodingType, contentMediaType);
            //}

            //Se ho un token autorizzativo
            if (authentication != null)
            {
                message.Headers.Authorization = authentication;
                message.Headers.Add(authentication.Scheme, authentication.Parameter);
            }


            //Eseguo la chiamata del client
            var response = await _Client.SendAsync(message);

            //Eseguo la creazione della response
            HttpResponseMessage<TResponse> contractResponse = new HttpResponseMessage<TResponse>(response);

            //Se la chiamata ha avuto successo (200 - Ok)
            if (response.IsSuccessStatusCode)
            {
                //Recupero il contento presente nella response
                string jsonContent = await response.Content.ReadAsStringAsync();

                //Deserializzo la response (se valida)
                if (!string.IsNullOrEmpty(jsonContent))
                    contractResponse.Data = JsonConvert.DeserializeObject<TResponse>(jsonContent);

                //Ritorno la response
                return contractResponse;
            }

            //Ritorno semplicemente la response
            return contractResponse;
        }


        /// <summary>
        /// Send a request on remote server without send and return classes Synchronously
        /// </summary>
        /// <param name="partialUrl"></param>
        /// <param name="method"></param>
        /// <param name="authentication"></param>
        /// <param name="acceptedResponseType"></param>
        /// <param name="encodingType"></param>
        /// <param name="contentMediaType"></param>
        /// <returns>Returns response</returns>
        public HttpResponseMessage InvokeBaseSync(string partialUrl,
            HttpMethod method, AuthenticationHeaderValue authentication = null,
            string acceptedResponseType = null, Encoding encodingType = null, string contentMediaType = null)
        {
            var task = Task.Run(() => InvokeBase(partialUrl,
                                                 method,
                                                 authentication,
                                                 acceptedResponseType,
                                                 encodingType,
                                                 contentMediaType));
            task.Wait();
            return task.Result;

        }



        /// <summary>
        /// Send a request on remove server Synchronously
        /// If you can use async method for best performance
        /// </summary>
        /// <typeparam name="TRequest">Type of request</typeparam>
        /// <typeparam name="TResponse">Type or response</typeparam>
        /// <param name="partialUrl">Partial URL</param>
        /// <param name="method">HTTP method</param>
        /// <param name="request">Request</param>
        /// <param name="authentication">Authentication header</param>
        /// <param name="acceptedResponseType">default is "application/json"</param>
        /// <param name="encodingType">default is UTF8 </param>
        /// <param name="contentMediaType">default is "application/json"</param>
        /// <returns>Returns response</returns>
        public HttpResponseMessage<TResponse> InvokeSync<TRequest, TResponse>(string partialUrl,
            HttpMethod method, TRequest request = null, AuthenticationHeaderValue authentication = null,
            string acceptedResponseType = null, Encoding encodingType = null, string contentMediaType = null)
            where TRequest : class, new()
            where TResponse : class, new()
        {
            var task = Task.Run(() => Invoke<TRequest, TResponse>(partialUrl,
                                                                  method,
                                                                  request,
                                                                  authentication,
                                                                  acceptedResponseType,
                                                                  encodingType,
                                                                  contentMediaType));
            task.Wait();
            return task.Result;
        }



        /// <summary>
        /// Send a request on remote server Synchronously without return a class
        /// If you can use async method for best performance
        /// </summary>
        /// <typeparam name="TRequest">Type of request</typeparam>
        /// <param name="partialUrl">Partial URL</param>
        /// <param name="method">HTTP method</param>
        /// <param name="request">Request</param>
        /// <param name="authentication">Authentication header</param>
        /// <param name="acceptedResponseType">default is "application/json"</param>
        /// <param name="encodingType">default is UTF8 </param>
        /// <param name="contentMediaType">default is "application/json"</param>
        /// <returns>Returns response</returns>
        public HttpResponseMessage InvokeNoResponseSync<TRequest>(string partialUrl,
            HttpMethod method, TRequest request = null, AuthenticationHeaderValue authentication = null,
            string acceptedResponseType = null, Encoding encodingType = null, string contentMediaType = null)
            where TRequest : class, new()
        {
            var task = Task.Run(() => InvokeNoResponse<TRequest>(partialUrl,
                                                                 method,
                                                                 request, 
                                                                 authentication,
                                                                 acceptedResponseType,
                                                                 encodingType,
                                                                 contentMediaType));
            task.Wait();
            return task.Result;

        }


        /// <summary>
        /// Send a request on remote server Synchronously without send a class
        /// If you can use async method for best performance
        /// </summary>
        /// <typeparam name="TResponse">Type of response</typeparam>
        /// <param name="partialUrl">Partial URL</param>
        /// <param name="method">HTTP method</param>
        /// <param name="queryStringParameter">Query string parameters</param>
        /// <param name="authentication">Authentication header</param>
        /// <param name="acceptedResponseType">default is "application/json"</param>
        /// <param name="encodingType">default is UTF8 </param>
        /// <param name="contentMediaType">default is "application/json"</param>
        /// <returns>Returns response</returns>
        public HttpResponseMessage<TResponse> InvokeNoRequestSync<TResponse>(string partialUrl,
            HttpMethod method, string queryStringParameter = null, AuthenticationHeaderValue authentication = null,
            string acceptedResponseType = null, Encoding encodingType = null, string contentMediaType = null)
            where TResponse : class, new()
        {
            var task = Task.Run(() => InvokeNoRequest<TResponse>(partialUrl,
                                                                 method,
                                                                 queryStringParameter,
                                                                 authentication,
                                                                 acceptedResponseType,
                                                                 encodingType,
                                                                 contentMediaType));
            task.Wait();
            return task.Result;

        }

        /// <summary>
        /// Finalizer that ensures the object is correctly disposed of.
        /// </summary>
        ~MainHttpClient()
        {
            //Richiamo i dispose implicito
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //Eseguo una dispose esplicita
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="isDisposing">Explicit dispose</param>
        protected virtual void Dispose(bool isDisposing)
        {
            //Se l'oggetto è già rilasciato, esco
            if (_IsDisposed)
                return;

            //Se è richiesto il rilascio esplicito
            if (!isDisposing)
            {
                //RIlascio della logica non finalizzabile
                if (_Client != null)
                    _Client.Dispose();
            }

            //Marco il dispose e invoco il GC
            _IsDisposed = true;
        }
    }
}

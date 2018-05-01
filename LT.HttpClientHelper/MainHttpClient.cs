﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using LT.HttpClientHelper.Models;
using Newtonsoft.Json;

namespace LT.HttpClientHelper
{
    /// <summary>
    /// Main client for service requests
    /// </summary>
    public class MainHttpClient : IDisposable
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
        /// Constructor
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
        /// Execute a post on remove server
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
                _Client.Dispose();
            }

            //Marco il dispose e invoco il GC
            _IsDisposed = true;
        }
    }
}

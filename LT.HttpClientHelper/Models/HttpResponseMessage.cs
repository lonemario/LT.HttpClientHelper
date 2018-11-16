using System;
using System.Net.Http;

namespace LT.HttpClientHelper.Models
{
    /// <summary>
    /// Complex response message for HTTP invokes
    /// </summary>
    /// <typeparam name="TData">Type of parsed data</typeparam>
    public class HttpResponseMessage<TData>
        where TData : class, new()
    {
        /// <summary>
        /// Original HTTP response message
        /// </summary>
        public HttpResponseMessage Response { get; set; }

        /// <summary>
        /// Parsed data
        /// </summary>
        public TData Data { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="response">HTTP response</param>
        public HttpResponseMessage(HttpResponseMessage response)
        {
            //Validazione argomenti
            if (response == null) throw new ArgumentNullException(nameof(response));

            //Imposto il valore
            Response = response;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="response">HTTP response</param>
        /// <param name="data">Parsed response data</param>
        public HttpResponseMessage(HttpResponseMessage response, TData data)
        {
            //Validazione argomenti
            if (response == null) throw new ArgumentNullException(nameof(response));
            if (data == null) throw new ArgumentNullException(nameof(response));

            //Imposto il valore
            Response = response;
            Data = data;
        }
    }

}

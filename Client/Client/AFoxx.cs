﻿using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.Protocol;

namespace ArangoDriver.Client
{
    public class AFoxx
    {
        private readonly RequestFactory _requestFactory;
        readonly ADatabase _connection;

        private dynamic _body;

        internal AFoxx(RequestFactory requestFactory, ADatabase connection)
        {
            _requestFactory = requestFactory;
            _connection = connection;
        }

        #region Parameters

        /// <summary>
        /// Serializes specified value as JSON object into request body.
        /// </summary>
        public AFoxx Body<T>(T value)
        {
            _body = value;

            return this;
        }

        #endregion

        /// <summary>
        /// Sends GET request to specified foxx service location.
        /// </summary>
        public Task<AResult<T>> Get<T>(string relativeUri)
        {
            return Request<T>(HttpMethod.Get, relativeUri);
        }

        /// <summary>
        /// Sends POST request to specified foxx service location.
        /// </summary>
        public Task<AResult<T>> Post<T>(string relativeUri)
        {
            return Request<T>(HttpMethod.Post, relativeUri);
        }

        /// <summary>
        /// Sends PUT request to specified foxx service location.
        /// </summary>
        public Task<AResult<T>> Put<T>(string relativeUri)
        {
            return Request<T>(HttpMethod.Put, relativeUri);
        }

        /// <summary>
        /// Sends PATCH request to specified foxx service location.
        /// </summary>
        public Task<AResult<T>> Patch<T>(string relativeUri)
        {
            return Request<T>(HttpMethod.Patch, relativeUri);
        }

        /// <summary>
        /// Sends DELETE request to specified foxx service location.
        /// </summary>
        public Task<AResult<T>> Delete<T>(string relativeUri)
        {
            return Request<T>(HttpMethod.Delete, relativeUri);
        }

        private async Task<AResult<T>> Request<T>(HttpMethod httpMethod, string relativeUri)
        {
            var request = _requestFactory.Create(httpMethod, relativeUri);

            if (_body != null)
            {
                request.SetBody(_body);
            }

            var response = await _connection.Send(request);
            var result = new AResult<T>(response);

            result.Value = response.ParseBody<T>();

            return result;
        }
    }
}

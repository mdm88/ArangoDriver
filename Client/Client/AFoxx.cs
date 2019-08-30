using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.External.dictator;
using ArangoDriver.Protocol;
using fastJSON;

namespace ArangoDriver.Client
{
    public class AFoxx
    {
        readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        readonly ADatabase _connection;

        internal AFoxx(ADatabase connection)
        {
            _connection = connection;
        }

        #region Parameters

        /// <summary>
        /// Serializes specified value as JSON object into request body.
        /// </summary>
        public AFoxx Body(object value)
        {
            _parameters.Object(ParameterName.Body, value);

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
            var request = new Request(httpMethod, relativeUri);

            if (_parameters.Has(ParameterName.Body))
            {
                request.Body = JSON.ToJSON(_parameters.Object(ParameterName.Body), ASettings.JsonParameters);
            }

            var response = await _connection.Send(request);
            var result = new AResult<T>(response);

            result.Value = response.ParseBody<T>();

            _parameters.Clear();

            return result;
        }
    }
}

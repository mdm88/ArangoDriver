using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.Protocol;

namespace ArangoDriver.Client
{
    public class AFoxxAdmin
    {
        private readonly RequestFactory _requestFactory;
        readonly ADatabase _connection;

        internal AFoxxAdmin(RequestFactory requestFactory, ADatabase connection)
        {
            _requestFactory = requestFactory;
            _connection = connection;
        }

        /// <summary>
        /// Sends GET request to specified foxx service location.
        /// </summary>
        public Task<AResult<T>> Install<T>(string mount, Stream zip)
        {
            return Request<T>(HttpMethod.Post, "/_api/foxx", mount, zip);
        }

        /// <summary>
        /// Sends GET request to specified foxx service location.
        /// </summary>
        public Task<AResult<T>> Upgrade<T>(string mount, Stream zip)
        {
            return Request<T>(HttpMethod.Patch, "/_api/foxx/service", mount, zip);
        }

        /// <summary>
        /// Sends GET request to specified foxx service location.
        /// </summary>
        public Task<AResult<T>> Replace<T>(string mount, Stream zip)
        {
            return Request<T>(HttpMethod.Put, "/_api/foxx/service", mount, zip);
        }

        /// <summary>
        /// Sends GET request to specified foxx service location.
        /// </summary>
        public Task<AResult<T>> Uninstall<T>(string mount)
        {
            return Request<T>(HttpMethod.Delete, "/_api/foxx/service", mount);
        }

        private async Task<AResult<T>> Request<T>(HttpMethod httpMethod, string relativeUri, string mount, Stream zip = null)
        {
            var request = _requestFactory.Create(httpMethod, relativeUri);
            
            request.QueryString.Add("mount", mount);
            request.Body = zip;

            var response = await _connection.Send(request);
            var result = new AResult<T>(response);

            result.Value = response.ParseBody<T>();

            return result;
        }
    }
}

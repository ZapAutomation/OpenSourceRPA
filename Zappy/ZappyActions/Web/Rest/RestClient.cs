using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Zappy.ZappyActions.Web.Rest
{
    public class RestClient
    {
        private RestClient client;
        private IRestRequest request;

        public Uri BaseUrl
        {
            get { return this.client.BaseUrl; }
            set { this.client.BaseUrl = (value); }
        }

        public RestClient()
        {
            this.client = new RestClient();
            RestRequest restRequest = new RestRequest();
            restRequest.UseDefaultCredentials = (true);
            this.request = (IRestRequest)restRequest;
        }

        public RestClient(Uri endpointUrl, IDictionary<string, string> headers, IDictionary<string, string> parameters,
            IDictionary<string, string> urlSegments, IDictionary<string, string> attachments)
            : this()
        {
            this.BaseUrl = endpointUrl;
            this.AddHeaders(headers);
            this.AddParameters(parameters);
            this.AddUrlSegments(urlSegments);
            this.AddAttachments(attachments);
        }

        public void AddHeaders(IDictionary<string, string> headers)
        {
            if (headers == null)
                return;
            foreach (KeyValuePair<string, string> header in (IEnumerable<KeyValuePair<string, string>>)headers)
                this.AddHeader(header.Key, header.Value);
        }

        public void AddHeader(string name, string value)
        {
            this.request.AddParameter(name, (object)value, (ParameterType)3);
        }

        public void AddParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null)
                return;
            foreach (KeyValuePair<string, string> parameter in (IEnumerable<KeyValuePair<string, string>>)parameters)
                this.AddParameter(parameter.Key, parameter.Value);
        }

        public void AddParameter(string name, string value)
        {
            this.request.AddParameter(name, (object)value);
        }

        public void AddUrlSegments(IDictionary<string, string> urlSegments)
        {
            if (urlSegments == null)
                return;
            foreach (KeyValuePair<string, string> urlSegment in (IEnumerable<KeyValuePair<string, string>>)urlSegments)
                this.AddUrlSegment(urlSegment.Key, urlSegment.Value);
        }

        public void AddUrlSegment(string name, string value)
        {
            this.request.AddParameter(name, (object)value, (ParameterType)2);
        }

        public void AddAttachments(IDictionary<string, string> files)
        {
            if (files == null)
                return;
            foreach (KeyValuePair<string, string> file in (IEnumerable<KeyValuePair<string, string>>)files)
                this.request.AddFile(file.Key, file.Value);
        }

        public void AddAttachment(string name, string path)
        {
            this.request.AddFile(name, path);
        }

        public void AddBody(string type, string body)
        {
            this.request.AddParameter(type, (object)body, (ParameterType)4);
        }

        public void Authenticate(string consumerKey, string consumerSecret, string OAuth1Token,
            string OAuth1TokenSecret)
        {
            if (this.client == null)
                return;
            this.client.Authenticator =
                ((IAuthenticator)OAuth1Authenticator.ForProtectedResource(consumerKey, consumerSecret, OAuth1Token,
                    OAuth1TokenSecret));
        }

        public IAuthenticator Authenticator { get; set; }

        public void Authenticate(string oAuth2Token)
        {
            if (this.client == null)
                return;
            this.client.Authenticator =
                ((IAuthenticator)new OAuth2AuthorizationRequestHeaderAuthenticator(oAuth2Token));
        }

        public void Authenticate(string username, string password)
        {
            if (this.client == null)
                return;
            this.client.Authenticator = ((IAuthenticator)new HttpBasicAuthenticator(username, password));
        }

        public Task<RestResponse> ExecuteRequestAsync(Method method, AcceptHeaderType acceptFormat, int timeoutMS,
            CancellationToken cancellationToken)
        {
            this.request.Method = (RestSharp.Method)(method);
            this.request.Timeout = (timeoutMS);
            switch (acceptFormat)
            {
                case AcceptHeaderType.JSON:
                    this.request.AddHeader("Accept", "application/json");
                    break;
                case AcceptHeaderType.XML:
                    this.request.AddHeader("Accept", "application/xml");
                    break;
                default:
                    this.request.AddHeader("Accept", "*/*");
                    break;
            }

            return null;
                                                                                                                    }

        public byte[] DownloadData(string filePath)
        {
            FileStream writer = File.OpenWrite(filePath);
            try
            {
                this.request.ResponseWriter =
                    ((Action<Stream>)(responseStream => responseStream.CopyTo((Stream)writer)));
                return this.client.DownloadData(this.request.ToString());
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();
            }
        }

    }
}



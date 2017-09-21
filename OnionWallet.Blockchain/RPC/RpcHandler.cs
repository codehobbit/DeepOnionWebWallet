using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OnionWallet.Blockchain.RPC
{
    public class RpcHandler
    {
        /// <summary>
        /// Source  used from BitcoinLib, thanks at this place. 
        /// </summary>

        private int _nodeRpcTimeout = 0;
        private string _nodeRpcAddress = "";
        private string _nodeRpcUser = "";
        private string _nodeRpcPassword = "";
        private string _onionWalletPassword = "";

        public RpcHandler()
        {
            _nodeRpcTimeout = int.Parse(ConfigurationManager.AppSettings["NodeRpcTimeout"].ToString());
            _nodeRpcAddress = ConfigurationManager.AppSettings["NodeRpcAddress"].ToString();
            _nodeRpcUser = ConfigurationManager.AppSettings["NodeRpcUser"].ToString();
            _nodeRpcPassword = ConfigurationManager.AppSettings["NodeRpcPassword"].ToString();
            _onionWalletPassword = ConfigurationManager.AppSettings["OnionWalletPassword"].ToString();
        }

        public T MakeRequest<T>(RpcMethodsEnum rpcMethod, params object[] parameters)
        {
            var jsonRpcRequest = new RpcRequest(1, rpcMethod.ToString(), parameters);
            var webRequest = (HttpWebRequest)WebRequest.Create(_nodeRpcAddress);
            SetBasicAuthHeader(webRequest, _nodeRpcUser, _nodeRpcPassword);
            webRequest.Credentials = new NetworkCredential(_nodeRpcUser, _nodeRpcPassword);
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";
            webRequest.Proxy = null;
            webRequest.Timeout = _nodeRpcTimeout * 1000;
            var byteArray = jsonRpcRequest.GetBytes();
            webRequest.ContentLength = jsonRpcRequest.GetBytes().Length;

            try
            {
                using (var dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Dispose();
                }
            }
            catch (Exception exception)
            {
                throw new Exception("There was a problem sending the request to the wallet", exception);
            }

            try
            {
                string json;

                using (var webResponse = webRequest.GetResponse())
                {
                    using (var stream = webResponse.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var result = reader.ReadToEnd();
                            reader.Dispose();
                            json = result;
                        }
                    }
                }

                var rpcResponse = JsonConvert.DeserializeObject<RpcResponse<T>>(json);
                return rpcResponse.Result;
            }
            catch (WebException webException)
            {
                #region RPC Internal Server Error (with an Error Code)

                var webResponse = webException.Response as HttpWebResponse;

                if (webResponse != null)
                {
                    switch (webResponse.StatusCode)
                    {
                        case HttpStatusCode.InternalServerError:
                            {
                                using (var stream = webResponse.GetResponseStream())
                                {
                                    if (stream == null)
                                    {
                                        throw new Exception("The RPC request was either not understood by the server or there was a problem executing the request", webException);
                                    }

                                    using (var reader = new StreamReader(stream))
                                    {
                                        var result = reader.ReadToEnd();
                                        reader.Dispose();

                                        try
                                        {
                                            var jsonRpcResponseObject = JsonConvert.DeserializeObject<RpcResponse<object>>(result);

                                            var internalServerErrorException = new Exception(jsonRpcResponseObject.Error.Message, webException)
                                            {
                                                //RpcErrorCode = jsonRpcResponseObject.Error.Code
                                            };

                                            throw internalServerErrorException;
                                        }
                                        catch (JsonException)
                                        {
                                            throw new Exception(result, webException);
                                        }
                                    }
                                }
                            }

                        default:
                            throw new Exception("The RPC request was either not understood by the server or there was a problem executing the request", webException);
                    }
                }

                #endregion

                #region RPC Time-Out

                if (webException.Message == "The operation has timed out")
                {
                    throw new Exception(webException.Message);
                }

                #endregion

                throw new Exception("An unknown web exception occured while trying to read the JSON response", webException);
            }
            catch (JsonException jsonException)
            {
                throw new Exception("There was a problem deserializing the response from the wallet", jsonException);
            }
            catch (ProtocolViolationException protocolViolationException)
            {
                throw new Exception("Unable to connect to the server", protocolViolationException);
            }
            catch (Exception exception)
            {
                var queryParameters = jsonRpcRequest.Parameters.Cast<string>().Aggregate(string.Empty, (current, parameter) => current + (parameter + " "));
                throw new Exception($"A problem was encountered while calling MakeRpcRequest() for: {jsonRpcRequest.Method} with parameters: {queryParameters}. \nException: {exception.Message}");
            }
        }

        private static void SetBasicAuthHeader(WebRequest webRequest, string username, string password)
        {
            var authInfo = username + ":" + password;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            webRequest.Headers["Authorization"] = "Basic " + authInfo;
        }
    }
}

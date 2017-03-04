/*
 * Author: Tin Trinh
 * General Web Client based HTTP protocol to submit Post/Get request to Web Server
 * All rights reserved by Tin Trinh
 */

using System;
using System.Text;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace TinTrinhLibrary
{
    public class WebClient
    {
        private CookieContainer cookieContainer = new CookieContainer(); //Keep cookies and feed cookies next request

        public WebClient()
        {
            //Support HTTPS/SSL
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
        }

        /// <summary>
        /// Send GET HTTPRequest
        /// </summary>
        /// <param name="url"></param>
        /// <param name="referer"></param>
        /// <param name="returnCookie">CookieName that you want to check</param>
        /// <returns>If returnCookie is set, return cookie value. Else return html code</returns>
        public String Get(string url, string referer, string returnCookie)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.AllowAutoRedirect = false;
            webRequest.AutomaticDecompression = DecompressionMethods.GZip;

            webRequest.CookieContainer = cookieContainer;

            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:49.0) Gecko/20100101 Firefox/49.0";
            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            webRequest.Headers[HttpRequestHeader.AcceptLanguage] = "vi-VN,vi;q=0.8,en-US;q=0.5,en;q=0.3";
            webRequest.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate, br";
            webRequest.KeepAlive = false; //Fix "The server committed a protocol violation. Section=ResponseStatusLine"
            webRequest.ContentType = "text/html; charset=UTF-8";

            webRequest.Referer = referer;

            webRequest.Method = "GET";

            try
            {
                // get the response
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    if (webResponse == null)
                    { return null; }

                    if (returnCookie != "" && returnCookie != null)
                    {
                        //Developer can check cookies here at webResponse.Cookies
                        foreach (Cookie cookie in webResponse.Cookies)
                        {
                            if (cookie.Name == returnCookie)
                                return cookie.Value;
                        }
                        return null;
                    }

                    using (StreamReader sr = new StreamReader(webResponse.GetResponseStream()))
                    {
                        return sr.ReadToEnd().Trim();
                    }
                }
            }
            catch (WebException ex)
            {
                throw new Exception(GetInternalException(ex));
            }
            finally
            {
                webRequest.Abort();
            }
            return null;
        }

        /// <summary>
        /// Send POST HTTPRequest
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="referer"></param>
        /// <param name="returnCookie">CookieName that you want to check</param>
        /// returns>If returnCookie is set, return cookie value. Else return html code</returns>
        public String Post(string url, string parameters, string referer, string returnCookie)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url); ;

            webRequest.AllowAutoRedirect = false;
            webRequest.AutomaticDecompression = DecompressionMethods.GZip;

            webRequest.CookieContainer = cookieContainer;

            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:13.0) Gecko/20100101 Firefox/13.0.1";
            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            webRequest.Headers[HttpRequestHeader.AcceptLanguage] = "en-us,en;q=0.5";
            webRequest.Headers[HttpRequestHeader.AcceptEncoding] = "gzip,deflate";
            webRequest.Headers[HttpRequestHeader.AcceptCharset] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
            webRequest.KeepAlive = false; //Fix "The server committed a protocol violation. Section=ResponseStatusLine"
            webRequest.ContentType = "application/x-www-form-urlencoded";

            webRequest.Referer = referer;

            webRequest.Method = "POST";
            byte[] bytes = Encoding.UTF8.GetBytes(parameters);
            Stream os = null;
            try
            { // send the Post
                webRequest.ContentLength = bytes.Length; //Count bytes to send
                os = webRequest.GetRequestStream();
                os.Write(bytes, 0, bytes.Length); //Send it
            }
            catch (WebException ex)
            {
                throw new Exception(GetInternalException(ex));
            }
            finally
            {
                if (os != null)
                {
                    os.Close();
                }
            }

            try
            {
                // get the response
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    if (webResponse == null)
                    { return null; }

                    if (returnCookie != "" && returnCookie != null)
                    {
                        //Developer can check cookies here at webResponse.Cookies
                        foreach (Cookie ck in webResponse.Cookies)
                        {
                            if (ck.Name == returnCookie)
                                return ck.Value;
                        }
                        return null;
                    }

                    StreamReader sr = new StreamReader(webResponse.GetResponseStream());
                    return sr.ReadToEnd().Trim();
                }
            }
            catch (WebException ex)
            {
                throw new Exception(GetInternalException(ex));
            }
            finally
            {
                webRequest.Abort();
            }
            return null;
        }

        public string GetInternalException(WebException ex)
        {
            StreamReader sr = new StreamReader(ex.Response.GetResponseStream());
            string errorMessage = sr.ReadToEnd().Trim();
            return errorMessage;
        }
    }
}
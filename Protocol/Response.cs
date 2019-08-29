﻿using System;
using System.Net;
using System.Net.Http.Headers;
using ArangoDriver.Client;
using fastJSON;

namespace ArangoDriver.Protocol
{
    internal class Response
    {
        internal int StatusCode { get; set; }
        internal HttpResponseHeaders Headers { get; set; }
        internal string Body { get; set; }
        internal BodyType BodyType { get; set; }
        //internal Exception Exception { get; set; }
        //internal AEerror Error { get; set; }
        
        internal void GetBodyDataType()
        {            
            if (string.IsNullOrEmpty(Body))
            {
                BodyType = BodyType.Null;
            }
            else
            {
                var trimmedBody = Body.Trim();

                switch (trimmedBody[0])
                {
                    // body contains JSON array
                    case '[':
                        BodyType = BodyType.List;
                        break;
                    // body contains JSON object
                    case '{':
                        BodyType = BodyType.Document;
                        break;
                    default:
                        BodyType = BodyType.Primitive;
                        break;
                }
            }
        }

        internal T ParseBody<T>()
        {
            if (string.IsNullOrEmpty(Body))
            {
                return default(T);
            }

            return JSON.ToObject<T>(Body);
        }
    }
}

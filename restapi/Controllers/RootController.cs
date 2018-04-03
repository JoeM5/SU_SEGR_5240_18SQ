﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using restapi.Models;

namespace restapi.Controllers
{
    public class RootController : Controller
    {
        // GET api/values
        [Route("~/")]
        [HttpGet]
        [Produces(ContentTypes.Root)]
        [ProducesResponseType(typeof(IDictionary<string, DocumentLink>), 200)]
        public IDictionary<string, DocumentLink> Get()
        {
            return new Dictionary<string, DocumentLink>()
            {  
                { 
                    "timesheets", new DocumentLink() 
                    { 
                        Method = Method.Get,
                        Type = ContentTypes.Timesheets,
                        Relationship = DocumentRelationship.Timesheets,
                        Reference = "/timesheets"
                    }   
                }
            };
        }
    }
}

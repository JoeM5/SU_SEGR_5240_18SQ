using System;
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
        [ProducesResponseType(typeof(IDictionary<ApplicationRelationship, IList<DocumentLink>>), 200)]
        public IDictionary<ApplicationRelationship, IList<DocumentLink>> Get()
        {
            return new Dictionary<ApplicationRelationship, IList<DocumentLink>>()
            {  
                { 
                    ApplicationRelationship.Timesheets, new List<DocumentLink>()
                    {
                        new DocumentLink()
                        {
                            Method = Method.Get,
                            Type = ContentTypes.Timesheets,
                            Relationship = DocumentRelationship.Timesheets,
                            Reference = "/timesheets"
                        },
                        new DocumentLink()
                        {
                            Method = Method.Post,
                            Type = ContentTypes.Timesheet,
                            Relationship = DocumentRelationship.Timesheets,
                            Reference = "/timesheets"
                        },
                        new DocumentLink()
                        {
                            Method = Method.Get,
                            Type = ContentTypes.Timesheet,
                            Relationship = DocumentRelationship.Timesheets,
                            Reference = "/timesheets/{timesheetId}"
                        },
                        new DocumentLink()
                        {
                            Method = Method.Delete,
                            Relationship = DocumentRelationship.Timesheets,
                            Reference = "/timesheets/{timesheetId}"
                        },
                        new DocumentLink()
                        {
                            Method = Method.Get,
                            Type = ContentTypes.TimesheetLines,
                            Relationship = DocumentRelationship.Timesheets,
                            Reference = "/timesheets/{timesheetId}/lines"
                        },
                        new DocumentLink()
                        {
                            Method = Method.Post,
                            Type = ContentTypes.TimesheetLine,
                            Relationship = DocumentRelationship.Timesheets,
                            Reference = "/timesheets/{timesheetId}/lines"
                        },
                        new DocumentLink()
                        {
                            Method = Method.Post,
                            Type = ContentTypes.TimesheetLine,
                            Relationship = DocumentRelationship.Timesheets,
                            Reference = "/timesheets/{timesheetId}/lines/{lineId}"
                        },
                        new DocumentLink()
                        {
                            Method = Method.Patch,
                            Type = ContentTypes.TimesheetLine,
                            Relationship = DocumentRelationship.Timesheets,
                            Reference = "/timesheets/{timesheetId}/lines/{lineId}"
                        },
                        new DocumentLink()
                        {
                            Method = Method.Get,
                            Type = ContentTypes.Transitions,
                            Relationship = DocumentRelationship.Transitions,
                            Reference = "/timesheets/{timesheetId}/transitions"
                        },
                        new DocumentLink()
                        {
                            Method = Method.Get,
                            Type = ContentTypes.Transition,
                            Relationship = DocumentRelationship.Transitions,
                            Reference = "/timesheets/{timesheetId}/submittal"
                        },
                        new DocumentLink()
                        {
                            Method = Method.Post,
                            Type = ContentTypes.Transition,
                            Relationship = DocumentRelationship.Transitions,
                            Reference = "/timesheets/{timesheetId}/submittal"
                        },
                        new DocumentLink()
                        {
                            Method = Method.Get,
                            Type = ContentTypes.Transition,
                            Relationship = DocumentRelationship.Transitions,
                            Reference = "/timesheets/{timesheetId}/cancellation"
                        },
                        new DocumentLink()
                        {
                            Method = Method.Post,
                            Type = ContentTypes.Transition,
                            Relationship = DocumentRelationship.Transitions,
                            Reference = "/timesheets/{timesheetId}/cancellation"
                        },
                        new DocumentLink()
                        {
                            Method = Method.Get,
                            Type = ContentTypes.Transition,
                            Relationship = DocumentRelationship.Transitions,
                            Reference = "/timesheets/{timesheetId}/rejection"
                        },
                        new DocumentLink()
                        {
                            Method = Method.Post,
                            Type = ContentTypes.Transition,
                            Relationship = DocumentRelationship.Transitions,
                            Reference = "/timesheets/{timesheetId}/rejection"
                        },
                        new DocumentLink()
                        {
                            Method = Method.Get,
                            Type = ContentTypes.Transition,
                            Relationship = DocumentRelationship.Transitions,
                            Reference = "/timesheets/{timesheetId}/approval"
                        },
                        new DocumentLink()
                        {
                            Method = Method.Post,
                            Type = ContentTypes.Transition,
                            Relationship = DocumentRelationship.Transitions,
                            Reference = "/timesheets/{timesheetId}/approval"
                        }
                    }
                }
            };
        }
    }
}

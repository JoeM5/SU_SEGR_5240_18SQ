using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using restapi.Models;

namespace restapi.Controllers
{
    [Route("[controller]")]
    public class TimesheetsController : Controller
    {
        [HttpGet]
        [Produces(ContentTypes.Timesheets)]
        [ProducesResponseType(typeof(IEnumerable<Timecard>), 200)]
        public IEnumerable<Timecard> GetAll()
        {
            return Database
                .All
                .OrderBy(t => t.Opened);
        }

        [HttpGet("{timesheetId}")]
        [Produces(ContentTypes.Timesheet)]
        [ProducesResponseType(typeof(Timecard), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetOne(string timesheetId)
        {
            Timecard timecard = Database.Find(timesheetId);

            if (timecard != null) 
            {
                return Ok(timecard);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Produces(ContentTypes.Timesheet)]
        [ProducesResponseType(typeof(Timecard), 200)]
        public Timecard Create([FromBody] DocumentResource resource)
        {
            var timecard = new Timecard(resource.Resource);

            var entered = new Entered() { Resource = resource.Resource };

            timecard.Transitions.Add(new Transition(entered));

            Database.Add(timecard);

            return timecard;
        }

        [HttpDelete("{timesheetId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(InvalidStateError), 409)]
        public IActionResult Delete(string timesheetId) 
        {
            Timecard timecard = Database.Find(timesheetId);

            if (timecard != null) 
            {
                if (timecard.Status != TimecardStatus.Draft && timecard.Status != TimecardStatus.Cancelled) 
                {
                    return StatusCode(409, new InvalidStateError() { });
                }

                Database.Delete(timecard);

                return NoContent();
            }
            else 
            {
                return NotFound();
            }
        }

        [HttpGet("{timesheetId}/lines")]
        [Produces(ContentTypes.TimesheetLines)]
        [ProducesResponseType(typeof(IEnumerable<AnnotatedTimecardLine>), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetLines(string timesheetId)
        {
            Timecard timecard = Database.Find(timesheetId);

            if (timecard != null)
            {
                var lines = timecard.Lines
                    .OrderBy(l => l.WorkDate)
                    .ThenBy(l => l.Recorded);

                return Ok(lines);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("{timesheetId}/lines")]
        [Produces(ContentTypes.TimesheetLine)]
        [ProducesResponseType(typeof(AnnotatedTimecardLine), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(InvalidStateError), 409)]
        public IActionResult AddLine(string timesheetId, [FromBody] TimecardLine timecardLine)
        {
            Timecard timecard = Database.Find(timesheetId);

            if (timecard != null)
            {
                if (timecard.Status != TimecardStatus.Draft)
                {
                    return StatusCode(409, new InvalidStateError() { });
                }

                var annotatedLine = timecard.AddLine(timecardLine);

                return Ok(annotatedLine);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("{timesheetId}/lines/{lineId}")]
        [Produces(ContentTypes.TimesheetLine)]
        [ProducesResponseType(typeof(AnnotatedTimecardLine), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(InvalidStateError), 409)]
        public IActionResult ReplaceLine(string timesheetId, string lineId, [FromBody] TimecardLine timecardLine)
        {
            Timecard timecard = Database.Find(timesheetId);

            if (timecard != null) 
            {
                if (timecard.Status != TimecardStatus.Draft) 
                {
                    return StatusCode(409, new InvalidStateError() { });
                }

                var lineIndex = timecard.GetLineIndex(lineId);

                if (lineIndex == -1) 
                {
                    return NotFound();
                }

                var annotatedLine = timecard.ReplaceLine(lineIndex, timecardLine);

                return Ok(annotatedLine);
            }
            else 
            {
                return NotFound();
            }
        }

        [HttpPatch("{timesheetId}/lines/{lineId}")]
        [Produces(ContentTypes.TimesheetLine)]
        [ProducesResponseType(typeof(AnnotatedTimecardLine), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(InvalidStateError), 409)]
        public IActionResult UpdateLine(string timesheetId, string lineId, int? week, int? year, DayOfWeek? day, float? hours, string project)
        {
            Timecard timecard = Database.Find(timesheetId);

            if (timecard != null)
            {
                if (timecard.Status != TimecardStatus.Draft)
                {
                    return StatusCode(409, new InvalidStateError() { });
                }

                var lineIndex = timecard.GetLineIndex(lineId);

                if (lineIndex == -1)
                {
                    return NotFound();
                }

                var annotatedLine = timecard.UpdateLine(lineIndex, week, year, day, hours, project);

                return Ok(annotatedLine);
            }
            else 
            {
                return NotFound();
            }
        }
        
        [HttpGet("{timesheetId}/transitions")]
        [Produces(ContentTypes.Transitions)]
        [ProducesResponseType(typeof(IEnumerable<Transition>), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetTransitions(string timesheetId)
        {
            Timecard timecard = Database.Find(timesheetId);

            if (timecard != null)
            {
                return Ok(timecard.Transitions);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("{timesheetId}/submittal")]
        [Produces(ContentTypes.Transition)]
        [ProducesResponseType(typeof(Transition), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(InvalidStateError), 409)]
        [ProducesResponseType(typeof(EmptyTimecardError), 409)]
        public IActionResult Submit(string timesheetId, [FromBody] Submittal submittal)
        {
            Timecard timecard = Database.Find(timesheetId);

            if (timecard != null)
            {
                if (submittal.Resource != timecard.Resource)
                {
                    return StatusCode(409, new InvalidOperationException("The resource should match the timecard.") { });
                }

                if (timecard.Status != TimecardStatus.Draft)
                {
                    return StatusCode(409, new InvalidStateError() { });
                }

                if (timecard.Lines.Count < 1)
                {
                    return StatusCode(409, new EmptyTimecardError() { });
                }
                
                var transition = new Transition(submittal, TimecardStatus.Submitted);
                timecard.Transitions.Add(transition);
                return Ok(transition);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{timesheetId}/submittal")]
        [Produces(ContentTypes.Transition)]
        [ProducesResponseType(typeof(Transition), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(MissingTransitionError), 409)]
        public IActionResult GetSubmittal(string timesheetId)
        {
            Timecard timecard = Database.Find(timesheetId);

            if (timecard != null)
            {
                if (timecard.Status == TimecardStatus.Submitted)
                {
                    var transition = timecard.Transitions
                                        .Where(t => t.TransitionedTo == TimecardStatus.Submitted)
                                        .OrderByDescending(t => t.OccurredAt)
                                        .FirstOrDefault();

                    return Ok(transition);                                        
                }
                else 
                {
                    return StatusCode(409, new MissingTransitionError() { });
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("{timesheetId}/cancellation")]
        [Produces(ContentTypes.Transition)]
        [ProducesResponseType(typeof(Transition), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(InvalidStateError), 409)]
        [ProducesResponseType(typeof(EmptyTimecardError), 409)]
        public IActionResult Cancel(string timesheetId, [FromBody] Cancellation cancellation)
        {
            Timecard timecard = Database.Find(timesheetId);

            if (timecard != null)
            {
                if (cancellation.Resource != timecard.Resource)
                {
                    return StatusCode(409, new InvalidOperationException("The resource should match the timecard.") { });
                }

                if (timecard.Status != TimecardStatus.Draft && timecard.Status != TimecardStatus.Submitted)
                {
                    return StatusCode(409, new InvalidStateError() { });
                }
                
                var transition = new Transition(cancellation, TimecardStatus.Cancelled);
                timecard.Transitions.Add(transition);
                return Ok(transition);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{timesheetId}/cancellation")]
        [Produces(ContentTypes.Transition)]
        [ProducesResponseType(typeof(Transition), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(MissingTransitionError), 409)]
        public IActionResult GetCancellation(string timesheetId)
        {
            Timecard timecard = Database.Find(timesheetId);

            if (timecard != null)
            {
                if (timecard.Status == TimecardStatus.Cancelled)
                {
                    var transition = timecard.Transitions
                                        .Where(t => t.TransitionedTo == TimecardStatus.Cancelled)
                                        .OrderByDescending(t => t.OccurredAt)
                                        .FirstOrDefault();

                    return Ok(transition);                                        
                }
                else 
                {
                    return StatusCode(409, new MissingTransitionError() { });
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("{timesheetId}/rejection")]
        [Produces(ContentTypes.Transition)]
        [ProducesResponseType(typeof(Transition), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(InvalidStateError), 409)]
        [ProducesResponseType(typeof(EmptyTimecardError), 409)]
        public IActionResult Close(string timesheetId, [FromBody] Rejection rejection)
        {
            Timecard timecard = Database.Find(timesheetId);

            if (timecard != null)
            {
                if (rejection.Resource == timecard.Resource)
                {
                    return StatusCode(409, new InvalidOperationException("The resource should not match the timecard.") { });
                }

                if (timecard.Status != TimecardStatus.Submitted)
                {
                    return StatusCode(409, new InvalidStateError() { });
                }
                
                var transition = new Transition(rejection, TimecardStatus.Rejected);
                timecard.Transitions.Add(transition);
                return Ok(transition);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{timesheetId}/rejection")]
        [Produces(ContentTypes.Transition)]
        [ProducesResponseType(typeof(Transition), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(MissingTransitionError), 409)]
        public IActionResult GetRejection(string timesheetId)
        {
            Timecard timecard = Database.Find(timesheetId);

            if (timecard != null)
            {
                if (timecard.Status == TimecardStatus.Rejected)
                {
                    var transition = timecard.Transitions
                                        .Where(t => t.TransitionedTo == TimecardStatus.Rejected)
                                        .OrderByDescending(t => t.OccurredAt)
                                        .FirstOrDefault();

                    return Ok(transition);                                        
                }
                else 
                {
                    return StatusCode(409, new MissingTransitionError() { });
                }
            }
            else
            {
                return NotFound();
            }
        }
        
        [HttpPost("{timesheetId}/approval")]
        [Produces(ContentTypes.Transition)]
        [ProducesResponseType(typeof(Transition), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(InvalidStateError), 409)]
        [ProducesResponseType(typeof(EmptyTimecardError), 409)]
        public IActionResult Approve(string timesheetId, [FromBody] Approval approval)
        {
            Timecard timecard = Database.Find(timesheetId);

            if (timecard != null)
            {
                if (approval.Resource == timecard.Resource)
                {
                    return StatusCode(409, new InvalidOperationException("The resource should not match the timecard.") { });
                }

                if (timecard.Status != TimecardStatus.Submitted)
                {
                    return StatusCode(409, new InvalidStateError() { });
                }
                
                var transition = new Transition(approval, TimecardStatus.Approved);
                timecard.Transitions.Add(transition);
                return Ok(transition);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{timesheetId}/approval")]
        [Produces(ContentTypes.Transition)]
        [ProducesResponseType(typeof(Transition), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(MissingTransitionError), 409)]
        public IActionResult GetApproval(string timesheetId)
        {
            Timecard timecard = Database.Find(timesheetId);

            if (timecard != null)
            {
                if (timecard.Status == TimecardStatus.Approved)
                {
                    var transition = timecard.Transitions
                                        .Where(t => t.TransitionedTo == TimecardStatus.Approved)
                                        .OrderByDescending(t => t.OccurredAt)
                                        .FirstOrDefault();

                    return Ok(transition);                                        
                }
                else 
                {
                    return StatusCode(409, new MissingTransitionError() { });
                }
            }
            else
            {
                return NotFound();
            }
        }        
    }
}

using Leagues.League.queryDefinition;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TheLongRun.Common;
using TheLongRun.Common.Attributes;
using TheLongRun.Common.Bindings;

namespace TheLongRunLeaguesFunction.Queries
{
    public static class CreateQueryRequestActivity
    {

        [QueryName("Get League Summary")]
        [FunctionName("GetLeagueSummaryCreateQueryRequestActivity")]
        public static async Task<Guid> GetLeagueSummaryCreateQueryRequestActivity([ActivityTrigger] QueryRequest<Get_League_Summary_Definition> queryRequest, 
            ILogger log)
        {

            if (queryRequest.QueryUniqueIdentifier.Equals(Guid.Empty) )
            {
                queryRequest.QueryUniqueIdentifier = Guid.NewGuid(); 
            }

            // Create a new Query record to hold the event stream for this query...
            QueryLogRecord<Get_League_Summary_Definition> qryRecord = QueryLogRecord<Get_League_Summary_Definition>.Create(
                queryRequest.QueryName ,
                queryRequest.Parameters,
                queryRequest.ReturnTarget,
                queryRequest.ReturnPath,
                queryRequest.QueryUniqueIdentifier );


            if (null != qryRecord)
            {
                EventStream queryEvents = new EventStream(@"Query",
                        queryRequest.QueryName ,
                        qryRecord.QueryUniqueIdentifier.ToString());

                if (null != queryEvents)
                {
                    // Log the query creation
                    await queryEvents.AppendEvent(new TheLongRun.Common.Events.Query.QueryCreated(queryRequest.QueryName,
                                qryRecord.QueryUniqueIdentifier));
                }

                // Return the ID of the query record we created
                return qryRecord.QueryUniqueIdentifier;
            }

            // If we got here there was a problem so return an empty GUID
            return Guid.Empty;
        }


    }
}
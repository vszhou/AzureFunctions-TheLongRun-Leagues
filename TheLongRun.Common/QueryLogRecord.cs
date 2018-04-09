﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLongRun.Common
{
    public class QueryLogRecord
    {

        /// <summary>
        /// How to indicate to a caller application that a requested query is complete
        /// </summary>
        /// <remarks>
        /// Other ways of notifying the caller that a query has completed can be added to this
        /// list as and when required
        /// </remarks>
        public enum QueryReturnTarget
        {
            /// <summary>
            /// Store the results of the query in the named azure storage blob, which will either trigger
            /// the next step or can be polled for (not ideal)
            /// </summary>
            AzureBlobStorage = 0,
            /// <summary>
            /// Use the defined webhook to pass the results back for the query
            /// </summary>
            WebHook = 1,
            /// <summary>
            /// Fire off an event grid event with the specified custom topic name to notify that the
            /// query results are available
            /// </summary>
            CustomEventGridTopic = 2
        }

        /// <summary>
        /// The folder that new commands are logged to
        /// </summary>
        public const string DEFAULT_CONTAINER_NAME = @"query-log";

        /// <summary>
        /// The connection string name to use to write to it
        /// </summary>
        public const string DEFAULT_CONNECTION = @"QueryStorageConnectionAppSetting";

        /// <summary>
        /// The unique identifier of this instance of the query
        /// </summary>
        public Guid QueryUniqueIdentifier { get; internal set; }

        /// <summary>
        /// The name of the query that was executed
        /// </summary>
        public string QueryName { get; internal set; }


        /// <summary>
        /// The status of the query when last executed
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Who triggered this query
        /// </summary>
        public string Who { get; set; }

        /// <summary>
        /// When was this command query
        /// </summary>
        public Nullable<DateTime> When { get; set; }

        /// <summary>
        /// Hash of the query name and parameters used to see if two queries are for the same thing
        /// </summary>
        /// <remarks>
        /// This can be used for results cacheing
        /// </remarks>
        public string Hash { get; }

        /// <summary>
        /// Where to notify the requestor that this query has completed
        /// </summary>
        /// <remarks>
        /// This does not preclude other observers also being notified that the 
        /// query has completed
        /// </remarks>
        public QueryReturnTarget ReturnTarget { get; set; }

        /// <summary>
        /// Depending on the return target, this tells the query processor where 
        /// exactly to put the results
        /// </summary>
        /// <remarks>
        /// This could be a storeage URI or a webhook or the custom topic name, for instance
        /// </remarks>
        public string ReturnPath { get; set; }

        public static string MakeFilename(QueryLogRecord queryInstance)
        {
            return queryInstance.QueryName.ToLowerInvariant().Trim() + @"-" + queryInstance.QueryUniqueIdentifier.ToString(@"N") + @".qry";
        }



    }

    public class QueryLogRecord<TQueryParameters>
        : QueryLogRecord 
    {




        /// <summary>
        /// The parameters of the query instance - these will be saved as JSON
        /// </summary>
        public TQueryParameters Parameters { get; set; }




        /// <summary>
        /// Create a new command log record
        /// </summary>
        /// <param name="commandName">
        /// The name of the command we are going to log
        /// </param>
        public static QueryLogRecord<TQueryParameters> Create(string queryName,
            TQueryParameters parameters,
            QueryReturnTarget returnTarget,
            string returnPath)
        {
            QueryLogRecord< TQueryParameters> ret = new QueryLogRecord<TQueryParameters>();
            ret.QueryUniqueIdentifier = System.Guid.NewGuid();
            ret.QueryName  = queryName;
            ret.When = DateTime.UtcNow;
            if (null != parameters )
            {
                ret.Parameters = parameters;
            }
            ret.ReturnTarget = returnTarget;
            ret.ReturnPath = returnPath;
            return ret;

        }


    }
}

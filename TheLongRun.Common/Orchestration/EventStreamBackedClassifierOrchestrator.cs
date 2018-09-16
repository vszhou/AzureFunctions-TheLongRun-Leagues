﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLongRun.Common.Orchestration
{
    /// <summary>
    /// A base class for any CLASSIFIER orchestrator functions
    /// </summary>
    public abstract class EventStreamBackedClassifierOrchestrator
        : IEventStreamBackedOrchestrator
    {
        public abstract bool IsComplete { get; }

        /// <summary>
        /// Orchestrator classification type is a CLASSIFIER
        /// </summary>
        public string ClassificationTypeName
        {
            get
            {
                return EventStreamBackedClassifierOrchestrator.ClassifierTypeName ;
            }
        }


        public abstract string ClassificationInstanceName { get; }

        private readonly Guid _uniqueIdentifier;
        public Guid UniqueIdentifier
        {
            get
            {
                return _uniqueIdentifier;
            }
        }

        private readonly string _instanceIdentifier;
        /// <summary>
        /// The unique key of the (entity) event stream over which the classifier
        /// will be run
        /// </summary>
        public string InstanceIdentifier
        {
            get
            {
                return _instanceIdentifier;
            }
        }

        public abstract IEventStreamBackedOrchestratorContext Context { get; set; }

        /// <summary>
        /// The identity by which any called orchestrations can call back with the 
        /// results (a return address style identity)
        /// </summary>
        public OrchestrationCallbackIdentity CallbackIdentity
        {
            get
            {
                return OrchestrationCallbackIdentity.Create(
                    OrchestrationCallbackIdentity.OrchestrationClassifications.Classifier,
                    ClassificationInstanceName,
                    UniqueIdentifier);
            }
        }

        public abstract void RunNextStep();

        protected internal EventStreamBackedClassifierOrchestrator(Guid uniqueIdentifier,
            string instanceIdentifierKey)
        {
            if (uniqueIdentifier.Equals(Guid.Empty))
            {
                _uniqueIdentifier = Guid.NewGuid();
            }
            else
            {
                _uniqueIdentifier = uniqueIdentifier;
            }
            _instanceIdentifier = instanceIdentifierKey;
        }

        public static string ClassifierTypeName
        {
            get
            {
                return @"CLASSIFIER";
            }
        }
    }
}
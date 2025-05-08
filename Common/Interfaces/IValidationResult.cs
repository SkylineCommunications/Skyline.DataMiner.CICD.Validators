namespace Skyline.DataMiner.CICD.Validators.Common.Interfaces
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    /// <summary>
    /// Represents a validator result.
    /// </summary>
    public interface IValidationResult
    {
        /// <summary>
        /// Gets the sub results.
        /// </summary>
        /// <value>The sub results.</value>
        List<IValidationResult> SubResults { get; }

        /// <summary>
        /// Gets the check ID.
        /// </summary>
        /// <value>The check ID.</value>
        uint CheckId { get; }

        /// <summary>
        /// Gets the error ID.
        /// </summary>
        /// <value>The error ID.</value>
        uint ErrorId { get; }

        /// <summary>
        /// Gets the full ID.
        /// </summary>
        /// <value>The full ID.</value>
        string FullId { get; }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>The category.</value>
        Category Category { get; }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        /// <value>The severity.</value>
        Severity Severity { get; }

        /// <summary>
        /// Gets the certainty.
        /// </summary>
        /// <value>The certainty.</value>
        Certainty Certainty { get; }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>The source.</value>
        Source Source { get; }

        /// <summary>
        /// Gets the fix impact.
        /// </summary>
        /// <value>The fix impact.</value>
        FixImpact FixImpact { get; }

        /// <summary>
        /// Gets the group description.
        /// </summary>
        /// <value>The group description.</value>
        string GroupDescription { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        string Description { get; }

        /// <summary>
        /// Gets information about how to fix the issue.
        /// </summary>
        /// <value>Information about how to fix the issue.</value>
        string HowToFix { get; }

        /// <summary>
        /// Gets example code.
        /// </summary>
        /// <value>Example code.</value>
        [Obsolete("Has been moved to the dedicated markdown file about the error. Will be removed in the next major change update.")]
        string ExampleCode { get; }

        /// <summary>
        /// Gets additional details.
        /// </summary>
        /// <value>The additional details.</value>
        [Obsolete("Has been moved to the dedicated markdown file about the error. Will be removed in the next major change update.")]
        string Details { get; }

        /// <summary>
        /// Gets the reference node.
        /// </summary>
        /// <value>The reference node.</value>
        IReadable ReferenceNode { get; }

        /// <summary>
        /// Gets a value indicating whether a code fix is available.
        /// </summary>
        /// <value><c>true</c> if a code fix is available; otherwise, <c>false</c>.</value>
        bool HasCodeFix { get; }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        int Position { get; }

        /// <summary>
        /// Gets the position of the node.
        /// </summary>
        /// <value>The position of the node.</value>
        IReadable PositionNode { get; }

        /// <summary>
        /// Gets the auto fix warnings.
        /// </summary>
        /// <value>The auto fix warnings.</value>
        List<(string Message, bool AutoFixPopup)> AutoFixWarnings { get; }

        /// <summary>
        /// Gets the DVE export info.
        /// </summary>
        /// <value>The DVE export info.</value>
        (int TablePid, string Name)? DveExport { get; }


        // Old stuff that is still needed...
        int Line { get; }
        string DescriptionFormat { get; }
        object[] DescriptionParameters { get; }
    }
}

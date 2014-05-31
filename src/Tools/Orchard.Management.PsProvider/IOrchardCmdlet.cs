// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrchardCmdlet.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider
{
    using System.Management.Automation;

    /// <summary>
    /// Defines the API for classes which implement Orchard cmdlets.
    /// </summary>
    public interface IOrchardCmdlet
    {
        /// <summary>
        /// Reports a nonterminating error to the error pipeline when the cmdlet cannot process a record but can
        /// continue to process other records.
        /// </summary>
        /// <param name="errorRecord">An <see cref="ErrorRecord"/> object that describes the error condition.</param>
        void WriteError(ErrorRecord errorRecord);

        /// <summary>
        /// Reports a terminating error when the cmdlet cannot continue, or when you do not want the cmdlet to continue
        /// to process records.
        /// </summary>
        /// <param name="errorRecord">An <see cref="ErrorRecord"/> object that describes the error condition.</param>
        void ThrowTerminatingError(ErrorRecord errorRecord);
    }
}
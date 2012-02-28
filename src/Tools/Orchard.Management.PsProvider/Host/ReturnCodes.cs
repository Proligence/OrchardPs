// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReturnCodes.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Host 
{
    /// <summary>
    /// Defines return codes used in <see cref="OrchardSession"/> class.
    /// </summary>
    public enum ReturnCodes
    {
        /// <summary>
        /// Operation succeeded.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// Operation failed.
        /// </summary>
        Fail = 5,
        
        /// <summary>
        /// Operation should be retried.
        /// </summary>
        Retry = 240
    }
}
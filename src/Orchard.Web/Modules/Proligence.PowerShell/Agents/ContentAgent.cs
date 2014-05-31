// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentAgent.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Agents 
{
    using Orchard.Management.PsProvider.Agents;

    /// <summary>
    /// Implements the agent which exposes content types and content items.
    /// </summary>
    public class ContentAgent : AgentBase 
    {
        /// <summary>
        /// Test method.
        /// </summary>
        /// <returns>Array of test strings.</returns>
        public string[] Hello() 
        {
            return new[]
                   {
                       "Hello", 
                       "World", 
                       "! "
                   };
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentAgentProxy.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Agents 
{
    using Orchard.Management.PsProvider.Agents;

    /// <summary>
    /// Implements the proxy for the agent which exposes content types and content items.
    /// </summary>
    [Agent("Proligence.PowerShell.Agents.ContentAgent, Proligence.PowerShell, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    public class ContentAgentProxy : AgentProxy 
    {
        /// <summary>
        /// Test method.
        /// </summary>
        /// <returns>Array of test strings.</returns>
        public string[] Hello() 
        {
            return (string[])Invoke("Hello");
        }
    }
}
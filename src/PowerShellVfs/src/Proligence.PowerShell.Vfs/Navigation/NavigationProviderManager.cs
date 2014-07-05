﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationProviderManager.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Management.Automation;
    using System.Reflection;
    using System.Text;
    using Autofac;
    using Proligence.PowerShell.Vfs.Core;
    using Proligence.PowerShell.Vfs.Provider;
    using Proligence.PowerShell.Vfs.Utils;

    /// <summary>
    /// Implements the PS navigation provider manager.
    /// </summary>
    public class NavigationProviderManager : INavigationProviderManager 
    {
        /// <summary>
        /// The object which exposes the PowerShell-controlled console.
        /// </summary>
        private readonly IPowerShellConsole console;

        /// <summary>
        /// The dependency injection container for the VFS drive which the manager belongs to.
        /// </summary>
        private readonly ILifetimeScope scope;

        /// <summary>
        /// Caches discovered PS navigation providers.
        /// </summary>
        private IPsNavigationProvider[] navigationProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationProviderManager"/> class.
        /// </summary>
        /// <param name="console">The object which exposes the PowerShell-controlled console.</param>
        /// <param name="scope">
        /// The dependency injection container for the VFS drive which the manager belongs to.
        /// </param>
        public NavigationProviderManager(IPowerShellConsole console, ILifetimeScope scope) 
        {
            this.console = console;
            this.scope = scope;
        }

        /// <summary>
        /// Gets all registered PS navigation providers.
        /// </summary>
        /// <returns>A sequence of navigation providers.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By design")]
        public IEnumerable<IPsNavigationProvider> GetProviders() 
        {
            if (this.navigationProviders == null) 
            {
                IEnumerable<Type> types = this.GetNavigationProviderTypes();

                var providers = new List<IPsNavigationProvider>();
                foreach (Type providerType in types) 
                {
                    try 
                    {
                        IPsNavigationProvider provider = this.CreateNavigationProvider(providerType);
                        providers.Add(provider);
                    }
                    catch (Exception ex) 
                    {
                        string message = 
                            "Failed to create instance of type '" + providerType.FullName + "'. " 
                            + ex.CollectMessages();

                        var exception = new VfsProviderException(
                            message, ex, false, ErrorIds.CreateNavigationProviderFailed);
                        
                        this.console.WriteError(exception, exception.ErrorId, ErrorCategory.NotSpecified);
                    }
                }

                this.navigationProviders = providers.ToArray();
            }

            return (IEnumerable<IPsNavigationProvider>)this.navigationProviders.Clone();
        }

        /// <summary>
        /// Gets all registered PS navigation providers which add a node of the specified type.
        /// </summary>
        /// <param name="nodeType">The type of node added by the navigation providers.</param>
        /// <returns>A sequence of navigation providers.</returns>
        public IEnumerable<IPsNavigationProvider> GetProviders(NodeType nodeType) 
        {
            return this.GetProviders()
                .OrderBy(np => np.GetPathLength())
                .Where(np => np.NodeType == nodeType)
                .ToArray();
        }

        /// <summary>
        /// Writes the details of the specified exception to <see cref="Trace"/>.
        /// </summary>
        /// <param name="ex">The exception object.</param>
        /// <param name="message">The error message.</param>
        private static void TraceException(Exception ex, string message) 
        {
            var sb = new StringBuilder(message);
            sb.Append(ex.Message);

            var inner = ex.InnerException;
            while (inner != null) 
            {
                sb.Append(inner.Message);
                inner = inner.InnerException;
            }

            var typeLoadException = ex as ReflectionTypeLoadException;
            if (typeLoadException != null) 
            {
                foreach (Exception loaderException in typeLoadException.LoaderExceptions) 
                {
                    sb.Append(loaderException.Message);
                }
            }

            Trace.WriteLine(sb.ToString());
        }

        /// <summary>
        /// Gets all types which implement the <see cref="IPsNavigationProvider"/> interface from the current
        /// <see cref="AppDomain"/>.
        /// </summary>
        /// <returns>Sequence of types.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By design")]
        private IEnumerable<Type> GetNavigationProviderTypes() 
        {
            List<Type> types = new List<Type>();

            Type navigationProviderType = typeof(IPsNavigationProvider);
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies) 
            {
                if (!Attribute.IsDefined(assembly, typeof(PowerShellExtensionContainerAttribute)))
                {
                    continue;
                }
                
                try 
                {
                    Type[] assemblyTypes = assembly.GetTypes();
                    foreach (Type type in assemblyTypes) 
                    {
                        try 
                        {
                            if (type.IsInterface || type.IsAbstract) 
                            {
                                continue;
                            }
                            
                            if (navigationProviderType.IsAssignableFrom(type)) 
                            {
                                types.Add(type);
                            }
                        }
                        catch (Exception ex) 
                        {
                            TraceException(ex, "Failed to get information about type '" + type.FullName + "'. ");
                        }
                    }
                }
                catch (Exception ex) 
                {
                    TraceException(ex, "Failed to get types for assembly '" + assembly.FullName + "'. ");
                }
            }
            
            return types;
        }

        /// <summary>
        /// Creates and initializes a navigation provider of the specified type.
        /// </summary>
        /// <param name="providerType">The type of the navigation provider to create.</param>
        /// <returns>The created navigation provider instance.</returns>
        private IPsNavigationProvider CreateNavigationProvider(Type providerType) 
        {
            IPsNavigationProvider provider = (IPsNavigationProvider)Activator.CreateInstance(providerType);
            this.scope.InjectUnsetProperties(provider);
            provider.Initialize();
            
            return provider;
        }
    }
}
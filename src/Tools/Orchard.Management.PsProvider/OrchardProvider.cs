// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardProvider.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider 
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Management.Automation;
    using System.Management.Automation.Host;
    using System.Management.Automation.Provider;
    using System.Reflection;
    using Autofac;
    using Orchard.Management.PsProvider.Vfs;

    /// <summary>
    /// Implements the Orchard PS Provider.
    /// </summary>
    [CmdletProvider("Orchard", ProviderCapabilities.ShouldProcess)]
    public class OrchardProvider : NavigationCmdletProvider 
    {
        /// <summary>
        /// The objects which provides access to the PowerShell-controlled console.
        /// </summary>
        private IPowerShellConsole console;

        /// <summary>
        /// Gets the object which represents the state of the Orchard PS provider.
        /// </summary>
        public OrchardProviderInfo OrchardProviderInfo
        {
            get
            {
                return (OrchardProviderInfo)ProviderInfo;
            }
        }

        /// <summary>
        /// Gets the object which can be used to access the PowerShell-controlled output console.
        /// </summary>
        public IPowerShellConsole Console 
        { 
            get 
            {
                if (this.console == null) 
                {
                    this.console = OrchardProviderInfo.Container.Resolve<IPowerShellConsole>(
                        new NamedParameter("provider", this));
                }

                return this.console;
            }
        }

        /// <summary>
        /// Starts the specified provider. This method is called by the Windows PowerShell runtime to initialize the
        /// provider when the provider is loaded into a session.
        /// </summary>
        /// <param name="providerInfo">
        /// A <see cref="ProviderInfo"/> object that describes the provider to be initialized.
        /// </param>
        /// <returns>A <see cref="ProviderInfo"/> object that contains information about the provider.</returns>
        protected override ProviderInfo Start(ProviderInfo providerInfo) 
        {
            // Increase console window size
            this.IncreaseWindowSize(120, 50);

            IContainer container = OrchardProviderContainer.GetContainer();
            return new OrchardProviderInfo(providerInfo, container);
        }

        /// <summary>
        /// Frees resources before the provider is removed from the runspace. The Windows PowerShell runtime calls this
        /// method to allow the provider a chance to stop and clean up its resources before the runtime removes the
        /// provider.
        /// </summary>
        protected override void Stop() 
        {
            IContainer container = OrchardProviderInfo.Container;
            if (container != null) 
            {
                container.Dispose();
            }
        }

        /// <summary>
        /// Creates a new drive based on a specified <see cref="PSDriveInfo"/> object. Overriding this method gives the
        /// provider an opportunity to validate or modify the drive information before the drive is created.
        /// </summary>
        /// <param name="drive">A <see cref="PSDriveInfo"/> object that describes the drive.</param>
        /// <returns>
        /// A <see cref="PSDriveInfo"/> object that represents the new drive. The returned object can be the same
        /// object passed in by the drive parameter or a modified version of it. The default behavior is to return the
        /// passed-in PSDriveInfo object.
        /// If an error occurs, an error record should be sent to the error pipeline using the <c>WriteError</c> method
        /// and null should be returned.
        /// </returns>
        protected override PSDriveInfo NewDrive(PSDriveInfo drive) 
        {
            if (drive == null) 
            {
                this.WriteError(new ArgumentNullException("drive"), ErrorIds.NullDrive, ErrorCategory.InvalidArgument);
                return null;
            }

            if (drive is OrchardDriveInfo) 
            {
                return drive;
            }

            var driveParameters = (OrchardDriveParameters)DynamicParameters;
            if (driveParameters == null) 
            {
                return null;
            }
            
            if (!OrchardPsSnapIn.VerifyOrchardDirectory(driveParameters.OrchardRoot)) 
            {
                this.WriteError(
                    ThrowHelper.InvalidRootPathException(driveParameters.OrchardRoot), 
                    ErrorIds.InvalidRootDirectory, 
                    ErrorCategory.InvalidArgument, 
                    drive);

                return null;
            }

            PSDriveInfo orchardDrive = null;
            this.TryCritical(
                () => orchardDrive = this.InitializeOrchardDrive(drive, driveParameters), 
                ErrorIds.OrchardInitFailed, 
                ErrorCategory.OpenError);

            return orchardDrive;
        }

        /// <summary>
        /// Returns any additional parameters required by this instance of the New-Drive cmdlet. 
        /// </summary>
        /// <returns>
        /// An object that has properties and fields with parsing attributes similar to a cmdlet class.
        /// </returns>
        protected override object NewDriveDynamicParameters() 
        {
            return new OrchardDriveParameters();
        }

        /// <summary>
        /// Removes a Windows PowerShell drive. Implementing this method gives the provider an opportunity to clean up
        /// any provider-specific data for the drive that is going to be removed.
        /// </summary>
        /// <param name="drive">
        /// A <see cref="PSDriveInfo"/> object that represents the mounted drive to be removed.
        /// </param>
        /// <returns>
        /// If the drive can be removed, it should return the <see cref="PSDriveInfo"/> object that was passed in. If
        /// the drive cannot be removed, <c>null</c> should be returned or an exception should be thrown. The default
        /// implementation returns the drive that was passed.
        /// </returns>
        protected override PSDriveInfo RemoveDrive(PSDriveInfo drive) 
        {
            if (drive == null) 
            {
                this.WriteError(new ArgumentNullException("drive"), ErrorIds.NullDrive, ErrorCategory.InvalidArgument);
                return null;
            }

            var orchardDrive = drive as OrchardDriveInfo;
            if (orchardDrive == null) 
            {
                return null;
            }

            this.Try(orchardDrive.Close, ErrorIds.CloseDriveFailed, ErrorCategory.CloseError, orchardDrive);

            return orchardDrive;
        }

        /// <summary>
        /// Gives the provider the ability to map drives after initialization.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="PSDriveInfo"/> objects for each drive the provider wants to mount. If no drives
        /// should be mounted, an empty collection should be returned.
        /// </returns>
        protected override Collection<PSDriveInfo> InitializeDefaultDrives() 
        {
            var drives = new Collection<PSDriveInfo>();

            this.Try(
                () => 
                {
                    Assembly entryAssembly = Assembly.GetEntryAssembly();
                    string path = entryAssembly != null ? entryAssembly.Location : Environment.CurrentDirectory;

                    for (var di = new DirectoryInfo(path); di != null; di = di.Parent) 
                    {
                        if (OrchardPsSnapIn.VerifyOrchardDirectory(di.FullName)) 
                        {
                            var drive = new PSDriveInfo("Orchard", ProviderInfo, "\\", "Orchard drive", Credential);
                            var driveParameters = new OrchardDriveParameters { OrchardRoot = di.FullName };
                            drives.Add(InitializeOrchardDrive(drive, driveParameters));
                            break;
                        }
                    }
                }, 
                ErrorIds.DefaultDrivesInitFailed, 
                ErrorCategory.OpenError);

            return drives;
        }

        /// <summary>
        /// Determines if a path is syntactically and semantically valid for the provider.
        /// </summary>
        /// <param name="path">The path to the item to be examined.</param>
        /// <returns>
        /// <c>true</c> if the path is syntactically and semantically valid for the provider; otherwise <c>false</c>.
        /// </returns>
        protected override bool IsValidPath(string path) 
        {
            return OrchardPath.IsValidPath(path);
        }

        /// <summary>
        /// Determines if an item exists at the specified path.
        /// </summary>
        /// <param name="path">The path to the item to be examined.</param>
        /// <returns><c>true</c> if an item exists at the specified path, otherwise <c>false</c>.</returns>
        protected override bool ItemExists(string path) 
        {
            string normalizedPath = OrchardPath.NormalizePath(path);
            return this.GetNodeByPath(normalizedPath) != null;
        }

        /// <summary>
        /// Retrieves the item at the specified path.
        /// </summary>
        /// <param name="path">The path to the item to retrieve.</param>
        protected override void GetItem(string path) 
        {
            this.Trace("GetItem(Path='{0}')", path);

            this.ForNode(path, node => this.WriteItemNode(node, n => n.Item));
        }

        /// <summary>
        /// Sets the item at the specified path.
        /// </summary>
        /// <param name="path">The path to the item to be set.</param>
        /// <param name="value">The value of the item.</param>
        protected override void SetItem(string path, object value)
        {
            string valueStr = value != null ? value.ToString() : "(null)";
            this.Trace("SetItem(Path='{0}', Value='{1}')", path, valueStr);

            if (!this.InvokeHandler<OrchardVfsNode, object>(
                node => node.SetItemHandler, 
                node => node.SetItemName, 
                path, 
                value)) 
            {
                base.SetItem(path, value);
            }
        }

        /// <summary>
        /// Clears the item at the specified path.
        /// </summary>
        /// <param name="path">The path to the item to be cleared.</param>
        protected override void ClearItem(string path) 
        {
            this.Trace("ClearItem(Path='{0}')", path);

            if (!this.InvokeHandler(node => node.ClearItemHandler, node => node.ClearItemName, path)) 
            {
                base.ClearItem(path);
            }
        }

        /// <summary>
        /// Invokes the default action on the specified item.
        /// </summary>
        /// <param name="path">The path to the item to perform the default operation on.</param>
        protected override void InvokeDefaultAction(string path)
        {
            this.Trace("InvokeDefaultAction(Path='{0}')", path);

            if (!this.InvokeHandler(
                node => node.InvokeDefaultActionHandler, 
                node => node.InvokeDefaultActionName, 
                path))
            {
                base.InvokeDefaultAction(path);
            }
        }

        /// <summary>
        /// Returns strings that contain the names of the child items of the specified path.
        /// </summary>
        /// <param name="path">The path to the item from which to retrieve the child names.</param>
        /// <param name="returnContainers">
        /// <c>true</c> if all containers are returned, even if they do not match the filter. <c>false</c> if the
        /// provider should return only those containers that match the filter.
        /// </param>
        protected override void GetChildNames(string path, ReturnContainers returnContainers)
        {
            this.Trace("GetChildNames(Path='{0}', ReturnContainers='{1}')", path, returnContainers);

            this.ForEachChildNode(path, node => this.WriteItemNode(node, n => n.Name));
        }

        /// <summary>
        /// Determines if the specified item has any child items.
        /// </summary>
        /// <param name="path">The path to the item in question.</param>
        /// <returns>
        /// A <see cref="bool"/> value that indicates <c>true</c> if the item has children, <c>false</c> otherwise.
        /// </returns>
        protected override bool HasChildItems(string path) 
        {
            this.Trace("HasChildItems(Path='{0}')", path);

            bool hasChildItems = false;

            this.ForNode(
                path, 
                node => 
                {
                    ContainerNode containerNode = node as ContainerNode;
                    if (containerNode != null) 
                    {
                        hasChildItems = containerNode.HasChildItems;
                    }
                    else 
                    {
                        hasChildItems = false;
                    }
                });

            return hasChildItems;
        }

        /// <summary>
        /// Gets the children of the item at the specified path.
        /// </summary>
        /// <param name="path">
        /// The path (or name in a flat namespace) to the item from which to retrieve the children.
        /// </param>
        /// <param name="recurse">
        /// <c>true</c> if all children should be retrieved, <c>false</c> if only a single level of children should be
        /// retrieved.
        /// </param>
        protected override void GetChildItems(string path, bool recurse) 
        {
            this.Trace("GetChildItems(Path='{0}')", path);

            this.ForEachChildNode(path, this.WriteItemNode);
        }

        /// <summary>
        /// Creates a new item at the specified path.
        /// </summary>
        /// <param name="path">The path to where the item is to be created.</param>
        /// <param name="itemTypeName">The provider defined type for the item to create.</param>
        /// <param name="newItemValue">
        /// The provider specific type that can be used to create a new instance of an item at the specified path.
        /// </param>
        protected override void NewItem(string path, string itemTypeName, object newItemValue)
        {
            string newItemValueStr = newItemValue != null ? newItemValue.ToString() : "(null)";
            this.Trace("NewItem(Path='{0}', Type='{1}', Value='{2}')", path, itemTypeName, newItemValueStr);

            if (!this.InvokeHandler<ContainerNode, string, object>(
                node => node.NewItemHandler, 
                node => node.NewItemName, 
                path, 
                itemTypeName, 
                newItemValue)) 
            {
                base.NewItem(path, itemTypeName, newItemValue);
            }
        }

        /// <summary>
        /// Copies an item from one path to another.
        /// </summary>
        /// <param name="path">The path of the item that is being copied.</param>
        /// <param name="copyPath">The path to where the item is copied to.</param>
        /// <param name="recurse">Instructs the provider to recursively copy all items.</param>
        protected override void CopyItem(string path, string copyPath, bool recurse) 
        {
            this.Trace("CopyItem(Path='{0}', CopyPath='{1}', Recurse='{2}')", path, copyPath, recurse);

            bool handled = false;
            
            this.ForNode(
                path, 
                node => 
                {
                    if (!this.ShouldProcess(node.Name, node.CopyItemName)) 
                    {
                        handled = true;
                        return;
                    }

                    this.ForNode(
                        copyPath, 
                        copyNode => 
                            this.Try(
                            () => 
                            {
                                node.CopyItemHandler(node, copyNode, recurse);
                                this.WriteItemNode(node);
                            }, 
                            ErrorIds.HandlerError, 
                            ErrorCategory.NotSpecified, 
                            node));
                });

            if (!handled) 
            {
                base.CopyItem(path, copyPath, recurse);
            }
        }

        /// <summary>
        /// Removes (deletes) the item at the specified path.
        /// </summary>
        /// <param name="path">The path to the item to be removed.</param>
        /// <param name="recurse">
        /// <c>true</c> if all children of the item should be removed, <c>false</c> if only a single level of children
        /// should be removed.
        /// </param>
        protected override void RemoveItem(string path, bool recurse) 
        {
            this.Trace("RemoveItem(Path='{0}', Recurse='{1}')", path, recurse);

            if (!this.InvokeHandler<OrchardVfsNode, bool>(
                node => node.RemoveItemHandler, 
                node => node.RemoveItemName, 
                path, 
                recurse)) 
            {
                base.RemoveItem(path, recurse);
            }
        }

        /// <summary>
        /// Renames the item at the specified path.
        /// </summary>
        /// <param name="path">The path to the item to be renamed.</param>
        /// <param name="newName">
        /// The new name for the item. This name should always be relative to the parent container.
        /// </param>
        protected override void RenameItem(string path, string newName) 
        {
            this.Trace("RenameItem(Path='{0}', NewName='{1}')", path, newName);

            if (!this.InvokeHandler<OrchardVfsNode, string>(
                node => node.RenameItemHandler, 
                node => node.RenameItemName, 
                path, 
                newName)) 
            {
                base.RenameItem(path, newName);
            }
        }

        /// <summary>
        /// Determines whether the item is or is not a container item. In this context, the term container refers to an
        /// item that contains other items. In terms of a multi-level data store, a container is a node that has child
        /// nodes that represent another level in the store.
        /// </summary>
        /// <param name="path">The path to be tested.</param>
        /// <returns>
        /// <c>true</c> if the current location is a container item (the current node has child nodes); otherwise
        /// <c>false</c>.
        /// </returns>
        protected override bool IsItemContainer(string path) 
        {
            this.Trace("IsItemContainer(Path='{0}')", path);

            return this.GetNodeByPath(path) is ContainerNode;
        }

        /// <summary>
        /// Moves an item from its current location to another location.
        /// </summary>
        /// <param name="path">The path of the item to be moved.</param>
        /// <param name="destination">The path to the destination container.</param>
        protected override void MoveItem(string path, string destination) 
        {
            this.Trace("MoveItem(Path='{0}', Destination='{1}')", path, destination);

            bool handled = false;
            
            this.ForNode(
                path, 
                node => 
                {
                    if (!ShouldProcess(node.Name, node.MoveItemName)) 
                    {
                        handled = true;
                        return;
                    }

                    this.ForNode(
                        destination, 
                        destinationNode => 
                            this.Try(
                            () => 
                            {
                                node.MoveItemHandler(node, destinationNode);
                                this.WriteItemNode(node);
                            },
                            ErrorIds.HandlerError, 
                            ErrorCategory.NotSpecified, 
                            node));
                });

            if (!handled) 
            {
                base.MoveItem(path, destination);
            }
        }

        /// <summary>
        /// Increases the size of the console window of the host process.
        /// </summary>
        /// <param name="width">The new width of the window.</param>
        /// <param name="height">The new height of the window.</param>
        private void IncreaseWindowSize(int width, int height) 
        {
            Size maximumSize = Host.UI.RawUI.MaxPhysicalWindowSize;
            Size bufferSize = Host.UI.RawUI.BufferSize;
            Size windowSize = Host.UI.RawUI.WindowSize;

            bool updateNeeded = false;

            if (bufferSize.Width < width) 
            {
                bufferSize.Width = (width <= maximumSize.Width) ? width : maximumSize.Width;
                updateNeeded = true;
            }

            if (windowSize.Width < width) 
            {
                windowSize.Width = (width <= maximumSize.Width) ? width : maximumSize.Width;
                updateNeeded = true;
            }

            if (bufferSize.Height < height) 
            {
                bufferSize.Height = (height < maximumSize.Height) ? height : maximumSize.Height;
                updateNeeded = true;
            }

            if (windowSize.Height < height) 
            {
                windowSize.Height = (height <= maximumSize.Height) ? height : maximumSize.Height;
                updateNeeded = true;
            }

            if (updateNeeded) 
            {
                Host.UI.RawUI.BufferSize = bufferSize;
                Host.UI.RawUI.WindowSize = windowSize;
            }
        }

        /// <summary>
        /// Initializes the specified Orchard drive.
        /// </summary>
        /// <param name="drive">The drive to initialize.</param>
        /// <param name="driveParameters">The dynamic parameters passed to the <c>New-PSDrive</c> cmdlet.</param>
        /// <returns>The <see cref="OrchardDriveInfo"/> object which represents the initialized drive.</returns>
        private OrchardDriveInfo InitializeOrchardDrive(PSDriveInfo drive, OrchardDriveParameters driveParameters) 
        {
            this.Console.WriteVerbose("Initializing Orchard session. (This might take a few seconds...)");
            this.Console.WriteLine();
            
            ILifetimeScope lifetimeScope = OrchardProviderInfo.Container.BeginLifetimeScope();

            var orchardDrive = lifetimeScope.Resolve<OrchardDriveInfo>(
                new NamedParameter("driveInfo", drive),
                new NamedParameter("orchardRoot", driveParameters.OrchardRoot));
            orchardDrive.Initialize();

            return orchardDrive;
        }

        /// <summary>
        /// Gets the Orchard VFS node under the specified path in the current Orchard drive.
        /// </summary>
        /// <param name="path">The path of the node to get.</param>
        /// <returns>
        /// The node under the specified path or <c>null</c> if the current drive is not an Orchar drive or there is no
        /// node under the specified path.
        /// </returns>
        private OrchardVfsNode GetNodeByPath(string path) 
        {
            OrchardVfsNode node = null;
            
            this.Try(
                () => 
                {
                    var driveInfo = PSDriveInfo as OrchardDriveInfo;
                    if (driveInfo != null) 
                    {
                        if (OrchardPath.IsDrivePath(path, PSDriveInfo.Root)) 
                        {
                            node = driveInfo.Vfs.Root;
                        }
                        else 
                        {
                            node = driveInfo.Vfs.NavigatePath(path);
                        }
                    }
                },
                ErrorIds.FailedToGetNode, 
                ErrorCategory.ReadError);

            return node;
        }

        /// <summary>
        /// Invokes the specified action for the node under the specified path.
        /// </summary>
        /// <param name="path">The Orchard VFS path.</param>
        /// <param name="action">The action to invoke.</param>
        private void ForNode(string path, Action<OrchardVfsNode> action) 
        {
            string normalizedPath = OrchardPath.NormalizePath(path);

            OrchardVfsNode node = this.GetNodeByPath(normalizedPath);

            if (node != null) 
            {
                this.Try(() => action(node), ErrorIds.NodeEnumerationFailed, ErrorCategory.NotSpecified, node);
            }
            else 
            {
                this.WriteError(
                    ThrowHelper.InvalidPathException(path), 
                    ErrorIds.ItemNotFound, 
                    ErrorCategory.ObjectNotFound);
            }
        }

        /// <summary>
        /// Invokes the specified action for each child node of the node under the specified path.
        /// </summary>
        /// <param name="path">The Orchard VFS path.</param>
        /// <param name="action">The action to invoke.</param>
        private void ForEachChildNode(string path, Action<OrchardVfsNode> action) 
        {
            string normalizedPath = OrchardPath.NormalizePath(path);

            ContainerNode parentNode = this.GetNodeByPath(normalizedPath) as ContainerNode;
            if (parentNode != null) 
            {
                IEnumerable<OrchardVfsNode> items = parentNode.GetChildNodes();
                foreach (OrchardVfsNode node in items) 
                {
                    OrchardVfsNode currentNode = node;
                    this.Try(
                        () => action(currentNode), 
                        ErrorIds.NodeEnumerationFailed, 
                        ErrorCategory.NotSpecified, 
                        node);
                }
            }
            else 
            {
                this.WriteError(
                    ThrowHelper.InvalidPathException(path), 
                    ErrorIds.ItemNotFound, 
                    ErrorCategory.ObjectNotFound);
            }
        }

        /// <summary>
        /// Invokes a node handler.
        /// </summary>
        /// <param name="handlerSelector">The function which returns the action to invoke.</param>
        /// <param name="actionNameSelector">The name of the invoked action.</param>
        /// <param name="path">The Orchard VFS path.</param>
        /// <returns><c>true</c> if a handler was invoked; otherwise, <c>false</c>.</returns>
        private bool InvokeHandler(
            Func<OrchardVfsNode, Action<OrchardVfsNode>> handlerSelector, 
            Func<OrchardVfsNode, string> actionNameSelector,
            string path) 
        {
            return this.InvokeHandlerGeneric(
                path, 
                actionNameSelector, 
                node => 
                {
                    Action<OrchardVfsNode> handler = handlerSelector(node);
                    if (handler == null) 
                    {
                        return false;
                    }

                    this.Try(
                        () => 
                        {
                            handler(node);
                            this.WriteItemNode(node);
                        },
                        ErrorIds.HandlerError, 
                        ErrorCategory.NotSpecified, 
                        node);
                    
                    return true;
                });
        }

        /// <summary>
        /// Invokes a node handler.
        /// </summary>
        /// <typeparam name="TNode">The type of the Orchard VFS node on which the handler will be invoked.</typeparam>
        /// <typeparam name="T">The type of argument for the handler.</typeparam>
        /// <param name="handlerSelector">The function which returns the action to invoke.</param>
        /// <param name="actionNameSelector">The name of the invoked action.</param>
        /// <param name="path">The Orchard VFS path.</param>
        /// <param name="arg">The argument for the handler.</param>
        /// <returns><c>true</c> if a handler was invoked; otherwise, <c>false</c>.</returns>
        private bool InvokeHandler<TNode, T>(
            Func<TNode, Action<TNode, T>> handlerSelector, 
            Func<TNode, string> actionNameSelector,
            string path, 
            T arg) 
            where TNode : OrchardVfsNode 
        {    
            return this.InvokeHandlerGeneric(
                path, 
                actionNameSelector, 
                node => 
                {
                    Action<TNode, T> handler = handlerSelector(node);
                    if (handler == null) 
                    {
                        return false;
                    }

                    this.Try(
                        () => 
                        {
                            handler(node, arg);
                            this.WriteItemNode(node);
                        },
                        ErrorIds.HandlerError, 
                        ErrorCategory.NotSpecified, 
                        node);
                    
                    return true;
                });
        }

        /// <summary>
        /// Invokes a node handler.
        /// </summary>
        /// <typeparam name="TNode">The type of the Orchard VFS node on which the handler will be invoked.</typeparam>
        /// <typeparam name="T1">The type of the first argument for the handler.</typeparam>
        /// <typeparam name="T2">The type of the second argument for the handler.</typeparam>
        /// <param name="handlerSelector">The function which returns the action to invoke.</param>
        /// <param name="actionNameSelector">The name of the invoked action.</param>
        /// <param name="path">The Orchard VFS path.</param>
        /// <param name="arg1">The first argument for the handler.</param>
        /// <param name="arg2">The second argument for the handler.</param>
        /// <returns><c>true</c> if a handler was invoked; otherwise, <c>false</c>.</returns>
        private bool InvokeHandler<TNode, T1, T2>(
            Func<TNode, Action<TNode, T1, T2>> handlerSelector,
            Func<TNode, string> actionNameSelector,
            string path,
            T1 arg1,
            T2 arg2) 
            where TNode : OrchardVfsNode 
        {
            return this.InvokeHandlerGeneric(
                path, 
                actionNameSelector, 
                node => 
                {
                    Action<TNode, T1, T2> handler = handlerSelector(node);
                    if (handler == null) 
                    {
                        return false;
                    }

                    this.Try(
                        () => 
                        {
                            handler(node, arg1, arg2);
                            this.WriteItemNode(node);
                        },
                        ErrorIds.HandlerError, 
                        ErrorCategory.NotSpecified, 
                        node);
                    
                    return true;
                });
        }

        /// <summary>
        /// Invokes a node handler.
        /// </summary>
        /// <typeparam name="TNode">The type of the Orchard VFS node on which the handler will be invoked.</typeparam>
        /// <param name="path">The path of the node on which the handler will be invoked.</param>
        /// <param name="actionNameSelector">The name of the invoked action.</param>
        /// <param name="action">The handler action to invoke.</param>
        /// <returns><c>true</c> if a handler was invoked; otherwise, <c>false</c>.</returns>
        private bool InvokeHandlerGeneric<TNode>(
            string path,
            Func<TNode, string> actionNameSelector,
            Func<TNode, bool> action) 
            where TNode : OrchardVfsNode 
        {
            bool handled = false;
            
            this.ForNode(
                path, 
                node => 
                {
                    TNode typedNode = node as TNode;
                    if (typedNode == null) 
                    {
                        this.WriteError(
                            new InvalidOperationException("The operation is not valid for this item."), 
                            ErrorIds.HandlerError, 
                            ErrorCategory.InvalidArgument, 
                            node);

                        handled = true;
                        return;
                    }

                    if (!this.ShouldProcess(node.Name, actionNameSelector(typedNode))) 
                    {
                        handled = true;
                        return;
                    }

                    handled = action(typedNode);
                });

            return handled;
        }
    }
}
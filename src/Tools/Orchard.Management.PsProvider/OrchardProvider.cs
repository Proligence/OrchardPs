using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Reflection;
using Autofac;
using Orchard.Management.PsProvider.Vfs;

namespace Orchard.Management.PsProvider {
    [CmdletProvider("Orchard", ProviderCapabilities.ShouldProcess)]
    public class OrchardProvider : NavigationCmdletProvider {
        private IPowerShellConsole _console;

        public OrchardProviderInfo OrchardProviderInfo {
            get { return (OrchardProviderInfo)ProviderInfo; }
        }

        public IPowerShellConsole Console { 
            get {
                if (_console == null) {
                    _console = OrchardProviderInfo.Container.Resolve<IPowerShellConsole>(
                        new NamedParameter("provider", this));
                }

                return _console;
            }
        }

        protected override ProviderInfo Start(ProviderInfo providerInfo) {
            IContainer container = OrchardProviderContainer.GetContainer();
            return new OrchardProviderInfo(providerInfo, container);
        }

        protected override void Stop() {
            IContainer container = OrchardProviderInfo.Container;
            if (container != null) {
                container.Dispose();
            }
        }

        protected override PSDriveInfo NewDrive(PSDriveInfo drive) {
            if (drive == null) {
                this.WriteError(new ArgumentNullException("drive"), ErrorIds.NullDrive, ErrorCategory.InvalidArgument);
                return null;
            }

            if (drive is OrchardDriveInfo) {
                return drive;
            }

            var driveParameters = (OrchardDriveParameters)DynamicParameters;
            if (driveParameters == null) {
                return null;
            }
            if (!OrchardPsSnapIn.VerifyOrchardDirectory(driveParameters.OrchardRoot)) {
                this.WriteError(
                    ThrowHelper.InvalidRootPathException(driveParameters.OrchardRoot), 
                    ErrorIds.InvalidRootDirectory, 
                    ErrorCategory.InvalidArgument, 
                    drive);

                return null;
            }

            try {
                return InitializeOrchardDrive(drive, driveParameters);
            }
            catch (Exception ex) {
                this.ThrowTerminatingError(ex, ErrorIds.OrchardInitFailed, ErrorCategory.OpenError);
                return null;
            }
        }

        protected override object NewDriveDynamicParameters() {
            return new OrchardDriveParameters();
        }

        protected override PSDriveInfo RemoveDrive(PSDriveInfo drive) {
            if (drive == null) {
                this.WriteError(new ArgumentNullException("drive"), ErrorIds.NullDrive, ErrorCategory.InvalidArgument);
                return null;
            }

            var orchardDrive = drive as OrchardDriveInfo;
            if (orchardDrive == null) {
                return null;
            }

            try {
                orchardDrive.Close();
            }
            catch (Exception ex) {
                this.WriteError(ex, ErrorIds.CloseDriveFailed, ErrorCategory.CloseError, orchardDrive);
            }

            return orchardDrive;
        }

        protected override Collection<PSDriveInfo> InitializeDefaultDrives() {
            var drives = new Collection<PSDriveInfo>();

            try {
                Assembly entryAssembly = Assembly.GetEntryAssembly();
                string path = entryAssembly != null ? entryAssembly.Location : System.Environment.CurrentDirectory;

                for (var di = new DirectoryInfo(path); di != null; di = di.Parent) {
                    if (OrchardPsSnapIn.VerifyOrchardDirectory(di.FullName)) {
                        var drive = new PSDriveInfo("Orchard", ProviderInfo, "\\", "Orchard drive", Credential);
                        var driveParameters = new OrchardDriveParameters { OrchardRoot = di.FullName };
                        drives.Add(InitializeOrchardDrive(drive, driveParameters));
                        break;
                    }
                }
            }
            catch (Exception ex) {
                this.WriteError(ex, ErrorIds.DefaultDrivesInitFailed, ErrorCategory.OpenError);
            }

            return drives;
        }

        protected override bool IsValidPath(string path) {
            return OrchardPath.IsValidPath(path);
        }

        protected override bool ItemExists(string path) {
            string normalizedPath = OrchardPath.NormalizePath(path);
            return GetNodeByPath(normalizedPath) != null;
        }

        protected override void GetItem(string path) {
            this.Trace("GetItem(Path='{0}')", path);

            ForNode(path, node => this.WriteItemNode(node, n => n.Item));
        }

        protected override void SetItem(string path, object value) {
            this.Trace("SetItem(Path='{0}', Value='{1}')", path, value.ToString());

            if (!InvokeHandler<OrchardVfsNode, object>(node => node.SetItemHandler, node => node.SetItemName, path, value)) {
                base.SetItem(path, value);
            }
        }

        protected override void ClearItem(string path) {
            this.Trace("ClearItem(Path='{0}')", path);

            if (!InvokeHandler(node => node.ClearItemHandler, node => node.ClearItemName, path)) {
                base.ClearItem(path);
            }
        }

        protected override void InvokeDefaultAction(string path) {
            this.Trace("InvokeDefaultAction(Path='{0}')", path);

            if (!InvokeHandler(node => node.InvokeDefaultActionHandler, node => node.InvokeDefaultActionName, path)) {
                base.InvokeDefaultAction(path);
            }
        }

        protected override void GetChildNames(string path, ReturnContainers returnContainers) {
            this.Trace("GetChildNames(Path='{0}', ReturnContainers='{1}')", path, returnContainers);

            ForEachChildNode(path, node => this.WriteItemNode(node, n => n.Name));
        }

        protected override bool HasChildItems(string path) {
            this.Trace("HasChildItems(Path='{0}')", path);

            bool hasChildItems = false;

            ForNode(path, 
                node => {
                    ContainerNode containerNode = node as ContainerNode;
                    if (containerNode != null) {
                        hasChildItems = containerNode.HasChildItems;
                    }
                    else {
                        hasChildItems = false;
                    }
                });

            return hasChildItems;
        }

        protected override void GetChildItems(string path, bool recurse) {
            this.Trace("GetChildItems(Path='{0}')", path);

            ForEachChildNode(path, node => this.WriteItemNode(node));
        }

        protected override void NewItem(string path, string itemTypeName, object newItemValue) {
            this.Trace("NewItem(Path='{0}', Type='{1}', Value='{2}')", path, itemTypeName, newItemValue.ToString());

            if (!InvokeHandler<ContainerNode, string, object>(node => node.NewItemHandler, node => node.NewItemName, path, itemTypeName, newItemValue)) {
                base.NewItem(path, itemTypeName, newItemValue);
            }
        }

        protected override void CopyItem(string path, string copyPath, bool recurse) {
            this.Trace("CopyItem(Path='{0}', CopyPath='{1}', Recurse='{2}')", path, copyPath, recurse);

            bool handled = false;
            
            ForNode(path, 
                node => {
                    if (!ShouldProcess(node.Name, node.CopyItemName)) {
                        handled = true;
                        return;
                    }

                    ForNode(copyPath, 
                        copyNode => {
                            try {
                                node.CopyItemHandler(node, copyNode, recurse);
                                this.WriteItemNode(node);
                            }
                            catch (Exception ex) {
                                this.WriteError(ex, ErrorIds.HandlerError, ErrorCategory.NotSpecified, node);
                            }
                        });
                });

            if (!handled) {
                base.CopyItem(path, copyPath, recurse);
            }
        }

        protected override void RemoveItem(string path, bool recurse) {
            this.Trace("RemoveItem(Path='{0}', Recurse='{1}')", path, recurse);

            if (!InvokeHandler<OrchardVfsNode, bool>(node => node.RemoveItemHandler, node => node.RemoveItemName, path, recurse)) {
                base.RemoveItem(path, recurse);
            }
        }

        protected override void RenameItem(string path, string newName) {
            this.Trace("RenameItem(Path='{0}', NewName='{1}')", path, newName);

            if (!InvokeHandler<OrchardVfsNode, string>(node => node.RenameItemHandler, node => node.RenameItemName, path, newName)) {
                base.RenameItem(path, newName);
            }
        }

        protected override bool IsItemContainer(string path) {
            this.Trace("IsItemContainer(Path='{0}')", path);

            return GetNodeByPath(path) is ContainerNode;
        }

        protected override void MoveItem(string path, string destination) {
            this.Trace("MoveItem(Path='{0}', Destination='{1}')", path, destination);

            bool handled = false;
            
            ForNode(path, 
                node => {
                    if (!ShouldProcess(node.Name, node.MoveItemName)) {
                        handled = true;
                        return;
                    }

                    ForNode(destination, 
                        destinationNode => {
                            try {
                                node.MoveItemHandler(node, destinationNode);
                                this.WriteItemNode(node);
                            }
                            catch (Exception ex) {
                                this.WriteError(ex, ErrorIds.HandlerError, ErrorCategory.NotSpecified, node);
                            }
                        });
                });

            if (!handled) {
                base.MoveItem(path, destination);
            }
        }

        private PSDriveInfo InitializeOrchardDrive(PSDriveInfo drive, OrchardDriveParameters driveParameters) {
            Console.WriteWarning("Initializing Orchard session. (This might take a few seconds...)");
            
            ILifetimeScope lifetimeScope = OrchardProviderInfo.Container.BeginLifetimeScope();

            var orchardDrive = lifetimeScope.Resolve<OrchardDriveInfo>(
                new NamedParameter("driveInfo", drive),
                new NamedParameter("orchardRoot", driveParameters.OrchardRoot));
            orchardDrive.Initialize();

            return orchardDrive;
        }

        private OrchardVfsNode GetNodeByPath(string path) {
            try {
                var driveInfo = PSDriveInfo as OrchardDriveInfo;
                if (driveInfo != null) {
                    if (OrchardPath.IsDrivePath(path, PSDriveInfo.Root)) {
                        return driveInfo.Vfs.Root;
                    }

                    return driveInfo.Vfs.NavigatePath(path);
                }
            }
            catch (Exception ex) {
                this.WriteError(ex, ErrorIds.FailedToGetNode, ErrorCategory.ReadError);
            }

            return null;
        }

        private void ForNode(string path, Action<OrchardVfsNode> action) {
            string normalizedPath = OrchardPath.NormalizePath(path);

            OrchardVfsNode node = GetNodeByPath(normalizedPath);
            if (node != null) {
                try {
                    action(node);
                }
                catch (Exception ex) {
                    this.WriteError(ex, ErrorIds.NodeEnumerationFailed, ErrorCategory.NotSpecified, node);
                }
            }
            else {
                this.WriteError(ThrowHelper.InvalidPathException(path), ErrorIds.ItemNotFound, ErrorCategory.ObjectNotFound);
            }
        }

        private void ForEachChildNode(string path, Action<OrchardVfsNode> action) {
            string normalizedPath = OrchardPath.NormalizePath(path);

            ContainerNode parentNode = GetNodeByPath(normalizedPath) as ContainerNode;
            if (parentNode != null) {
                IEnumerable<OrchardVfsNode> items = parentNode.GetChildNodes();
                foreach (OrchardVfsNode node in items) {
                    try {
                        action(node);
                    }
                    catch (Exception ex) {
                        this.WriteError(ex, ErrorIds.NodeEnumerationFailed, ErrorCategory.NotSpecified, node);
                    }
                }
            }
            else {
                this.WriteError(ThrowHelper.InvalidPathException(path), ErrorIds.ItemNotFound, ErrorCategory.ObjectNotFound);
            }
        }

        private bool InvokeHandler(
            Func<OrchardVfsNode, Action<OrchardVfsNode>> handlerSelector, 
            Func<OrchardVfsNode, string> actionNameSelector,
            string path) {

            return InvokeHandlerGeneric(path, actionNameSelector, 
                node => {
                    Action<OrchardVfsNode> handler = handlerSelector(node);
                    if (handler == null) {
                        return false;
                    }
                    
                    try {
                        handler(node);
                        this.WriteItemNode(node);
                    }
                    catch (Exception ex) {
                        this.WriteError(ex, ErrorIds.HandlerError, ErrorCategory.NotSpecified, node);
                    }

                    return true;
                });
        }

        private bool InvokeHandler<TNode, T>(
            Func<TNode, Action<TNode, T>> handlerSelector, 
            Func<TNode, string> actionNameSelector,
            string path, 
            T arg) where TNode : OrchardVfsNode {
            
            return InvokeHandlerGeneric(path, actionNameSelector, 
                node => {
                    Action<TNode, T> handler = handlerSelector(node);
                    if (handler == null) {
                        return false;
                    }
                    
                    try {
                        handler(node, arg);
                        this.WriteItemNode(node);
                    }
                    catch (Exception ex) {
                        this.WriteError(ex, ErrorIds.HandlerError, ErrorCategory.NotSpecified, node);
                    }

                    return true;
                });
        }

        private bool InvokeHandler<TNode, T1, T2>(
            Func<TNode, Action<TNode, T1, T2>> handlerSelector,
            Func<TNode, string> actionNameSelector,
            string path,
            T1 arg1,
            T2 arg2) where TNode : OrchardVfsNode {
            
            return InvokeHandlerGeneric(path, actionNameSelector, 
                node => {
                    Action<TNode, T1, T2> handler = handlerSelector(node);
                    if (handler == null) {
                        return false;
                    }
                    
                    try {
                        handler(node, arg1, arg2);
                        this.WriteItemNode(node);
                    }
                    catch (Exception ex) {
                        this.WriteError(ex, ErrorIds.HandlerError, ErrorCategory.NotSpecified, node);
                    }

                    return true;
                });
        }

        private bool InvokeHandlerGeneric<TNode>(
            string path,
            Func<TNode, string> actionNameSelector,
            Func<TNode, bool> action) where TNode : OrchardVfsNode {

            bool handled = false;
            
            ForNode(path, 
                node => {
                    TNode typedNode = node as TNode;
                    if (typedNode == null) {
                        this.WriteError(
                            new InvalidOperationException("The operation is not supported for this item."), 
                            ErrorIds.HandlerError, ErrorCategory.InvalidArgument, node);
                        handled = true;
                        return;
                    }

                    if (!ShouldProcess(node.Name, actionNameSelector(typedNode))) {
                        handled = true;
                        return;
                    }

                    handled = action(typedNode);
                });

            return handled;
        }
    }
}
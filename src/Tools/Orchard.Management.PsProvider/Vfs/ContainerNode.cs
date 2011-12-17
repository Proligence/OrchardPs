using System;
using System.Collections.Generic;
using System.Linq;

namespace Orchard.Management.PsProvider.Vfs {
    public class ContainerNode : OrchardVfsNode {
        private readonly List<OrchardVfsNode> _staticNodes;

        public ContainerNode(string name) : base(name) {
            _staticNodes = new List<OrchardVfsNode>();
            NewItemName = "New item";
        }

        public ContainerNode(string name, IEnumerable<OrchardVfsNode> staticNodes) : base(name) {
            _staticNodes = new List<OrchardVfsNode>();
            NewItemName = "New item";

            foreach (OrchardVfsNode node in staticNodes) {
                AddStaticNode(node);
            }
        }

        public Action<OrchardVfsNode, string, object> NewItemHandler { get; protected set; }
        public string NewItemName { get; protected set; }

        public bool HasChildItems {
            get {
                if (_staticNodes.Count > 0) {
                    return true;
                }

                IEnumerable<OrchardVfsNode> nodes = GetVirtualNodes();
                if ((nodes != null) && (nodes.Count() > 0)) {
                    return true;
                }

                return false;
            }
        }

        public IEnumerable<OrchardVfsNode> StaticNodes {
            get { return _staticNodes; }
        }

        public virtual IEnumerable<OrchardVfsNode> GetVirtualNodes() {
            return new OrchardVfsNode[0];
        }

        public IEnumerable<OrchardVfsNode> GetChildNodes(bool recurse = false) {
            var nodes = new List<OrchardVfsNode>(_staticNodes);
            
            IEnumerable<OrchardVfsNode> virtualNodes = GetVirtualNodes();
            if (virtualNodes != null) {
                nodes.AddRange(virtualNodes);
            }

            nodes.ForEach(node => node.Parent = this);
            nodes = nodes.OrderBy(node => node.Name).ToList();

            if (recurse) {
                var subnodes = new List<OrchardVfsNode>();
                foreach (OrchardVfsNode node in nodes) {
                    ContainerNode containerNode = node as ContainerNode;
                    if (containerNode != null) {
                        subnodes.AddRange(containerNode.GetChildNodes(true));
                    }
                }

                nodes.AddRange(subnodes);
            }

            return nodes;
        }

        public override OrchardVfsNode NavigatePath(string path) {
            OrchardVfsNode node = base.NavigatePath(path);
            if (node != null) {
                return node;
            }

            if (path.StartsWith("\\")) {
                path = path.Substring(1);
            }

            string[] pathParts = path.Split(new[] { '\\' });

            node = this;
            for (int i = 0; i < pathParts.Length; i++) {
                int index = i;

                ContainerNode containerNode = node as ContainerNode;
                if (containerNode == null) {
                    return null;
                }

                OrchardVfsNode nextNode = containerNode.StaticNodes
                    .Where(n => n.Name.Equals(pathParts[index], StringComparison.InvariantCulture))
                    .FirstOrDefault();

                if (nextNode == null) {
                    IEnumerable<OrchardVfsNode> virtualNodes = containerNode.GetVirtualNodes();
                    if (virtualNodes != null) {
                        nextNode = virtualNodes
                            .Where(n => n.Name.Equals(pathParts[index], StringComparison.InvariantCulture))
                            .FirstOrDefault();
                    }
                }

                if (nextNode == null) {
                    return null;
                }

                node = nextNode;
            }

            return node;
        }

        internal void AddStaticNode(OrchardVfsNode node) {
            _staticNodes.Add(node);
            node.Parent = this;
        }
    }
}
using System;
using System.ComponentModel;

namespace DSLink.SDK.Nodes
{
    public class Node
    {
        /// <summary>
        /// Array that contains illegal characters in paths for the DSA protocol.
        /// </summary>
        private static readonly char[] BannedChars =
        {
            '%', '.', '/', '\\', '?', '*', ':', '|', '<', '>', '$', '@', ',', '\'', '"'
        };

        /// <summary>
        /// Parent to the this Node.
        /// If this is null, this Node is either the super-root Node
        /// or is a standalone Node.
        /// </summary>
        public readonly Node Parent;

        /// <summary>
        /// Path of this Node.
        /// <example>
        /// Node paths appear to be Unix-like filesystem paths.
        /// The root node is `/`
        /// A Node under the root would be `/node`
        /// A Node under the above Node could be `/node/nodeChild`
        /// </example>
        /// </summary>
        public readonly string Path;

        /// <summary>
        /// Class name of this Node.
        /// Classes are used to initialize a state that has to
        /// do with this specific Node. This is especially important
        /// on future restarts of the DSLink, as the Node tree is
        /// restored from a serialized file on disk.
        /// </summary>
        public string ClassName { get; internal set; }

        public Node(string pathName, Node parent, string className = "node")
        {
            if (string.IsNullOrEmpty(pathName))
            {
                throw new ArgumentException("pathName cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(className))
            {
                throw new ArgumentException("className cannot be null or empty.");
            }

            if (pathName.IndexOfAny(BannedChars) != -1)
            {
                throw new ArgumentException("pathName cannot contain illegal characters.");
            }

            ClassName = className;
            Parent = parent;
        }

        internal Node(Container container)
        {
            
        }

        /// <summary>
        /// Recursively runs up the Node tree to determine the 
        /// root Node of the tree. If the parent of this Node is
        /// null, return the current Node.
        /// </summary>
        /// <returns>Root node of this Node tree</returns>
        public Node GetRootNode()
        {
            return Parent == null ? this : Parent.GetRootNode();
        }
    }
}

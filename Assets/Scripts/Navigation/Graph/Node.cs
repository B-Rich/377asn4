#region Copyright © ThotLab Games 2011. Licensed under the terms of the Microsoft Reciprocal Licence (Ms-RL).

// Microsoft Reciprocal License (Ms-RL)
//
// This license governs use of the accompanying software. If you use the software, you accept this
// license. If you do not accept the license, do not use the software.
//
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same
// meaning here as under U.S. copyright law.
// A "contribution" is the original software, or any additions or changes to the software.
// A "contributor" is any person that distributes its contribution under this license.
// "Licensed patents" are a contributor's patent claims that read directly on its contribution.
//
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and
// limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free
// copyright license to reproduce its contribution, prepare derivative works of its contribution,
// and distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and
// limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free
// license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or
// otherwise dispose of its contribution in the software or derivative works of the contribution in
// the software.
//
// 3. Conditions and Limitations
// (A) Reciprocal Grants- For any file you distribute that contains code from the software (in
// source code or binary format), you must provide recipients the source code to that file along
// with a copy of this license, which license will govern that file. You may license other files
// that are entirely your own work and do not contain code from the software under any terms you
// choose.
// (B) No Trademark License- This license does not grant you rights to use any contributors' name,
// logo, or trademarks.
// (C) If you bring a patent claim against any contributor over patents that you claim are
// infringed by the software, your patent license from such contributor to the software ends
// automatically.
// (D) If you distribute any portion of the software, you must retain all copyright, patent,
// trademark, and attribution notices that are present in the software.
// (E) If you distribute any portion of the software in source code form, you may do so only under
// this license by including a complete copy of this license with your distribution. If you
// distribute any portion of the software in compiled or object code form, you may only do so under
// a license that complies with this license.
// (F) The software is licensed "as-is." You bear the risk of using it. The contributors give no
// express warranties, guarantees or conditions. You may have additional consumer rights under your
// local laws which this license cannot change. To the extent permitted under your local laws, the
// contributors exclude the implied warranties of merchantability, fitness for a particular purpose
// and non-infringement.

#endregion Copyright © ThotLab Games 2011. Licensed under the terms of the Microsoft Reciprocal Licence (Ms-RL).

namespace Thot.GameAI
{
	using UnityEngine;
	
	/// <summary>
    /// Node class to be used with graphs.
    /// </summary>
	public sealed class Node 
	{
		/// <summary>
        /// Valid node indices are positive. This signifies an invalid index.
        /// </summary>
        public const int INVALID_NODE_INDEX = -1;
		
		public GameObject gameObject;

        public Node()
        {
            Index = INVALID_NODE_INDEX;
        }

        public Node(int index)
        {
            Index = index;
        }

        /// <summary>
        /// Gets or sets the node index. Every node has an index. A valid index is >= 0.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the node position.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Tests if the given node index is invalid.
        /// </summary>
        /// <param name="index">The node index.</param>
        /// <returns>True if the node index is valid. Otherwise, false.</returns>
        public static bool IsInvalidIndex(int index)
        {
            return index == INVALID_NODE_INDEX;
        }

        /// <summary>
        /// Generate a string representation of this node.
        /// </summary>
        /// <returns>A string representation of this node.</returns>
        public override string ToString()
        {
            return "NodeIndex: " + (IsInvalidIndex(Index) ? "INVALID" : Index.ToString());
        }
	}
}
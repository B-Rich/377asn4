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
	using System.Collections.Generic;
	
	using UnityEngine;
	
	/// <summary>
    /// Class to represent a path. This path can be used by a path planner in the creation of paths.
    /// </summary>
	public sealed class Path 
	{
		public Path(
		    PathManager pathManager,
		    GameObject movingObject,
            Vector2 source,
            Vector2 destination,
            List<int> pathNodeIndices,
            SparseGraph graph)
        {
            PathEdgeList = new List<PathEdge>();
            Source = source;
            Destination = destination;
			PathManager = pathManager;
			MovingObject = movingObject;
			
			int fromNodeIndex = -1;
			int toNodeIndex = -1;
			Node fromNode = null;
			Node toNode = null;
			Edge currentEdge = null;
			
            Vector2 from = source;
            foreach (int nodeIndex in pathNodeIndices)
            {
				fromNodeIndex = toNodeIndex;
				toNodeIndex = nodeIndex;
				
				fromNode = toNode;	
				toNode = graph.GetNode(nodeIndex);
			
                Vector2 to = toNode.Position;
                if (from == to)
                {
                    // this could happen when source is exactly on a node
                    // position. In this case, skip this redundant edge
                    continue;
                }
				
				if (fromNodeIndex != -1 && toNodeIndex != -1)
				{
					currentEdge = graph.GetEdge(fromNodeIndex, toNodeIndex);
				}

                PathEdgeList.Add(new PathEdge(from, to, fromNode, toNode, currentEdge, PathManager, MovingObject));
                from = to; // copy
            }

            if (from != destination)
            {
                PathEdgeList.Add(new PathEdge(from, destination, toNode, null, null, PathManager, MovingObject));
            }
        }

        /// <summary>
        /// Gets or sets the path source position.
        /// </summary>
        public Vector2 Source { get; set; }

        /// <summary>
        /// Gets or sets the path destination position.
        /// </summary>
        public Vector2 Destination { get; set; }

        /// <summary>
        /// Gets the list of edges in in the path.
        /// </summary>
        public List<PathEdge> PathEdgeList { get; private set; }
		
		public PathManager PathManager { get; private set; }
		public GameObject MovingObject { get; private set; }
		
		public bool IsEmpty
		{
			get
			{
				return PathEdgeList == null || PathEdgeList.Count == 0;
			}
		}
		
		public PathEdge Dequeue()
		{
			if (IsEmpty)
	        {
	            return null;
	        }
			//8. This seems fine
	        PathEdge firstEdge = PathEdgeList[0];
	        PathEdgeList.RemoveAt(0);
			return firstEdge;
		}
		
		public override string ToString()
		{
			// TODO: use StringBuilder
			string pathString = string.Empty;
			for (int i = 0; i < PathEdgeList.Count; i++)
			{
				pathString += "[ " + PathEdgeList[i].ToString() + " ] ";
			}
			
			return pathString;
		}
		
		public void ShowPath(bool show)
		{
			if (PathEdgeList != null)
			{
				foreach (PathEdge pathEdge in PathEdgeList)
				{
					pathEdge.ShowEdge(show);
				}
			}
		}
	}
}

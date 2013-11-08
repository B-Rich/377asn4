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

using Thot.GameAI;

using UnityEngine;

public struct FollowCompletedEventPayload
{
    public GameObject gameObject;
    public Path path;

    public FollowCompletedEventPayload(
        GameObject gameObject,
        Path path)
    {
        this.gameObject = gameObject;
        this.path = path;
    }
}

public struct FollowFailedEventPayload
{
    public GameObject gameObject;
    public Path path;

    public FollowFailedEventPayload(
        GameObject gameObject,
        Path path)
    {
        this.gameObject = gameObject;
        this.path = path;
    }
}

public sealed class PathFollower : MonoBehaviour 
{
	private EdgeTraverser edgeTraverser;
	
	/// <summary>
    /// Gets a local copy of the path returned by the path planner.
    /// </summary>
    public Path PathToFollow { get; private set; }
	
	public bool IsFollowing { get; private set; }
	
	public bool BrakeOnFinalApproach { get; private set; }

	public bool StopOnFinalArrival { get; private set; }

	public bool BrakeOnEachApproach { get; private set; }

	public bool StopOnEachArrival { get; private set; }
	
	private void OnEnable()
	{
		EventManager.Instance.Subscribe<TraversalCompletedEventPayload>(
			Events.TraversalCompleted, 
		    OnTraversalCompleted);
		
		EventManager.Instance.Subscribe<TraversalFailedEventPayload>(
			Events.TraversalFailed, 
		    OnTraversalFailed);
		
		EventManager.Instance.Subscribe<PathReadyEventPayload>(
			Events.PathReady, 
		    OnPathReady);
	}
	
	private void OnDisable()
	{
		EventManager.Instance.Unsubscribe<TraversalCompletedEventPayload>(
			Events.TraversalCompleted, 
		    OnTraversalCompleted);
		
		EventManager.Instance.Unsubscribe<TraversalFailedEventPayload>(
			Events.TraversalFailed, 
		    OnTraversalFailed);
		
		EventManager.Instance.Unsubscribe<PathReadyEventPayload>(
			Events.PathReady, 
		    OnPathReady);
	}
	
	public bool Follow(Path pathToFollow)
    {
        return Follow(pathToFollow, true, true, false, false);
    }
	
	public bool Follow(
		Path pathToFollow,
        bool brakeOnFinalApproach,
        bool stopOnFinalArrival,
        bool brakeOnEachApproach,
        bool stopOnEachArrival)
    {
		if (edgeTraverser == null)
		{
        	edgeTraverser = GetComponent<EdgeTraverser>();
		}
		
        if (edgeTraverser == null)
        {
            return false;
        }
		
		StopIfFollowingPath();
		
        PathToFollow = pathToFollow;
        BrakeOnFinalApproach = brakeOnFinalApproach;
        StopOnFinalArrival = stopOnFinalArrival;
        BrakeOnEachApproach = brakeOnEachApproach;
        StopOnEachArrival = stopOnEachArrival;
        IsFollowing = true;
		
		if (PathToFollow != null)
		{
			PathToFollow.ShowPath(true);
		}

        TraverseNextEdge();

        return true;
    }
	
	private void StopIfFollowingPath()
	{
		if (PathToFollow != null)
		{
			PathToFollow.ShowPath(false);
			
			if (IsFollowing)
			{
				IsFollowing = false;
				
				// TODO: Perhaps this should be a FollowCancelled event
				EventManager.Instance.Enqueue<FollowCompletedEventPayload>(
				    Events.FollowCompleted,
	                new FollowCompletedEventPayload(gameObject, PathToFollow)); 
			}	
		}
	}
	
	private void TraverseNextEdge()
    {
        if (PathToFollow == null)
        {
            return;
        }
		
        ////TODO: probably should add NextEdge method to Path class
        PathEdge edgeToFollow = PathToFollow.Dequeue();
		
		if (edgeToFollow == null)
		{
			return;
		}

        if (PathToFollow.IsEmpty) // last edge
        {
            edgeTraverser.Traverse(
                 edgeToFollow,
                 BrakeOnFinalApproach,
                 StopOnFinalArrival);
        }
        else
        {
            edgeTraverser.Traverse(
                 edgeToFollow,
                 BrakeOnEachApproach,
                 StopOnEachArrival);
        }
    }

    private void OnTraversalCompleted(Event<TraversalCompletedEventPayload> eventArg)
    {
		TraversalCompletedEventPayload payload = eventArg.EventData;
		
        if (payload.gameObject != gameObject) // event not for us
        {
            return;
        }
		
		if (payload.edge != null)
		{
			payload.edge.ShowEdge(false);
		}

        if (PathToFollow != null && PathToFollow.IsEmpty)
        {
            IsFollowing = false;
            EventManager.Instance.Enqueue<FollowCompletedEventPayload>(
			    Events.FollowCompleted,
                new FollowCompletedEventPayload(payload.gameObject, PathToFollow));
            return;
		}

        TraverseNextEdge();
    }

    private void OnTraversalFailed(Event<TraversalFailedEventPayload> eventArg)
    {
		TraversalFailedEventPayload payload = eventArg.EventData;
		
        if (payload.gameObject != gameObject) // event not for us
        {
            return;
        }
		
		if (payload.edge != null)
		{
			payload.edge.ShowEdge(false);
		}

        IsFollowing = false;
        EventManager.Instance.Enqueue<FollowFailedEventPayload>(
		    Events.FollowFailed,
            new FollowFailedEventPayload(payload.gameObject, PathToFollow));
    }

    private void OnPathReady(Event<PathReadyEventPayload> eventArg)
    {	
		PathReadyEventPayload payload = eventArg.EventData;
			
        if (payload.gameObject != gameObject) // event not for us
        {
            return;
        }

        Follow(payload.path);
    }
}
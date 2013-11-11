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

public struct TraversalCompletedEventPayload
{
    public GameObject gameObject;
    public PathEdge edge;

    public TraversalCompletedEventPayload(
        GameObject gameObject,
        PathEdge edge)
    {
        this.gameObject = gameObject;
        this.edge = edge;
    }
}

public struct TraversalFailedEventPayload
{
    public GameObject gameObject;
    public PathEdge edge;

    public TraversalFailedEventPayload(
        GameObject gameObject,
        PathEdge edge)
    {
        this.gameObject = gameObject;
        this.edge = edge;
    }
}

public sealed class EdgeTraverser : MonoBehaviour
{
	private Vector2 previousPosition;// = new Vector2(float.MaxValue, float.MaxValue);
	
	private Vector3 stuckDetector;
	
	private MovingEntity movingEntity;
	private AiController aiController;
	private Steering steering;
	private Seek seek;
	private Arrive arrive;
	
	/// <summary>
    /// Gets the edge to traverse.
    /// </summary>
    public PathEdge EdgeToFollow { get; private set; }

    public int TraversalMargin { get; set; }
	
	public void Awake()
	{
		TraversalMargin = 3;
		movingEntity = GetComponent<MovingEntity>();
		aiController = GetComponent<AiController>();
		seek = GetComponent<Seek>();
		arrive = GetComponent<Arrive>();
	}
	
	public void Update()
	{
		CheckIfStuck();
	}
	
	private void OnEnable()
	{
		EventManager.Instance.Subscribe<ArrivalEventPayload>(
			Events.Arrival, 
		    OnArrival);
	}
	
	private void OnDisable()
	{
		EventManager.Instance.Unsubscribe<ArrivalEventPayload>(
			Events.Arrival, 
		    OnArrival);
	}
	
	public bool Traverse(PathEdge edgeToFollow, bool brakeOnApproach, bool stopOnArrival)
    {	
		if ((movingEntity == null || !movingEntity.enabled) && 
		    (aiController == null || !aiController.enabled))
		{
			return false;
		}
		
		// Seek must exist and not be active
		if (seek == null || seek.TargetPosition.HasValue)
		{
			return false;
		}
		
		// Arrive must exist and not be active
		if (arrive == null || arrive.TargetPosition.HasValue)
		{
			return false;
		}
		
		seek.enabled = seek.isOn = false;
		arrive.enabled = arrive.isOn = false;

        EdgeToFollow = edgeToFollow;

        if (brakeOnApproach)
        {
			if (steering != null)
			{
				steering.enabled = steering.isOn = false;
			}
			
			steering = arrive;
            steering.TargetPosition = 
				(movingEntity != null && movingEntity.enabled) 
					? movingEntity.PositionAt(EdgeToFollow.Destination) 
					: World.Instance.GroundPositionAt(EdgeToFollow.Destination);
			steering.enabled = steering.isOn = true;
			if ((movingEntity == null || !movingEntity.enabled) && aiController != null && aiController.enabled)
			{
				aiController.SetSteering(steering);
			}
        }
        else
        {
			if (steering != null)
			{
				steering.enabled = steering.isOn = false;
			}
			
			steering = seek;
            steering.TargetPosition = 
				(movingEntity != null && movingEntity.enabled) 
					? movingEntity.PositionAt(EdgeToFollow.Destination) 
					: World.Instance.GroundPositionAt(EdgeToFollow.Destination);
			steering.enabled = steering.isOn = true;
			if ((movingEntity == null || !movingEntity.enabled) && aiController != null && aiController.enabled)
			{
				aiController.SetSteering(steering);
			}
        }

        return true;
    }

    private void OnArrival(Event<ArrivalEventPayload> eventArg)
    {
		ArrivalEventPayload payload = eventArg.EventData;
	
        if (payload.gameObject != gameObject) // event not for us
        {
            return;
        }

        if (EdgeToFollow != null && payload.destination == EdgeToFollow.Destination)
        {
            EventManager.Instance.Enqueue<TraversalCompletedEventPayload>(
                Events.TraversalCompleted,
                new TraversalCompletedEventPayload(payload.gameObject, EdgeToFollow));
        }
    }

    private bool IsStuck()
    {
        //// TODO: add stuck test based on difference between current and
        //// previous position (perhaps taking expected velocity and elapsed time into account)
        //return false; // for now, just return false
		bool xcond = false;
		bool zcond = false;
		
		if (Mathf.Abs(movingEntity.Position.x - stuckDetector.x) < 1) xcond = true;
		if (Mathf.Abs(movingEntity.Position.z - stuckDetector.z) < 1) zcond = true;
		
		stuckDetector = movingEntity.Position;
		
		if (xcond && zcond)
			return true;
		else
			return false;
    }

    private void CheckIfStuck()
    {
        if (IsStuck())
        {
			GameObject traverserGameObject/* = gameObject */; // depends if the EdgeTraverser is attached to the traversing object
			
			if (movingEntity != null && movingEntity.enabled)
			{
				traverserGameObject = movingEntity.gameObject;
			}
			else if (aiController != null && aiController.enabled)
			{
				traverserGameObject = aiController.gameObject;
			}
			else
			{
				traverserGameObject = gameObject;
			}
		 
            EventManager.Instance.Enqueue<TraversalFailedEventPayload>(
                Events.TraversalFailed,
                new TraversalFailedEventPayload(traverserGameObject, EdgeToFollow));
        }
		
		if (movingEntity != null && movingEntity.enabled)
		{
			previousPosition = movingEntity.Position2D;
		}
		else if (aiController != null && aiController.enabled)
		{
			previousPosition = aiController.transform.position.To2D();
		}
		else
		{
			previousPosition = transform.position.To2D();
		}
    }
}
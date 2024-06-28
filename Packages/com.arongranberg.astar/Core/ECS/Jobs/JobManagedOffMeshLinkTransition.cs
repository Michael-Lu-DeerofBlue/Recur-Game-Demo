#pragma warning disable 0282 // Allows the 'partial' keyword without warnings
#if MODULE_ENTITIES
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using Unity.Transforms;
using Unity.Collections.LowLevel.Unsafe;

namespace Pathfinding.ECS {
	using Pathfinding;

	public partial struct JobManagedOffMeshLinkTransition : IJobEntity {
		public EntityCommandBuffer commandBuffer;
		public float deltaTime;

		public void Execute (Entity entity, ManagedState state, ref LocalTransform transform, ref AgentMovementPlane movementPlane, ref MovementControl movementControl, ref MovementSettings movementSettings, ref AgentOffMeshLinkTraversal linkInfo, ManagedAgentOffMeshLinkTraversal managedLinkInfo) {
			if (!MoveNext(entity, state, ref transform, ref movementPlane, ref movementControl, ref movementSettings, ref linkInfo, managedLinkInfo, deltaTime)) {
				commandBuffer.RemoveComponent<AgentOffMeshLinkTraversal>(entity);
				commandBuffer.RemoveComponent<ManagedAgentOffMeshLinkTraversal>(entity);
			}
		}

		public static bool MoveNext (Entity entity, ManagedState state, ref LocalTransform transform, ref AgentMovementPlane movementPlane, ref MovementControl movementControl, ref MovementSettings movementSettings, ref AgentOffMeshLinkTraversal linkInfo, ManagedAgentOffMeshLinkTraversal managedLinkInfo, float deltaTime) {
			unsafe {
				managedLinkInfo.context.SetInternalData(entity, ref transform, ref movementPlane, ref movementControl, ref movementSettings, ref linkInfo, state, deltaTime);
			}

			// Initialize the coroutine during the first step.
			// This can also happen if the entity is duplicated, since the coroutine cannot be cloned.
			if (managedLinkInfo.coroutine == null) {
				// If we are calculating a path right now, cancel that path calculation.
				// We don't want to calculate a path while we are traversing an off-mesh link.
				state.CancelCurrentPathRequest();

				if (managedLinkInfo.stateMachine == null) {
					managedLinkInfo.stateMachine = managedLinkInfo.handler != null? managedLinkInfo.handler.GetOffMeshLinkStateMachine(managedLinkInfo.context) : null;
				}
				managedLinkInfo.coroutine = managedLinkInfo.stateMachine != null? managedLinkInfo.stateMachine.OnTraverseOffMeshLink(managedLinkInfo.context).GetEnumerator() : JobStartOffMeshLinkTransition.DefaultOnTraverseOffMeshLink(managedLinkInfo.context).GetEnumerator();
			}

			bool finished;
			bool error = false;
			bool popParts = true;
			try {
				finished = !managedLinkInfo.coroutine.MoveNext();
			} catch (AgentOffMeshLinkTraversalContext.AbortOffMeshLinkTraversal) {
				error = true;
				finished = true;
				popParts = false;
			}
			catch (System.Exception e) {
				Debug.LogException(e, managedLinkInfo.context.gameObject);
				// Teleport the agent to the end of the link as a fallback, if there's an exception
				managedLinkInfo.context.Teleport(managedLinkInfo.context.link.relativeEnd);
				finished = true;
				error = true;
			}

			if (finished) {
				try {
					if (managedLinkInfo.stateMachine != null) {
						if (error) managedLinkInfo.stateMachine.OnAbortTraversingOffMeshLink();
						else managedLinkInfo.stateMachine.OnFinishTraversingOffMeshLink(managedLinkInfo.context);
					}
				} catch (System.Exception e) {
					// If an exception happens when exiting the state machine, log it, and then continue with the cleanup
					Debug.LogException(e, managedLinkInfo.context.gameObject);
				}

				managedLinkInfo.context.Restore();
				if (popParts) {
					// Pop the part leading up to the link, and the link itself
					state.PopNextLinkFromPath();
				}
			}
			return !finished;
		}
	}

	public partial struct JobManagedOffMeshLinkTransitionCleanup : IJobEntity {
		public void Execute (ManagedAgentOffMeshLinkTraversal managedLinkInfo) {
			// The state machine may be null if the default off-mesh link logic is used, or if the entity is destroyed on the first frame
			// that it starts to traverse an off-mesh link.
			if (managedLinkInfo.stateMachine != null) managedLinkInfo.stateMachine.OnAbortTraversingOffMeshLink();
		}
	}
}
#endif

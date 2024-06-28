#pragma warning disable CS0282
#if MODULE_ENTITIES
using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;

namespace Pathfinding.ECS {
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using Pathfinding;
	using Pathfinding.Drawing;
	using Pathfinding.Util;
	using Unity.Transforms;
	using UnityEngine.Profiling;

	[UpdateBefore(typeof(FollowerControlSystem))]
	[UpdateInGroup(typeof(AIMovementSystemGroup))]
	[RequireMatchingQueriesForUpdate]
	[BurstCompile]
	public partial struct MovementPlaneFromGraphSystem : ISystem {
		public EntityQuery entityQueryGraph;
		public EntityQuery entityQueryNormal;
		NativeArray<float3> sphereSamplePoints;
		// Store the queue in a GCHandle to avoid restrictions on ISystem
		GCHandle graphNodeQueue;

		public void OnCreate (ref SystemState state) {
			entityQueryGraph = state.GetEntityQuery(ComponentType.ReadOnly<MovementState>(), ComponentType.ReadWrite<AgentMovementPlane>(), ComponentType.ReadOnly<AgentMovementPlaneSource>());
			entityQueryGraph.SetSharedComponentFilter(new AgentMovementPlaneSource { value = MovementPlaneSource.Graph });
			entityQueryNormal = state.GetEntityQuery(
				ComponentType.ReadWrite<ManagedState>(),
				ComponentType.ReadOnly<LocalTransform>(),
				ComponentType.ReadWrite<AgentMovementPlane>(),
				ComponentType.ReadOnly<AgentCylinderShape>(),
				ComponentType.ReadOnly<AgentMovementPlaneSource>()
				);
			entityQueryNormal.AddSharedComponentFilter(new AgentMovementPlaneSource { value = MovementPlaneSource.NavmeshNormal });

			// Number of samples to use when approximating the normal, when using the NavmeshNormal mode.
			const int Samples = 16;
			sphereSamplePoints = new NativeArray<float3>(Samples, Allocator.Persistent);
			var randomState = UnityEngine.Random.state;
			UnityEngine.Random.InitState(0);
			for (int i = 0; i < Samples; i++) {
				sphereSamplePoints[i] = (float3)UnityEngine.Random.insideUnitSphere;
			}
			UnityEngine.Random.state = randomState;

			graphNodeQueue = GCHandle.Alloc(new List<GraphNode>(32));
		}

		public void OnDestroy (ref SystemState state) {
			sphereSamplePoints.Dispose();
			graphNodeQueue.Free();
		}

		public void OnUpdate (ref SystemState systemState) {
			var graphs = AstarPath.active?.data.graphs;
			if (graphs == null) return;

			var movementPlanes = CollectionHelper.CreateNativeArray<AgentMovementPlane>(graphs.Length, systemState.WorldUpdateAllocator, NativeArrayOptions.UninitializedMemory);
			for (int i = 0; i < graphs.Length; i++) {
				var graph = graphs[i];
				var plane = new NativeMovementPlane(quaternion.identity);
				if (graph is NavmeshBase navmesh) {
					plane = new NativeMovementPlane(navmesh.transform.rotation);
				} else if (graph is GridGraph grid) {
					plane = new NativeMovementPlane(grid.transform.rotation);
				}
				movementPlanes[i] = new AgentMovementPlane {
					value = plane,
				};
			}

			if (!entityQueryNormal.IsEmpty) {
				systemState.CompleteDependency();
				var vertices = new NativeList<float3>(16, Allocator.Temp);
				new JobMovementPlaneFromNavmeshNormal {
					dt = AIMovementSystemGroup.TimeScaledRateManager.CheapStepDeltaTime,
					sphereSamplePoints = sphereSamplePoints,
					vertices = vertices,
					que = (List<GraphNode>)graphNodeQueue.Target,
				}.Run(entityQueryNormal);
			}

			systemState.Dependency = new JobMovementPlaneFromGraph {
				movementPlanes = movementPlanes,
			}.Schedule(entityQueryGraph, systemState.Dependency);
		}

		partial struct JobMovementPlaneFromNavmeshNormal : IJobEntity {
			public float dt;
			[ReadOnly]
			public NativeArray<float3> sphereSamplePoints;
			public NativeList<float3> vertices;
			public List<GraphNode> que;

			public void Execute (ManagedState managedState, in LocalTransform localTransform, ref AgentMovementPlane agentMovementPlane, in AgentCylinderShape shape) {
				var sphereSamplePointsSpan = sphereSamplePoints.AsUnsafeSpan();
				var node = managedState.pathTracer.startNode;
				// TODO: Expose these parameters?
				float size = shape.radius * 1.5f;
				const float InverseSmoothness = 20f;
				if (node != null) {
					vertices.Clear();
					que.Clear();
					var position = localTransform.Position;
					var bounds = new UnityEngine.Bounds(position, new float3(size, size, size));
					int queStart = 0;
					node.TemporaryFlag1 = true;
					que.Add(node);

					while (queStart < que.Count) {
						var current = que[queStart++] as TriangleMeshNode;

						current.GetVertices(out var v0, out var v1, out var v2);
						var p0 = (float3)v0;
						var p1 = (float3)v1;
						var p2 = (float3)v2;
						Polygon.ClosestPointOnTriangleByRef(in p0, in p1, in p2, in position, out var closest);
						if (math.lengthsq(closest - position) < size*size) {
							vertices.Add(p0);
							vertices.Add(p1);
							vertices.Add(p2);
							current.GetConnections((GraphNode con, ref List<GraphNode> que) => {
								if (!con.TemporaryFlag1) {
									con.TemporaryFlag1 = true;
									que.Add(con);
								}
							}, ref que);
						}
					}

					// Reset temporary flags
					for (int i = 0; i < que.Count; i++) {
						que[i].TemporaryFlag1 = false;
					}

					var verticesSpan = vertices.AsUnsafeSpan();
					SampleTriangleNormals(ref sphereSamplePointsSpan, ref position, size, ref verticesSpan, ref agentMovementPlane, dt * InverseSmoothness);
				}
			}
		}

		[BurstCompile]
		partial struct JobMovementPlaneFromGraph : IJobEntity {
			[ReadOnly]
			public NativeArray<AgentMovementPlane> movementPlanes;

			public void Execute (in MovementState movementState, ref AgentMovementPlane movementPlane) {
				if (movementState.graphIndex < (uint)movementPlanes.Length) {
					movementPlane = movementPlanes[(int)movementState.graphIndex];
				} else {
					// This can happen if the agent has no path, or if the path is stale.
					// Potentially also if a graph has been removed.
				}
			}
		}

		[BurstCompile(FloatMode = FloatMode.Fast)]
		static void SampleTriangleNormals (ref UnsafeSpan<float3> samplePoints, ref float3 sampleOrigin, float sampleScale, ref UnsafeSpan<float3> triangleVertices, ref AgentMovementPlane agentMovementPlane, float alpha) {
			var targetNormal = float3.zero;
			int normalWeight = 0;
			for (int i = 0; i < triangleVertices.Length; i += 3) {
				var p0 = triangleVertices[i + 0];
				var p1 = triangleVertices[i + 1];
				var p2 = triangleVertices[i + 2];
				var triangleNormal = math.normalizesafe(math.cross(p1 - p0, p2 - p0));

				for (int j = 0; j < samplePoints.Length; j++) {
					var p = samplePoints[j] * sampleScale + sampleOrigin;
					if (Polygon.ClosestPointOnTriangleByRef(in p0, in p1, in p2, in p, out var closest) && math.lengthsq(closest - sampleOrigin) < sampleScale*sampleScale) {
						targetNormal += triangleNormal;
						normalWeight++;
					}
				}
			}

			if (normalWeight > 0) {
				targetNormal = math.normalizesafe(targetNormal / normalWeight);

				var currentNormal = agentMovementPlane.value.up;
				var nextNormal = math.lerp(currentNormal, targetNormal, math.clamp(0, 1, alpha));
				JobApplyGravity.UpdateMovementPlaneFromNormal(nextNormal, ref agentMovementPlane);
			}
		}
	}
}
#endif

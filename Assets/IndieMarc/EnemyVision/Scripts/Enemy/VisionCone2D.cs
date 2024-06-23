using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.EnemyVision
{
    /// <summary>
    /// Generates and updates the vision cone displayed
    /// </summary>
    
    public class VisionCone2D : MonoBehaviour
    {
        [Header("Linked Enemy")]
        public EnemyVision2D target;

        [Header("Vision")]
        public float vision_angle = 30f;
        public float vision_range = 5f;
        public float vision_near_range = 3f;
        public LayerMask obstacle_mask = ~(0);
        public bool show_two_levels = false;

        [Header("Material")]
        public Material cone_material;
        public Material cone_far_material;
        public int sort_order = 1;

        [Header("Optimization")]
        public int precision = 60;
        public float refresh_rate = 0f;

        private MeshRenderer render;
        private MeshFilter mesh;
        private MeshRenderer render_far;
        private MeshFilter mesh_far;
        private float timer = 0f;

        private void Awake()
        {
            render = gameObject.AddComponent<MeshRenderer>();
            mesh = gameObject.AddComponent<MeshFilter>();
            render.sharedMaterial = cone_material;
            render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            render.receiveShadows = false;
            render.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            render.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            render.allowOcclusionWhenDynamic = false;
            render.sortingOrder = sort_order;

            if (show_two_levels)
            {
                GameObject far_cone = new GameObject("FarCone");
                far_cone.transform.position = transform.position;
                far_cone.transform.SetParent(gameObject.transform);

                render_far = far_cone.AddComponent<MeshRenderer>();
                mesh_far = far_cone.AddComponent<MeshFilter>();
                render_far.sharedMaterial = cone_far_material;
                render_far.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                render_far.receiveShadows = false;
                render_far.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                render_far.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                render_far.allowOcclusionWhenDynamic = false;
                render_far.sortingOrder = sort_order;
            }
        }

        private void Start()
        {
            InitMesh(mesh, false);

            if (show_two_levels)
                InitMesh(mesh_far, true);
        }

        private void InitMesh(MeshFilter mesh, bool far)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();

            if (!far)
            {
                vertices.Add(new Vector3(0f, 0f, 0f));
                normals.Add(Vector3.up);
                uv.Add(Vector2.zero);
            }

            int minmax = Mathf.RoundToInt(vision_angle / 2f);

            //Create vision shape
            int tri_index = 0;
            float step_jump = Mathf.Clamp(vision_angle / precision, 0.01f, minmax);

            for (float i = -minmax; i <= minmax; i += step_jump)
            {
                float angle = (float)(i) * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(angle) * vision_range, Mathf.Sin(angle) * vision_range, 0f);

                vertices.Add(dir);
                normals.Add(Vector2.up);
                uv.Add(Vector2.zero);

                if (far)
                {
                    vertices.Add(dir);
                    normals.Add(Vector2.up);
                    uv.Add(Vector2.zero);
                }

                if (tri_index > 0)
                {
                    if (far)
                    {
                        triangles.Add(tri_index);
                        triangles.Add(tri_index + 1);
                        triangles.Add(tri_index - 2);

                        triangles.Add(tri_index - 2);
                        triangles.Add(tri_index + 1);
                        triangles.Add(tri_index - 1);
                    }
                    else
                    {
                        triangles.Add(0);
                        triangles.Add(tri_index + 1);
                        triangles.Add(tri_index);
                    }
                }
                tri_index += far ? 2 : 1;
            }

            mesh.mesh.vertices = vertices.ToArray();
            mesh.mesh.triangles = triangles.ToArray();
            mesh.mesh.normals = normals.ToArray();
            mesh.mesh.uv = uv.ToArray();
        }

        private void Update()
        {
            timer += Time.deltaTime;

            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            //Update vision transform
            transform.position = target.eye.transform.position;
            transform.rotation = Quaternion.AngleAxis(target.GetFaceAngle(), Vector3.forward * target.GetSide()) * target.transform.rotation;
            transform.localScale = new Vector3(1f, 1f, 1f);

            //Update vision shape
            if (timer > refresh_rate)
            {
                timer = 0f;

                float range = vision_range;
                if (show_two_levels)
                    range = vision_near_range;

                UpdateMainLevel(mesh, range);

                if (show_two_levels)
                    UpdateFarLevel(mesh_far, vision_near_range, vision_range - vision_near_range);
            }
        }

        private void UpdateMainLevel(MeshFilter mesh, float range)
        {
            List<Vector3> vertices = new List<Vector3>();
            vertices.Add(new Vector3(0f, 0f, 0f));

            int minmax = Mathf.RoundToInt(vision_angle / 2f);
            float step_jump = Mathf.Clamp(vision_angle / precision, 0.01f, minmax);
            for (float i = -minmax; i <= minmax; i += step_jump)
            {
                float angle = (float)(i) * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(angle) * range, Mathf.Sin(angle) * range, 0f);

                Vector3 pos_world = transform.TransformPoint(Vector3.zero);
                Vector3 dir_world = transform.TransformDirection(dir.normalized);
                RaycastHit2D hit = Physics2D.Raycast(pos_world, dir_world, range, obstacle_mask.value);
                if (hit.collider)
                    dir = dir.normalized * hit.distance;
                Debug.DrawRay(pos_world, dir_world * (hit.collider ? hit.distance : range));

                vertices.Add(dir);
            }

            mesh.mesh.vertices = vertices.ToArray();
            mesh.mesh.RecalculateBounds();
        }

        private void UpdateFarLevel(MeshFilter mesh, float offset, float range)
        {
            List<Vector3> vertices = new List<Vector3>();

            int minmax = Mathf.RoundToInt(vision_angle / 2f);
            float step_jump = Mathf.Clamp(vision_angle / precision, 0.01f, minmax);
            for (float i = -minmax; i <= minmax; i += step_jump)
            {
                float angle = (float)(i) * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(angle) * offset, Mathf.Sin(angle) * offset, 0f);

                Vector3 pos_world = transform.TransformPoint(Vector3.zero);
                Vector3 dir_world = transform.TransformDirection(dir.normalized);
                RaycastHit2D hit = Physics2D.Raycast(pos_world, dir_world, range + offset, obstacle_mask.value);

                float tot_dist = hit.collider != null ? hit.distance : range + offset;
                Vector3 dir1 = dir.normalized * offset;
                Vector3 dir2 = dir.normalized * Mathf.Max(tot_dist, offset);

                if (hit.collider != null && tot_dist < offset)
                    dir2 = dir1;

                Debug.DrawRay(pos_world + dir_world * offset, dir_world * Mathf.Max(tot_dist - offset, 0f), Color.blue);

                vertices.Add(dir1);
                vertices.Add(dir2);
            }

            mesh.mesh.vertices = vertices.ToArray();
            mesh.mesh.RecalculateBounds();
        }
    }
}

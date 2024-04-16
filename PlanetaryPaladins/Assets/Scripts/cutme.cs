using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.XR;

public class cutme : MonoBehaviour
{
    /*
    class for cutting objects
    mostly adapted from https://github.com/JustinCragg/MeshSlicing and https://github.com/kurtdekker/makegeo
    add "Sliceable" component to things that can be cut
     */
    public Material tst;
    private Transform blade;
    private Transform handle;

    private Vector3 lastVelocity;
    private Vector3 lastPosition;
    private bool isExtended;
    private Vector3 extendedScale;
    private Vector3 extendedPos;
    private Collider bladeCollider;
    private Vector3 tipVelocity;
    private AudioSource audioSource;
    private AudioSource humSource;
    private AudioClip[] swingClips;
    private AudioClip[] parryClips;
    private AudioClip extendClip;
    private AudioClip retractClip;

    // Start is called before the first frame update
    void Start()
    {
        blade = transform.Find("Blade");
        handle = transform.Find("Handle");
        bladeCollider = blade.gameObject.GetComponent<Collider>();
        extendedScale = blade.localScale;
        extendedPos = blade.localPosition;
        lastPosition = GetComponent<Rigidbody>().position;
        audioSource = GetComponent<AudioSource>();
        humSource = handle.GetComponent<AudioSource>();
        swingClips = Resources.LoadAll<AudioClip>("Audio/swings");
        parryClips = Resources.LoadAll<AudioClip>("Audio/parrys");
        extendClip = Resources.Load<AudioClip>("Audio/saber_startup");
        retractClip = Resources.Load<AudioClip>("Audio/saber_off");
        Debug.Log("size: " + swingClips.Length);
        isExtended = true;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.anyKey)
        // {
        //     cut(GetComponent<Collider>());
        // }


        //plane
        //fixme y is just controller forward, x needs to be vector showing movement
        // Vector3 planeXVec = new Vector3(0, 0, 0) * this.GetComponent<Collider>().bounds.size.x;
        // Vector3 planeYVec = new Vector3(0, 1, 1) * this.GetComponent<Collider>().bounds.size.y * 2;
        // DrawPlane(planeXVec, planeYVec);

        // Vector3 x = new Vector3(0.02f, 0.03f, 1.46f);
        // Vector3 y = new Vector3(-0.02f, -0.03f, -1.46f);
        // Vector3 z = gameObject.GetComponentInParent<Rigidbody>().velocity;
        // Debug.DrawLine(x, y, Color.red);
        // Debug.DrawLine(y, z, Color.blue);
        // Debug.DrawLine(x, z, Color.green);

    }

    private void FixedUpdate()
    {
        lastVelocity = GetComponent<Rigidbody>().velocity;
        Vector3 calcVelocity = (GetComponent<Rigidbody>().position - lastPosition) * Time.deltaTime;
        Debug.Log(calcVelocity.magnitude);
        lastPosition = GetComponent<Rigidbody>().position;
        if (calcVelocity.magnitude > 1e-5f && !audioSource.isPlaying && isExtended)
        {
            audioSource.clip = swingClips[UnityEngine.Random.Range(0, 4)];
            audioSource.Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        /*if (collision.gameObject.transform.parent == null && collision.gameObject.tag == "Sliceable") //not null
        {
            cut(collision);
        }*/
        if (isExtended)
        {
            if (collision.gameObject.GetComponent<Sliceable>() && isExtended) //held not null
            {
                cut(collision);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isExtended)
        {
            if (other.gameObject.GetComponent<Projectile>() && other.tag == "EnemyProj")
            {
                //deflect(other)
                other.tag = "AllyProj";
                Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
                Vector3 newForceDirection = -rb.velocity.normalized * (rb.velocity.magnitude);
                rb.velocity = Vector3.zero;
                rb.AddForce(newForceDirection, ForceMode.VelocityChange);

                audioSource.clip = parryClips[UnityEngine.Random.Range(0, 2)];
                audioSource.Play();
            }
        }
    }

    public void toggleBlade()
    {
        if (isExtended)
        {
            StartCoroutine(retract());
        }
        else
        {
            StartCoroutine(extend());
        }
        isExtended = !isExtended;
    }

    IEnumerator extend()
    {
        humSource.mute = false;
        audioSource.clip = extendClip;
        audioSource.Play();
        Vector3 startScale = blade.localScale;
        Vector3 targetScale = extendedScale;
        Vector3 startPos = blade.localPosition;
        Vector3 targetPos = extendedPos;

        float elapsedTime = 0.0f;
        while (elapsedTime < 0.5f)
        {
            blade.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime * 2);
            blade.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime * 2);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        blade.localScale = targetScale;
        blade.localPosition = targetPos;
    }

    IEnumerator retract()
    {
        audioSource.clip = retractClip;
        audioSource.Play();
        humSource.mute = true;
        Vector3 startScale = blade.localScale;
        Vector3 targetScale = new Vector3(extendedScale.x, 0, extendedScale.z);
        Vector3 startPos = blade.localPosition;
        Vector3 targetPos = new Vector3(extendedPos.x, handle.localPosition.y, extendedPos.z);

        float elapsedTime = 0.0f;
        while (elapsedTime < 1.5f)
        {
            blade.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime * 2 / 1.5f);
            blade.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime * 2 / 1.5f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        blade.localScale = targetScale;
        blade.localPosition = targetPos;
    }
    void deflect(Collider other)
    {
        //make plane

        /* other.tag = "AllyProj";
         bladeCollider.gameObject.GetComponent<Rigidbody>();

         Vector3 p1 = bladeCollider.bounds.center + bladeCollider.bounds.max.y * -transform.up;
         Vector3 p2 = bladeCollider.bounds.center + bladeCollider.bounds.max.y * transform.up;
         Vector3 p3 = bladeCollider.bounds.center + lastVelocity * 1000;

         Plane plane = new Plane(p1, p2, p3);

         //deflect it xd
         Vector3 direction = other.gameObject.GetComponent<Rigidbody>().velocity;
         Vector3 reflected = Vector3.Reflect(direction, plane.normal);

         other.gameObject.GetComponent<Rigidbody>().AddForce(reflected * -1, ForceMode.Impulse);
         other.gameObject.transform.LookAt(reflected);

         //haptic stuff*/


    }


    void cut(Collision col)
    {
        Collider other = col.collider;
        Mesh m = other.gameObject.GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = m.vertices;
        int[] triangles = m.triangles;
        Vector2[] uvs = m.uv;
        //Debug.Log(uvs);

        //make plane
        bladeCollider.gameObject.GetComponent<Rigidbody>();

        Vector3 p1 = bladeCollider.bounds.center + bladeCollider.bounds.max.y * -transform.up * 2;
        Vector3 p2 = bladeCollider.bounds.center + bladeCollider.bounds.max.y * transform.up * 2;
        //Vector3 p3 = gameObject.GetComponent<Rigidbody>().velocity * 10;
        Vector3 p3 = (lastVelocity.magnitude > 0.5f) ? bladeCollider.bounds.center + lastVelocity * 100 : bladeCollider.bounds.center + lastVelocity * 50 + col.relativeVelocity * -1;
        Debug.Log("last " + lastVelocity);
        Debug.Log("relvel " + col.relativeVelocity);
        Plane cutPlane = new Plane(p1, p2, p3);

        List<Vector3> intersections = new List<Vector3>();
        List<Vector3> triangles1 = new List<Vector3>(); //these hold the new vertices
        List<Vector3> triangles2 = new List<Vector3>();
        List<Vector2> newUVs1 = new List<Vector2>();
        List<Vector2> newUVs2 = new List<Vector2>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // indices for vertex array same as opengl
            int t1 = triangles[i];
            int t2 = triangles[i + 1];
            int t3 = triangles[i + 2];
            Vector3 v1 = other.transform.TransformPoint(vertices[t1]);
            Vector3 v2 = other.transform.TransformPoint(vertices[t2]);
            Vector3 v3 = other.transform.TransformPoint(vertices[t3]);
            Vector2 uv1 = uvs[t1];
            Vector2 uv2 = uvs[t2];
            Vector2 uv3 = uvs[t3];
            //Debug.Log(uv1 + " " + uv2 + " " + uv3);

            //determine if plane intersects triangle, then which side of plane each point is on
            List<Vector3> curIntersections = new List<Vector3>();
            //ray for each side, checking if went through plane
            float enter;
            if (cutPlane.Raycast(new Ray(v1, v2 - v1), out enter) && enter < Vector3.Distance(v1, v2))
            {
                Vector3 point = v1 + Vector3.Normalize(v2 - v1) * enter;
                intersections.Add(point);
                curIntersections.Add(point);
                //Debug.Log("yipiy");
            }
            if (cutPlane.Raycast(new Ray(v2, v3 - v2), out enter) && enter < Vector3.Distance(v2, v3))
            {
                Vector3 point = v2 + Vector3.Normalize(v3 - v2) * enter;
                intersections.Add(point);
                curIntersections.Add(point);
                //Debug.Log("yay");
            }
            if (cutPlane.Raycast(new Ray(v3, v1 - v3), out enter) && enter < Vector3.Distance(v3, v1))
            {
                Vector3 point = v3 + Vector3.Normalize(v1 - v3) * enter;
                intersections.Add(point);
                curIntersections.Add(point);
                //Debug.Log("yaho");
            }

            List<Vector3> verts1 = new List<Vector3>(curIntersections);
            List<Vector3> verts2 = new List<Vector3>(curIntersections);
            if (curIntersections.Count == 2)
            {
                // triangle splits into triangle and trapezoid
                // add intersection points to v1 v2 lists
                // determine which side of plane its on, and 3 or 4 point
                // 3 point becomes new triangle, added in correct order
                if (cutPlane.GetSide(v1)) { verts1.Add(v1); } else { verts2.Add(v1); }
                if (cutPlane.GetSide(v2)) { verts1.Add(v2); } else { verts2.Add(v2); }
                if (cutPlane.GetSide(v3)) { verts1.Add(v3); } else { verts2.Add(v3); }

                if (verts1.Count == 4)
                {
                    //trapezoid, split down the second intersect vertex
                    triangles1.Add(verts1[0]);
                    triangles1.Add(verts1[1]);
                    triangles1.Add(verts1[3]);

                    newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2());

                    triangles1.Add(verts1[0]);
                    triangles1.Add(verts1[3]);
                    triangles1.Add(verts1[2]);

                    newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2());

                    //other
                    triangles1.Add(verts1[1]); //figure out how to tell which side is right instead of adding both
                    triangles1.Add(verts1[0]);
                    triangles1.Add(verts1[2]);

                    newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2());

                    triangles1.Add(verts1[1]);
                    triangles1.Add(verts1[2]);
                    triangles1.Add(verts1[3]);

                    newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2());
                }
                else //if (verts1.Count == 3)
                {
                    triangles1.Add(verts1[1]);
                    triangles1.Add(verts1[2]);
                    triangles1.Add(verts1[0]);

                    newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2());

                    //other
                    triangles1.Add(verts1[0]);
                    triangles1.Add(verts1[2]);
                    triangles1.Add(verts1[1]);

                    newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2());
                }
                if (verts2.Count == 4)
                {
                    triangles2.Add(verts2[1]);
                    triangles2.Add(verts2[0]);
                    triangles2.Add(verts2[2]);

                    newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2());

                    triangles2.Add(verts2[1]);
                    triangles2.Add(verts2[2]);
                    triangles2.Add(verts2[3]);

                    newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2());

                    //other
                    triangles2.Add(verts2[0]);
                    triangles2.Add(verts2[1]);
                    triangles2.Add(verts2[3]);

                    newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2());

                    triangles2.Add(verts2[0]);
                    triangles2.Add(verts2[3]);
                    triangles2.Add(verts2[2]);

                    newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2());
                }
                else
                {
                    triangles2.Add(verts2[0]);
                    triangles2.Add(verts2[2]);
                    triangles2.Add(verts2[1]);

                    newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2());

                    //other
                    triangles2.Add(verts2[1]);
                    triangles2.Add(verts2[2]);
                    triangles2.Add(verts2[0]);

                    newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2());
                }


            }
            else
            {
                if (cutPlane.GetSide(v1))
                {
                    triangles1.Add(v1);
                    triangles1.Add(v2);
                    triangles1.Add(v3);

                    newUVs1.Add(uv1);
                    newUVs1.Add(uv2);
                    newUVs1.Add(uv3);
                }
                else
                {
                    triangles2.Add(v1);
                    triangles2.Add(v2);
                    triangles2.Add(v3);

                    newUVs2.Add(uv1);
                    newUVs2.Add(uv2);
                    newUVs2.Add(uv3);
                }
            }
        }
        //make the cut face, like a pizza
        int N = intersections.Count;
        if (N > 0)
        {
            Vector3 center = Vector3.zero;
            for (int i = 0; i < N; i++)
            {
                center += intersections[i];
            }
            center /= N;
            for (int i = 0; i < intersections.Count; i++)
            {

                if (i < N - 1)
                {
                    //get normal of triangle, match with normal of plane
                    if (Vector3.Dot(cutPlane.normal, Vector3.Cross(center - intersections[i], center - intersections[i + 1])) > 0) //add this to same case above
                    {
                        triangles1.Add(intersections[i]);
                        triangles1.Add(center);
                        triangles1.Add(intersections[i + 1]);
                        newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2());
                        triangles2.Add(intersections[i + 1]);
                        triangles2.Add(center);
                        triangles2.Add(intersections[i]);
                        newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2());
                    }
                    else
                    {
                        triangles1.Add(intersections[i + 1]);
                        triangles1.Add(center);
                        triangles1.Add(intersections[i]);
                        newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2());
                        triangles2.Add(intersections[i]);
                        triangles2.Add(center);
                        triangles2.Add(intersections[i + 1]);
                        newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2());
                    }
                }
                else
                {
                    if (Vector3.Dot(cutPlane.normal, Vector3.Cross(center - intersections[i], center - intersections[0])) > 0)
                    {
                        triangles1.Add(intersections[i]);
                        triangles1.Add(center);
                        triangles1.Add(intersections[0]);
                        newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2());
                        triangles2.Add(intersections[0]);
                        triangles2.Add(center);
                        triangles2.Add(intersections[i]);
                        newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2());
                    }
                    else
                    {
                        triangles1.Add(intersections[0]);
                        triangles1.Add(center);
                        triangles1.Add(intersections[i]);
                        newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2()); newUVs1.Add(new Vector2());
                        triangles2.Add(intersections[i]);
                        triangles2.Add(center);
                        triangles2.Add(intersections[0]);
                        newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2()); newUVs2.Add(new Vector2());
                    }
                }
            }


            //make the 2 new mesh

            int[] tris1 = Enumerable.Range(0, triangles1.Count - (intersections.Count * 3)).ToArray();
            int[] tris1m = Enumerable.Range(triangles1.Count - (intersections.Count * 3), intersections.Count * 3).ToArray();
            int[] tris2 = Enumerable.Range(0, triangles2.Count - (intersections.Count * 3)).ToArray();
            int[] tris2m = Enumerable.Range(triangles2.Count - (intersections.Count * 3), intersections.Count * 3).ToArray();

            Material[] mats = other.gameObject.GetComponent<MeshRenderer>().materials;
            Mesh m1 = new Mesh();
            m1.subMeshCount = m.subMeshCount + 1;
            m1.vertices = triangles1.ToArray();
            //Debug.Log("sizes: " + triangles1.Count + " " + newUVs1.Count);
            m1.uv = newUVs1.ToArray();
            m1.SetTriangles(tris1, 0);
            //m1.triangles = tris1;
            m1.SetTriangles(tris1m, m1.subMeshCount - 1);
            GameObject g1 = new GameObject();
            MeshRenderer mr1 = g1.AddComponent<MeshRenderer>();
            mats = mats.Concat(new Material[] { tst }).ToArray();
            mr1.materials = mats;
            MeshFilter mf1 = g1.AddComponent<MeshFilter>();
            mf1.mesh = m1;
            MeshCollider mc1 = g1.AddComponent<MeshCollider>();
            mc1.convex = true;
            mc1.AddComponent<Rigidbody>().AddForce(cutPlane.normal * 150.1f);
            g1.tag = "Forceable";
            //g1.AddComponent<Sliceable>();

            Mesh m2 = new Mesh();
            m2.subMeshCount = m.subMeshCount + 1;
            m2.vertices = triangles2.ToArray();
            //Debug.Log("sizes: " + triangles2.Count + " " + newUVs2.Count);

            m2.uv = newUVs2.ToArray();
            m2.SetTriangles(tris2, 0);
            //m2.triangles = tris2;
            m2.SetTriangles(tris2m, m2.subMeshCount - 1);
            GameObject g2 = new GameObject();
            MeshRenderer mr2 = g2.AddComponent<MeshRenderer>();
            mr2.materials = mats;
            MeshFilter mf2 = g2.AddComponent<MeshFilter>();
            mf2.mesh = m2;
            MeshCollider mc2 = g2.AddComponent<MeshCollider>();
            mc2.convex = true;
            mc2.AddComponent<Rigidbody>().AddForce(cutPlane.normal * -150.1f);
            g2.tag = "Forceable";
            //g2.AddComponent<Sliceable>();

            m1.RecalculateNormals();
            m1.RecalculateBounds();
            m2.RecalculateNormals();
            m2.RecalculateBounds();

            Destroy(other.gameObject);
            StartCoroutine(waitBeforeAssign(0.5f, g1, g2));
        }
    }
    IEnumerator waitBeforeAssign(float time, GameObject ob1, GameObject ob2)
    {
        yield return new WaitForSeconds(time);
        ob1.AddComponent<Sliceable>();
        ob2.AddComponent<Sliceable>();
    }
}

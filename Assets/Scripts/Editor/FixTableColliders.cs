using UnityEngine;
using UnityEditor;

public class FixTableColliders
{
    [MenuItem("Tools/Fix All Scene Colliders")]
    static void Fix()
    {
        // Remove Rigidbody from any static mesh object that has a concave MeshCollider
        MeshCollider[] allColliders = Object.FindObjectsByType<MeshCollider>(FindObjectsSortMode.None);
        int removedRigidbodies = 0;
        int enabledColliders = 0;

        foreach (MeshCollider mc in allColliders)
        {
            // Enable any disabled MeshCollider on non-dynamic objects
            if (!mc.enabled && mc.GetComponent<Rigidbody>() == null)
            {
                mc.enabled = true;
                enabledColliders++;
            }

            if (mc.convex) continue;
            Rigidbody rb = mc.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Object.DestroyImmediate(rb);
                removedRigidbodies++;
            }
        }

        // Also enable any other disabled Collider types (BoxCollider, etc.) on static objects
        Collider[] allCollidersGeneric = Object.FindObjectsByType<Collider>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Collider col in allCollidersGeneric)
        {
            if (!col.enabled && col.GetComponent<Rigidbody>() == null)
            {
                col.enabled = true;
                enabledColliders++;
            }
        }

        // Add MeshColliders to any MeshFilter that still has no collider (Table_5, Cube.003, etc.)
        MeshFilter[] allFilters = Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int addedColliders = 0;
        foreach (MeshFilter mf in allFilters)
        {
            // Check for any collider component even if disabled
            Collider existing = mf.gameObject.GetComponent<Collider>();
            if (existing != null)
            {
                // Make sure it's enabled
                if (!existing.enabled)
                {
                    existing.enabled = true;
                    enabledColliders++;
                }
                continue;
            }
            if (mf.gameObject.GetComponent<Rigidbody>() != null) continue; // skip dynamic objects
            if (mf.sharedMesh == null) continue;

            MeshCollider mc = mf.gameObject.AddComponent<MeshCollider>();
            mc.sharedMesh = mf.sharedMesh;
            addedColliders++;
        }

        // Specifically ensure table_5 has an active collider
        GameObject table5 = GameObject.Find("table_5");
        if (table5 != null)
        {
            Collider col = table5.GetComponent<Collider>();
            if (col == null)
            {
                MeshFilter mf = table5.GetComponent<MeshFilter>();
                if (mf != null && mf.sharedMesh != null)
                {
                    MeshCollider mc = table5.AddComponent<MeshCollider>();
                    mc.sharedMesh = mf.sharedMesh;
                    addedColliders++;
                }
            }
            else if (!col.enabled)
            {
                col.enabled = true;
                enabledColliders++;
            }
            Debug.Log("table_5 collider fixed.");
        }
        else
        {
            Debug.LogWarning("table_5 not found in scene. Make sure it is loaded.");
        }

        Debug.Log($"Done: {removedRigidbodies} Rigidbodies removed, {enabledColliders} colliders enabled, {addedColliders} colliders added.");
    }
}

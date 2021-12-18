using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Custom/Quest")]
public class Quest : ScriptableObject
{
    public Color outfit;
    public Vector3 size;

    // null is the criterion passing
    public System.Tuple<string, System.Func<Customer, string>> criterion { get { return new System.Tuple<string, System.Func<Customer, string>>(requests[id], criteria[id]); } }

    public int id { get { return int.Parse(name); } }

    static string[] requests = {
        "WASD to move and E to interact. Right mouse button to change the view; left to take a picture.",
        "This person likes to gaze far ahead. A picture with the horizon would be preferable.",
        "Early worm or late worm? Get a view of the dawn... or dusk",
        "Trees bore this person. Please take a picture with NO trees",
        "Higher! Get a picture above the clouds!",
        "Dude really likes trees. Get a picture with a LOT of trees",
        "This highly prestiged individual wants a picture of the whole planet",
        "The baby cries. You don't want it to keep crying, don't you?"
    };

    static System.Func<Customer, string>[] criteria =
    {
        // quests for each character
        (_)=>{ return null; },
        (_)=>{
            List<Ray> samples = new List<Ray>();
            // checks if horizon is in view
            for (float i = 0; i <=1; i += 0.1f)
            {
                samples.Add(Manager.Instance.mCam.ViewportPointToRay(new Vector3(i, 0, 0)));
                samples.Add(Manager.Instance.mCam.ViewportPointToRay(new Vector3(i, 1, 0)));
                samples.Add(Manager.Instance.mCam.ViewportPointToRay(new Vector3(0, i, 0)));
                samples.Add(Manager.Instance.mCam.ViewportPointToRay(new Vector3(1, i, 0)));
            }

            float through = 0, not_through = 0; // there must be a diversity of rays that are either through or not through on the border
            // if all of them are through, then the photoshoot is too far
            // if none of them are going through, then the shoot is too close
            RaycastHit rh; // placeholder
            foreach (Ray r in samples)
            {
                
                if (Manager.Instance.Globe.bound.Raycast(r, out rh, float.PositiveInfinity))
                {
                    Debug.DrawRay(r.origin, 10000 * r.direction, Color.red, 10f);
                    not_through++;
                } else
                {
                    Debug.DrawRay(r.origin, 10000 * r.direction, Color.blue, 10f);
                    through++;
                }
            }
            if (through > 8 && not_through > 8) return null;
            return "That's... not quite it.";
        },
        (_)=>{
            // checks for being dawn/dusk
            float t = Manager.Instance.mCam.GetComponent<OverheadCamera>().t;

            if (t <= 0.78f)
            {
                return "It's a bit too bright, isn't it?";
            }
            else if (t >= 0.85f)
            {
                return "It's dark already...";
            }

            return null;
        },
        (_)=>{
            // checks for no trees in view
            if (Manager.Instance.Globe.CheckCameraInclusion().Count == 0) return null;
            else
            {
                return "Too many trees...";
            }
        },
        (_)=>{
            // checks for above clouds
            if (Vehicle.Instance.transform.position.magnitude > Manager.Instance.Globe.Radius * 1.51f) return null;
            else
            {
                return "Higher!";
            }
        },
        (_)=>{
            // checks for more than 100 trees in view
            if (Manager.Instance.Globe.CheckCameraInclusion().Count > 100) return null;
            else
            {
                return "It's a little... underpopulated";
            }
        },
        (_)=>{
            // checks for view of the entire planet
            Vector3 center = Manager.Instance.mCam.WorldToViewportPoint(Vector3.zero);
            if (!Manager.Instance.Globe.InScreen(center))
            {
                return "Please face the planet";
            }
            float minBounds = Mathf.Min(new float[]{ center.x, center.y, 1-center.x, 1-center.y});
            Vector3 up = Manager.Instance.mCam.WorldToViewportPoint(Manager.Instance.mCam.transform.up * Manager.Instance.Globe.Radius);
            Vector3 radial = up - center;
            radial.z = 0;
            if (radial.magnitude > minBounds)
            {
                return "Please include the whole view, thanks.";
            }
            return null;
        },
        (c)=>{
            // checks for both characters in view, and the 
            if (Manager.Instance.Globe.InScreen(
                    Manager.Instance.mCam.WorldToViewportPoint(Player.instance.transform.position)
                ))
            {
                return "[cries]";
            }
            if (Manager.Instance.Globe.InScreen(
                    Manager.Instance.mCam.WorldToViewportPoint(c.transform.position)
                ))
            {
                return "Don't leave q-q";
            }
            if (!c.picked_up) return "[cries, she wants a hug]";
            return null;
        }
    };
}

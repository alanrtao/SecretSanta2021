using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Custom/Quest")]
public class Quest : ScriptableObject
{
    public Color outfit;
    public Sprite sprite;
    public string display_name;
    [TextArea]
    public string prediag, postdiag;
    public Vector3 size;

    // null is the criterion passing
    public System.Func<Customer, string> criterion { get { return criteria[id]; } }

    public int id { get { return int.Parse(name); } }

    static System.Func<Customer, string>[] criteria =
    {
        // quests for each character
        (_)=>{ return null; },
        (_)=>{
            List<Ray> samples = new List<Ray>();
            // checks if horizon is in view
            for (float i = 0; i <=1; i += 0.1f)
            {
                samples.Add(Manager.Instance.mCam.ScreenPointToRay(new Vector3(i, 0)));
                samples.Add(Manager.Instance.mCam.ScreenPointToRay(new Vector3(i, 1)));
                samples.Add(Manager.Instance.mCam.ScreenPointToRay(new Vector3(0, i)));
                samples.Add(Manager.Instance.mCam.ScreenPointToRay(new Vector3(1, i)));
            }

            bool through = false, not_through = false; // there must be a diversity of rays that are either through or not through on the border
            // if all of them are through, then the photoshoot is too far
            // if none of them are going through, then the shoot is too close
            RaycastHit rh; // placeholder
            foreach (Ray r in samples)
            {
                if (Manager.Instance.Globe.bound.Raycast(r, out rh, 1000f))
                {
                    through = true;
                } else
                {
                    not_through = true;
                }
            }
            if (through & not_through) return null;
            if (through) { return "You're going way too far on this...\nI want to see the horizon, not the whole planet!"; }
            return "Uhh... we're basically still on the ground, aren't we?";
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
            if (Vehicle.Instance.transform.position.magnitude > Manager.Instance.Globe.Radius * 3.51f) return null;
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
                return "It's a little... underpopulated, isn't it?";
            }
        },
        (_)=>{
            // checks for going down at a high altitude
            if (Vector3.Dot(Vehicle.Instance.transform.forward, Vehicle.Instance.transform.position.normalized) < -0.3f)
            {
                return "Not steep enough, I don't feel it :/";
            }
            if (Vehicle.Instance.transform.position.magnitude < Manager.Instance.Globe.Radius * 3f)
            {
                return "Go higher!--and then dive down.";
            }
            return null;
        },
        (_)=>{
            // checks for facing the sun
            if ( Manager.Instance.mCam.GetComponent<OverheadCamera>().t > 0.5f )
            {
                return "Not. Enough. Sun.";
            } else if (Vector3.Dot(
                Vehicle.Instance.transform.position - Manager.Instance.mCam.transform.position,
                Manager.Instance.Sun.transform.position - Vehicle.Instance.transform.position
                ) < 0.1f)
            {
                return "You need to like, face the sun :/";
            }
            return null;
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

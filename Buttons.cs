using BananaSpawner;
using System.Collections;
using UnityEngine;

namespace BananaSpawner
{

    public class NoCD: GorillaPressableButton
    {
        public Material pressedMat;
        public Material unpressedMat;

        public bool CD = false;

        public void Awake()
        {
            pressedMat = new Material(Shader.Find("GorillaTag/UberShader")) { color = Color.red };
            unpressedMat = new Material(Shader.Find("GorillaTag/UberShader")) { color = new Color(0.9f, 0.9f, 0.9f) };
            transform.GetComponent<Renderer>().material = this.unpressedMat;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (!CD)
            {
                StartCoroutine(Press());
            }
        }

        private IEnumerator Press()
        {
            CD = true;
            transform.GetComponent<Renderer>().material = this.pressedMat;
            Plugin.NoCD();
            yield return new WaitForSeconds(0.25f);
            transform.GetComponent<Renderer>().material = this.unpressedMat;
            CD = false;
        }
    }

    public class DeleteAllBananas : GorillaPressableButton
    {
        public Material pressedMat;
        public Material unpressedMat;

        public bool CD = false;

        public void Awake()
        {
            pressedMat = new Material(Shader.Find("GorillaTag/UberShader")) { color = Color.red };
            unpressedMat = new Material(Shader.Find("GorillaTag/UberShader")) { color = new Color(0.9f, 0.9f, 0.9f) };
            transform.GetComponent<Renderer>().material = this.unpressedMat;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (!CD)
            {
                StartCoroutine(Press());
            }
        }

        private IEnumerator Press()
        {
            CD = true;
            transform.GetComponent<Renderer>().material = this.pressedMat;
            Plugin.ClearClones();
            yield return new WaitForSeconds(0.25f);
            transform.GetComponent<Renderer>().material = this.unpressedMat;
            CD = false;
        }
    }

    public class ModOnAndOff : GorillaPressableButton
    {
        public Material pressedMat;
        public Material unpressedMat;

        public bool CD = false;

        public void Awake()
        {
            pressedMat = new Material(Shader.Find("GorillaTag/UberShader")) { color = Color.red };
            unpressedMat = new Material(Shader.Find("GorillaTag/UberShader")) { color = new Color(0.9f, 0.9f, 0.9f) };
            transform.GetComponent<Renderer>().material = this.unpressedMat;
        }
        private void OnTriggerEnter(Collider collider)
        {
            if (!CD)
            {
                StartCoroutine(Press());
            }
        }

        private IEnumerator Press()
        {
            CD = true;
            transform.GetComponent<Renderer>().material = this.pressedMat;
            Plugin.ModOFf();
            yield return new WaitForSeconds(0.25f);
            transform.GetComponent<Renderer>().material = this.unpressedMat;
            CD = false;
        }
    }
}

using UnityEngine;

namespace Kamgam.SettingsGenerator.Examples
{
    public class ObjectActivator : MonoBehaviour
    {
        public GameObject[] Objects;

        public void Activate(int numOfObjectsToActive)
        {
            if (Objects == null)
                return;

            for (int i = 0; i < Objects.Length; i++)
            {
                if (Objects[i] == null)
                    continue;

                Objects[i].SetActive(i < numOfObjectsToActive);
            }
        }
    }
}

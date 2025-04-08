using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Script.Managers
{
    public class DeathManager : MonoBehaviour
    {
        public void onButtonPress(string scene)
        {
            SceneManager.LoadScene(scene);
        }
    }
}
